//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using Polyglot.Data;
using UnityEditor;
using UnityEngine;

namespace Polyglot.Editor
{
    /// <summary>
    ///     Custom inspector for the LocKeys scriptable object
    /// </summary>
    [CustomEditor(typeof(LocKeys))]
    public class LocKeysEditor : UnityEditor.Editor
    {
        private static bool _expandList;
        private GUIContent _foldoutLabel;
        private SerializedProperty[] _keys;

        private void OnEnable()
        {
            var strings = serializedObject.FindProperty("_stringKeys");
            _keys = strings.GetAllElements();

            _foldoutLabel = new GUIContent(string.Format("Keys ({0})", _keys.Length));
        }

        public override void OnInspectorGUI()
        {
            _expandList = EditorGUILayout.Foldout(_expandList, _foldoutLabel, true);
            if (!_expandList) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var count = _keys.Length;
            for (var i = 0; i < count; ++i)
                EditorGUILayout.PropertyField(_keys[i], GUIContent.none);
            EditorGUILayout.EndVertical();
        }
    }
    
    /// <summary>
    ///     Custom inspector for the LocLanguage scriptable object
    /// </summary>
    [CustomEditor(typeof(LocLanguage))]
    public class LocLanguageEditor : UnityEditor.Editor
    {
        private static bool _expandList;
        private GUIContent _foldoutLabel;
        private GUIContent[] _keys;
        private SerializedProperty[] _strings;

        private void OnEnable()
        {
            var path = LocEditorTools.GetAssetPath(LocManager.KeysName);
            var keys = AssetDatabase.LoadAssetAtPath<LocKeys>(path);
            if (keys == null) return;

            _keys = new GUIContent[keys.Strings.Length];
            for (var i = 0; i < _keys.Length; ++i)
            {
                var key = keys.Strings[i];
                _keys[i] = new GUIContent(key, key);
            }

            var strings = serializedObject.FindProperty("_strings");
            _strings = strings.GetAllElements();

            _foldoutLabel = new GUIContent(string.Format("Values ({0})", _strings.Length));
        }

        public override void OnInspectorGUI()
        {
            if (_keys == null)
            {
                base.OnInspectorGUI();
                return;
            }

            _expandList = EditorGUILayout.Foldout(_expandList, _foldoutLabel, true);
            if (!_expandList) return;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var count = Mathf.Min(_keys.Length, _strings.Length);
            for (var i = 0; i < count; ++i)
                EditorGUILayout.PropertyField(_strings[i], _keys[i]);
            EditorGUILayout.EndVertical();
        }
    }
}