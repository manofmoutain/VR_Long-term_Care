using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global.Pateint
{
    public partial class Patient
    {
        public bool isUsingSFX;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] sFXs;

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