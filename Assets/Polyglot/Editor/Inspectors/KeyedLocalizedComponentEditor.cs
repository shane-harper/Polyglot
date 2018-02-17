//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Base editor for keyed localized components
    /// </summary>
    public abstract class KeyedLocalizedComponentEditor : UnityEditor.Editor
    {
        private static readonly string[] HiddenParameters = {"m_Script", "_key", "_target"};

        private SerializedProperty _key;
        private int _selectedIndex = -1;
        private LocalizationKeyDropdown _keyDropdown;

        protected abstract string Property { get; }

        private void OnEnable()
        {
            // Cache key property for later
            _key = serializedObject.FindProperty("_key");

            // Get property from localization data
            var data = LocalizationFileManager.LoadData();
            var editor = CreateEditor(data);
            var property = editor.serializedObject.FindProperty(Property);

            // Create dropdown
            var label = new GUIContent("Key");
            _keyDropdown = new LocalizationKeyDropdown(label, property);
            
            // Find currently selected key
            _selectedIndex = GetSelectedIndex();
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            DrawKey();
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, HiddenParameters);
            if (EditorGUI.EndChangeCheck())this.Save();
        }

        protected void DrawKey()
        {
            // If no keys are found, display an error
            if (_keyDropdown.Length <= 0)
            {
                EditorGUILayout.HelpBox("No keys found in localization data", MessageType.Error);
                return;
            }
            EditorGUI.BeginChangeCheck();

            // Draw dropdown
            var selectedIndex = _keyDropdown.Draw(_selectedIndex);

            // Validate selection
            if (_selectedIndex < 0)
                EditorGUILayout.HelpBox(string.Format("Key '{0}' was not found in localization data", _key.stringValue),
                    MessageType.Error);

            // Save changes
            if (EditorGUI.EndChangeCheck())
            {
                _selectedIndex = selectedIndex;
                _key.stringValue = _keyDropdown.GetName(_selectedIndex);
                this.Save();                

                // Re-apply localization with new value
                ((LocalizedComponent) target).RefreshLocalization();
            }
        }

        private int GetSelectedIndex()
        {
            var currentKey = _key.stringValue;
            for (var i = 0; i < _keyDropdown.Length; ++i)
                if (_keyDropdown.GetName(i) == currentKey) return i;

            return -1;
        }
    }

    [CustomEditor(typeof(LocalizedText), true)]
    public sealed class LocalizedTextEditor : KeyedLocalizedComponentEditor
    {
        protected override string Property
        {
            get { return "_strings"; }
        }
    }

    [CustomEditor(typeof(LocalizedImage), true)]
    public sealed class LocalizedImageEditor : KeyedLocalizedComponentEditor
    {
        protected override string Property
        {
            get { return "_sprites"; }
        }
    }

    [CustomEditor(typeof(LocalizedAudioSource), true)]
    public sealed class LocalizedAudioSourceEditor : KeyedLocalizedComponentEditor
    {
        protected override string Property
        {
            get { return "_strings"; }
        }
    }
}