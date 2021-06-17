using System;
using UnityEditor;
using UnityEngine;

namespace Global.Pateint
{
    [CustomEditor(typeof(Patient))]
    public class PatientEditor : Editor
    {
        private Patient patient;
        private GUIStyle mainHeaderStyle;
        private GUIStyle secondHeaderStyle;

        private void OnEnable()
        {
            patient = target as Patient;
            mainHeaderStyle = new GUIStyle() {fontStyle = FontStyle.Bold, fontSize = 14, alignment = TextAnchor.MiddleLeft};
            secondHeaderStyle = new GUIStyle() {fontStyle = FontStyle.Italic, fontSize = 13, alignment = TextAnchor.MiddleLeft};
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("使用者物件",mainHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("playerPosition"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("病患物件",mainHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("patient"));
            EditorGUILayout.LabelField("是否要變換位置",secondHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("changeTransformMode"));
            if (patient.changeTransformMode)
            {
                EditorGUILayout.LabelField("原始父物件",secondHeaderStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("originTransform"));
                EditorGUILayout.LabelField("改變後的父物件",secondHeaderStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("changedTransform"));
            }
            EditorGUILayout.LabelField("是否要使用音效",secondHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingSFX"));
            if (patient.isUsingSFX)
            {
                EditorGUILayout.LabelField("音效組",secondHeaderStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sFXs"));
            }
            EditorGUILayout.LabelField("是否需要使用血壓測試",secondHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingBloodPressure"));
            if (patient.isUsingBloodPressure)
            {
                EditorGUILayout.LabelField("血壓變數組",secondHeaderStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pressures"));
                EditorGUILayout.LabelField("目前血壓",secondHeaderStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bloodPressure"));
            }

            // EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}

