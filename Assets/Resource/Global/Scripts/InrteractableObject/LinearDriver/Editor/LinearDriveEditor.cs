using System;
using UnityEditor;
using UnityEngine;

namespace InteractableObject
{
    [CustomEditor(typeof(Interact_LinearDrive))]
    public class LinearDriveEditor : Editor
    {
        private Interact_LinearDrive linearDrive;
        GUIStyle  headerStyle;

        private void OnEnable()
        {
            headerStyle = new GUIStyle() {fontStyle = FontStyle.Bold, fontSize = 13, alignment = TextAnchor.MiddleLeft};
            linearDrive = target as Interact_LinearDrive;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("是否要使用雙手操作"),headerStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingTwoHands"));
            if (linearDrive.isUsingTwoHands)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHand"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rightAttachedObjet"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHand"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("leftAttachedObject"));

            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("抓住的設定"),headerStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attachmentFlags"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("驅動設定"),headerStyle);
            EditorGUILayout.LabelField(new GUIContent("鬆手之後是否要回歸為最小值"),new GUIStyle(){fontStyle = FontStyle.Bold,fontSize = 12});
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isDetachToResetPosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startPosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("endPosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("linearMapping"));
            EditorGUILayout.LabelField(new GUIContent("是否允許驅動"),new GUIStyle(){fontStyle = FontStyle.Bold,fontSize = 12});
            EditorGUILayout.PropertyField(serializedObject.FindProperty("repositionGameObject"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("是否使用事件"),headerStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingEvent"));
            if (linearDrive.isUsingEvent)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minEvent"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxEvent"));
            }



            serializedObject.ApplyModifiedProperties();
        }
    }
}

