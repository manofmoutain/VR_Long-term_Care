using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PrepareMeal
{
    public class InjectWater : MonoBehaviour
    {
        [SerializeField] private UnityEvent waterInjectEvent;
        private void OnTriggerStay(Collider other)
        {
            if (other.name=="裝在量杯裡面的水")
            {
                waterInjectEvent.Invoke();
            }
        }
    }
}

