using System;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;
using Global.Pateint;
using InteractableObject;
using Valve.VR.InteractionSystem;

namespace Heimlich_maneuver
{
    public partial class HamlickPatient
    {
        [Header("生成物件")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject bean;
        /// <summary>
        /// 給予豆子噴出的力道
        /// </summary>
        [SerializeField] private float beanSpeed;
        /// <summary>
        /// 豆子噴出的方向
        /// </summary>
        [SerializeField] private Vector3 velocityDirection;

        /// <summary>
        /// 是否處於哽塞狀態
        /// </summary>
        public bool isChoking;

        /// <summary>
        /// 最小按壓次數
        /// </summary>
        [SerializeField] private int minPushCount;

        /// <summary>
        /// 施作次數
        /// </summary>
        [SerializeField] private int pushCount;

        [SerializeField] private GameObject pushPoint;
        [SerializeField] private Interact_LinearMapping linearMapping;


        /// <summary>
        /// 掛在線性運動抓取點的InteractableHoverEvents的OnAttachedToHand上
        /// </summary>
        /// <param name="index">考題編號</param>
        public void StartPush(int index)
        {
            if (GetComponent<HamlickPatient>().canHug)
            {
                //項目2：找出正確按壓位置
                ScoreManager.Instance.DecreaseOperateSteps(index);
                ScoreManager.Instance.SetDone(index);
            }
        }

        /// <summary>
        /// 掛在線性運動中的移動終點的onLinearInteractTriggerEvent上
        /// </summary>
        /// <param name="index">考題編號</param>
        public void Push(int index)
        {

                pushCount++;

                //項目四：握拳向內壓數次
            ScoreManager.Instance.DecreaseOperateSteps(index);
            ScoreManager.Instance.SetDone(index);
            Spit();
        }

        void Spit()
        {
            if (pushCount > minPushCount && isChoking)
            {
                int random = Random.Range(0, 10);
                if (random >= 7)
                {
                    //項目五：豆子擠出
                    ScoreManager.Instance.DecreaseOperateSteps(4);
                    ScoreManager.Instance.SetDone(4);
                    SpeechManager.Instance.StopAudio();
                    GetComponent<Patient>().PlaySFX(0);
                    GameObject go = Instantiate(bean, spawnPoint.position, Quaternion.identity,
                        GetComponent<Patient>().GetPatientTransform);
                    go.GetComponent<Rigidbody>().velocity = velocityDirection * beanSpeed;
                    if (GetComponent<AudioSource>().isPlaying)
                    {
                        GetComponent<AudioSource>().Stop();
                    }

                    if (SpeechManager.Instance.IsAudioPlaying())
                    {
                        SpeechManager.Instance.StopAudio();
                    }
                    Destroy(go, 3f);
                    GetComponent<Patient>().patient.GetComponent<TakeEvent_HandGrab>().DetachHand();
                    isChoking = false;
                }
            }
        }
    }
}