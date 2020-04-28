using UnityEngine;
using UnityEditor;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.Objects.CharacterAssist;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Editor.Managers;
using Opsive.UltimateCharacterController.Editor.Inspectors.Utility;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Editor
{
    /// <summary>
    /// Draws the inspector for the pun add-on.
    /// </summary>
    [OrderedEditorItem("PUN Multiplayer", 0)]
    public class PunMultiplayerAddOnInspector : AddOnInspector
    {
        [SerializeField] private bool m_AddSceneIdentifiers = true;
        [SerializeField] private GameObject m_Object;

        /// <summary>
        /// Draws the add-on inspector.
        /// </summary>
        public override void DrawInspector()
        {
            ManagerUtility.DrawControlBox("Scene Setup", DrawSceneSetup, "Adds the PUN objects to your scene. These objects should be added to each PUN room.", true, "Setup Scene",
                            SetupScene, "The scene objects were added successfully.");
            ManagerUtility.DrawControlBox("Object Setup", DrawObjectSelection, "Adds the PUN component to the character, projectile, grenade, health, respawner, " +
                                            "moving platform, or interactable object.",
                            IsValidObject(), "Setup Object", SetupObject, "The object was setup successfully.");
        }

        /// <summary>
        /// Is the object a valid object that can be converted to pun?
        /// </summary>
        /// <returns>True if the object is a valid object that can be converted to pun.</returns>
        private bool IsValidObject()
        {
            if (m_Object == null) {
                return false;
            }

            if (m_Object.GetComponent<UltimateCharacterLocomotion>() == null &&
                m_Object.GetComponent<Projectile>() == null &&
                m_Object.GetComponent<Grenade>() == null &&
                m_Object.GetComponent<ItemPickup>() == null &&
                m_Object.GetComponent<AttributeManager>() == null &&
                m_Object.GetComponent<Health>() == null &&
                m_Object.GetComponent<Respawner>() == null &&
                m_Object.GetComponent<MovingPlatform>() == null &&
                m_Object.GetComponent<Interactable>() == null) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Draws the additional controls for the scene setup.
        /// </summary>
        private void DrawSceneSetup()
        {
            GUILayout.Space(3);
            m_AddSceneIdentifiers = EditorGUILayout.Toggle("Add Scene Identifiers", m_AddSceneIdentifiers);
        }

        /// <summary>
        /// Draws the additional controls for the object setup.
        /// </summary>
        private void DrawObjectSelection()
        {
            m_Object = EditorGUILayout.ObjectField("Object", m_Object, typeof(GameObject), true) as GameObject;
            if (m_Object != null) {
                if (!IsValidObject()) {
                    EditorGUILayout.HelpBox("The object must be an already created object that contains one of the following components:\n" +
                                            "- Ultimate Character Locomotion\n" +
                                            "- Projectile\n" +
                                            "- Grenade\n" +
                                            "- Item Pickup\n" +
                                            "- Health\n" +
                                            "- Respawner\n" +
                                            "- Moving Platform\n" +
                                            "- Interactable\n", MessageType.Error);
                }
            }
            GUILayout.Space(5);
        }

        /// <summary>
        /// Sets up the room to work with Photon PUN.
        /// </summary>
        private void SetupScene()
        {
            // Create the pun components if they doesn't already exists.
            GameObject punGameObject;
            StateSystem.PunStateManager stateManager;
            if ((stateManager = GameObject.FindObjectOfType<StateSystem.PunStateManager>()) == null) {
                punGameObject = new GameObject("PunGame");
            } else {
                punGameObject = stateManager.gameObject;
            }

            // Add the Singletons.
            InspectorUtility.AddComponent<Game.PunObjectPool>(punGameObject);
            InspectorUtility.AddComponent<Game.RuntimePickups>(punGameObject);
            InspectorUtility.AddComponent<Game.SingleCharacterSpawnManager>(punGameObject);
            var itemTypeTracker = InspectorUtility.AddComponent<Game.ItemTypeTracker>(punGameObject);
            if (itemTypeTracker.ItemCollection == null) {
                itemTypeTracker.ItemCollection = ManagerUtility.FindItemCollection(m_MainManagerWindow);
            }
            InspectorUtility.AddComponent<StateSystem.PunStateManager>(punGameObject);

            // The camera controller should not initialize the character on awake.
            var cameraController = GameObject.FindObjectOfType<Camera.CameraController>();
            if (cameraController != null) {
                cameraController.InitCharacterOnAwake = false;
            }

            // The SceneObjectIdentifier should be added to every collider within the scene.
            if (m_AddSceneIdentifiers) {
                var sceneObjectID = 1;
                var colliders = GameObject.FindObjectsOfType<Collider>();
                for (int i = 0; i < colliders.Length; ++i) {
                    if (colliders[i].isTrigger || colliders[i].gameObject.scene.rootCount == 0 || colliders[i].GetComponent<PhotonView>() != null) {
                        continue;
                    }

                    // The ID should be unique within the scene.
                    var sceneObjectIdentifier = InspectorUtility.AddComponent<Objects.SceneObjectIdentifier>(colliders[i].gameObject);
                    sceneObjectIdentifier.ID = sceneObjectID;
                    sceneObjectID++;

                    InspectorUtility.SetDirty(sceneObjectIdentifier);
                }
            }
        }

        /// <summary>
        /// Sets up the object to be able to work with pun.
        /// </summary>
        private void SetupObject()
        {
            if (m_Object == null) {
                return;
            }

            if (m_Object.GetComponent<UltimateCharacterLocomotion>() != null) {
                SetupCharacter();
                return;
            }

            InspectorUtility.AddComponent<PhotonView>(m_Object);

            // The object isn't a character.
            var synchronizeActiveState = false;
            var projectile = m_Object.GetComponent<Projectile>();
            if (projectile != null) {
                // Replace the projectile with pun projectile.
                if (m_Object.GetComponent<Objects.PunProjectile>() == null) {
                    var punProjectile = InspectorUtility.AddComponent<Objects.PunProjectile>(m_Object);
                    EditorUtility.CopySerialized(projectile, punProjectile);
                    GameObject.DestroyImmediate(projectile, true);
                }
                synchronizeActiveState = true;
            }
            var grenade = m_Object.GetComponent<Grenade>();
            if (grenade != null) {
                // Replace the grenade with pun grenade.
                if (m_Object.GetComponent<Objects.PunGrenade>() == null) {
                    var punGrenade = InspectorUtility.AddComponent<Objects.PunGrenade>(m_Object);
                    EditorUtility.CopySerialized(grenade, punGrenade);
                    GameObject.DestroyImmediate(grenade, true);
                }
                synchronizeActiveState = true;
            }
            var movingPlatform = m_Object.GetComponent<MovingPlatform>();
            if (movingPlatform != null) {
                // Replace the moving platform with pun moving platform.
                if (m_Object.GetComponent<Objects.PunMovingPlatform>() == null) {
                    var punMovingPlatform = InspectorUtility.AddComponent<Objects.PunMovingPlatform>(m_Object);
                    EditorUtility.CopySerialized(movingPlatform, punMovingPlatform);
                    GameObject.DestroyImmediate(movingPlatform, true);
                }
            }
            var itemPickup = m_Object.GetComponent<ItemPickup>();
            if (itemPickup != null) {
                // Replace the item pickup with pun item pickup.
                if (m_Object.GetComponent<Objects.PunItemPickup>() == null) {
                    var punItemPickup = InspectorUtility.AddComponent<Objects.PunItemPickup>(m_Object);
                    EditorUtility.CopySerialized(itemPickup, punItemPickup);
                    GameObject.DestroyImmediate(itemPickup, true);
                }
                synchronizeActiveState = true;
            }
            if (m_Object.GetComponent<AttributeManager>() != null) {
                InspectorUtility.AddComponent<Traits.PunAttributeMonitor>(m_Object);
            }
            if (m_Object.GetComponent<Health>() != null) {
                InspectorUtility.AddComponent<Traits.PunHealthMonitor>(m_Object);
                InspectorUtility.AddComponent<PunNetworkInfo>(m_Object);
                synchronizeActiveState = true;
            }
            if (m_Object.GetComponent<Respawner>() != null) {
                InspectorUtility.AddComponent<Traits.PunRespawnerMonitor>(m_Object);
                InspectorUtility.AddComponent<PunNetworkInfo>(m_Object);
                synchronizeActiveState = true;
            }
            if (m_Object.GetComponent<Interactable>() != null) {
                InspectorUtility.AddComponent<Traits.PunInteractableMonitor>(m_Object);
                InspectorUtility.AddComponent<PunNetworkInfo>(m_Object);
            }
            if (synchronizeActiveState) {
                var locationMonitor = InspectorUtility.AddComponent<Objects.PunLocationMonitor>(m_Object);
                locationMonitor.SynchronizePosition = locationMonitor.SynchronizeRotation = false;
            }
            InspectorUtility.SetDirty(m_Object);
        }

        /// <summary>
        /// Sets up the character to be able to work with pun.
        /// </summary>
        private void SetupCharacter()
        {
            if (m_Object == null) {
                return;
            }

            // Remove the single player variants of the necessary components.
            var animatorMonitor = m_Object.GetComponent<AnimatorMonitor>();
            if (animatorMonitor != null) {
                GameObject.DestroyImmediate(animatorMonitor, true);
            }
            var characterLocomotionHandler = m_Object.GetComponent<UltimateCharacterLocomotionHandler>();
            if (characterLocomotionHandler != null) {
                GameObject.DestroyImmediate(characterLocomotionHandler, true);
                InspectorUtility.AddComponent<Multiplayer.Character.NetworkCharacterLocomotionHandler>(m_Object);
            }

            // The PhotonView will keep the character in sync.
            var photonView = InspectorUtility.AddComponent<PhotonView>(m_Object);
            if (photonView.ObservedComponents == null) {
                photonView.ObservedComponents = new System.Collections.Generic.List<Component>();
            }
            photonView.Synchronization = ViewSynchronization.UnreliableOnChange;

            // Add the pun variants.
            var punTransformMonitor = InspectorUtility.AddComponent<Character.PunCharacterTransformMonitor>(m_Object);
            var punAnimatorMonitor = InspectorUtility.AddComponent<Character.PunAnimatorMonitor>(m_Object);
            var punLookSource = InspectorUtility.AddComponent<Character.PunLookSource>(m_Object);
            if (!photonView.ObservedComponents.Contains(punTransformMonitor)) {
                photonView.ObservedComponents.Add(punTransformMonitor);
            }
            if (!photonView.ObservedComponents.Contains(punAnimatorMonitor)) {
                photonView.ObservedComponents.Add(punAnimatorMonitor);
            }
            if (!photonView.ObservedComponents.Contains(punLookSource)) {
                photonView.ObservedComponents.Add(punLookSource);
            }

            // Add the independent pun components.
            InspectorUtility.AddComponent<Character.PunCharacter>(m_Object);
            InspectorUtility.AddComponent<PunNetworkInfo>(m_Object);

            // Certain components may be necessary if their single player components is added to the character.
            if (m_Object.GetComponent<AttributeManager>() != null) {
                InspectorUtility.AddComponent<Traits.PunAttributeMonitor>(m_Object);
            }
            if (m_Object.GetComponent<Health>() != null) {
                InspectorUtility.AddComponent<Traits.PunHealthMonitor>(m_Object);
            }
            if (m_Object.GetComponent<Respawner>() != null) {
                InspectorUtility.AddComponent<Traits.PunRespawnerMonitor>(m_Object);
            }
            if (m_Object.GetComponent<Destructible>() != null) {
                InspectorUtility.AddComponent<Objects.PunDestructibleMonitor>(m_Object);
            }

            // The RemotePlayerPerspectiveMonitor will switch out the first person materials if the third person Perspective Monitor doesn't exist.
#if THIRD_PERSON_CONTROLLER
            var addRemotePlayerPerspectiveMonitor = m_Object.GetComponent<ThirdPersonController.Character.PerspectiveMonitor>() != null;
#else
            var addRemotePlayerPerspectiveMonitor = true;
#endif
            var invisibleShadowCastor = ManagerUtility.FindInvisibleShadowCaster(m_MainManagerWindow);
            if (addRemotePlayerPerspectiveMonitor) {
                var remotePlayerPerspectiveMonitor = InspectorUtility.AddComponent<Multiplayer.Character.RemotePlayerPerspectiveMonitor>(m_Object);
                if (remotePlayerPerspectiveMonitor.InvisibleMaterial == null) {
                    remotePlayerPerspectiveMonitor.InvisibleMaterial = invisibleShadowCastor;
                }
            }

            // Any invisible shadow castor materials should be swapped out for a default material.
            var renderers = m_Object.GetComponentsInChildren<Renderer>(true);
            var updatedMaterialCount = 0;
            var defaultShader = Shader.Find("Standard");
            for (int i = 0; i < renderers.Length; ++i) {
                var materials = renderers[i].sharedMaterials;
                for (int j = 0; j < materials.Length; ++j) {
                    if (materials[j] == invisibleShadowCastor) {
                        materials[j] = new Material(defaultShader);
                        updatedMaterialCount++;
                    }
                }
                renderers[i].sharedMaterials = materials;
            }
            if (updatedMaterialCount > 0) {
                Debug.Log("Updated " + updatedMaterialCount + " invisible shadow castor materials. Ensure the correct material has been assigned before continuing.");
            }

            // Add the ObjectInspector to any character or ragdoll colliders. This will allow the collider GameObjects to be identifiable over the network.
            var maxID = -1;
            var existingIdentifiers = m_Object.GetComponentsInChildren<ObjectIdentifier>(true);
            for (int i = 0; i < existingIdentifiers.Length; ++i) {
                var collider = existingIdentifiers[i].GetComponent<Collider>();
                if (collider != null) {
                    // The collider may be used for a ragdoll. Ragdoll colliders should not contribute to the max id.
                    if (!collider.isTrigger && 
                        (collider.gameObject.layer == LayerManager.Character || 
                        (collider.gameObject.layer == LayerManager.SubCharacter && collider.GetComponent<Rigidbody>() != null))) {
                        continue;
                    }
                }

                if (existingIdentifiers[i].ID > maxID) {
                    maxID = existingIdentifiers[i].ID;
                }
            }

            // The max available ID has been determined. Add the ObjectIdentifier.
            var colliders = m_Object.GetComponentsInChildren<Collider>(true);
            var IDOffset = 1;
            for (int i = 0; i < colliders.Length; ++i) {
                if (colliders[i].isTrigger ||
                    (colliders[i].gameObject.layer != LayerManager.Character &&
                    (colliders[i].gameObject.layer != LayerManager.SubCharacter || colliders[i].GetComponent<Rigidbody>() == null))) {
                    continue;
                }

                var objectIdentifier = InspectorUtility.AddComponent<ObjectIdentifier>(colliders[i].gameObject);
                objectIdentifier.ID = maxID + IDOffset;
                IDOffset++;
            }

            InspectorUtility.SetDirty(m_Object);
        }
    }
}