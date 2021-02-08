using System;
using UnityEngine;

namespace InrteractableObject
{
    public class OnTriggerResetZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ResetPosition>())
            {
                throw new NotImplementedException();
            }
        }
    }
}