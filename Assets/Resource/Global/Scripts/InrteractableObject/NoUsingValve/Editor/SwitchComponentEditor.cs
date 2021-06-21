using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InteractableObject
{
    [CustomEditor(typeof(SwitchComponent))]
    public class SwitchComponentEditor : Editor
    {
        private SwitchComponent switchComponent;
        private GUIStyle mainHeader;

        private void OnEnable()
        {
            switchComponent = target as SwitchComponent;
            mainHeader = new GUIStyle() {fontStyle = FontStyle.Bold, fontSize = 14 , alignment = TextAnchor.MiddleLeft};
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("是否有判斷的項目",mainHeader);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingCheckPoint"));
            if (switchComponent.isUsingCheckPoint)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("achieveCheckpointEvent"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

