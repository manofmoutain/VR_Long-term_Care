using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Global.Pateint;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace DetectLife
{
    public class EarThermometer : MonoBehaviour
    {
        [SerializeField] private bool isOn;
        [SerializeField] private GameObject pointLight;
        [SerializeField] private Material offMaterial;
        [SerializeField] private Material onMaterial;
        [SerializeField] private MeshRenderer meshRenderer;

        [SerializeField] private GameObject temperatureUI;
        [SerializeField] private TextMeshProUGUI temperatureText;

        [SerializeField] private AudioClip sFX;
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private PatientTemperature patient;
        private void Start()
        {
            pointLight.SetActive(false);
            temperatureUI.SetActive(false);
        }


        void HandAttachedUpdate(Hand hand)
        {
            GrabTypes grabTypes = hand.GetGrabStarting();

            if (grabTypes == GrabTypes.Pinch)
            {
                // print("123");
                if (isOn)
                {
                    isOn = false;
                    meshRenderer.material = offMaterial;
                    CancelInvoke(nameof(Switch));
                }
                else
                {
                    isOn = true;
                    meshRenderer.material = onMaterial;
                    InvokeRepeating(nameof(Switch), 1, 0.5f);
                }
            }
        }

        public void Switch()
        {
            pointLight.SetActive(!pointLight.activeSelf);
        }


        public void ShowTemperature(float temp)
        {
            audioSource.PlayOneShot(sFX);
            temperatureUI.SetActive(true);
            temperatureText.text = temp.ToString();
        }
    }
}