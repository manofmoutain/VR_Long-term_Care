using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class TakeEvent_ResetZone : MonoBehaviour
    {
        public UnityEvent onPatientEnter;
        public UnityEvent onTriggerEnter;
        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke();
            if (other.GetComponent<TakeEvent_SingleHandSnapPutZone>() || other.GetComponent<TakeEvent_TwoHandSnapPuZone>())
            {
                other.GetComponent<TakeEvent_ToResetPosition>().isEntry = true;
                onPatientEnter.Invoke();
            }

        }
    }
}