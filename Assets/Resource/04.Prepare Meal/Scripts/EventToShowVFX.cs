using System;
using UnityEngine;

namespace PrepareMeal
{
    public class FaucetCreateFlow : MonoBehaviour
    {
        [SerializeField] private ParticleSystem vfx;
        [SerializeField] private bool isFlowing;
        [SerializeField] private float timer;
        [SerializeField] private GameObject hands;

        private void Start()
        {
            vfx.gameObject.SetActive(false);
            isFlowing = false;
            hands.SetActive(false);
        }

        private void Update()
        {
            if (isFlowing)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
            }
        }

        public void SwitchFlow(bool on)
        {
            isFlowing = on;
            vfx.gameObject.SetActive(on);
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

