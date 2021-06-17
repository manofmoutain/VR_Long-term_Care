using System;
using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Global.Pateint
{
    public partial class Patient : MonoBehaviour
    {
      public bool changeTransformMode;

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




    }
}