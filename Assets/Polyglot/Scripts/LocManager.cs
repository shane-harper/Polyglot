﻿//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using System;
using System.Collections;
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

        public static int LastLoc
        {
            get { return PlayerPrefs.GetInt(LastLocPrefKey, -1); }
            private set { PlayerPrefs.SetInt(LastLocPrefKey, value); }
        }
        
        private static int _loadedLoc = -1;
        private static Dictionary<string, string> _strings;

        /// <summary>
        ///     Returns true if the LocManager has already been initialized
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        ///     Load localization to last set language index Async
        /// </summary>
        /// <returns></returns>
        public static IEnumerator LoadLocalizationAsync()
        {
            return LoadLocalizationAsync(LastLoc);
        }
        
        /// <summary>
        ///     Load localization language index Async
        /// </summary>
        public static IEnumerator LoadLocalizationAsync(int index)
        {
            var languageName = GetLanguageName(ref index);
            using (var request = new LocLoadRequest(SetLocalizationSuccessHandler, SetLocalizationErrorHandler))
            {
                yield return request.GetAsync(index, languageName);
            }
        }
        
        private static void SetLocalizationSuccessHandler(int index, LocKeys locKeys, LocLanguage language) 
        {
            _strings = CreateDictionary(locKeys.Strings, language.Strings);
            _loadedLoc = index;
            LastLoc = index;
                
            IsInitialized = true;
            if (OnLocChanged != null) OnLocChanged();
        }

        private static void SetLocalizationErrorHandler(LocLoadRequest.ErrorCode error)
        {
            Debug.LogErrorFormat("{0} Failed to set localization: {1}", LogHeader, error);
        }

        private static string GetLanguageName(ref int index)
        {
            var settings = Resources.Load<LocSettings>(SettingsName);

            // Validate language index, use default if out of range
            if (index < 0 || index >= settings.Languages.Length)
            {
                var newIndex = settings.GetDefaultLanguage(Application.systemLanguage);
                Debug.LogFormat("{0} selected language index is out of range ({1}) using default: {2}", LogHeader,
                    index, newIndex);
                index = newIndex;
            }

            // Get name and unload settings when done
            var name = settings.Languages[index];
            Resources.UnloadAsset(settings);
            return name;
        }

        public static Dictionary<T1, T2> CreateDictionary<T1, T2>(IList<T1> keys, IList<T2> values)
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
            if (Application.isPlaying) throw new Exception(LogHeader + " Loc Manager not initialized");
            
            value = null;
            return false;
        }

#if UNITY_EDITOR
        /// <summary>
        ///     (Editor Only) Method to set preview data. Use SetLocalization in application
        /// </summary>
        public static void SetEditorPreview(int index, Dictionary<string, string> strings)
        {
            _loadedLoc = index;
            _strings = strings;
            IsInitialized = true;
            LastLoc = index;
        }
#endif
    }
}