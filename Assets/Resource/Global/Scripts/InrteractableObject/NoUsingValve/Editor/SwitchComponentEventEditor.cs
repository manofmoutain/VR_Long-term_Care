using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InteractableObject
{
    [CustomEditor(typeof(SwitchComponentEvent))]
    public class SwitchComponentEventEditor : Editor
    {
        private SwitchComponentEvent switchComponentEvent;
        private GUIStyle mainHeader;

        private void OnEnable()
        {
            switchComponentEvent = target as SwitchComponentEvent;
            mainHeader = new GUIStyle() {fontStyle = FontStyle.Bold, fontSize = 14, alignment = TextAnchor.MiddleLeft};
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("是否在打開物件時觸發事件",mainHeader);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingOnEnableEvent"));
            if (switchComponentEvent.isUsingOnEnableEvent)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onEnableEvent"));
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("觸碰到物件後是否要清除關鍵字",mainHeader);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isRemoveKeyword"));
            EditorGUILayout.LabelField("Trigger事件",mainHeader);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingTriggerEvent"));
            if (switchComponentEvent.isUsingTriggerEvent)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerOBJs"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerEvent"));
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Collider事件",mainHeader);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingCollisionEvent"));
            if (switchComponentEvent.isUsingCollisionEvent)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionOBJs"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionEvent"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

