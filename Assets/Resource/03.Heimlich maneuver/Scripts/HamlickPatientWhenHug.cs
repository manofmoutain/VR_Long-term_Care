using System;
using System.Collections;
using System.Collections.Generic;
using InrteractableObject;
using UnityEngine;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatientWhenHug : MonoBehaviour
    {
        [Header("模型")] [SerializeField] private GameObject hugRightHand;
        [SerializeField] private GameObject hugLeftHand;
        [SerializeField] private GameObject patient;

        /// <summary>
        /// 是否處於被抱起來的狀態
        /// </summary>
        [Header("狀態")]
        [SerializeField] private bool isHuging;

        private void Start()
        {
            isHuging = false;
            if (patient==null)
            {
                patient = transform.GetChild(0).gameObject;
            }
        }

        private void Update()
        {
            hugLeftHand.SetActive(isHuging);
            hugRightHand.SetActive(isHuging);
            // patient.GetComponent<TakeEvent_SnapPutZone>().enabled = isHuging;
        }

        public void Hugging(bool hugging)
        {
            isHuging = hugging;
        }
    }
}

