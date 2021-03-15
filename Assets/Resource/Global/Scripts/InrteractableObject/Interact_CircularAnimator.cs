using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractableObject
{
    public class Interact_CircularAnimator : MonoBehaviour
    {
        public GameObject circularDriveGameObject;
        [SerializeField] private Animator animator;

        public float currentCircularMapping = float.NaN;
        [SerializeField] private int framesUnchanged = 0;


        private void Awake()
        {
            if (animator == null)
            {
                animator.GetComponent<Animator>();
            }

            animator.speed = 0.0f;
        }


        private void Update()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
 // Oculus Quest代碼

#else
// SteamVR代碼
            if (currentCircularMapping != circularDriveGameObject.GetComponent<Interact_CircularDrive>().linearMapping.value)
            {
                currentCircularMapping = circularDriveGameObject.GetComponent<Interact_CircularDrive>().linearMapping.value;
                animator.enabled = true;
                animator.Play(0, 0, currentCircularMapping);
                framesUnchanged = 0;
            }
            else
            {
                framesUnchanged++;
                if (framesUnchanged > 2)
                {
                    animator.enabled = false;
                }
            }
#endif
        }
    }
}

