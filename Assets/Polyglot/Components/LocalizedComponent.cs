//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Base Localization component class, used to refresh all components from one list
    /// </summary>
    public abstract class LocalizedComponent : MonoBehaviour
    {
        private void Start()
        {
            RefreshLocalization();
        }

        /// <summary>
        ///     Refresh component from localization
        /// </summary>
        public abstract void RefreshLocalization();

        protected virtual void OnValidate()
        {
            RefreshLocalization();
        }
    }

    /// <summary>
    ///     Localization class to avoid copy/paste when creating localized components that require a key
    /// </summary>
    public abstract class KeyedLocalizedComponent<T> : LocalizedComponent where T : Component
    {
        protected abstract string Key { get; }

        protected T Target
        {
            get { return _target; }
        }

        protected override void OnValidate()
        {
            if (_target == null) _target = GetComponent<T>();
            base.OnValidate();
        }

        #region Inspector

        [SerializeField] private T _target;

        #endregion
    }
}