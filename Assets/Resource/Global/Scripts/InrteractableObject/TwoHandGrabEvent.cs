using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using UnityEngine;
using UnityEngine.Events;

namespace InrteractableObject
{
    [RequireComponent( typeof( Interactable ) )]
    public class TwoHandGrabEvent : MonoBehaviour
    {
        public UnityEvent onHoverBegin;
        public UnityEvent onHoverEnd;
        public UnityEvent attachToHand;
        public UnityEvent detachToHand;
        [SerializeField] private GameObject AttachObjectLeft;
        [SerializeField] private GameObject AttachObjectRight;
        [SerializeField] private Hand playerHand_L;
        [SerializeField] private Hand playerHand_R;


        private void OnHandHoverBegin()
        {
            onHoverBegin.Invoke();
        }


        //-------------------------------------------------
        private void OnHandHoverEnd()
        {
            onHoverEnd.Invoke();
        }


        //-------------------------------------------------
        private void OnAttachedToHand(Hand hand)
        {
            switch (hand.name)
            {
                case "LeftHand":
                    playerHand_L = hand;
                    AttachObjectLeft = playerHand_L.currentAttachedObject.gameObject;
                    break;
                case "RightHand":
                    playerHand_R = hand;
                    AttachObjectRight = playerHand_R.currentAttachedObject.gameObject;
                    break;
            }

            attachToHand.Invoke();
        }


        //-------------------------------------------------
        private void OnDetachedFromHand(Hand hand)
        {
            switch (hand.name)
            {
                case "LeftHand":

                    AttachObjectLeft = null;
                    playerHand_L = null;
                    break;
                case "RightHand":
                    AttachObjectRight = null;
                    playerHand_R = null;
                    break;
            }

            detachToHand.Invoke();
        }
    }
}