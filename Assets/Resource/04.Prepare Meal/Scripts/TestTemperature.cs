using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Manager;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace PrepareMeal
{
    public class TestTemperature : MonoBehaviour
    {
        [SerializeField] private List<GameObject> riceOBJ;
        [SerializeField] private Hand grabHand;

        private void Start()
        {
            grabHand = null;
        }

        void TestingTemp(int index)
        {
            if (riceOBJ.Count>0)
            {
                for (int i = 0; i < riceOBJ.Count; i++)
                {
                    if (riceOBJ[i].activeSelf)
                    {
                        riceOBJ.Clear();
                        ScoreManager.Instance.DecreaseOperateSteps(index);
                    }
                }
            }

        }

        private void OnCollisionEnter(Collision other)
        {
            if (grabHand != null)
            {
                if (grabHand.name=="LeftHand")
                {
                    if (other.gameObject.name == "HandColliderRight(Clone)")
                    {
                        TestingTemp(10);
                    }
                }
                else
                {
                    if (other.gameObject.name == "HandColliderLeft(Clone)")
                    {
                        TestingTemp(10);
                    }
                }

            }
        }

        void OnHandHoverBegin(Hand hand)
        {
            // TestingTemp(10);
        }

        void OnAttachedToHand(Hand hand)
        {
            grabHand = hand;
        }

        void OnDetachedFromHand(Hand hand)
        {
            grabHand = null;
        }
    }
}