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
            if (other.GetComponent<TakeEvent_SingleHandSnapPutZone>() || other.GetComponent<TakeEvent_TwoHandSnapPutZone>() || other.GetComponent<TakeEvent_TwoHandGrab>())
            {
                print($"碰觸到了{gameObject.name}");
                other.GetComponent<TakeEvent_ToResetPosition>().isEntry = true;
                onPatientEnter.Invoke();
            }

        }
    }
}