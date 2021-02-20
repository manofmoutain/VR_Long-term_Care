using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatientWhenHug : MonoBehaviour
    {
        [Header("手部模型")] [SerializeField] private GameObject hugRightHand;
        [SerializeField] private GameObject hugLeftGand;

        /// <summary>
        /// 是否處於被抱起來的狀態
        /// </summary>
        [Header("狀態")]
        [SerializeField] private bool isHuging;

        private void Start()
        {
            isHuging = false;
        }

        private void Update()
        {
            hugLeftGand.SetActive(isHuging);
            hugRightHand.SetActive(isHuging);
        }

        public void Hugging(bool hugging)
        {
            isHuging = hugging;
        }
    }
}

