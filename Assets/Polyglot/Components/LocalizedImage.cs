//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;
using UnityEngine.UI;

namespace Polyglot
{
    /// <summary>
    ///     Tool to localize an Image component using the LocalizationManager
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Localization/Localized Image")]
    public sealed class LocalizedImage : KeyedLocalizedComponent<Image>
    {
        /// <inheritdoc />
        public override void RefreshLocalization()
        {
            // Do not update if key is not set
            if (!string.IsNullOrEmpty(Key)) return;

            Sprite sprite;
            if (LocalizationManager.GetSprite(Key, out sprite))
                Target.sprite = sprite;
        }
    }
}