//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot.Components
{
    [HelpURL("https://github.com/shane-harper/Polyglot")]
    public abstract class LocComponent : MonoBehaviour
    {
        public enum Axis
        {
            None,
            Horizontal,
            Vertical,
            Both
        }

        public enum TextCase
        {
            NoChange,
            ToUpper,
            ToLower
        }

        protected virtual void OnEnable()
        {
            LocManager.OnLocChanged += RefreshLocalization;
            if (LocManager.IsInitialized) RefreshLocalization();
        }

        protected virtual void OnDisable()
        {
            LocManager.OnLocChanged -= RefreshLocalization;
        }

        /// <summary>
        ///     Refresh component using currently loaded localization
        /// </summary>
        public abstract void RefreshLocalization();

        protected virtual void OnValidate()
        {
            RefreshLocalization();
        }
    }

    public abstract class LocComponentKeyed : LocComponent
    {
        #region Inspector

        [Tooltip("Key used to retrieve localized value")] [KeySearch] [SerializeField]
        private string _key;

        #endregion

        /// <summary>
        ///     Key used to retrieve localized value
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        ///     Update the localization key of this asset at runtime. Useful for dynamic text content
        /// </summary>
        public void SetLocalizationKey(string key)
        {
            _key = key;
            RefreshLocalization();
        }
    }

    public abstract class LocSwitch : LocComponent
    {
        #region Inspector

        [SerializeField] private bool[] _enable;

        #endregion

        protected bool[] Enable
        {
            get { return _enable; }
        }
    }

    public class KeySearchAttribute : PropertyAttribute
    {
    }
}