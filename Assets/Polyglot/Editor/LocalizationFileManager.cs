//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Class to handle the localization data file (asset)
    /// </summary>
    internal static class LocalizationFileManager
    {
        public const string AssetExtension = "asset";
        public const string CsvExtension = "csv";

        /// <summary>
        ///     Location of Resources folder containing Localization Data
        /// </summary>
        private const string ResourcesFolderLocation = "Assets/Polyglot/Resources";

        /// <summary>
        ///     Create new localization data object
        /// </summary>
        /// <remarks>Will overwrite existing objects</remarks>
        internal static LocalizationData CreateNewData()
        {
            var assetPath = GetAssetPath();

            // If asset exists, return existing
            if (AssetExists()) return LoadData();

            // Create and return new asset
            var data = ScriptableObject.CreateInstance<LocalizationData>();
            AssetDatabase.CreateAsset(data, assetPath);
            AssetDatabase.SaveAssets();
            return data;
        }

        /// <summary>
        ///     Get localization data from asset database
        /// </summary>
        /// <returns>Returns false if no asset is found</returns>
        internal static LocalizationData LoadData()
        {
            // Load existing data
            return AssetDatabase.LoadAssetAtPath<LocalizationData>(GetAssetPath());
        }

        private static string GetAssetPath()
        {
            return string.Format("{0}/{1}.{2}",
                ResourcesFolderLocation,
                LocalizationManager.ResourcePath,
                AssetExtension);
        }

        /// <summary>
        ///     Check if the localization data asset exists
        /// </summary>
        internal static bool AssetExists()
        {
            return File.Exists(GetAssetPath());
        }
    }
}