//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Polyglot
{
    /// <summary>
    ///     Uses a Unity Dropdown component to switch languages
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    [AddComponentMenu("Polyglot/Loc Language Select")]
    public class LocLanguageSelect : MonoBehaviour
    {
        #region Inspector

        [Tooltip("Dropdown to control selected localization")]
        [SerializeField] private Dropdown _target;

        #endregion

        public bool IsInitialized { get; private set; }
        
        /// <summary>
        ///     Dropdown used by this component
        /// </summary>
        public Dropdown Target
        {
            get { return _target; }
        }

        private void Start()
        {
            LocManager.OnLocChanged += OnLocChanged;
            if (LocManager.IsInitialized) OnLocChanged();
        }

        private void OnLocChanged()
        {
            if (IsInitialized)
            {
                // Set current selection to correct value
                _target.value = LocManager.LoadedLoc;
                return;
            }

            // Populate with all languages
            _target.ClearOptions();
            var languages = new List<string>(LocManager.GetLanguages());
            _target.AddOptions(languages);

            // Set current selection to correct value
            _target.value = LocManager.LoadedLoc;

            // Add listener to set localization when a value is selected
            _target.onValueChanged.AddListener(LocManager.SetLocalization);
            IsInitialized = true;
        }

        private void OnValidate()
        {
            // Get attached dropdown component if not set
            if (_target == null) _target = GetComponent<Dropdown>();
        }
    }
}