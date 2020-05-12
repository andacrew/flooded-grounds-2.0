/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEditor;
using Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game;
using Opsive.UltimateCharacterController.Editor.Inspectors;
using Opsive.UltimateCharacterController.Editor.Inspectors.Utility;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Editor.Inspectors.Game
{
    /// <summary>
    /// Shows a custom inspector for the SingleCharacterSpawnManager.
    /// </summary>
    [CustomEditor(typeof(SingleCharacterSpawnManager), true)]
    public class SingleCharacterSpawnManagerInspector : SpawnManagerBaseInspector
    {
        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(PropertyFromName("m_HCharacter"));

            if (EditorGUI.EndChangeCheck()) {
                InspectorUtility.RecordUndoDirtyObject(target, "Change Value");
                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }
    }
}