//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using System.Collections.Generic;
using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Object containing ALL localization data
    /// </summary>
    public sealed class LocalizationData : ScriptableObject
    {
        /// <summary>
        ///     Returns the number of localizations
        /// </summary>
        public int Length
        {
            get { return _names.Length; }
        }

        /// <summary>
        ///     Localization to default to if no localization is loaded
        /// </summary>
        public int DefaultLocalization
        {
            get { return _defaultLocalization; }
        }

        /// <summary>
        ///     Get localization name
        /// </summary>
        /// <param name="index">Index of localization</param>
        public string GetName(int index)
        {
            // Check that names array exists
            if (_names == null || _names.Length == 0) return string.Empty;

            // Check that index is within range
            if (index < -1 || index >= _names.Length) return string.Empty;

            return _names[index];
        }

        internal Dictionary<string, string> GetStrings(int index)
        {
            return BuildDictionary(_strings, index);
        }

        internal Dictionary<string, Sprite> GetSprites(int index)
        {
            return BuildDictionary(_sprites, index);
        }

        internal Dictionary<string, AudioClip> GetAudioClips(int index)
        {
            return BuildDictionary(_sounds, index);
        }

        internal Dictionary<Font, Font> GetFonts(int index)
        {
            return BuildDictionary(_fonts, index);
        }

        /// <summary>
        ///     Builds a dictionary for use in the LocalizationManager
        /// </summary>
        /// <param name="array">Array to extract data from</param>
        /// <param name="index">Index of localization to include</param>
        private static Dictionary<T1, T2> BuildDictionary<T1, T2>(SerializableArray<T1, T2>[] array, int index)
        {
            // If array is null, return empty dictionary
            if (array == null) return new Dictionary<T1, T2>(0);

            // Create dictionary of values
            var count = array.Length;
            var dictionary = new Dictionary<T1, T2>(count);

            // Populate dictionary
            for (var i = 0; i < count; ++i)
            {
                var entry = array[i];
                var value = index >= entry.Values.Length ? default(T2) : entry.Values[index];
                dictionary[entry.Key] = value;
            }

            return dictionary;
        }

        #region Inspector

        [SerializeField] private int _defaultLocalization;
        [SerializeField] private string[] _names;
        [SerializeField] private SerializableStringArray[] _strings;
        [SerializeField] private SerializableSpriteArray[] _sprites;
        [SerializeField] private SerializableAudioClipArray[] _sounds;
        [SerializeField] private SerializableFontArray[] _fonts;

        #endregion
    }
}