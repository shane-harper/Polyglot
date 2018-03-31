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
    [AddComponentMenu("Polyglot/Localized Audio Source")]
    public sealed class LocalizedAudioSource : KeyedLocalizedComponent<AudioSource>
    {
        #region Inspector

        [SerializeField, LocalizationKey(LocalizationData.Type.Sound)] private string _key;
    
        #endregion
        
        protected override string Key
        {
            get { return _key; }
        }
        
        /// <inheritdoc />
        public override void RefreshLocalization()
        {
            // Do not update if key is not set
            if (string.IsNullOrEmpty(Key)) return;

            AudioClip clip;
            if (!LocalizationManager.GetAudioClip(Key, out clip)) return;
            Target.clip = clip;

            // Re-play audio source with new AudioClip
            if (Target.playOnAwake) Target.Play();
        }
    }
}