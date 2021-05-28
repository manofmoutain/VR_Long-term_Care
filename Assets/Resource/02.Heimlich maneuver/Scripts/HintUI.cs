using System;
using System.Collections;
using System.Collections.Generic;
using Global.Pateint;
using InteractableObject;
using UnityEditor;
using UnityEngine;

namespace Heimlich_maneuver
{
    public class HintUI : MonoBehaviour
    {
        [SerializeField] private HamlickPatient patient;
        [SerializeField] private GameObject speechHintUI;
        [SerializeField] private GameObject moveToBackUI1;
        [SerializeField] private GameObject moveToBackUI2;
        [SerializeField] private GameObject movePatientUI1;
        [SerializeField] private GameObject movePatientUI2;


        private void Start()
        {
            speechHintUI.SetActive(true);
            moveToBackUI1.SetActive(false);
            moveToBackUI2.SetActive(false);
            movePatientUI1.SetActive(false);
            movePatientUI2.SetActive(false);
        }


        private void Update()
        {
            moveToBackUI1.SetActive(patient.GetComponent<Patient>().isAtOriginPosition);
            moveToBackUI2.SetActive(patient.GetComponent<Patient>().isAtChangedPosition);
            movePatientUI1.SetActive(patient.GetComponentInChildren<TakeEvent_HandGrab>().snapTakeObject && !patient.GetComponent<HamlickPatient>().isChoking);
            movePatientUI2.SetActive(patient.GetComponentInChildren<TakeEvent_HandGrab>().snapTakeObject && patient.GetComponent<HamlickPatient>().isChoking);
        }


        public void SwitchSpeechHint(bool on)
        {
            speechHintUI.SetActive(on);
        }
    }
}

