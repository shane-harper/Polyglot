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
        protected readonly GUIContent Label;
        protected readonly SerializedProperty Source;
        protected GUIContent[] NameList;

        protected BaseLocalizationDropdown(GUIContent label, SerializedProperty source)
        {
            Source = source;
            Label = label;
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
        public int Draw(int selectedIndex)
        {
            return EditorGUILayout.Popup(Label, selectedIndex, NameList);
        }

        /// <summary>
        ///     Draw with a serialized property
        /// </summary>
        /// <returns>Returns true if the value changed</returns>
        public bool Draw(SerializedProperty property)
        {
            var value = property.intValue;
            var newValue = Draw(value);

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
        public LocalizationDropdown(GUIContent label, SerializedProperty nameSource) : base(label, nameSource)
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
        public LocalizationKeyDropdown(GUIContent label, SerializedProperty source) : base(label, source)
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