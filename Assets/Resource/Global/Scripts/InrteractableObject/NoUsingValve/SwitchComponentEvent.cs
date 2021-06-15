using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class SwitchComponentEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent triggerEvent;
        [SerializeField] private UnityEvent collisionEvent;
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Interact_TriggerComponent>())
            {
                print($"Touched {other.gameObject.name}");
                triggerEvent.Invoke();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<Interact_TriggerComponent>())
            {
                print($"Touched {other.gameObject.name}");
                collisionEvent.Invoke();
            }
        }
    }
}

