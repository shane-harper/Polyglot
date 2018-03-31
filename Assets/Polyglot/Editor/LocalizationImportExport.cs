//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Tool to import/export localization data to a csv
    /// </summary>
    internal static class LocalizationImportExport
    {
        /// <summary>
        ///     Default path to open save/load dialogs
        /// </summary>
        private static string DefaultPath
        {
            get { return Application.dataPath + "/../"; }
        }

        /// <summary>
        ///     Event triggered when a localization file is imported
        /// </summary>
        public static event UnityAction OnDataImport;

        /// <summary>
        ///     Import localization from file
        /// </summary>
        /// <returns>Returns true if read is successful</returns>
        [MenuItem("Tools/Polyglot/Import Data")]
        public static bool ImportData()
        {
            // Get file to import
            var path = EditorUtility.OpenFilePanel("Import CSV",
                DefaultPath,
                LocalizationFileManager.CsvExtension);
            if (string.IsNullOrEmpty(path)) return false;

            // Validate import path
            if (!File.Exists(path)) return false;

            // Get current data
            var data = LocalizationFileManager.LoadData();
            if (data == null) data = LocalizationFileManager.CreateNewData();
            var serializedObject = new SerializedObject(data);

            // Read file
            using (var file = new StreamReader(path))
            {
                var line = file.ReadLine();
                if (line != null)
                {
                    // Read headers
                    var headers = ParseLine(line);

                    var idCount = headers.Length - 1;
                    var ids = serializedObject.FindProperty("_names");
                    ids.arraySize = idCount;
                    for (var i = 0; i < idCount; ++i)
                        ids.GetArrayElementAtIndex(i).stringValue = headers[i + 1];

                    // Clear existing string localizations
                    var strings = serializedObject.FindProperty("_strings");
                    strings.ClearArray();

                    // Read string localizations from file
                    var counter = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        // Ignore empty lines
                        if (string.IsNullOrEmpty(line)) continue;

                        // Split line and create entry
                        var split = ParseLine(line);
                        strings.InsertArrayElementAtIndex(counter);
                        var entry = strings.GetArrayElementAtIndex(counter);

                        // Set key
                        var keyProperty = entry.FindPropertyRelative("_key");
                        keyProperty.stringValue = split[0];

                        // Set values
                        var valuesProperty = entry.FindPropertyRelative("_values");
                        valuesProperty.arraySize = idCount;
                        for (var v = 0; v < idCount; ++v)
                        {
                            var valueProperty = valuesProperty.GetArrayElementAtIndex(v);
                            valueProperty.stringValue = Regex.Unescape(split[v + 1]);
                        }

                        ++counter;
                    }
                }
            }

            // Save data
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(data);

            if (OnDataImport != null) OnDataImport();
            return true;
        }

        /// <summary>
        ///     Export localization data to file
        /// </summary>
        /// <returns>Returns true if write is successful</returns>
        [MenuItem("Tools/Polyglot/Export Data")]
        public static bool ExportData()
        {
            // Get export path
            var fileName = Path.GetFileName(LocalizationManager.ResourcePath);
            var path = EditorUtility.SaveFilePanel("Export CSV",
                DefaultPath,
                fileName,
                LocalizationFileManager.CsvExtension);
            if (string.IsNullOrEmpty(path)) return false;

            // Get localization data to export
            var data = LocalizationFileManager.LoadData();
            if (data == null)
            {
                Debug.LogError("Could not find Localization Data");
                return false;
            }

            var serializedObject = new SerializedObject(data);
            var ids = serializedObject.FindProperty("_names");
            var strings = serializedObject.FindProperty("_strings");

            try
            {
                // Open file to write, do not append
                using (var file = new StreamWriter(path, false))
                {
                    var builder = new StringBuilder();
                    var stringsCount = strings.arraySize;

                    // Add headers
                    var idCount = ids.arraySize;
                    builder.Append("Key,");
                    for (var h = 0; h < idCount; ++h)
                    {
                        builder.AppendFormat("\"{0}\"", ids.GetArrayElementAtIndex(h).stringValue);
                        if (h + 1 < idCount) builder.Append(Characters.Separator);
                    }
                    file.WriteLine(builder.ToString());
                    builder.Length = 0;

                    // Add all strings
                    for (var k = 0; k < stringsCount; ++k)
                    {
                        // Add key
                        var entry = strings.GetArrayElementAtIndex(k);
                        builder.AppendFormat("\"{0}\"", entry.FindPropertyRelative("_key").stringValue);
                        builder.Append(Characters.Separator);

                        // Add all keys
                        var values = entry.FindPropertyRelative("_values");
                        var valueCount = values.arraySize;
                        for (var v = 0; v < valueCount; ++v)
                        {
                            builder.AppendFormat("\"{0}\"", values.GetArrayElementAtIndex(v).stringValue);
                            builder.Append(Characters.Separator);
                        }

                        // Remove last comma and add to lines list
                        builder.Remove(builder.Length - 1, 1);
                        file.WriteLine(builder.ToString());

                        // Clear string builder
                        builder.Length = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Localization: Failed to export data: {0}", ex.Message);
                return false;
            }

            Debug.Log("Localization: Export complete");
            return true;
        }

        private static class Characters
        {
            public const char Separator = ',';
            public const char QuotationMark = '\"';
        }
        
        private static string[] ParseLine(string line)
        {
            const string pattern = @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)"; 
            return (from Match m in Regex.Matches(line, pattern, RegexOptions.ExplicitCapture) 
                select m.Groups[1].Value).ToArray();
        }
    }
}