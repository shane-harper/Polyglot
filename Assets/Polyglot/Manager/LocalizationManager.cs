//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Manager script for Localization. Used to load localizations and retrieve localization values
    /// </summary>
    /// <remarks>This will typically be the only script that is accessed at run time</remarks>
    public static class LocalizationManager
    {
        public delegate void LocalizationChangeHandler(int index);

        private const string LastLocalizationPref = "Localization_LastLocalization";
        public const string ResourcePath = "Polyglot/LocalizationData";

        private static Dictionary<string, Sprite> _sprites;
        private static Dictionary<string, string> _strings;
        private static Dictionary<string, AudioClip> _sounds;
        private static Dictionary<Font, Font> _fonts;

        /// <summary>
        ///     Returns true if localization values have been loaded
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        ///     Index of the current localization
        /// </summary>
        public static int LoadedIndex { get; private set; }

        /// <summary>
        ///     Name of the current localization
        /// </summary>
        public static string LoadedName { get; private set; }

        /// <summary>
        ///     Change localization
        /// </summary>
        /// <param name="index">Index as found in the LocalizationData</param>
        /// <returns>Returns false if no localization data is found</returns>
        public static bool SetLocalization(int index)
        {
            // Load data
            var data = Resources.Load<LocalizationData>(ResourcePath);
            if (data == null)
            {
                Debug.LogErrorFormat("Polyglot: Failed to set Localization. Could not find localization data resource at {0}", ResourcePath);
                return false;
            }
            
            if (data.Length <= 0)
            {
                Debug.LogError("Polyglot: Failed to set Localization. Localization data is empty");
                return false;
            }

            // Ensure index is in range and save setting
            index = ValidateIndex(data, index);
            PlayerPrefs.SetInt(LastLocalizationPref, index);

            // Update loaded values
            LoadedIndex = index;
            LoadedName = data.GetName(index);

            // Build dictionaries
            _strings = data.GetStrings(index);
            _sprites = data.GetSprites(index);
            _sounds = data.GetAudioClips(index);
            _fonts = data.GetFonts(index);

            // Trigger event
            IsInitialized = true;
            if (OnLocalizationChanged != null) OnLocalizationChanged(LoadedIndex);
            return true;
        }

        private static int ValidateIndex(LocalizationData data, int index)
        {
            // If index is out of range, try and load previous setting
            if (index < 0 || index >= data.Length)
            {
                if (index >= data.Length)
                    Debug.LogErrorFormat("Polyglot: Requested localization {0} is out of range.", index);
                
                // Load previous setting
                index = PlayerPrefs.GetInt(LastLocalizationPref, data.DefaultLocalization);

                // Check that too as it may no longer exist
                if (index < 0 || index >= data.Length)
                {
                    Debug.LogWarning("Polyglot: Saved localization index is out of range");
                    index = Mathf.Clamp(data.DefaultLocalization, 0, data.Length);
                }
            }
            return index;
        }

        /// <summary>
        ///     Get localized string for the loaded localization
        /// </summary>
        public static string GetString(string key)
        {
            if (string.IsNullOrEmpty(key)) 
                throw new ArgumentNullException("key", "Polyglot: Failed to get string. Key is null");

            string value;
            if (_strings.TryGetValue(key, out value)) return value;
            throw new KeyNotFoundException("Polyglot: Failed to get string. No entry found with key " + key);
        }
        
        /// <summary>
        ///     Get localized string for the loaded localization
        /// </summary>
        /// <returns>Returns false if key was not found</returns>
        public static bool TryGetString(string key, out string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Polyglot: Failed to get string. Key is null");
                value = string.Empty;
                return false;
            }
            
            if (IsInitialized || SetLocalization(-1)) return _strings.TryGetValue(key, out value);
            Debug.LogErrorFormat("Polyglot: Failed to get string. No entry found with key {0}", key);
            value = string.Empty;
            return false;
        }
        
        /// <summary>
        ///     Get localized sprite for the loaded localization
        /// </summary>
        public static Sprite GetSprite(string key)
        {
            if (string.IsNullOrEmpty(key)) 
                throw new ArgumentNullException("key", "Polyglot: Failed to get sprite. Key is null");

            Sprite value;
            if (_sprites.TryGetValue(key, out value)) return value;
            throw new KeyNotFoundException("Polyglot: Failed to get sprite. No entry found with key " + key);
        }
        
        /// <summary>
        ///     Get localized Sprite for the loaded localization
        /// </summary>
        /// <returns>Returns false if key was not found</returns>
        public static bool TryGetSprite(string key, out Sprite sprite)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Polyglot: Failed to get sprite. Key is null");
                sprite = null;
                return false;
            }

            if (IsInitialized || SetLocalization(-1)) return _sprites.TryGetValue(key, out sprite);
            Debug.LogErrorFormat("Polyglot: Failed to get sprite. No entry found with key {0}", key);
            sprite = null;
            return false;
        }
        
        /// <summary>
        ///     Get localized AudioClip for the loaded localization
        /// </summary>
        public static AudioClip GetAudioClip(string key)
        {
            if (string.IsNullOrEmpty(key)) 
                throw new ArgumentNullException("key", "Polyglot: Failed to get audio clip. Key is null");

            AudioClip value;
            if (_sounds.TryGetValue(key, out value)) return value;
            throw new KeyNotFoundException("Polyglot: Failed to get audio clip. No entry found with key " + key);
        }

        /// <summary>
        ///     Get localized AudioClip for the loaded localization
        /// </summary>
        /// <returns>Returns false if key was not found</returns>
        public static bool TryGetAudioClip(string key, out AudioClip clip)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Polyglot: Failed to get audio clip. Key is null");
                clip = null;
                return false;
            }

            if (IsInitialized || SetLocalization(-1)) return _sounds.TryGetValue(key, out clip);
            Debug.LogErrorFormat("Polyglot: Failed to get audio clip. No entry found with key {0}", key);
            clip = null;
            return false;
        }
        
        /// <summary>
        ///     Get localized Font for the loaded localization
        /// </summary>
        public static Font GetFont(Font original)
        {
            if (original == null) 
                throw new ArgumentNullException("original", "Polyglot: Failed to get font. Key is null");

            Font value;
            if (_fonts.TryGetValue(original, out value)) return value;
            throw new KeyNotFoundException("Polyglot: Failed to get audio clip. No entry found with key " + original);
        }

        /// <summary>
        ///     Get override Font for the loaded localization
        /// </summary>
        /// <returns>Returns false if key was not found</returns>
        public static bool TryGetFont(Font original, out Font font)
        {
            if (original == null)
            {
                Debug.LogError("Polyglot: Failed to get font. Key is null");
                font = null;
                return false;
            }
            
            if (IsInitialized || SetLocalization(-1)) return _fonts.TryGetValue(original, out font);
            Debug.LogErrorFormat("Polyglot: Failed to get font. No entry found with key {0}", original);
            font = null;
            return false;
        }

        /// <summary>
        ///     Event triggered when localization is changed
        /// </summary>
        public static event LocalizationChangeHandler OnLocalizationChanged;
    }
}