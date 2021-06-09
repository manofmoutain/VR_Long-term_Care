using System;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class SwitchComponent : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] private UnityEvent triggerEvent;
        [SerializeField] private UnityEvent collisionEvent;

        private void Start()
        {
            if (GetComponent<Animator>())
            {
                animator = GetComponent<Animator>();
            }
        }


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

        public void ActiveGameObject(GameObject go)
        {
            go.SetActive(true);
        }

        public void DeactiveGameObject(GameObject go)
        {
            go.SetActive(false);
        }

        public void SwitchMeshRender(bool switcher)
        {
            GetComponent<MeshRenderer>().enabled = switcher;
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

        public void SwitchRigidbodyKinematic(bool switcher)
        {
            GetComponent<Rigidbody>().isKinematic = switcher;
        }

        public void AnimatorSetTrigger(string parameter)
        {
            animator.SetTrigger(parameter);
        }

        public void AnimatorSetBool(string parameter)
        {

            animator.SetBool(parameter,!animator.GetBool(parameter));
        }

        public void ResetRotation(float value)
        {
            transform.localPosition = new Vector3(value,value,value);
        }
    }
}