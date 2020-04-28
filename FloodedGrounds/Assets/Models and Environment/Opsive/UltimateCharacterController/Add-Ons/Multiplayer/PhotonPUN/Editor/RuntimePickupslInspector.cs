/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game;
using Opsive.UltimateCharacterController.Editor.Inspectors;
using Opsive.UltimateCharacterController.Editor.Inspectors.Utility;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Editor.Inspectors.Game
{
    /// <summary>
    /// Shows a custom inspector for the RuntimePickups.
    /// </summary>
    [CustomEditor(typeof(RuntimePickups))]
    public class RuntimePickupsInspector : InspectorBase
    {
        private ReorderableList m_RuntimeItemsReorderableList;

        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            if (m_RuntimeItemsReorderableList == null) {
                var preloadedPrefabsProperty = PropertyFromName("m_RuntimeItems");
                m_RuntimeItemsReorderableList = new ReorderableList(serializedObject, preloadedPrefabsProperty, true, false, true, true);
                m_RuntimeItemsReorderableList.drawHeaderCallback = OnNetworkRuntimeItemsDrawHeader;
                m_RuntimeItemsReorderableList.drawElementCallback = OnNetworkRuntimeItemsElementDraw;
            }
            m_RuntimeItemsReorderableList.DoLayoutList();
            if (EditorGUI.EndChangeCheck()) {
                InspectorUtility.RecordUndoDirtyObject(target, "Change Value");
                serializedObject.ApplyModifiedProperties();
                InspectorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// Draws the header for the RuntimeItems list.
        /// </summary>
        private void OnNetworkRuntimeItemsDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x + 12, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Runtime Items");
        }

        /// <summary>
        /// Draws the RuntimeItems ReordableList element.
        /// </summary>
        private void OnNetworkRuntimeItemsElementDraw(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.BeginChangeCheck();

            var runtimeItem = m_RuntimeItemsReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.ObjectField(new Rect(rect.x, rect.y + 1, rect.width, EditorGUIUtility.singleLineHeight), runtimeItem, new GUIContent());

            if (EditorGUI.EndChangeCheck()) {
                var serializedObject = m_RuntimeItemsReorderableList.serializedProperty.serializedObject;
                InspectorUtility.RecordUndoDirtyObject(serializedObject.targetObject, "Change Value");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}