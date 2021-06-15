using System;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class SwitchComponent : MonoBehaviour
    {
        private Animator animator;

        private void Start()
        {
            if (GetComponent<Animator>())
            {
                animator = GetComponent<Animator>();
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

        public void ChangeParent(Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition=Vector3.zero;
            transform.localPosition=Vector3.zero;
            transform.localScale=Vector3.one;
        }
    }
}