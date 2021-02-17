using System;
using InrteractableObject;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

        [SerializeField] private Transform originTransform;
        [SerializeField] private Transform sitTransform;

        [SerializeField] private GameObject interactHint;
        [SerializeField] private LinearInteractable interactPoint;

        [SerializeField] private GameObject patient;

        private void Start()
        {
            player = FindObjectOfType<Player>().gameObject;
            isPushed = false;
        }

        private void Update()
        {
            interactHint.SetActive(transform.parent==sitTransform);
            interactPoint.gameObject.SetActive(transform.parent==sitTransform && player.transform.position.z>transform.position.z);
            // patient.GetComponent<SnapTakeDropZone>().enabled = patient.transform.parent==originTransform;
            // GetComponent<LinearAnimation>().enabled = isPushed;

        }


        public void SetPushed(bool pushed)
        {
            isPushed = pushed;
        }
    }
}