using System;
using UnityEngine;
using UnityEngine.Events;

namespace InrteractableObject
{
    public class TakeEvent_ResetZone : MonoBehaviour
    {
        public UnityEvent onTriggerEnter;
        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke();
            if (other.GetComponent<TakeEvent_SingleHandSnapPutZone>() || other.GetComponent<TakeEvent_TwoHandSnapPuZone>())
            {
                other.GetComponent<TakeEvent_ToResetPosition>().isEntry = true;
            }

        }
    }
}