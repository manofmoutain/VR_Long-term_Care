using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrepareMeal
{
    public class Salt : MonoBehaviour
    {
        [SerializeField] private float minAngle;
        [SerializeField] private float maxAngle;
        [SerializeField] private bool isSpawn;
        [SerializeField] private Transform spwanPosition;
        [SerializeField] private GameObject salt;
        [SerializeField] private float timer;
        [SerializeField] private bool isUsingKinematic;
        [SerializeField] private Rigidbody rigidbody;


        private void Start()
        {
            if (GetComponent<Rigidbody>())
            {
                rigidbody = GetComponent<Rigidbody>();
                // rigidbody.isKinematic = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // print(transform.localEulerAngles.z);
            if (transform.eulerAngles.z >= minAngle && transform.eulerAngles.z <= maxAngle)
            {
                timer += Time.deltaTime;
                if (timer > 1.0f)
                {
                    timer = 0;
                    if (isSpawn)
                    {
                        GameObject go = Instantiate(salt,spwanPosition);
                        Destroy(go , 2);
                    }

                }

                if (isUsingKinematic)
                {
                    rigidbody.isKinematic = false;
                }
            }
        }

        public void TurnKinematic(bool switcher)
        {
            isUsingKinematic = switcher;
        }
    }
}