//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Tool to localize an AudioSource component using the LocalizationManager
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Localization/Localized Audio Source")]
    public sealed class LocalizedAudioSource : KeyedLocalizedComponent<AudioSource>
    {
        /// <inheritdoc />
        public override void RefreshLocalization()
        {
            AudioClip clip;
            if (!LocalizationManager.GetAudioClip(Key, out clip)) return;
            Target.clip = clip;

            // Re-play audio source with new AudioClip
            if (Target.playOnAwake) Target.Play();
        }
    }
}