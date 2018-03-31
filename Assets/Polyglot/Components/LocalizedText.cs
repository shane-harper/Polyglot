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
    [AddComponentMenu("Polyglot/Localized Text")]
    public sealed class LocalizedText : KeyedLocalizedComponent<Text>
    {
        #region Inspector

        [SerializeField, LocalizationKey(LocalizationData.Type.String)] private string _key;
        [SerializeField] private bool _allowFontOverride = true;

        #endregion

        protected override string Key
        {
            get { return _key; }
        }

        /// <inheritdoc />
        public override void RefreshLocalization()
        {
            string value;

            // Do not update if key is not set
            if (!string.IsNullOrEmpty(Key) && LocalizationManager.TryGetString(Key, out value))
                Target.text = value;

            if (_allowFontOverride && Target.font != null)
            {
                // Check for replacement font and override
                Font font;
                if (LocalizationManager.TryGetFont(Target.font, out font) && font != null)
                    Target.font = font;
            }
        }
    }
}