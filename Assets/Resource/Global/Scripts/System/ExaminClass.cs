using UnityEngine;

namespace System
{
    [CreateAssetMenu(fileName = "ExamClass",menuName="Exam/Class",order = 0)]
    public class ExaminClass : ScriptableObject
    {
        public int examSceneIndex;
        public string fileName;
        public int operateCount;
    }
}