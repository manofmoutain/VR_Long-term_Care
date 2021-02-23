﻿using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatientInteract : MonoBehaviour
    {
        public Transform originTransform;

        [Tooltip("替換位置")] [SerializeField]  Transform sitTransform;
        [Tooltip("提示UI")] [SerializeField]  GameObject interactHint;
        [Tooltip("要按壓的位置")] [SerializeField]  GameObject interactPoint;
        [Tooltip("病人")] [SerializeField] GameObject patient;

        private void Start()
        {
            if (patient == null)
            {
                patient = transform.GetChild(0).gameObject;
            }
        }

        private void Update()
        {
            interactHint.SetActive(patient.transform.parent == sitTransform );

            interactPoint.SetActive(patient.transform.parent == sitTransform);
        }

        public void ResetToOriginPosition()
        {
            if (!GetComponent<HamlickPatientSpit>().isChoking && patient.transform.parent==originTransform)
            {
                //項目六：將案主移回原位
                ScoreManager.Instance.DecreaseOperateSteps(5);
                ScoreManager.Instance.SetDone(5);
            }
        }
    }
}

