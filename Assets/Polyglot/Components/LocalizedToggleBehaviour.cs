//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Tool to toggle a Behaviour on/off depending on the selected localization
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Polyglot/Localized Toggle (Behaviour)")]
    public sealed class LocalizedToggleBehaviour : LocalizedToggle<Behaviour>
    {
        protected override void ToggleObject(bool enable)
        {
            if (Target != null) Target.enabled = enable;
        }
    }
}