using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class SwitchComponentEvent : MonoBehaviour
    {
        [Tooltip("要互動對象的名稱")][SerializeField] private string[] triggerName;
        public bool collapseTriggerEvent;
        [SerializeField] private UnityEvent triggerEvent;

        public bool collapseCollisionEvent;
        [SerializeField] private UnityEvent collisionEvent;
        private void OnTriggerEnter(Collider other)
        {
            if (triggerName.Length>0)
            {
                foreach (var s in triggerName)
                {
                    if (other.name==s)
                    {
                        triggerEvent.Invoke();
                    }
                }
            }
            // if (other.GetComponent<Interact_TriggerComponent>() && other.GetComponent<Interact_TriggerComponent>().enabled)
            // {
            //     // print($"Touched {other.gameObject.name}");
            //     triggerEvent.Invoke();
            // }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (triggerName.Length>0)
            {
                foreach (var s in triggerName)
                {
                    if (other.gameObject.name==s)
                    {
                        collisionEvent.Invoke();
                    }
                }
            }
            // if (other.gameObject.GetComponent<Interact_TriggerComponent>() && other.gameObject.GetComponent<Interact_TriggerComponent>().enabled)
            // {
            //     // print($"Touched {other.gameObject.name}");
            //     collisionEvent.Invoke();
            // }
        }
    }
}

