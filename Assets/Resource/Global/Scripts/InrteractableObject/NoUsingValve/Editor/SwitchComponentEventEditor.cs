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
            EditorGUILayout.LabelField("摺疊Trigger事件",mainHeader);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("collapseTriggerEvent"));
            if (switchComponentEvent.collapseTriggerEvent)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerEvent"));
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("摺疊用Collider事件",mainHeader);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("collapseCollisionEvent"));
            if (switchComponentEvent.collapseCollisionEvent)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionEvent"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

