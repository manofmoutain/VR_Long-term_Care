using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractableObject
{
    public class Interact_Count : MonoBehaviour
    {
        public int count;
        [SerializeField] private int maxCount;

        public void AddCount()
        {
            if (count>=maxCount)
            {
                count = maxCount;
            }
            else
            {
                count++;
            }
        }
    }
}

