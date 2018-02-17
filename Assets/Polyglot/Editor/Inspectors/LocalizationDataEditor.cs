//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Custom editor for LocalizationData
    /// </summary>
    [CustomEditor(typeof(LocalizationData), true)]
    public sealed class LocalizationDataEditor : UnityEditor.Editor
    {
        private static int _expanded;

        internal readonly LocalizationProperty[] Properties =
        {
            new LocalizationProperty("String", "_strings", LocalizationProperty.StringComparison,
                LocalizationProperty.StringComparison, MessageType.Warning),
            new LocalizationProperty("Sprite", "_sprites", LocalizationProperty.StringComparison,
                LocalizationProperty.ObjectComparison, MessageType.Warning),
            new LocalizationProperty("Sound", "_sounds", LocalizationProperty.StringComparison,
                LocalizationProperty.ObjectComparison, MessageType.Warning),
            new LocalizationProperty("Font", "_fonts", LocalizationProperty.ObjectComparison,
                LocalizationProperty.ObjectComparison, MessageType.Info)
        };

        private SerializedProperty _names;

        private void OnEnable()
        {
            // Load data
            if (target == null) return;
            _names = serializedObject.FindProperty("_names");

            // Initialize properties
            foreach (var property in Properties)
                property.Initialize(serializedObject, _names.arraySize);
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            // Draw localizations
            GUILayout.Label("Localizations", EditorStyles.boldLabel);

            // Draw all localizations
            EditorGUI.BeginChangeCheck();
            for (var i = 0; i < _names.arraySize; ++i)
                DrawLocalization(i);

            // Draw button to add new localization
            if (LocalizationGUI.AddButton())
                AddLocalization(_names.arraySize);

            EditorGUILayout.Space();

            // Draw properties
            for (var i = 0; i < Properties.Length; ++i)
            {
                var property = Properties[i];
                var expanded = _expanded == i;
                var newExpanded = LocalizationGUI.DrawTitle(property, expanded);
                if (expanded != newExpanded) _expanded = newExpanded ? i : -1;

                if (expanded) DrawProperty(i);
            }

            if (EditorGUI.EndChangeCheck()) this.Save();
        }

        private void DrawLocalization(int index)
        {
            EditorGUILayout.BeginHorizontal();

            // Draw localization name
            var name = _names.GetArrayElementAtIndex(index);
            EditorGUILayout.PropertyField(name, new GUIContent("#" + index));

            // Draw delete button
            if (LocalizationGUI.DeleteButton())
                DeleteLocalization(index);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawProperty(int index)
        {
            var property = Properties[index];

            // Draw items
            for (var i = 0; i < property.KeyCount; ++i)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                // Get and display key
                var key = property.GetKey(i);
                EditorGUILayout.PropertyField(key);

                // Draw delete button
                if (LocalizationGUI.DeleteButton())
                {
                    DeleteKey(property, i);
                    return;
                }
                EditorGUILayout.EndHorizontal();

                // Draw values
                ++EditorGUI.indentLevel;
                var values = property.GetValues(i);
                for (var v = 0; v < values.arraySize; ++v)
                {
                    var value = values.GetArrayElementAtIndex(v);
                    var name = _names.GetArrayElementAtIndex(v);
                    EditorGUILayout.PropertyField(value, LocalizationGUI.Label(name));
                }
                --EditorGUI.indentLevel;
                EditorGUILayout.EndVertical();
            }

            // Draw add key button
            if (LocalizationGUI.AddButton()) AddKey(property);

            EditorGUILayout.Space();
        }

        #region Add/Remove items

        private void AddKey(LocalizationProperty property)
        {
            var count = property.KeyCount;

            // Increase array size and add new element
            property.SerializedProperty.arraySize = count + 1;
            var newEntry = property.SerializedProperty.GetArrayElementAtIndex(count);

            // Attempt to clear key
            var key = newEntry.FindPropertyRelative("_key");
            key.ResetProperty();

            // Ensure new values are empty
            var values = newEntry.FindPropertyRelative("_values");
            values.arraySize = _names.arraySize;
            values.ClearArray();

            LocalizationEditorTools.ValidateValueCounts(_names.arraySize, Properties);
            this.Save();
        }

        private void DeleteKey(LocalizationProperty property, int index)
        {
            property.SerializedProperty.DeleteArrayElementAtIndex(index);
            this.Save();
        }

        private void AddLocalization(int index)
        {
            // Insert new element and clear value
            _names.InsertArrayElementAtIndex(index);
            var newEntry = _names.GetArrayElementAtIndex(index);
            newEntry.stringValue = string.Empty;

            // Update values count in properties
            LocalizationEditorTools.ValidateValueCounts(_names.arraySize, Properties);
            this.Save();
        }

        private void DeleteLocalization(int index)
        {
            // Display dialog before deleting localization
            const string title = "Delete Localization";
            var message = string.Format("Are you sure you want to delete #{1}: '{0}'?",
                _names.GetArrayElementAtIndex(index).stringValue, index);

            if (EditorUtility.DisplayDialog(title, message, "Yes", "No"))
            {
                // Remove element and update values count in properties
                _names.DeleteArrayElementAtIndex(index);
                LocalizationEditorTools.ValidateValueCounts(_names.arraySize, Properties);
                this.Save();
            }
        }

        #endregion
    }
}