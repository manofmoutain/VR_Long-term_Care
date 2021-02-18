﻿using System;
using System.Collections;
using System.Collections.Generic;
using Heimlich_maneuver.Patient;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace Resource.Global.Scripts.Patient
{
    public class OnTriggerEnterCount : MonoBehaviour
    {
        [SerializeField] private GameObject patient;
        public UnityEvent onTriggerenter;
        [SerializeField] private int count;
        public int Count => count;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerenter.Invoke();
            // if (other.GetComponent<Interactable>())
            // {
            //     count++;
            //     if (patient.GetComponent<HamlickPatient>())
            //     {
            //         patient.GetComponent<HamlickPatient>().Push();
            //     }
            // }
        }
    }
}

