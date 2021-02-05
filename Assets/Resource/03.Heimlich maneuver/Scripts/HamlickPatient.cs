using System;
using InrteractableObject;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatient : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private LinearInteractable interactPoint;
        public bool isPushed;


        private void Start()
        {
            player = FindObjectOfType<Player>().gameObject;
        }

        private void Update()
        {
            // interactPoint.gameObject.SetActive(player.transform.position.z > transform.position.z);
            if (isPushed)
            {


            }

        }


        public void SetPushed(bool pushed)
        {
            isPushed = pushed;
        }
    }
}