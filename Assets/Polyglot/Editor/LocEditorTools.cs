//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using System.IO;
using System.Linq;
using Polyglot.Components;
using Polyglot.Data;
using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Editor only tools use for Polyglot Localization
    /// </summary>
    public static class LocEditorTools
    {
        public const string Menu = "Edit/Polyglot";
        public const string Folder = "Assets/Polyglot";
        public const string LanguageSubFolder = "Languages";
        private const string StreamingAssetsFolder = "Assets/StreamingAssets";
        
        /// <summary>
        ///     Selects the settings file in the inspector
        /// </summary>
        [MenuItem(Menu + "/Show Settings", false, 102)]
        public static void ShowSettings()
        {
            var settings = Resources.Load<LocSettings>(LocManager.SettingsName);
            if (settings != null) Selection.activeObject = settings;
        }
        
        [MenuItem(Menu +"/Show Settings", true)]
        private static bool ValidateShowSettings()
        {
            return Resources.Load<LocSettings>(LocManager.SettingsName);
        }
        
        #if !POLYGLOT_ADDRESSABLES
        /// <summary>
        ///     Rebuild streaming assets for current build target
        /// </summary>
        [MenuItem(Menu + "/Build Streaming Assets", false, 3)]
        public static void BuildStreamingAssets()
        {
            BuildStreamingAssets(EditorUserBuildSettings.activeBuildTarget);
        }

        /// <summary>
        ///     Rebuild streaming assets
        /// </summary>
        /// <param name="buildTarget">Target build target</param>
        public static void BuildStreamingAssets(BuildTarget buildTarget)
        {
            const string subFolder = LocManager.StreamingAssetsFolder;

            // Delete any existing streaming assets
            var path = string.Format("{0}/{1}", StreamingAssetsFolder, subFolder);
            if (Directory.Exists(path)) Directory.Delete(path, true);

            // Ensure languages folder has correct asset bundle labels
            var languageFolder = string.Format("{0}/{1}", Folder, LanguageSubFolder);
            var folder = AssetImporter.GetAtPath(languageFolder);
            folder.assetBundleName = LocManager.BundleName;

            // Gather build information
            var assetBundleName = folder.assetBundleName;
            var assetBundleVariant = folder.assetBundleVariant;
            var assetBundleFullName = string.IsNullOrEmpty(assetBundleVariant)
                ? assetBundleName
                : assetBundleName + "." + assetBundleVariant;
            var builds = new[]
            {
                new AssetBundleBuild
                {
                    assetBundleName = assetBundleName,
                    assetBundleVariant = assetBundleVariant,
                    assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleFullName)
                }
            };

            // Make sure target folder exists
            Directory.CreateDirectory(path);

            // Generate new asset bundles
            BuildPipeline.BuildAssetBundles(StreamingAssetsFolder, builds,
                BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
            AssetDatabase.ImportAsset(path);
        }
        #endif

        /// <summary>
        ///     Returns the project path to a localization asset
        /// </summary>
        /// <param name="name">Asset name</param>
        /// <returns></returns>
        public static string GetAssetPath(string name)
        {
            return string.Format("{0}/{1}/{2}.asset", Folder, LanguageSubFolder, name);
        }

        /// <summary>
        ///     Generic method to create a scriptable object and save it
        /// </summary>
        /// <param name="path">Path fo save the asset</param>
        /// <typeparam name="T">Type of object to create</typeparam>
        /// <returns>Returns new instance</returns>
        public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            // Ensure directory exists
            var directory = path.Substring(0, path.LastIndexOf('/'));
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            
            // Create asset
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }

        /// <summary>
        ///     Apply modifier properties and set editor as dirty
        /// </summary>
        public static void Save(this UnityEditor.Editor editor, bool allowUndo = true)
        {
            if (allowUndo) editor.serializedObject.ApplyModifiedProperties();
            else editor.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(editor.target);
        }

        /// <summary>
        ///     Get array of properties from a serialized property array
        /// </summary>
        public static SerializedProperty[] GetAllElements(this SerializedProperty array)
        {
            var length = array.arraySize;
            var result = new SerializedProperty[length];
            for (var i = 0; i < length; ++i)
                result[i] = array.GetArrayElementAtIndex(i);
            return result;
        }

        public static void LoadPreview(int languageIndex)
        {
            // Validate language index and get language name
            var settings = Resources.Load<LocSettings>(LocManager.SettingsName);
            languageIndex = Mathf.Clamp(languageIndex, 0, settings.Languages.Length - 1);
            var languageName = settings.Languages[languageIndex];
            if (Selection.activeObject != settings) Resources.UnloadAsset(settings);
            
            var keysPath = GetAssetPath(LocManager.KeysName);
            var keys = AssetDatabase.LoadAssetAtPath<LocKeys>(keysPath).Strings;
            var languagePath = GetAssetPath(languageName);
            var values = AssetDatabase.LoadAssetAtPath<LocLanguage>(languagePath).Strings;
                        
            var strings = LocManager.CreateDictionary(keys, values);
            LocManager.SetEditorPreview(languageIndex, strings);
                        
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

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            LoadPreview(LocManager.LastLoc);
        }
    }

    /// <summary>
    ///     Editor GUI popup for searching localization keys
    /// </summary>
    public class KeySearchPopup : PopupWindowContent
    {
        public delegate void SelectionHandler(string selection);

        private const string SearchControlName = "Search";
        private static string _search = "";
        
        private readonly string[] _options;
        private readonly GUIStyle _searchButtonStyle;
        private readonly GUIStyle _searchFieldStyle;
        private readonly SelectionHandler _selectionHandler;
        private readonly GUIStyle _selectionStyle;
        
        private bool _initialized;
        private string _manualInput;
        private Vector2 _scroll = Vector2.zero;

        public KeySearchPopup(string currentValue, string[] options, SelectionHandler selectionHandler)
        {
            _manualInput = currentValue;
            _selectionHandler = selectionHandler;
            _options = options;

            // Create/get styles
            var selectionStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                normal = {background = null}
            };
            _selectionStyle = selectionStyle;
            _searchFieldStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
            _searchButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
        }

        public override void OnGUI(Rect rect)
        {
            if (_options != null)
            {
                // Draw search bar
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUI.SetNextControlName(SearchControlName);
                _search = GUILayout.TextField(_search, _searchFieldStyle);
                if (GUILayout.Button("", _searchButtonStyle)) _search = "";
                EditorGUILayout.EndHorizontal();

                // Show list of options
                _scroll = EditorGUILayout.BeginScrollView(_scroll);
                var useFilter = !string.IsNullOrEmpty(_search);
                foreach (var option in _options)
                {
                    if (useFilter && !option.ToLower().Contains(_search.ToLower())) continue;
                    if (GUILayout.Button(option, _selectionStyle))
                        Select(option);
                }

                EditorGUILayout.EndScrollView();
            }

            // Draw manual input
            EditorGUILayout.BeginHorizontal();
            _manualInput = EditorGUILayout.TextField(_manualInput);
            if (GUILayout.Button("OK", EditorStyles.miniButton, GUILayout.ExpandWidth(false))
                || Event.current.keyCode == KeyCode.Return)
                Select(_manualInput);

            EditorGUILayout.EndHorizontal();
            
            if (Event.current.keyCode == KeyCode.Escape)
                editorWindow.Close();    

            // This is inelegant, but haven't found a better solution yet
            if (_initialized) return;
            EditorGUI.FocusTextInControl(SearchControlName);
            _initialized = true;
        }

        private void Select(string option)
        {
            if (_selectionHandler != null) _selectionHandler(option);
            editorWindow.Close();
        }
    }

    /// <summary>
    ///     Property drawer for KeySearch attribute
    /// </summary>
    [CustomPropertyDrawer(typeof(KeySearchAttribute))]
    public class StringSearchDrawer : PropertyDrawer
    {
        private static readonly Color Missing = new Color(1, 0.55f, 0.5f);
        private bool _requireRefresh = true;
        private bool _found;
        
        private static bool RefreshFoundState(string value)
        {
            // If string is null, it's not set
            if (string.IsNullOrEmpty(value)) return false;;
            
            var path = LocEditorTools.GetAssetPath(LocManager.KeysName);
            var keys = AssetDatabase.LoadAssetAtPath<LocKeys>(path);
            return keys != null && keys.Strings.Contains(value);
        }
        
        public override void OnGUI(Rect position, SerializedProperty key, GUIContent label)
        {
            if (_requireRefresh)
            {
                _found = RefreshFoundState(key.stringValue);
                _requireRefresh = false;
            }
            
            EditorGUI.BeginProperty(position, label, key);
            
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            // Change color of selection is missing
            var previousColor = GUI.backgroundColor;
            if (!_found) GUI.backgroundColor = Missing;
            
            // Draw dropdown
            if (GUI.Button(position, key.stringValue, EditorStyles.popup))
            {
                var path = LocEditorTools.GetAssetPath(LocManager.KeysName);
                var keys = AssetDatabase.LoadAssetAtPath<LocKeys>(path);

                // Show popup
                var popup = new KeySearchPopup(key.stringValue, keys.Strings, selection =>
                {
                    key.stringValue = selection;
                    key.serializedObject.ApplyModifiedProperties();
                    _requireRefresh = true;
                });
                PopupWindow.Show(position, popup);
            }
            
            GUI.backgroundColor = previousColor;
            EditorGUI.EndProperty();
        }
    }
}