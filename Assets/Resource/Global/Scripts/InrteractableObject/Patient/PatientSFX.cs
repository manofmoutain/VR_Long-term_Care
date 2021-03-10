using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global.Pateint
{
    public class PatientSFX : MonoBehaviour
    {
        [Header("音效")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] sFXs;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySFX(int index)
        {
            audioSource.PlayOneShot(sFXs[index]);
        }

        public void StopSFX()
        {
            audioSource.Stop();

        }
    }
}