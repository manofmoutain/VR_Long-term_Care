using System;
using InrteractableObject;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;
using LinearAnimation = InrteractableObject.LinearAnimation;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatient : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        // [SerializeField] private LinearInteractable interactPoint;
        public bool isPushed;


        private void Start()
        {
            player = FindObjectOfType<Player>().gameObject;
            isPushed = false;
        }

        private void Update()
        {
            // interactPoint.gameObject.SetActive(player.transform.position.z > transform.position.z);
            // GetComponent<LinearAnimation>().enabled = isPushed;

        }


        public void SetPushed(bool pushed)
        {
            isPushed = pushed;
        }
    }
}