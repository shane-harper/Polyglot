//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;

namespace Polyglot.Editor.Tabs
{
    /// <summary>
    ///     Window tab for displaying the LocalizationData Editor
    /// </summary>
    internal sealed class LocalizationDataTab : ILocalizationTab
    {
        private readonly UnityEditor.Editor _editor;

        public LocalizationDataTab(UnityEditor.Editor dataEditor)
        {
            _editor = dataEditor;
        }

        /// <inheritdoc />
        public void DrawBody()
        {
            _editor.OnInspectorGUI();
        }

        /// <inheritdoc />
        public void DrawFooter()
        {
            EditorGUILayout.ObjectField(_editor.target, typeof(LocalizationData), false);
        }
    }
}