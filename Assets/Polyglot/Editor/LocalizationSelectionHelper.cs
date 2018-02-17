//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;

namespace Polyglot.Editor
{
    /// <summary>
    ///     An editor tool to reload localization data on re-compile
    /// </summary>
    [InitializeOnLoad]
    internal static class LocalizationSelectionHelper
    {
        static LocalizationSelectionHelper()
        {
            // Localization manager to try and load previous setting
            LocalizationManager.SetLocalization(-1);
            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
        }

        private static void HandleOnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                LocalizationManager.SetLocalization(-1);
        }
    }
}