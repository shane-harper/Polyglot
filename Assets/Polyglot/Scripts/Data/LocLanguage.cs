//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot.Data
{
    /// <summary>
    ///     Contains data required for a single localization
    /// </summary>
    [HelpURL("https://github.com/shane-harper/Polyglot")]
    public class LocLanguage : ScriptableObject
    {
        [SerializeField] private string[] _strings;

        /// <summary>
        ///     Strings associated with this localization
        /// </summary>
        public string[] Strings
        {
            get { return _strings; }
        }
    }
}