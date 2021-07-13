using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace PrepareMeal
{
    public class PotionSink : MonoBehaviour
    {
        public bool isInject;

        [SerializeField] private float moveSpeed;
        private float timer;


        /// <summary>
        /// 針筒
        /// </summary>
        [SerializeField] private GameObject Syringe;

        [SerializeField] private GameObject plug;
        [SerializeField] private float start;
        [SerializeField] private float end;

        private void OnEnable()
        {
            transform.localScale = Vector3.one;
            timer = 0;
        }

        private void Update()
        {
            if (Syringe.transform.position.y>plug.transform.position.y && isInject)
            {
                timer += Time.deltaTime;
                transform.localScale = new Vector3(1,Mathf.Lerp(start,end,timer*moveSpeed),1);
                if (transform.localScale.y==end)
                {
                    isInject = false;
                    gameObject.SetActive(false);
                }
            }
        }


        public void InputWater()
        {
            timer += Time.deltaTime;
            transform.localScale = new Vector3(1,Mathf.Lerp(0,1,timer*moveSpeed),1);
            start = transform.localScale.y;
        }
    }
}

