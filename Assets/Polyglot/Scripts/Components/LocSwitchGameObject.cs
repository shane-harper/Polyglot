//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot.Components
{
    /// <summary>
    ///     Set GameObject active/inactive based on current location
    /// </summary>
    [AddComponentMenu("Polyglot/Loc Switch GameObject")]
    public class LocSwitchGameObject : LocSwitch
    {
        #region Inspector

        [Tooltip("GameObjects to be enabled/disabled based on localization")]
        [SerializeField] private GameObject[] _targets;

        #endregion

        public override void RefreshLocalization()
        {
            // If no target is set, don't switch
            if (_targets == null || _targets.Length <= 0) return;

            // Check the loaded loc is within range
            var loc = LocManager.LoadedLoc;
            if (loc < 0 || loc >= Enable.Length) return;

            // Switch target game object(s)
            foreach (var target in _targets)
                target.SetActive(Enable[loc]);
        }
    }
}