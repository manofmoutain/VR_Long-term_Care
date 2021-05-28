using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractableObject
{
    public class SwitchVFX : MonoBehaviour
    {
        public void SwitchOnVFX(GameObject vfx)
        {
            vfx.SetActive(true);
        }

        public void SwitchOffVFX(GameObject vfx)
        {
            vfx.SetActive(false);
        }
    }
}

