// /******************************************************************************
//  * File: KeySpecialButtonEditor.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEditor;
using UnityEngine;

namespace QCHT.Samples.XRKeyboard
{
    [CustomEditor(typeof(KeySpecialButton))]
    public class KeySpecialButtonEditor : Editor
    {
        private SerializedProperty _hoverButton, _selectButton;
        private SerializedProperty _audioSource;
        private SerializedProperty _inputEvent;
        private SerializedProperty _keySpecial;
        private SerializedProperty _objectToDeactivate;
        private SerializedProperty _objectToActivate;

        private void OnEnable()
        {
            _hoverButton = serializedObject.FindProperty("_hoverButton");
            _selectButton = serializedObject.FindProperty("_selectButton");
            _audioSource = serializedObject.FindProperty("_audioSource");
            _inputEvent = serializedObject.FindProperty("inputEvent");
            _keySpecial = serializedObject.FindProperty("keyKeySpecial");
            _objectToDeactivate = serializedObject.FindProperty("_objectToDeactivate");
            _objectToActivate = serializedObject.FindProperty("_objectToActivate");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.enabled = true;

            EditorGUILayout.PropertyField(_hoverButton);
            EditorGUILayout.PropertyField(_selectButton);
            EditorGUILayout.PropertyField(_audioSource);
            EditorGUILayout.PropertyField(_inputEvent);
            EditorGUILayout.PropertyField(_keySpecial);

            if ((KeySpecial)_keySpecial.enumValueIndex == KeySpecial.SwitchObject)
            {
                EditorGUILayout.PropertyField(_objectToDeactivate);
                EditorGUILayout.PropertyField(_objectToActivate);
            }
            else
            {
                GUI.enabled = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
