using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class AnimatorTrigger : MonoBehaviour
    {
        [SerializeField] Animator animator;
        // [SerializeField] private UnityEvent animatorTrigger;
        // [SerializeField] private UnityEvent animatorBool;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.GetComponent<Interact_TriggerComponent>())
        //     {
        //         animatorTrigger.Invoke();
        //         animatorBool.Invoke();
        //     }
        // }

        public void SetAnimatorTrigger(string parameter)
        {
            animator.SetTrigger(parameter);
        }

        public void SetAnimatorBool(string parameter)
        {
            animator.SetBool(parameter,!animator.GetBool(parameter));
        }
    }
}

