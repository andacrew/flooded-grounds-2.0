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
    /// Shows a custom inspector for the SpawnManagerBase.
    /// </summary>
    [CustomEditor(typeof(SpawnManagerBase), true)]
    public class SpawnManagerBaseInspector : InspectorBase
    {
        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            var modeProperty = PropertyFromName("m_Mode");
            EditorGUILayout.PropertyField(modeProperty);
            if (modeProperty.enumValueIndex == (int)SpawnManagerBase.SpawnMode.FixedLocation) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(PropertyFromName("m_SpawnLocation"));
                EditorGUILayout.PropertyField(PropertyFromName("m_SpawnLocationOffset"));
                EditorGUI.indentLevel--;
            } else {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(PropertyFromName("m_SpawnPointGrouping"));
                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck()) {
                InspectorUtility.RecordUndoDirtyObject(target, "Change Value");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}