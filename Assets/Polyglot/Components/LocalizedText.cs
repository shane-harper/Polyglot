//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;
using UnityEngine.UI;

namespace Polyglot
{
    /// <summary>
    ///     Tool to localize a Text component using the LocalizationManager
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("Localization/Localized Text")]
    public sealed class LocalizedText : KeyedLocalizedComponent<Text>
    {
        #region Inspector

        [SerializeField] private bool _allowFontOverride = true;

        #endregion

        /// <inheritdoc />
        public override void RefreshLocalization()
        {
            string value;
            if (LocalizationManager.GetString(Key, out value))
                Target.text = value;

            if (_allowFontOverride)
            {
                // Check for replacement font and override
                Font font;
                if (LocalizationManager.GetFont(Target.font, out font) && font != null)
                    Target.font = font;
            }
        }
    }
}