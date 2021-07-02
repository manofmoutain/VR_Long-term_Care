using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    [RequireComponent(typeof(MyInteractable))]
    public class Interact_LinearDrive : MonoBehaviour
    {
        public bool isUsingTwoHands;
        [HideInInspector] [SerializeField] private Hand rightHand;
        [HideInInspector] [SerializeField] private GameObject rightAttachedObjet;
        [HideInInspector] [SerializeField] private Hand leftHand;
        [HideInInspector] [SerializeField] private GameObject leftAttachedObject;

        /// <summary>
        /// 鬆開手後是否回到原位
        /// </summary>
        [SerializeField] private bool isDetachToResetPosition;

        public Transform startPosition;
        public Transform endPosition;
        public Interact_LinearMapping linearMapping;
        public bool repositionGameObject = true;
        public bool maintainMomemntum = true;
        public float momemtumDampenRate = 5.0f;

        [SerializeField] protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;
        [Tooltip("保持時用作位置和旋轉偏移量的局部點")]
        public Transform attachmentOffset;

        [SerializeField] protected float initialMappingOffset;
        [SerializeField] protected int numMappingChangeSamples = 5;
        [SerializeField] protected float[] mappingChangeSamples;
        [SerializeField] protected float prevMapping = 0.0f;
        [SerializeField] protected float mappingChangeRate;
        [SerializeField] protected int sampleCount = 0;

        [SerializeField] protected Interactable interactable;


        protected virtual void Awake()
        {
            mappingChangeSamples = new float[numMappingChangeSamples];
            interactable = GetComponent<Interactable>();
        }

        protected virtual void Start()
        {
            rightHand = null;
            leftHand = null;

            transform.position = startPosition.position;
            if (linearMapping == null)
            {
                linearMapping = GetComponent<Interact_LinearMapping>();
            }

            // if (linearMapping == null)
            // {
            //     linearMapping = gameObject.AddComponent<LinearMapping>();
            // }

            initialMappingOffset = linearMapping.value;

            if (repositionGameObject)
            {
                UpdateLinearMapping(transform);

            }
        }

        protected virtual void OnHandHoverBegin(Hand hand)
        {
            if (isUsingTwoHands)
            {
                switch (hand.gameObject.name)
                {
                    case "RightHand":
                        rightHand=hand;

                        break;
                    case "LeftHand":
                        leftHand = hand;

                        break;
                }
            }
        }

        protected virtual void OnHandHoverEnd(Hand hand)
        {
            if (isUsingTwoHands)
            {
                rightHand = null;
                leftHand = null;
            }
        }

        protected virtual void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
            {
                initialMappingOffset = linearMapping.value - CalculateLinearMapping(hand.transform);
                sampleCount = 0;
                mappingChangeRate = 0.0f;

                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);

            }
        }

        void OnAttachedToHand(Hand hand)
        {
            if (isUsingTwoHands)
            {
                switch (hand.gameObject.name)
                {
                    case "RightHand":
                        rightHand=hand;
                        rightAttachedObjet = gameObject;
                        break;
                    case "LeftHand":
                        leftHand = hand;
                        leftAttachedObject = gameObject;
                        break;
                }
            }

        }

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            if (isUsingTwoHands)
            {
                if (rightAttachedObjet != null && leftAttachedObject != null)
                {
                    UpdateLinearMapping(hand.transform);
                }
            }
            else
            {
                UpdateLinearMapping(hand.transform);
            }



            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject);
            }
        }

        protected virtual void OnDetachedFromHand(Hand hand)
        {
            if (isUsingTwoHands)
            {
                rightHand = null;
                rightAttachedObjet = null;
                leftHand = null;
                leftAttachedObject = null;
            }
            CalculateMappingChangeRate();
        }


        protected void CalculateMappingChangeRate()
        {
            if (isDetachToResetPosition)
            {
                linearMapping.value = 0;
                transform.localPosition = Vector3.zero;
            }

            //Compute the mapping change rate
            mappingChangeRate = 0.0f;
            int mappingSamplesCount = Mathf.Min(sampleCount, mappingChangeSamples.Length);
            if (mappingSamplesCount != 0)
            {
                for (int i = 0; i < mappingSamplesCount; ++i)
                {
                    mappingChangeRate += mappingChangeSamples[i];
                }

                mappingChangeRate /= mappingSamplesCount;
            }
        }

        protected void UpdateLinearMapping(Transform updateTransform)
        {
            // print("Go");
            prevMapping = linearMapping.value;
            linearMapping.value = Mathf.Clamp01(initialMappingOffset + CalculateLinearMapping(updateTransform));

            mappingChangeSamples[sampleCount % mappingChangeSamples.Length] = (1.0f / Time.deltaTime) * (linearMapping.value - prevMapping);
            sampleCount++;

            if (repositionGameObject)
            {
                transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
            }
        }

        protected float CalculateLinearMapping(Transform updateTransform)
        {
            Vector3 direction = endPosition.position - startPosition.position;
            float length = direction.magnitude;
            direction.Normalize();

            Vector3 displacement = updateTransform.position - startPosition.position;

            return Vector3.Dot(displacement, direction) / length;
        }


        protected virtual void Update()
        {
            if (maintainMomemntum && mappingChangeRate != 0.0f)
            {
                //Dampen the mapping change rate and apply it to the mapping
                mappingChangeRate = Mathf.Lerp(mappingChangeRate, 0.0f, momemtumDampenRate * Time.deltaTime);
                linearMapping.value = Mathf.Clamp01(linearMapping.value + (mappingChangeRate * Time.deltaTime));

                if (repositionGameObject)
                {
                    transform.position =
                        Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
                }
            }
        }
    }
}