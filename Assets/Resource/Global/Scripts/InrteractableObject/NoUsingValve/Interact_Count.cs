using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class Interact_Count : MonoBehaviour
    {
        public int count;
        [SerializeField] private int maxCount;
        [SerializeField] private UnityEvent onMaxCountEvent;


        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Interact_TriggerComponent>())
            {
                print($"AddCount");
                AddCount();
            }
        }

        public void AddCount()
        {
            if (count>=maxCount)
            {
                count = maxCount;
                onMaxCountEvent.Invoke();
            }
            else
            {
                count++;
            }
        }
    }
}

