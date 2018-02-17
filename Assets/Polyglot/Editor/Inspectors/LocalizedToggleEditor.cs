//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Base editor for localized toggle components
    /// </summary>
    public abstract class LocalizedToggleEditor : UnityEditor.Editor
    {
        private static readonly string[] HiddenParameters = { "m_Script", "EnabledLocalizations" };

        private SerializedProperty _enabledLocalizations;
        private GUIContent[] _names;

        private void OnEnable()
        {
            // Get toggle array
            _enabledLocalizations = serializedObject.FindProperty("EnabledLocalizations");

            // Load localization data
            var data = LocalizationFileManager.LoadData();
            var editor = CreateEditor(data);
            var property = editor.serializedObject.FindProperty("_names");

            // Get localization names
            int localizationCount = property.arraySize;
            _names = new GUIContent[localizationCount];
            for (int i = 0; i < localizationCount; ++i)
                _names[i] = new GUIContent(property.GetArrayElementAtIndex(i).stringValue);

            // Make sure the toggle array has the same count as the number of localizations
            if (_enabledLocalizations.arraySize != localizationCount)
            {
                _enabledLocalizations.arraySize = localizationCount;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Base inspector
            DrawPropertiesExcluding(serializedObject, HiddenParameters);

            GUILayout.Label("Localizations", EditorStyles.boldLabel);

            // Display localizations
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            int localizationCount = _names.Length;
            for (int i = 0; i < localizationCount; ++i)
            {
                var property = _enabledLocalizations.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(property, _names[i]);
            }
            EditorGUILayout.EndVertical();

            // Save changes
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }

    [CustomEditor(typeof(LocalizedToggleGameObject), true)]
    public sealed class LocalizedToggleGameObjectEditor : LocalizedToggleEditor
    {
    }

    [CustomEditor(typeof(LocalizedToggleBehaviour), true)]
    public sealed class LocalizedToggleBehaviourEditor : LocalizedToggleEditor
    {
    }
}