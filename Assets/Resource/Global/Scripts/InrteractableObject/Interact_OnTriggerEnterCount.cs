using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    public class Interact_OnTriggerEnterCount : MonoBehaviour
    {
        [SerializeField] private GameObject patient;
        public UnityEvent onTriggerEnter;
        public UnityEvent onLinearInteractTriggerEnter;
        [SerializeField] private int count;
        public int Count => count;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke();

            if (other.GetComponent<LinearDrive>() || other.GetComponent<Interact_TwoHandLinearDrive>())
            {
                onLinearInteractTriggerEnter.Invoke();
            }
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

