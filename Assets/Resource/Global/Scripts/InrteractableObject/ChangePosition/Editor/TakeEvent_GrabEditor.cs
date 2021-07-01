using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InteractableObject
{
    [CustomEditor(typeof(TakeEvent_HandGrab))]
    public class TakeEvent_GrabEditor : Editor
    {
        private TakeEvent_HandGrab grab;
        private GUIStyle headerLayout;

        private void OnEnable()
        {
            headerLayout = new GUIStyle(){fontStyle = FontStyle.Bold,fontSize = 13, alignment = TextAnchor.MiddleLeft};
            grab = target as TakeEvent_HandGrab;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("是否使用雙手"),headerLayout);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingTwoHands"));
            if (grab.isUsingTwoHands)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHand"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHandAttachedGameObject"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHand"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHandAttachedGameObject"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("位置參數"),headerLayout);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isStartTrigger"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isStartKinematic"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("originalTransform"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePosition"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("snapZoneArea"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("takeObject"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("抓取狀態"),headerLayout);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("snapTakeObject"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("snapReleaseGesture"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("snapFixed"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("手勢資訊"),headerLayout);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attachmentFlags"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("releaseVelocityStyle"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("事件"),headerLayout);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isHiddenEvents"));
            if (!grab.isHiddenEvents)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("snapIn"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("snapOut"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onPickUp"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dropDown"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

