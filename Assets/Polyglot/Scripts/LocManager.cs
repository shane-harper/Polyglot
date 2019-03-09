//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using System;
using System.Collections.Generic;
using Polyglot.Data;
using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Core manager for Polyglot Localization
    /// </summary>
    public static class LocManager
    {
        public delegate void LocChangeHandler();
        public static event LocChangeHandler OnLocChanged;
        
        public const string StreamingAssetsFolder = "polyglot";
        public const string BundleName = StreamingAssetsFolder + "/localization";
        public const string SettingsName = "Polyglot";
        public const string KeysName = "PolyglotKeys";
        public const string LastLocPrefKey = "Polyglot_LastLoc";
        private const string LogHeader = "[Polyglot]";
        
        /// <summary>
        ///     Index of currently loaded localization
        /// </summary>
        public static int LoadedLoc
        {
            get { return _loadedLoc; }
        }

        private static bool _isInitialized;
        private static int _loadedLoc = -1;
        private static LocSettings _settings;

        private static Dictionary<string, string> _strings;

        /// <summary>
        ///     Returns true if the LocManager has already been initialized
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying && !_isInitialized) Initialize();
                #endif
                return _isInitialized;
            }
        }
        
        public static void Initialize()
        {
            if (_isInitialized) return;

            _settings = Resources.Load<LocSettings>(SettingsName);
            var index = PlayerPrefs.GetInt(LastLocPrefKey, -1);
            SetLocalization(index);
        }

        public static void SetLocalization(int index)
        {
            if (_settings == null) _settings = Resources.Load<LocSettings>(SettingsName);
            
            // Load asset bundle
            var path = string.Format("{0}/{1}", Application.streamingAssetsPath, BundleName);
            var bundle = AssetBundle.LoadFromFile(path);
            if (bundle == null)
            {
                Debug.LogErrorFormat("{0} Could not find bundle at path {1}", LogHeader, path);
                return;
            }

            // Get localization data from bundle
            var keys = bundle.LoadAsset<LocKeys>(KeysName);
            if (keys != null)
            {
                // Make sure id is not null, attempt to revert to default if it is (first run)
                if (index < 0) index = _settings.GetDefaultLanguage(Application.systemLanguage);

                // Validate id index
                if (index >= 0 && index < _settings.Languages.Length)
                {
                    // Read localization from bundle
                    var languageName = _settings.Languages[index];
                    var language = bundle.LoadAsset<LocLanguage>(languageName);
                    if (language != null)
                    {
                        // Load strings from language
                        _strings = CreateDictionary(keys.Strings, language.Strings);

                        // Flag LocManager as initialized
                        _loadedLoc = index;
                        _isInitialized = true;

                        // Save current setting
                        PlayerPrefs.SetInt(LastLocPrefKey, index);

                        // Trigger events
                        if (OnLocChanged != null) OnLocChanged();
                    }
                }
                else
                {
                    Debug.LogErrorFormat("{0} Could not load localization, index out of range: {1}", LogHeader, index);
                }
            }
            else
            {
                Debug.LogErrorFormat("{0} Could not find keys asset in bundle", LogHeader);
            }
                
            // Always unload bundle when done
            bundle.Unload(true);
        }

        private static Dictionary<T1, T2> CreateDictionary<T1, T2>(IList<T1> keys, IList<T2> values)
        {
            // Create dictionary
            var length = Mathf.Min(keys.Count, values.Count);
            var dictionary = new Dictionary<T1, T2>(length);

            // Populate it
            for (var i = 0; i < length; ++i)
                dictionary.Add(keys[i], values[i]);
            return dictionary;
        }

        public static string[] GetLanguages()
        {
            var settings = Resources.Load<LocSettings>(SettingsName);
            return settings.Languages;
        }

        public static string GetString(string key)
        {
            string value;
            if (TryGetString(key, out value)) return value;
            throw new ArgumentOutOfRangeException("key", key, "Not found");
        }

        public static bool TryGetString(string key, out string value)
        {
            if (IsInitialized) return _strings.TryGetValue(key, out value);
            throw new Exception(LogHeader + " Loc Manager not initialized");
        }
    }
}