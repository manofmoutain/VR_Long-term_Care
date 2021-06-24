using System;
using UnityEngine;
using UnityEngine.Events;

namespace PrepareMeal
{
    public class Faucet : MonoBehaviour
    {
        [SerializeField] private bool isFlowing;
        [SerializeField] private float timer;
        [SerializeField] private GameObject hands;
        [SerializeField] private UnityEvent overFlowingEvent;

        private void Start()
        {
            isFlowing = false;
            hands.SetActive(false);
        }

        private void Update()
        {
            if (isFlowing)
            {
                timer += Time.deltaTime;
                if (timer>120.0f)
                {
                    overFlowingEvent.Invoke();
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }

        public void SwitchFlowing(bool switcher)
        {
            isFlowing = switcher;
        }


        public void WashHand()
        {
            if (isFlowing)
            {
                hands.SetActive(true);
            }
        }
    }
}

