using System;
using UnityEngine;

namespace PrepareMeal
{
    public class Faucet : MonoBehaviour
    {
        [SerializeField] private bool isFlowing;
        [SerializeField] private float timer;
        [SerializeField] private GameObject hands;

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

