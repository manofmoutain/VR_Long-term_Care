using System;
using System.Collections;
using Manager;
using UnityEngine;

namespace Global.Pateint
{
    public class PatientTransform : MonoBehaviour
    {
        [Tooltip("原始位置")] public Transform originTransform;

        [Tooltip("替換位置")] public Transform changedTransform;

        /// <summary>
        /// 病患模型是否在原始位置上
        /// </summary>
        public bool isAtOriginPosition;

        /// <summary>
        /// 病患模型是否在要調換的位置上
        /// </summary>
        public bool isAtChangedPosition;

        private void Update()
        {
            isAtOriginPosition = GetComponent<Patient>().GetPatientTransform.parent == originTransform;
            isAtChangedPosition = GetComponent<Patient>().GetPatientTransform.parent == changedTransform;
        }


    }
}