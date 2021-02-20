using System;
using UnityEngine;

namespace InrteractableObject
{
    public class TakeEvent_ResetZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<TakeEvent_SnapPutZone>())
            {
                other.GetComponent<TakeEvent_ToResetPosition>().isEntry = true;
            }
        }
    }
}