using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEditorInternal;
using UnityEngine;

namespace Manager
{
    public class Monosingleton<T> : MonoBehaviour where T : Monosingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get { return instance; }
        }

        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}