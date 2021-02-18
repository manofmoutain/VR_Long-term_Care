using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.Events;

namespace InrteractableObject
{
    public class OnTriggerRecognizeSpeech : MonoBehaviour

    {
        [SerializeField] bool isCorrectKeyWord;

        private void OnTriggerEnter(Collider other)
        {
            SpeechManager.Instance.StartRecognizeSpeech();
        }
    }
}

