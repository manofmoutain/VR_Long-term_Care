using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractableObject
{
    public class AnimatorTrigger : MonoBehaviour
    {
        [SerializeField] Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

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

