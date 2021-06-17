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


        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.GetComponent<Interact_TriggerComponent>())
        //     {
        //         if (count<maxCount)
        //         {
        //             AddCount();
        //         }
        //
        //     }
        // }

        public void AddCount()
        {
            count++;
            if (count==maxCount)
            {
                onMaxCountEvent.Invoke();
            }
        }
    }
}

