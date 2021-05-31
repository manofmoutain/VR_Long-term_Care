using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    [RequireComponent(typeof(MyInteractable))]
    [RequireComponent(typeof(Collider))]
    public class Interact_HoverEvents : MonoBehaviour
    {
        public UnityEvent onHandHoverBegin;
        public UnityEvent onHandHoverEnd;
        public UnityEvent onAttachedToHand;
        public UnityEvent onDetachedFromHand;
        public UnityEvent handAttachedUpdate;

        //-------------------------------------------------
        private void OnHandHoverBegin()
        {
            onHandHoverBegin.Invoke();
        }


        //-------------------------------------------------
        private void OnHandHoverEnd()
        {
            onHandHoverEnd.Invoke();
        }


        //-------------------------------------------------
        private void OnAttachedToHand( Hand hand )
        {
            onAttachedToHand.Invoke();
        }


        //-------------------------------------------------
        private void OnDetachedFromHand( Hand hand )
        {
            onDetachedFromHand.Invoke();
        }

        void HandAttachedUpdate(Hand hand)
        {
            handAttachedUpdate.Invoke();
        }
    }
}

