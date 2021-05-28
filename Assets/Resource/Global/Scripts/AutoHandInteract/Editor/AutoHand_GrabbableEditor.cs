using System;
using UnityEngine;
using UnityEditor;

namespace AutoHandInteract
{
    [CustomEditor(typeof(AutoHand_Grabbable))]
    public class AutoHand_GrabbableEditor : Editor
    {
        private AutoHand_Grabbable grabble;
        private GUIStyle headerStyle;

        private void OnEnable()
        {
            headerStyle = new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            grabble = target as AutoHand_Grabbable;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            if (grabble.transform.childCount>0)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("makeChildrenGrabbable"));
            }

            EditorGUILayout.LabelField(new GUIContent("Break Settings"),headerStyle);


            EditorGUI.BeginDisabledGroup(grabble.singleHandOnly);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pullApartBreakOnly"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("jointBreakForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jointBreakTorque"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Unity Events"), headerStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hideEvents"));

            if (!grabble.hideEvents)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGrab"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onRelease"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onSqueeze"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onUnsqueeze"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onJointBreak"));
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}

