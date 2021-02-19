using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}

