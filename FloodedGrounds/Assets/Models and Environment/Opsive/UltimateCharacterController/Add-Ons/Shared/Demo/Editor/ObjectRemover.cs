/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

#if !ULTIMATE_CHARACTER_CONTROLLER_ADDON_DEBUG
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
#if !FIRST_PERSON_CONTROLLER || !THIRD_PERSON_CONTROLLER
using Opsive.UltimateCharacterController.Camera.ViewTypes;
using Opsive.UltimateCharacterController.Character.MovementTypes;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Opsive.UltimateCharacterController.StateSystem;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.Editor.Inspectors.Utility;
using Opsive.UltimateCharacterController.Editor.Managers;
using System.Collections.Generic;
#endif

namespace Opsive.UltimateCharacterController.AddOns.Shared.Demo.Editor
{
    [InitializeOnLoad]
    public class ObjectRemover
    {
        private static Scene s_ActiveScene;

        /// <summary>
        /// Registers for the scene change callback.
        /// </summary>
        static ObjectRemover()
        {
            EditorApplication.update += Update;
        }

        /// <summary>
        /// The scene has been changed.
        /// </summary>
        private static void Update()
        {
            var scene = SceneManager.GetActiveScene();

            if (scene == s_ActiveScene || Application.isPlaying) {
                return;
            }
            s_ActiveScene = scene;

            // Only the add-ons demo scene should be affected.
            if (!s_ActiveScene.path.Replace("\\", "/").Contains("UltimateCharacterController/Add-Ons")) {
                return;
            }

            // Find the object which contains the objects that should be removed.
            var objectReferences = GameObject.FindObjectOfType<ObjectReferences>();
            ProcessObjectReferences(objectReferences, true);
        }

        /// <summary>
        /// Removes the objects specified by the object references object.
        /// </summary>
        private static void ProcessObjectReferences(ObjectReferences objectReferences, bool fromScene)
        {
            if (objectReferences == null) {
                return;
            }

            RemoveObjects(objectReferences.RemoveObjects);
            objectReferences.RemoveObjects = null;
#if !FIRST_PERSON_CONTROLLER
            RemoveObjects(objectReferences.FirstPersonObjects);
            objectReferences.FirstPersonObjects = null;
#endif
#if !THIRD_PERSON_CONTROLLER
            RemoveObjects(objectReferences.ThirdPersonObjects);
            objectReferences.ThirdPersonObjects = null;
#endif
#if !ULTIMATE_CHARACTER_CONTROLLER_SHOOTER
            RemoveObjects(objectReferences.ShooterObjects);
            objectReferences.ShooterObjects = null;
#endif
#if !ULTIMATE_CHARACTER_CONTROLLER_MELEE
            RemoveObjects(objectReferences.MeleeObjects);
            objectReferences.MeleeObjects = null;
#endif

#if !FIRST_PERSON_CONTROLLER || !THIRD_PERSON_CONTROLLER
            // Remove any view types and states that are no longer valid.
            Camera.CameraController cameraController;
            if (fromScene) {
                cameraController = GameObject.FindObjectOfType<Camera.CameraController>();
            } else {
                cameraController = objectReferences.GetComponent<Camera.CameraController>();
            }
            if (cameraController != null) {
                cameraController.DeserializeViewTypes();
                var viewTypes = new List<ViewType>(cameraController.ViewTypes);
                for (int i = viewTypes.Count - 1; i > -1; --i) {
                    if (viewTypes[i] == null) {
                        viewTypes.RemoveAt(i);
                        continue;
                    }
                    viewTypes[i].States = RemoveUnusedStates(viewTypes[i].States);
                }
                cameraController.ViewTypeData = Serialization.Serialize<ViewType>(viewTypes);
                cameraController.ViewTypes = viewTypes.ToArray();
                InspectorUtility.SetDirty(cameraController);
            }

            // Remove any movement types and states that are no longer valid.
            Character.UltimateCharacterLocomotion characterLocomotion;
            if (fromScene) {
                characterLocomotion = GameObject.FindObjectOfType<Character.UltimateCharacterLocomotion>();
            } else {
                characterLocomotion = objectReferences.GetComponent<Character.UltimateCharacterLocomotion>();
            }
            if (characterLocomotion != null) {
                characterLocomotion.DeserializeMovementTypes();
                var movementTypes = new List<MovementType>(characterLocomotion.MovementTypes);
                for (int i = movementTypes.Count - 1; i > -1; --i) {
                    if (movementTypes[i] == null) {
                        movementTypes.RemoveAt(i);
                        continue;
                    }
                    movementTypes[i].States = RemoveUnusedStates(movementTypes[i].States);
                }
                characterLocomotion.MovementTypeData = Serialization.Serialize<MovementType>(movementTypes);
                characterLocomotion.MovementTypes = movementTypes.ToArray();
#if FIRST_PERSON_CONTROLLER
                characterLocomotion.SetMovementType(Utility.UnityEngineUtility.GetType(characterLocomotion.FirstPersonMovementTypeFullName));
#else
                characterLocomotion.SetMovementType(Utility.UnityEngineUtility.GetType(characterLocomotion.ThirdPersonMovementTypeFullName));
#endif

                // Ensure the animator is pointing to the correct reference.
                var animator = characterLocomotion.GetComponent<Animator>();
                if (animator != null && animator.runtimeAnimatorController == null) {
                    animator.runtimeAnimatorController = ManagerUtility.FindAnimatorController(null);
                    InspectorUtility.SetDirty(animator);
                }

                // Check for unused ability states.
                var abilities = new List<Ability>(characterLocomotion.GetSerializedAbilities());
                for (int i = abilities.Count - 1; i > -1; --i) {
                    if (abilities[i] == null) {
                        abilities.RemoveAt(i);
                        continue;
                    }
                    abilities[i].States = RemoveUnusedStates(abilities[i].States);
                }
                characterLocomotion.AbilityData = Serialization.Serialize<Ability>(abilities);
                characterLocomotion.Abilities = abilities.ToArray();

                // Check for unused item ability states.
                var itemAbilities = new List<ItemAbility>(characterLocomotion.GetSerializedItemAbilities());
                for (int i = itemAbilities.Count - 1; i > -1; --i) {
                    if (itemAbilities[i] == null) {
                        itemAbilities.RemoveAt(i);
                        continue;
                    }
                    itemAbilities[i].States = RemoveUnusedStates(itemAbilities[i].States);
                }
                characterLocomotion.ItemAbilityData = Serialization.Serialize<ItemAbility>(itemAbilities);
                characterLocomotion.ItemAbilities = itemAbilities.ToArray();
                InspectorUtility.SetDirty(characterLocomotion);

                // Update the inventory.
                var inventory = characterLocomotion.GetComponent<Inventory.InventoryBase>();
                var loadout = new List<Inventory.ItemTypeCount>(inventory.DefaultLoadout);
                for (int i = loadout.Count - 1; i > -1; --i) {
                    if (loadout[i] == null) {
                        loadout.RemoveAt(i);
                    }
                }
                inventory.DefaultLoadout = loadout.ToArray();
                InspectorUtility.SetDirty(inventory);

                var itemSetManager = characterLocomotion.GetComponent<Inventory.ItemSetManager>();
                if (itemSetManager != null) {
                    var categoryItemSets = itemSetManager.CategoryItemSets;
                    for (int i = 0; i < categoryItemSets.Length; ++i) {
                        for (int j = categoryItemSets[i].ItemSetList.Count - 1; j > -1; --j) {
                            var nullItemType = true;
                            for (int k = 0; k < categoryItemSets[i].ItemSetList[j].Slots.Length; ++k) {
                                if (categoryItemSets[i].ItemSetList[j].Slots[k] != null) {
                                    nullItemType = false;
                                    break;
                                }
                            }
                            if (nullItemType) {
                                categoryItemSets[i].ItemSetList.RemoveAt(j);
                            }
                        }
                    };
                    InspectorUtility.SetDirty(itemSetManager);
                }
            }

#if !THIRD_PERSON_CONTROLLER
            // Set the shadow caster for the first person only objects.
            var shadowCaster = ManagerUtility.FindInvisibleShadowCaster(null);
            if (shadowCaster != null) {
                for (int i = 0; i < objectReferences.ShadowCasterObjects.Length; ++i) {
                    if (objectReferences.ShadowCasterObjects[i] == null) {
                        continue;
                    }

                    var renderers = objectReferences.ShadowCasterObjects[i].GetComponentsInChildren<Renderer>();
                    for (int j = 0; j < renderers.Length; ++j) {
                        var materials = renderers[j].sharedMaterials;
                        for (int k = 0; k < materials.Length; ++k) {
                            materials[k] = shadowCaster;
                        }
                        renderers[j].sharedMaterials = materials;
                        InspectorUtility.SetDirty(renderers[j]);
                    }
                }
            }
#endif

            // Ensure all of the states point to a preset
            StateBehavior[] stateBehaviors;
            if (fromScene) {
                stateBehaviors = GameObject.FindObjectsOfType<StateBehavior>();
            } else {
                stateBehaviors = objectReferences.GetComponentsInChildren<StateBehavior>(true);
            }
            if (stateBehaviors != null) {
                for (int i = 0; i < stateBehaviors.Length; ++i) {
                    stateBehaviors[i].States = RemoveUnusedStates(stateBehaviors[i].States);
                    InspectorUtility.SetDirty(stateBehaviors[i]);
                }
            }
#endif
            // Some doors should be locked.
#if !FIRST_PERSON_CONTROLLER
            LockDoors(objectReferences.FirstPersonDoors);
#endif
#if !THIRD_PERSON_CONTROLLER
            LockDoors(objectReferences.ThirdPersonDoors);
#endif

            for (int i = 0; i < objectReferences.NestedReferences.Length; ++i) {
                ProcessObjectReferences(objectReferences.NestedReferences[i], false);
            }

            Object.DestroyImmediate(objectReferences, true);
        }

        /// <summary>
        /// Removes the specified objects.
        /// </summary>
        private static void RemoveObjects(Object[] objects)
        {
            if (objects == null) {
                return;
            }

            for (int i = objects.Length - 1; i > -1; --i) {
                if (objects[i] == null
#if UNITY_2018_3_OR_NEWER
                    || PrefabUtility.GetPrefabAssetType(objects[i]) == PrefabAssetType.MissingAsset
#else
                    || PrefabUtility.GetPrefabType(objects[i]) == PrefabType.MissingPrefabInstance 
#endif
                    ) {
                    continue;
                }

                if (objects[i] is GameObject && (objects[i] as GameObject).transform.parent == null && AssetDatabase.GetAssetPath(objects[i]).Length > 0 &&
#if UNITY_2018_3_OR_NEWER
                    PrefabUtility.GetPrefabAssetType(objects[i]) == PrefabAssetType.Regular
#else
                    PrefabUtility.GetPrefabType(objects[i]) == PrefabType.Prefab
#endif
                    ) {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(objects[i]));
                } else {
                    Object.DestroyImmediate(objects[i], true);
                }
            }
        }

#if !FIRST_PERSON_CONTROLLER || !THIRD_PERSON_CONTROLLER
        /// <summary>
        /// Removes any states whose preset will be exlcluded.
        /// </summary>
        private static State[] RemoveUnusedStates(State[] stateArray)
        {
            var states = new List<State>(stateArray);
            var stateRemovals = new HashSet<string>();
            for (int i = states.Count - 2; i > -1; --i) {
                var preset = states[i].Preset;
                if (preset == null) {
                    stateRemovals.Add(states[i].Name);
                    states.RemoveAt(i);
                }
            }
            for (int i = 0; i < states.Count; ++i) {
                var blockList = new List<string>(states[i].BlockList);
                for (int j = blockList.Count - 1; j > -1; --j) {
                    if (stateRemovals.Contains(blockList[j])) {
                        blockList.RemoveAt(j);
                    }
                }
                states[i].BlockList = blockList.ToArray();
            }
            return states.ToArray();
        }

        /// <summary>
        /// Locks the unused doors.
        /// </summary>
        private static void LockDoors(GameObject[] doors)
        {
            if (doors == null) {
                return;
            }

            for (int i = 0; i < doors.Length; ++i) {
                if (doors[i] == null) {
                    continue;
                }
                var door = doors[i].GetComponent<Opsive.UltimateCharacterController.Demo.Objects.Door>();
                if (door == null) {
                    continue;
                }
                door.PermanentlyLocked = true;
                Opsive.UltimateCharacterController.Editor.Inspectors.Utility.InspectorUtility.SetDirty(door);
            }
        }
#endif
    }
}
#endif