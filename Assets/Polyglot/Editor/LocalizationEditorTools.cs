//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file for full license information.  
//  

using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Misc tools used by the Localization editor extensions
    /// </summary>
    internal static class LocalizationEditorTools
    {
        /// <summary>
        ///     Apply modifier properties and set editor as dirty
        /// </summary>
        public static void Save(this UnityEditor.Editor editor)
        {
            editor.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(editor.target);
        }

        /// <summary>
        ///     Make sure value counts are correct in properties
        /// </summary>
        /// <returns>Returns true if changes were made</returns>
        public static bool ValidateValueCounts(int localizationCount, LocalizationProperty[] properties)
        {
            var changed = false;
            foreach (var property in properties)
            {
                var count = property.KeyCount;
                for (var k = 0; k < count; ++k)
                {
                    // Check count
                    var values = property.SerializedProperty.GetArrayElementAtIndex(k).FindPropertyRelative("_values");
                    var valueCount = values.arraySize;
                    if (valueCount == localizationCount) continue;

                    // Update array size
                    values.arraySize = localizationCount;

                    // Clear new elements if applicable
                    for (var i = valueCount; i < localizationCount; ++i)
                        values.GetArrayElementAtIndex(i).ResetProperty();

                    changed = true;
                }
            }
            return changed;
        }

        /// <summary>
        ///     Resets the value of a serialized property
        /// </summary>
        /// <remarks>Surely there's a better way to do this?</remarks>
        public static void ResetProperty(this SerializedProperty property)
        {
            switch (property.type)
            {
                case "string":
                    property.stringValue = string.Empty;
                    break;
                case "int":
                    property.intValue = default(int);
                    break;
                case "float":
                    property.floatValue = default(float);
                    break;
                case "Vector2":
                    property.vector2Value = default(Vector2);
                    break;
                case "Vector2Int":
                    property.vector2IntValue = default(Vector2Int);
                    break;
                case "Vector3":
                    property.vector3Value = default(Vector3);
                    break;
                case "Vector3Int":
                    property.vector3IntValue = default(Vector3Int);
                    break;
                case "Vector4":
                    property.vector4Value = default(Vector4);
                    break;
                case "Color":
                    property.colorValue = default(Color);
                    break;
                case "vector":
                    property.arraySize = 0;
                    break;
                case "bool":
                    property.boolValue = default(bool);
                    break;
                case "Bounds":
                    property.boundsValue = default(Bounds);
                    break;
                case "BoundsInt":
                    property.boundsIntValue = default(BoundsInt);
                    break;
                case "Rect":
                    property.rectValue = default(Rect);
                    break;
                case "RectInt":
                    property.rectIntValue = default(RectInt);
                    break;
                case "Quaternion":
                    property.quaternionValue = default(Quaternion);
                    break;
                case "double":
                    property.doubleValue = default(double);
                    break;
                case "long":
                    property.longValue = default(long);
                    break;
                default:
                    property.objectReferenceValue = null;
                    break;
            }
        }
    }
}