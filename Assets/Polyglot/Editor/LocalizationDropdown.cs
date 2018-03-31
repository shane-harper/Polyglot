//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    internal abstract class BaseLocalizationDropdown
    {
        protected readonly SerializedProperty Source;
        protected GUIContent[] NameList;

        protected BaseLocalizationDropdown(SerializedProperty source)
        {
            Source = source;
        }

        /// <summary>
        ///     Returns the number of items in the dropdown
        /// </summary>
        public int Length
        {
            get { return NameList.Length; }
        }

        /// <summary>
        ///     Gets the string value for an item in the dropdown
        /// </summary>
        public string GetName(int index)
        {
            if (index < 0 || index >= NameList.Length) return string.Empty;
            return NameList[index].text;
        }

        /// <summary>
        ///     Draw with an integer
        /// </summary>
        /// <returns>Returns new value</returns>
        public int Draw(GUIContent label, int selectedIndex)
        {
            return EditorGUILayout.Popup(label, selectedIndex, NameList);
        }
        
        public int Draw(Rect position, GUIContent label, int selectedIndex)
        {
            return EditorGUI.Popup(position, label, selectedIndex, NameList);
        }

        /// <summary>
        ///     Draw with a serialized property
        /// </summary>
        /// <returns>Returns true if the value changed</returns>
        public bool Draw(SerializedProperty property)
        {
            return Draw(new GUIContent(property.displayName), property);
        }
        
        /// <summary>
        ///     Draw with a serialized property
        /// </summary>
        /// <returns>Returns true if the value changed</returns>
        public bool Draw(GUIContent label, SerializedProperty property)
        {
            var value = property.intValue;
            var newValue = Draw(label, value);

            if (value == newValue) return false;
            property.intValue = newValue;
            return true;
        }

        /// <summary>
        ///     Refresh dropdown item list
        /// </summary>
        public abstract void RefreshList();
    }

    /// <summary>
    ///     Editor UI element to draw a drop down containing localization names
    /// </summary>
    internal sealed class LocalizationDropdown : BaseLocalizationDropdown
    {
        public LocalizationDropdown(SerializedProperty nameSource) : base(nameSource)
        {
            RefreshList();
        }

        /// <inheritdoc />
        public override void RefreshList()
        {
            var count = Source.arraySize;
            NameList = new GUIContent[count];
            for (var i = 0; i < count; ++i)
                NameList[i] = new GUIContent(Source.GetArrayElementAtIndex(i).stringValue);
        }
    }

    /// <summary>
    ///     Editor UI element to draw a drop down containing localization keys
    /// </summary>
    internal sealed class LocalizationKeyDropdown : BaseLocalizationDropdown
    {
        public LocalizationKeyDropdown(SerializedProperty source) : base(source)
        {
            RefreshList();
        }

        /// <inheritdoc />
        public override void RefreshList()
        {
            var count = Source.arraySize;
            NameList = new GUIContent[count];
            for (var i = 0; i < count; ++i)
            {
                var key = Source.GetArrayElementAtIndex(i).FindPropertyRelative("_key");
                NameList[i] = new GUIContent(key.stringValue);
            }
        }
    }
}