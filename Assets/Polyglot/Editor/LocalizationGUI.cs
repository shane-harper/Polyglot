//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     GUI tools used by the Localization editor extensions
    /// </summary>
    internal static class LocalizationGUI
    {
        /// <summary>
        ///     Small right aligned 'Add' button
        /// </summary>
        /// <returns>Returns true if button is pressed</returns>
        public static bool AddButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add", EditorStyles.miniButton, GUILayout.Width(50)))
                return true;
            EditorGUILayout.EndHorizontal();
            return false;
        }

        /// <summary>
        ///     Small 'x' button
        /// </summary>
        /// <returns>Returns true if button is pressed</returns>
        public static bool DeleteButton()
        {
            return GUILayout.Button("X", EditorStyles.miniButton, GUILayout.ExpandWidth(false));
        }

        /// <summary>
        ///     Used to draw title foldout with item count
        /// </summary>
        /// <param name="property">Object to display title of</param>
        /// <param name="expanded">If it is expanded</param>
        /// <returns>Returns true if the property should be expanded</returns>
        public static bool DrawTitle(LocalizationProperty property, bool expanded)
        {
            EditorGUILayout.BeginHorizontal();
            var newExpanded = EditorGUILayout.Foldout(expanded, property.Name, true);
            EditorGUILayout.Space();

            // Draw number of items in array
            var alignment = EditorStyles.miniLabel.alignment;
            EditorStyles.miniLabel.alignment = TextAnchor.MiddleRight;
            GUILayout.Label(string.Format("({0} items)", property.KeyCount), EditorStyles.miniLabel);
            EditorStyles.miniLabel.alignment = alignment;
            EditorGUILayout.EndHorizontal();

            return newExpanded;
        }

        /// <summary>
        ///     Creates a GUIContent from a SerializedProperty string value, that uses default text if the value is null
        /// </summary>
        /// <param name="property">Property to extract the string value from</param>
        /// <param name="defaultText">Text to display if the string is null or empty</param>
        public static GUIContent Label(SerializedProperty property, string defaultText = "[Not set]")
        {
            var text = property.stringValue;
            if (string.IsNullOrEmpty(text)) text = "[Not set]";
            return new GUIContent(text);
        }
    }
}