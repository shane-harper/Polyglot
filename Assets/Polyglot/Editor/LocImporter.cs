//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Polyglot.Data;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Handles importing localization data from CSV
    /// </summary>
    public static class LocImporter
    {
        [MenuItem(LocEditorTools.Menu + "/Import Data...", false, 1)]
        private static void Import()
        {
            var path = EditorUtility.OpenFilePanel("Import Localization Data", null, "csv");
            if (!string.IsNullOrEmpty(path)) Import(path);
        }

        private static void Import(string path)
        {
            // Delete languages folder?
            var languagesFolder = string.Format("{0}/{1}", LocEditorTools.Folder, LocEditorTools.LanguageSubFolder);
            if (Directory.Exists(languagesFolder) && EditorUtility.DisplayDialog("Delete Previous Localization?",
                    "Would you like to remove any previous localization assets?", "Yes", "No"))
                Directory.Delete(languagesFolder, true);

            const string progressBarTitle = "Polyglot";
            float progress = 0;
            
            string[] ids;
            List<string> keys;
            List<string>[] values;
            EditorUtility.DisplayProgressBar(progressBarTitle, "Reading csv...", progress);
            using (var file = new StreamReader(path))
            {
                var line = file.ReadLine();
                if (line == null) throw new Exception("Failed to read data from file");

                // Read headers
                var headers = ParseLine(line);

                // Get localization language ids
                var idCount = headers.Length - 1;
                ids = new string[idCount];
                for (var i = 0; i < idCount; ++i)
                    ids[i] = headers[i + 1];
                    
                // Create lists to hold loc keys values
                keys = new List<string>();
                values = new List<string>[idCount];
                for (var i = 0; i < idCount; ++i)
                    values[i] = new List<string>();
                    
                // Read string localizations from file
                while ((line = file.ReadLine()) != null)
                {
                    // Ignore empty lines
                    if (string.IsNullOrEmpty(line)) continue;
                        
                    // Split line and get key
                    var split = ParseLine(line);
                    keys.Add(split[0]);

                    // Get values
                    for (var i = 1; i < split.Length; ++i)
                        values[i - 1].Add(split[i]);
                }
            }
            
            // Set keys in LocData
            EditorUtility.DisplayProgressBar(progressBarTitle, "Creating keys file...", progress += 0.2f);
            var keyPath = LocEditorTools.GetAssetPath(LocManager.KeysName);
            var keysAsset = LocEditorTools.CreateScriptableObject<LocKeys>(keyPath);
            CopyListToSerializedProperty(keys, keysAsset, "_stringKeys");
            
            // Update settings file
            var settings = Resources.Load<LocSettings>(LocManager.SettingsName);
            if (settings == null) settings = CreateSettings();
            CopyListToSerializedProperty(ids, settings, "_languages");
            
            // Create languages and populate
            const float maxLanguageProgress = 0.9f;
            for (var i = 0; i < values.Length; ++i)
            {
                EditorUtility.DisplayProgressBar(progressBarTitle, "Creating languages...",
                    progress + (float) i / values.Length * (maxLanguageProgress - progress));
                var languagePath = LocEditorTools.GetAssetPath(ids[i]);
                var language = LocEditorTools.CreateScriptableObject<LocLanguage>(languagePath);
                CopyListToSerializedProperty(values[i], language, "_strings");
            }
            
            // Rebuild asset bundles
            #if !POLYGLOT_ADDRESSABLES
            progress = maxLanguageProgress;
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var message = string.Format(
                    "Localization data import complete! Would you like to rebuild Polyglot asset bundles?\n\n({0})",
                    buildTarget);
            if (EditorUtility.DisplayDialog("Rebuild Asset Bundles", message, "Yes", "No"))
            {
                EditorUtility.DisplayProgressBar(progressBarTitle, "Rebuilding asset bundles...", progress);
                LocEditorTools.BuildStreamingAssets(buildTarget);
            }
            #endif

            // Clear progress bar
            EditorUtility.ClearProgressBar();
        }

        private static LocSettings CreateSettings()
        {
            var path = string.Format("{0}/Resources/{1}.asset", LocEditorTools.Folder, LocManager.SettingsName);
            return LocEditorTools.CreateScriptableObject<LocSettings>(path);
        }

        private static void CopyListToSerializedProperty(IList<string> list, Object target, string propertyName)
        {
            // Get property
            var editor = UnityEditor.Editor.CreateEditor(target);
            var property = editor.serializedObject.FindProperty(propertyName);
            
            // Set values
            property.arraySize = list.Count;
            for (var i = 0; i < list.Count; ++i)
                property.GetArrayElementAtIndex(i).stringValue = list[i];
            
            // Save changes
            editor.Save(false);
        }

        private static string[] ParseLine(string line)
        {
            const string pattern = @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)"; 
            return (from Match m in Regex.Matches(line, pattern, RegexOptions.ExplicitCapture) 
                select m.Groups[1].Value).ToArray();
        }
    }
}