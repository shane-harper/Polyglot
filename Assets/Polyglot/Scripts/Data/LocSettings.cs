//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot.Data
{
    /// <summary>
    ///     Resource file for localization settings
    /// </summary>
    [HelpURL("https://github.com/shane-harper/Polyglot")]
    public class LocSettings : ScriptableObject
    {
        [SerializeField] private int[] _defaults;
        [SerializeField] private string[] _languages;

        /// <summary>
        ///     Array containing available languages
        /// </summary>
        public string[] Languages
        {
            get { return _languages; }
        }

        /// <summary>
        ///     Get loc based on system language, default to unknown if not set
        /// </summary>
        public int GetDefaultLanguage(SystemLanguage systemLanguage)
        {
            var index = _defaults[(int) systemLanguage];
            if (index < 0) index = _defaults[(int) SystemLanguage.Unknown];
            return index;
        }
    }
}