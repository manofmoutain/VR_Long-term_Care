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

