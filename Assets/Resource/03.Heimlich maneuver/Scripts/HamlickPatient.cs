using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatient : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject interactPint;


        private void Start()
        {
            player = FindObjectOfType<Player>().gameObject;
        }

        private void Update()
        {
            interactPint.SetActive(player.transform.position.z > transform.position.z);
        }
    }
}