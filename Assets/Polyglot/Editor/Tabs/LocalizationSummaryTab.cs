//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using System.Text;
using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor.Tabs
{
    /// <summary>
    ///     Tab to show summary of localization data, including missing keys etc
    /// </summary>
    internal sealed class LocalizationSummaryTab : ILocalizationTab
    {
        private readonly SerializedProperty _defaultLocalization;
        private readonly LocalizationDataEditor _editor;
        private readonly LocalizationDropdown _localizationDropdown;
        private readonly SerializedProperty _names;

        public LocalizationSummaryTab(LocalizationDataEditor dataEditor)
        {
            _editor = dataEditor;

            // Cache serialized properties
            _names = _editor.serializedObject.FindProperty("_names");
            _defaultLocalization = _editor.serializedObject.FindProperty("_defaultLocalization");

            // Initialize default localization dropdown
            var label = new GUIContent("Default Localization",
                "Localization that will be used if no localization is set");
            _localizationDropdown = new LocalizationDropdown(label, _names);
        }

        /// <inheritdoc />
        public void DrawBody()
        {
            var localizationCount = _names.arraySize;

            // Draw keys
            GUILayout.Label("Keys", EditorStyles.boldLabel);
            var builder = new StringBuilder();
            var newLine = false;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (var summary in _editor.Properties)
            {
                EditorGUILayout.LabelField(string.Format("{0}s", summary.Name), summary.KeyCount.ToString());

                if (summary.MissingKeys > 0)
                {
                    if (newLine) builder.Append("\n");
                    builder.AppendFormat("{0} missing {1} key(s)", summary.MissingKeys,
                        summary.Name.ToLower());
                    newLine = true;
                }
            }
            EditorGUILayout.EndVertical();

            // Draw missing keys warning
            if (builder.Length > 0) EditorGUILayout.HelpBox(builder.ToString(), MessageType.Error);
            EditorGUILayout.Space();

            // Draw localizations
            GUILayout.Label("Localizations", EditorStyles.boldLabel);
            if (localizationCount <= 0) EditorGUILayout.HelpBox("No Localizations have been added", MessageType.Info);
            else
                for (var i = 0; i < localizationCount; ++i)
                    DrawLocalizationSummary(i);
        }

        /// <inheritdoc />
        public void DrawFooter()
        {
            // Draw default localization dropdown
            if (_names.arraySize > 0 && _localizationDropdown.Draw(_defaultLocalization))
                _editor.Save();

            // Draw import/export buttons
            EditorGUILayout.BeginHorizontal();
            var guiEnabled = GUI.enabled;
            if (GUILayout.Button("Import Data")) LocalizationImportExport.ImportData();
            GUI.enabled = LocalizationFileManager.AssetExists();
            if (GUILayout.Button("Export Data")) LocalizationImportExport.ExportData();
            GUI.enabled = guiEnabled;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLocalizationSummary(int index)
        {
            // Draw button to select localization
            EditorGUILayout.BeginHorizontal();
            var name = _names.GetArrayElementAtIndex(index).stringValue;
            if (string.IsNullOrEmpty(name)) name = "[Not set]";
            GUILayout.Label(name);
            var selected = index == LocalizationManager.LoadedIndex;
            var newSelected = GUILayout.Toggle(selected, "Preview", EditorStyles.miniButton,
                GUILayout.ExpandWidth(false));
            if (selected != newSelected && newSelected) SetLocalization(index);
            EditorGUILayout.EndHorizontal();

            // Draw missing values
            var builder = new StringBuilder();
            var newLine = false;
            var severity = MessageType.None;
            foreach (var summary in _editor.Properties)
            {
                var missingValues = summary.MissingValues[index];
                if (missingValues > 0)
                {
                    if (newLine) builder.Append("\n");
                    builder.AppendFormat("{0} missing {1} value(s)", missingValues, summary.Name.ToLower());
                    newLine = true;

                    severity = (MessageType) Mathf.Max((int) severity, (int) summary.MissingValuesSeverity);
                }
            }
            if (builder.Length > 0) EditorGUILayout.HelpBox(builder.ToString(), severity);
        }

        private static void SetLocalization(int index)
        {
            if (!LocalizationManager.SetLocalization(index)) return;

            // Refresh localization on all loaded assets
            var localizedComponents = Resources.FindObjectsOfTypeAll<LocalizedComponent>();
            foreach (var component in localizedComponents)
            {
                component.RefreshLocalization();
                EditorUtility.SetDirty(component);
            }

            // Force repaint
            SceneView.RepaintAll();
        }
    }
}