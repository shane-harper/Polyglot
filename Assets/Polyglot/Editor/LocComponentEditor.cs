//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using Polyglot.Components;
using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Editor that just hides the script reference, to look more like an official Component
    /// </summary>
    public class LocBasicEditor : UnityEditor.Editor
    {
        private readonly string[] _hiddenParameters = {"m_Script"};
    
        public override void OnInspectorGUI()
        {
            // Just hide the script field to look more like standard unity components
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                DrawPropertiesExcluding(serializedObject, _hiddenParameters);
                if (check.changed) this.Save();
            }
        }
    }

    /// <summary>
    ///     Custom inspector for the LocText component
    /// </summary>
    [CustomEditor(typeof(LocComponent), true)]
    public class LocTextEditor : LocBasicEditor
    {
    }

    /// <summary>
    ///     Custom inspector for the LocLanguageSelect component
    /// </summary>
    [CustomEditor(typeof(LocLanguageSelect))]
    public class LocLanguageSelectEditor : LocBasicEditor
    {
    }

    /// <summary>
    ///     Custom inspector for the LocSwitchBehaviour and LocSwitchGameObject components
    /// </summary>
    [CustomEditor(typeof(LocSwitch), true)]
    public class LocSwitchEditor : UnityEditor.Editor
    {
        private GUIContent[] _labels;
        private SerializedProperty _targets;
        private SerializedProperty[] _enable;
        private void OnEnable()
        {
            var languages = LocManager.GetLanguages();
            if (languages == null) return;

            // Get properties
            _targets = serializedObject.FindProperty("_targets");
            var enable = serializedObject.FindProperty("_enable");
            
            // Get language labels
            var count = languages.Length;            
            _labels = new GUIContent[count];
            const string tooltip = "Enable for this localization?";
            for (var i = 0; i < count; ++i)
                _labels[i] = new GUIContent(languages[i], tooltip);

            // Ensure enable list length
            if (enable.arraySize != count)
            {
                if (enable.arraySize > count)

                    enable.arraySize = count;
                else if (enable.arraySize < count)
                    for (var i = enable.arraySize; i < count; ++i)
                        enable.InsertArrayElementAtIndex(i);
                
                this.Save(false);
            }

            // Get switch properties
            _enable = new SerializedProperty[count];
            for (var i = 0; i < count; ++i)
                _enable[i] = enable.GetArrayElementAtIndex(i);
        }

        public override void OnInspectorGUI()
        {
            if (_labels == null)
            {
                base.OnInspectorGUI();
                return;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_targets, true);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                for (var i = 0; i < _labels.Length; ++i)
                    EditorGUILayout.PropertyField(_enable[i], _labels[i]);
                EditorGUILayout.EndVertical();
                
                if (!check.changed) return;
                this.Save();
            }
        }
    }
}