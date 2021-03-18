using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼
using Valve.VR.InteractionSystem;
#endif


namespace InteractableObject
{
    public class Interact_LinearAnimation : MonoBehaviour
    {
        public LinearMapping linearMappingGameObject;
        [SerializeField] private Animator animator;

        public float currentLinearMapping = float.NaN;
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
            if (currentLinearMapping != linearMappingGameObject.value)
            {
                currentLinearMapping = linearMappingGameObject.value;
                animator.enabled = true;
                animator.Play(0, 0, currentLinearMapping);
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