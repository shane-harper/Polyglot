using System;
using Polyglot.Editor;
using UnityEditor;
using UnityEngine;

namespace Polyglot
{
	[CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
	public class LocalizationKeyDrawer : PropertyDrawer
	{
		private LocalizationKeyDropdown _keyDropdown;
		private int _selectedIndex;

		public override float GetPropertyHeight(SerializedProperty property,
			GUIContent label)
		{	
			return  EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Create dropdown if it doesn't already exist
			if (_keyDropdown == null)
			{
				_keyDropdown = CreateDropdown(attribute);
				if (_keyDropdown == null)
				{
					EditorGUILayout.LabelField(label, "No localization data found");
					return;
				}
				
				_selectedIndex = GetSelectedIndex(property, _keyDropdown);
			}

			var backgroundColor = GUI.backgroundColor;
			if (_selectedIndex < 0)
			{
				GUI.backgroundColor = Color.red;
				label.tooltip += string.Format("{1} key '{0}' was not found in localization data",
					property.stringValue, ((LocalizationKeyAttribute) attribute).Type);
			}
			
			// Display dropdown and listen for changes
			EditorGUI.BeginChangeCheck();
			_selectedIndex = _keyDropdown.Draw(position, label, _selectedIndex);
			GUI.backgroundColor = backgroundColor;
			if (!EditorGUI.EndChangeCheck()) return;
			
			// Apply changes to property
			property.stringValue = _keyDropdown.GetName(_selectedIndex);
			property.serializedObject.ApplyModifiedProperties();
				
			// Re-apply localization with new value
			var localizedComponent = property.serializedObject.targetObject as LocalizedComponent;
			if (localizedComponent != null) localizedComponent.RefreshLocalization();
		}

		private static LocalizationKeyDropdown CreateDropdown(PropertyAttribute attribute)
		{
			var data = LocalizationFileManager.LoadData();
		    if (data == null) return null;

			var editor = UnityEditor.Editor.CreateEditor(data);
			var p = editor.serializedObject.FindProperty(GetStringValue(((LocalizationKeyAttribute) attribute).Type));
			return new LocalizationKeyDropdown(p);
		}
		
		private static int GetSelectedIndex(SerializedProperty property, LocalizationKeyDropdown dropdown)
		{
			var currentKey = property.stringValue;
			for (var i = 0; i < dropdown.Length; ++i)
				if (dropdown.GetName(i) == currentKey) return i;

			return -1;
		}

		private static string GetStringValue(LocalizationData.Type dataType)
		{
			switch (dataType)
			{
				case LocalizationData.Type.String:
					return "_strings";
				case LocalizationData.Type.Sprite:
					return "_sprites";
				case LocalizationData.Type.Sound:
					return "_sounds";
				case LocalizationData.Type.Font:
					return "_fonts";
				default:
					throw new ArgumentOutOfRangeException("dataType", dataType, null);
			}
		}
	}
}