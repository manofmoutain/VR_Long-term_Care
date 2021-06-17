using UnityEditor;
using UnityEngine;

namespace InteractableObject
{
    [CustomEditor(typeof(Interact_CircularDrive))]
    public class CircularDriverEditor : Editor
    {
        private Interact_CircularDrive circularDrive;
        private GUIStyle mainHeaderStyle;
        private GUIStyle secondHeaderStyle;

        private void OnEnable()
        {
            circularDrive = target as Interact_CircularDrive;
            mainHeaderStyle = new GUIStyle() {fontStyle = FontStyle.Bold , fontSize = 14, alignment = TextAnchor.MiddleLeft};
            secondHeaderStyle = new GUIStyle() {fontStyle = FontStyle.Normal, fontSize = 13, alignment = TextAnchor.MiddleLeft};
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("手勢資訊"),mainHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUsingBlenderPoser"));
            if (circularDrive.isUsingBlenderPoser)
            {
                EditorGUILayout.LabelField(new GUIContent("混合手勢"),secondHeaderStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("switchOnPoser"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("switchOffPoser"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("grabHand"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attachmentFlags"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isRoot"));
            if (circularDrive.transform.childCount>0)
            {
                // Debug.Log("123");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("childCollider"));
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("驅動狀態"),mainHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("axisOfRotation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateGameObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hoverLock"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("driving"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outAngle"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("是否設定極限值"),mainHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("limited"));
            if (circularDrive.limited)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isLimteToDetatch"));
                EditorGUILayout.LabelField(new GUIContent("最小角度的事件"),secondHeaderStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onMinAngle"));
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent("最大角度的事件"),secondHeaderStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onMaxAngle"));
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("是否設定角度事件"),mainHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isAngleEvent"));

            if (circularDrive.isAngleEvent)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isAngleEventToDetatch"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("eventAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("angleEvent"));
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("是否強制改變初始值"),mainHeaderStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("forceStart"));
            if (circularDrive.forceStart)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startAngle"));
            }


            // EditorGUILayout.PropertyField(serializedObject.FindProperty("startQuaternion"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("worldPlaneNormal"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("localPlaneNormal"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("lastHandProjected"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("minMaxAngularThreshold"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("interactable"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}

