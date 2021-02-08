using System;
using UnityEngine;

namespace InrteractableObject
{
    public class OnTriggerResetZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<SnapTakeDropZone>())
            {
                other.GetComponent<ResetPosition>().isEntry = true;
            }
        }
    }
}