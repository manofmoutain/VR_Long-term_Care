using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatientSpit : MonoBehaviour
    {
        [Header("音效")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip spit;


        [Header("生成物件")] [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject bean;

        /// <summary>
        /// 是否處於哽塞狀態
        /// </summary>
        [SerializeField] private bool isChoking;

        /// <summary>
        /// 施作次數
        /// </summary>
        [SerializeField] private int pushCount;

        private void Start()
        {
            isChoking = true;
        }

        void Spit()
        {
            if (pushCount > 5 && isChoking)
            {
                int random = Random.Range(0, 10);
                if (random >= 7)
                {
                    GameObject go = Instantiate(bean, spawnPoint.position, Quaternion.identity, transform);
                    go.AddComponent<Rigidbody>();
                    go.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 3);
                    Destroy(go, 3f);
                    isChoking = false;
                }
            }
        }

        public void Push()
        {
            // pushCount = triggerCount.GetComponent<OnTriggerEnterCount>().Count;
            pushCount++;
            Spit();
        }
    }
}

