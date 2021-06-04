using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    public class Interact_OnTriggerEnterCount : MonoBehaviour
    {
        public UnityEvent onTriggerEnter;
        public UnityEvent onLinearInteractTriggerEnter;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke();

            if (other.GetComponent<LinearDrive>() || other.GetComponent<Interact_LinearDrive>())
            {
                onLinearInteractTriggerEnter.Invoke();
            }
        }
    }
}

