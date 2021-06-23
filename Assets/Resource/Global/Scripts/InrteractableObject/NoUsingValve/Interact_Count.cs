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
        [SerializeField] private UnityEvent overCountEvent;


        public void ResetCount()
        {
            count = 0;
        }

        public void AddCount()
        {
            count++;
            if (count == maxCount)
            {
                onMaxCountEvent.Invoke();
            }
            else if (count>maxCount)
            {
                overCountEvent.Invoke();
            }
        }

        public void ChangeMaxCount(int value)
        {
            maxCount = value;
        }
    }
}