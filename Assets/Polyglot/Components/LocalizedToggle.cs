//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Tool to toggle an Object on/off depending on the selected localization
    /// </summary>
    public abstract class LocalizedToggle<T> : LocalizedComponent
    {
        protected T Target
        {
            get { return _target; }
        }

        /// <inheritdoc />
        public sealed override void RefreshLocalization()
        {
            // Check if target has been set
            if (Target == null) return;

            // Validate loaded config against array
            var currentLocalization = LocalizationManager.LoadedIndex;
            if (currentLocalization < 0 || currentLocalization >= EnabledLocalizations.Length) return;

            // Toggle target
            ToggleObject(EnabledLocalizations[currentLocalization]);
        }

        protected abstract void ToggleObject(bool enable);

        #region Inspector

        [SerializeField] protected bool[] EnabledLocalizations;
        [SerializeField] private T _target;

        #endregion
    }
}