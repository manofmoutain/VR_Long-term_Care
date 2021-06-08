using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class SwitchComponent : MonoBehaviour
    {
        [SerializeField] private UnityEvent triggerEvent;
        [SerializeField] private UnityEvent collisionEvent;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Interact_TriggerComponent>())
            {
                print($"Touched {other.gameObject.name}");
                triggerEvent.Invoke();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<Interact_TriggerComponent>())
            {
                print($"Touched {other.gameObject.name}");
                collisionEvent.Invoke();
            }
        }

        public void SwitchCollider(bool switcher)
        {
            GetComponent<Collider>().enabled = switcher;
        }

        public void SwitchTrigger(bool switcher)
        {
            GetComponent<Collider>().isTrigger = switcher;
        }

        public void SwtichGameObject(bool switcher)
        {
            gameObject.SetActive(switcher);
        }

        public void SwitchRiggibodyKinematic(bool switcher)
        {
            GetComponent<Rigidbody>().isKinematic = switcher;
        }
    }
}