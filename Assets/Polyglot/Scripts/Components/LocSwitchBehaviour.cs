//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot.Components
{
    /// <summary>
    ///     Enable/disable Behaviours based on current localization
    /// </summary>
    [AddComponentMenu("Polyglot/Loc Switch Behaviour")]
    public class LocSwitchBehaviour : LocSwitch
    {
        #region Inspector

        [Tooltip("Behaviours to be enabled/disabled based on localization")]
        [SerializeField] private Behaviour[] _targets;

        #endregion

        /// <inheritdoc />
        public override void RefreshLocalization()
        {
            // If no target is set, don't switch
            if (_targets == null || _targets.Length <= 0) return;

            // Check the loaded loc is within range
            var loc = LocManager.LoadedLoc;
            if (loc < 0 || loc >= Enable.Length) return;

            // Switch target Behaviour(s)
            foreach (var target in _targets)
                target.enabled = Enable[loc];
        }
    }
}