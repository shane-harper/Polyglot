//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

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
        public const string ResourcePath = "Localization/LocalizationData";

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
            if (data == null || data.Length <= 0) return false;

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
                // Load previous setting
                index = PlayerPrefs.GetInt(LastLocalizationPref, data.DefaultLocalization);

                // Check that too as it may no longer exist
                if (index < 0 || index >= data.Length)
                    index = Mathf.Clamp(data.DefaultLocalization, 0, data.Length);
            }
            return index;
        }

        /// <summary>
        ///     Get localized string for the loaded localization
        /// </summary>
        /// <returns>Returns false if key was not found</returns>
        public static bool GetString(string key, out string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                value = key;
                return false;
            }
            
            if (IsInitialized || SetLocalization(-1)) return _strings.TryGetValue(key, out value);
            value = string.Empty;
            return false;
        }

        /// <summary>
        ///     Get localized Sprite for the loaded localization
        /// </summary>
        /// <returns>Returns false if key was not found</returns>
        public static bool GetSprite(string key, out Sprite sprite)
        {
            if (string.IsNullOrEmpty(key))
            {
                sprite = null;
                return false;
            }

            if (IsInitialized || SetLocalization(-1)) return _sprites.TryGetValue(key, out sprite);
            sprite = null;
            return false;
        }

        /// <summary>
        ///     Get localized AudioClip for the loaded localization
        /// </summary>
        /// <returns>Returns false if key was not found</returns>
        public static bool GetAudioClip(string key, out AudioClip clip)
        {
            if (string.IsNullOrEmpty(key))
            {
                clip = null;
                return false;
            }

            if (IsInitialized || SetLocalization(-1)) return _sounds.TryGetValue(key, out clip);
            clip = null;
            return false;
        }

        /// <summary>
        ///     Get override Font for the loaded localization
        /// </summary>
        /// <returns>Returns false if key was not found</returns>
        public static bool GetFont(Font original, out Font font)
        {
            if (original == null)
            {
                font = original;
                return false;
            }
            
            if (IsInitialized || SetLocalization(-1)) return _fonts.TryGetValue(original, out font);
            font = null;
            return false;
        }

        /// <summary>
        ///     Event triggered when localization is changed
        /// </summary>
        public static event LocalizationChangeHandler OnLocalizationChanged;
    }
}