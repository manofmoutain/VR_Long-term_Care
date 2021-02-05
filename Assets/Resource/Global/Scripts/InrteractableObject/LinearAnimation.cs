using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace InrteractableObject
{
    public class LinearAnimation : MonoBehaviour
    {

        public LinearMapping linearMapping;
        [SerializeField] private Animator animator;

        [SerializeField] private float currentLinearMapping = float.NaN;
        [SerializeField] private int framesUnchanged = 0;


        private void Awake()
        {
            if (animator==null)
            {
                animator.GetComponent<Animator>();
            }

            animator.speed = 0.0f;

            if (linearMapping==null)
            {
                linearMapping = GetComponent<LinearMapping>();
            }
        }


        private void Update()
        {
            if (currentLinearMapping!=linearMapping.value)
            {
                currentLinearMapping = linearMapping.value;
                animator.enabled = true;
                animator.Play(0, 0 , currentLinearMapping);
                framesUnchanged = 0;
            }
            else
            {
                framesUnchanged++;
                if (framesUnchanged>2)
                {
                    animator.enabled = false;
                }
            }
        }
    }
}