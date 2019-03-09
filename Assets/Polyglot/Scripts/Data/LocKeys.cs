//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Contains localization keys
    /// </summary>
    [HelpURL("https://github.com/shane-harper/Polyglot")]
    public class LocKeys : ScriptableObject
    {
        [SerializeField] private string[] _stringKeys;

        /// <summary>
        ///     Array containing keys for localized strings
        /// </summary>
        public string[] Strings
        {
            get { return _stringKeys; }
        }
    }
}