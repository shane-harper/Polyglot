using System;
using NUnit.Framework;
using Polyglot.Data;
using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor.Tests
{
    public class LocTests : MonoBehaviour
    {
        private const string TestCategory = "Polyglot";

        private static LocSettings LoadSettings(out SerializedProperty languages, out SerializedProperty defaults)
        {
            // Load settings asset
            var settings = Resources.Load<LocSettings>(LocManager.SettingsName);
            Assert.NotNull(settings, "Could not find settings resource");
            
            // Get required serialized properties
            var editor = UnityEditor.Editor.CreateEditor(settings);
            languages = editor.serializedObject.FindProperty("_languages");
            defaults = editor.serializedObject.FindProperty("_defaults");
            Assert.NotNull(languages, "Could not find _languages property in settings asset");
            Assert.NotNull(defaults, "Could not find _defaults property in settings asset");
            
            return settings;
        }

        private static LocLanguage LoadLanguage(string name)
        {
            var assetPath = LocEditorTools.GetAssetPath(name);
            var language = AssetDatabase.LoadAssetAtPath<LocLanguage>(assetPath);
            Assert.NotNull(language, "No language found named {0}", name);
            return language;
        }
        
        [Test, Category(TestCategory)]
        public static void SettingsExists()
        {
            // Check asset exists
            SerializedProperty languages, defaults;
            LoadSettings(out languages, out defaults);
            
            // Check that an asset exists for each localization
            for (var i = 0; i < languages.arraySize; ++i)
                LoadLanguage(languages.GetArrayElementAtIndex(i).stringValue);

            // Check defaults count
            var expectedDefaultsCount = Enum.GetValues(typeof(SystemLanguage)).Length - 1;
            Assert.AreEqual(expectedDefaultsCount, defaults.arraySize);
            
            // Tests are allowed to be happy
            Assert.Pass("Settings file exists, {0} languages found", languages.arraySize);
        }

        [Test, Category(TestCategory)]
        public static void KeyValueCount()
        {
            // Load keys file
            var keysPath = LocEditorTools.GetAssetPath(LocManager.KeysName);
            var keys = AssetDatabase.LoadAssetAtPath<LocKeys>(keysPath);
            Assert.NotNull(keys);
            var stringsLength = keys.Strings.Length;
            
            // Load settings asset
            SerializedProperty languages, defaults;
            LoadSettings(out languages, out defaults);

            // Check that string values count matches keys
            for (var i = 0; i < languages.arraySize; ++i)
            {
                var name = languages.GetArrayElementAtIndex(i).stringValue;
                var language = LoadLanguage(name);
                Assert.AreEqual(stringsLength, language.Strings.Length,
                    "String Keys/Values count does not match for {0}. Keys {1} != Values {2}", 
                    name, stringsLength, language.Strings.Length);
            }
            
            Assert.Pass("{0} String Keys/Values", stringsLength);
        }
    }
}