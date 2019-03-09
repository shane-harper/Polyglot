//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using System;
using Polyglot.Components;
using Polyglot.Data;
using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Custom inspector for the LocSettings scriptable object
    /// </summary>
    [CustomEditor(typeof(LocSettings))]
    public class LocSettingsEditor : UnityEditor.Editor
    {
        private Defaults _defaults;
        private Languages _languages;

        private void OnEnable()
        {
            _languages = new Languages(serializedObject);
            _defaults = new Defaults(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                _languages.Draw();
                _defaults.Draw();

                if (check.changed) this.Save();
            }
        }

        private static void RefreshLocalization()
        {
            // Refresh each component
            var components = Resources.FindObjectsOfTypeAll<LocComponent>();
            foreach (var component in components)
            {
                component.RefreshLocalization();
                EditorUtility.SetDirty(component);
            }

            // Force repaint
            SceneView.RepaintAll();
        }

        private class Languages
        {
            private static bool _expand = true;
            private readonly GUIContent _label;
            private readonly SerializedProperty[] _languages;

            public Languages(SerializedObject serializedObject)
            {
                var languages = serializedObject.FindProperty("_languages");
                _languages = languages.GetAllElements();

                _label = new GUIContent(string.Format("Languages ({0})", _languages.Length));
            }

            public void Draw()
            {
                _expand = EditorGUILayout.Foldout(_expand, _label, true);
                if (!_expand) return;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                for (var i = 0; i < _languages.Length; ++i)
                {
                    var selected = PlayerPrefs.GetInt(LocManager.LastLocPrefKey, -1) == i;
                    var language = _languages[i];
                    EditorGUILayout.BeginHorizontal();

                    var bold = EditorStyles.label.fontStyle;
                    if (selected) EditorStyles.label.fontStyle = FontStyle.Bold;
                    EditorGUILayout.LabelField(language.stringValue);
                    EditorStyles.label.fontStyle = bold;

                    if (GUILayout.Toggle(selected, "Preview", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) &&
                        !selected)
                    {
                        LocManager.SetLocalization(i);
                        RefreshLocalization();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
        }

        private class Defaults
        {
            private static bool _expand = true;
            private readonly SerializedProperty[] _defaults;
            private readonly GUIContent _label;
            private readonly string[] _languageList;
            private readonly string[] _languageListWithEmpty;
            private readonly GUIContent[] _systemLanguageLabels;
            private readonly GUIContent _fallbackLabel;
            private bool _uiInitialized;

            public Defaults(SerializedObject serializedObject)
            {
                // Generate system language labels
                var systemLanguageCount = Enum.GetValues(typeof(SystemLanguage)).Length - 1; // -1 for 'Hugarian'
                _systemLanguageLabels = new GUIContent[systemLanguageCount];
                for (var i = 0; i < systemLanguageCount; ++i)
                    _systemLanguageLabels[i] = new GUIContent(((SystemLanguage) i).ToString());

                // Generate language list
                var languages = serializedObject.FindProperty("_languages");
                _languageList = new string[languages.arraySize];
                _languageListWithEmpty = new string[languages.arraySize+1];
                _languageListWithEmpty[0] = " -";
                for (var i = 0; i < languages.arraySize; ++i)
                {
                    _languageList[i] = languages.GetArrayElementAtIndex(i).stringValue;
                    _languageListWithEmpty[i + 1] = languages.GetArrayElementAtIndex(i).stringValue;
                }

                // Ensure defaults array is populated
                var defaultsArray = serializedObject.FindProperty("_defaults");
                var currentCount = defaultsArray.arraySize;
                for (var i = currentCount; i < systemLanguageCount; ++i)
                {
                    defaultsArray.InsertArrayElementAtIndex(i);
                    defaultsArray.GetArrayElementAtIndex(i).intValue = i == systemLanguageCount - 1 ? 0 : -1;
                }

                // Get labels
                _defaults = defaultsArray.GetAllElements();
                _label = new GUIContent("Default Localizations",
                    "Auto selected localizations based on system language");
                _fallbackLabel = new GUIContent("Default",
                    "Localization to fall back to when no default localization is found/set");
            }

            public void Draw()
            {
                _expand = EditorGUILayout.Foldout(_expand, _label, true);
                if (!_expand) return;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Draw fallback default
                var bold = EditorStyles.label.fontStyle;
                EditorGUILayout.BeginHorizontal();
                EditorStyles.label.fontStyle = FontStyle.Bold;
                var defaultLocalization = _defaults[_defaults.Length - 1];
                defaultLocalization.intValue =
                    EditorGUILayout.Popup(_fallbackLabel, defaultLocalization.intValue, _languageList);
                EditorStyles.label.fontStyle = bold;
                EditorGUILayout.EndHorizontal();
                
                // Draw system language defaults
                EditorGUILayout.Space();
                for (var i = 0; i < _defaults.Length-1; ++i)
                {
                    var element = _defaults[i];

                    // Enabled defaults should be bold
                    bold = EditorStyles.label.fontStyle;
                    if (element.intValue != -1) EditorStyles.label.fontStyle = FontStyle.Bold;
                    EditorGUILayout.BeginHorizontal();
                    element.intValue = EditorGUILayout.Popup(_systemLanguageLabels[i], 
                                           element.intValue + 1, _languageListWithEmpty) - 1;
                    EditorStyles.label.fontStyle = bold;

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}