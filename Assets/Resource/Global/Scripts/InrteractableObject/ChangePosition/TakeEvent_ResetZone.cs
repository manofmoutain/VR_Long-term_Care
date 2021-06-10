using System;
using AutoHandInteract;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class TakeEvent_ResetZone : MonoBehaviour
    {
        public UnityEvent onPatientEnter;
        public UnityEvent onTriggerEnter;
        public UnityEvent onCollisionEnter;
        private void OnTriggerEnter(Collider other)
        {
            //一般觸發
            onTriggerEnter.Invoke();


            //特定觸發
            if (other.GetComponent<TakeEvent_ToResetPosition>())
            {
                // print($"碰觸到了{gameObject.name}");
                other.GetComponent<TakeEvent_ToResetPosition>().isEntry = true;
                onPatientEnter.Invoke();
            }

        }

        private void OnCollisionEnter(Collision other)
        {
            onCollisionEnter.Invoke();

            if (other.gameObject.GetComponent<TakeEvent_SingleHandSnapPutZone>()
                // || other.GetComponent<TakeEvent_TwoHandSnapPutZone>()
                || other.gameObject.GetComponent<TakeEvent_HandGrab>()
                || other.gameObject.GetComponent<AutoHand_HandGrab>())
            {
                print($"碰觸到了{gameObject.name}");
                other.gameObject.GetComponent<TakeEvent_ToResetPosition>().isEntry = true;
                onPatientEnter.Invoke();
            }
        }
    }
}