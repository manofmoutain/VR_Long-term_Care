using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        private void Start()
        {
            pointLight.SetActive(false);
        }


        void HandAttachedUpdate(Hand hand)
        {
            GrabTypes grabTypes = hand.GetGrabStarting();

            if (grabTypes == GrabTypes.Pinch)
            {
                print("123");
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
    }
}