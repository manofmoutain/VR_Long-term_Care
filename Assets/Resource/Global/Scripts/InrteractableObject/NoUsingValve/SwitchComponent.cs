using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractableObject
{
    public class SwitchComponent : MonoBehaviour
    {
        public void SwitchCollider(GameObject go)
        {
            go.GetComponent<Collider>().enabled = !go.GetComponent<Collider>().enabled;
        }

        public void SwtichGameObject(GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }
    }
}

