//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using Polyglot.Editor.Tabs;
using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Editor window for handling localization data
    /// </summary>
    public sealed class LocalizationWindow : EditorWindow
    {
        private static readonly string[] TabNames = {"Summary", "Data"};

        private LocalizationDataEditor _dataEditor;
        private Vector2 _scroll = Vector2.zero;

        [MenuItem("Window/Localization")]
        public static LocalizationWindow CreateWindow()
        {
            // Get/Create window
            var window = GetWindow<LocalizationWindow>();
            window.titleContent = new GUIContent("Localization");
            window.Show();

            return window;
        }

        private void OnGUI()
        {
            // If data is null, show button to create it
            if (_dataEditor == null || _dataEditor.target == null)
            {
                EditorGUILayout.HelpBox("No Localization Data file found", MessageType.Info);
                if (GUILayout.Button("Create", GUILayout.Height(30)))
                {
                    LocalizationFileManager.CreateNewData();
                    OnEnable();
                }
                if (GUILayout.Button("Import Data"))
                    if (LocalizationImportExport.ImportData())
                        OnEnable();
                return;
            }

            // Draw tab bar
            var tab = GUILayout.Toolbar(_tabIndex, TabNames);
            if (tab != _tabIndex) SelectTab(tab);
            if (_tab == null) return;

            // Draw tab body
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _tab.DrawBody();
            EditorGUILayout.EndScrollView();

            // Draw tab footer
            _tab.DrawFooter();
        }

        private void OnEnable()
        {
            // Get asset data
            var asset = LocalizationFileManager.LoadData();
            if (asset == null) return;

            // Create editor
            _dataEditor = UnityEditor.Editor.CreateEditor(asset) as LocalizationDataEditor;

            // Load previous tab from preferences
            var tab = EditorPrefs.GetInt(SelectedTabPref, 0);
            SelectTab(tab);

            // Refresh event handler
            LocalizationImportExport.OnDataImport -= OnEnable;
            LocalizationImportExport.OnDataImport += OnEnable;
        }

        #region Tabs

        private const string SelectedTabPref = "Localization_SelectedTab";

        private static int _tabIndex = -1;
        private ILocalizationTab _tab;

        public void SelectTab(int index)
        {
            // Get tab from selected index
            ILocalizationTab tab = null;
            switch (index)
            {
                case 0:
                    tab = new LocalizationSummaryTab(_dataEditor);
                    break;
                case 1:
                    tab = new LocalizationDataTab(_dataEditor);
                    break;
            }

            // Validate new tab
            if (tab == null) return;

            // Update selected tab index
            _tabIndex = index;
            EditorPrefs.SetInt(SelectedTabPref, index);

            // Set selected tab
            _tab = tab;
        }

        #endregion
    }
}