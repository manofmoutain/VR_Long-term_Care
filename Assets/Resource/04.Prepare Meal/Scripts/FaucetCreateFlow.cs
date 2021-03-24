using System;
using UnityEngine;

namespace PrepareMeal
{
    public class FaucetCreateFlow : MonoBehaviour
    {
        [SerializeField] private ParticleSystem vfx;
        [SerializeField] private bool isFlowing;
        [SerializeField] private float timer;

        private void Start()
        {
            vfx.gameObject.SetActive(false);
            isFlowing = false;
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
    }
}

