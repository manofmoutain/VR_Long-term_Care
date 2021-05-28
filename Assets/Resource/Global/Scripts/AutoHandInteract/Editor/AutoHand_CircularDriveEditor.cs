using System;
using UnityEngine;
using UnityEditor;

namespace AutoHandInteract
{
    [CustomEditor(typeof(AutoHand_CircularDrive))]
    public class AutoHand_CircularDriveEditor : Editor
    {
        private AutoHand_CircularDrive circularDrive;
        private GUIStyle headerStyle;

        private void OnEnable()
        {
            headerStyle = new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            circularDrive = target as AutoHand_CircularDrive;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();



            if (circularDrive.limited)
            {
                EditorGUILayout.LabelField(new GUIContent("Limit Settings"),headerStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onMinAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onMaxAngle"));
            }

            if (circularDrive.isAngleEvent)
            {
                EditorGUILayout.LabelField(new GUIContent("Angle Event Settings"),headerStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("eventAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("angleEvent"));
            }

            if (circularDrive.forceStart)
            {
                EditorGUILayout.LabelField(new GUIContent("Force Start Angle Settings"),headerStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startAngle"));
            }

            if (circularDrive.isUnhiddenHiddenVector3)
            {
                EditorGUILayout.LabelField(new GUIContent("顯示座標相關的參數"),headerStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("start"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("worldPlaneNormal"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("localPlaneNormal"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lastHandProjected"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minMaxAngularThreshold"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

