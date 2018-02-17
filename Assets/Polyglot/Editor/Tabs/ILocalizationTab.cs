//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

namespace Polyglot.Editor.Tabs
{
    internal interface ILocalizationTab
    {
        /// <summary>
        ///     Draw tab body (in scroll area)
        /// </summary>
        void DrawBody();

        /// <summary>
        ///     Draw tab footer
        /// </summary>
        void DrawFooter();
    }
}