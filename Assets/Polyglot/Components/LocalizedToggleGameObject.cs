//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;

namespace Polyglot
{
    /// <summary>
    ///     Tool to toggle a GameObject on/off depending on the selected localization
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Localization/Localized Toggle (GameObject)")]
    public sealed class LocalizedToggleGameObject : LocalizedToggle<GameObject>
    {
        protected override void ToggleObject(bool enable)
        {
            if (Target != null) Target.SetActive(enable);
        }
    }
}