using System.Collections;
using Manager;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    [RequireComponent( typeof( Interactable ) )]
    public class WashHands : MonoBehaviour
    {
        [SerializeField] private GameObject hands;
        [SerializeField] private Animator animator;

        void Start()
        {
            hands.SetActive(false);
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        public void HandWash()
        {
            animator.SetTrigger("Pull");
            StartCoroutine(WashHand());
        }

        IEnumerator WashHand()
        {
            yield return new WaitForSeconds(0.5f);
            hands.SetActive(true);
            yield return new WaitForSeconds(3.5f);
            hands.SetActive(false);
        }
    }
}