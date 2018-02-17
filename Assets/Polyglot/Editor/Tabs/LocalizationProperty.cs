//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Object to contain details about each localization property (strings, sprites, etc)
    /// </summary>
    internal sealed class LocalizationProperty
    {
        public delegate bool NullCheckHandler(SerializedProperty property);

        private readonly NullCheckHandler _keyNullCheckHandler;
        private readonly NullCheckHandler _valueNullCheckHandler;
        public readonly MessageType MissingValuesSeverity;

        public readonly string Name;
        public readonly string SerializedName;

        public LocalizationProperty(string name, string serializedName, NullCheckHandler keyNullCheckHandler,
            NullCheckHandler valueNullCheckHandler, MessageType missingValuesSeverity)
        {
            Name = name;
            SerializedName = serializedName;
            _keyNullCheckHandler = keyNullCheckHandler;
            _valueNullCheckHandler = valueNullCheckHandler;
            MissingValuesSeverity = missingValuesSeverity;
        }

        public int KeyCount
        {
            get { return SerializedProperty.arraySize; }
        }

        /// <summary>
        ///     Number of missing keys in this property
        /// </summary>
        public int MissingKeys { get; private set; }

        /// <summary>
        ///     Number of missing values per localization in this property
        /// </summary>
        public int[] MissingValues { get; private set; }

        public SerializedProperty SerializedProperty { get; private set; }

        /// <summary>
        ///     Gather summary of serialized object, missing keys etc
        /// </summary>
        public void Initialize(SerializedObject serializedObject, int localizationCount)
        {
            SerializedProperty = serializedObject.FindProperty(SerializedName);
            MissingValues = new int[localizationCount];

            for (var i = 0; i < KeyCount; ++i)
            {
                // Count missing keys
                if (_keyNullCheckHandler(GetKey(i)))
                    ++MissingKeys;

                // Count missing values
                var values = GetValues(i);
                var valueCount = values.arraySize;
                for (var v = 0; v < valueCount && v < localizationCount; ++v)
                    if (_valueNullCheckHandler(values.GetArrayElementAtIndex(v)))
                        ++MissingValues[v];
            }
        }

        public static bool ObjectComparison(SerializedProperty property)
        {
            return property.objectReferenceValue == null;
        }

        public static bool StringComparison(SerializedProperty property)
        {
            return string.IsNullOrEmpty(property.stringValue);
        }

        /// <summary>
        ///     Get the localization key at the specified index
        /// </summary>
        /// <returns>Serialized property of key</returns>
        public SerializedProperty GetKey(int index)
        {
            return SerializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("_key");
        }

        /// <summary>
        ///     Get the localization values at the specified index
        /// </summary>
        /// <returns>Serialized property of values</returns>
        public SerializedProperty GetValues(int index)
        {
            return SerializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("_values");
        }
    }
}