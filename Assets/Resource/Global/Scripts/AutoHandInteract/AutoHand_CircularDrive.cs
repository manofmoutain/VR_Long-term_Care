﻿using System;
using System.Collections;
using Autohand;
using UnityEngine;
using UnityEngine.Events;
using InteractableObject;
using Hand = Autohand.Hand;

namespace AutoHandInteract
{
    [RequireComponent(typeof(AutoHand_Grabbable))]
    public class AutoHand_CircularDrive : MonoBehaviour
    {
        public enum Axis_t
        {
            XAxis,
            YAxis,
            ZAxis
        };

        private Grabbable grabbable;

        /// <summary>
        /// 抓握的是哪一隻手
        /// </summary>
        [Header("手勢")] [SerializeField] private Hand grabbedHand;

        /// <summary>
        /// 是否正在驅動
        /// </summary>
        [Header("旋轉角度")] public bool driving = false;

        [Tooltip("驅動器的輸出角度值（以度為單位，無限制）將無限制地增加或減少，採用360模數來查找轉數")]
        public float outAngle;

        /// <summary>
        /// 給予Animator的值(0-1)
        /// </summary>
        public float Value => linearMapping.value;

        [Header("驅動設定")] [Tooltip("圓形驅動器將在局部空間中繞其旋轉的軸")]
        public Axis_t axisOfRotation = Axis_t.XAxis;

        /// <summary>
        /// 是否可以轉動
        /// </summary>
        [Tooltip("是否可以轉動")] public bool rotateGameObject = true;

        // /// <summary>
        // /// 鬆手後是否要轉回原角度
        // /// </summary>
        // [SerializeField] private bool isDetachedToResetAngle;

        [Tooltip("具有Collider組件以啟動交互的子GameObject，僅當存在多個Collider子對象時才需要設置")]
        private Collider childCollider;

        [Tooltip("要驅動的LinearMapping組件（如果未指定）將動態添加到此GameObject中。")]
        private Interact_LinearMapping linearMapping;

       /// <summary>
        /// 是否要設定極限值
        /// </summary>
        [Header("旋轉的極限值")] [Tooltip("如果limited為true，則旋轉將限制為[minAngle，maxAngle]；如果為false，則旋轉將不受限制")]
        public bool limited;

        // /// <summary>
        // /// 如果limited為true，旋轉的極限值
        // /// </summary>
        // [Tooltip("如果limited為true，旋轉的極限值")] public Vector2 frozenDistanceMinMaxThreshold = new Vector2(0.1f, 0.2f);
        //
        // /// <summary>
        // /// 如果旋轉值>=frozenDistanceMinMaxThreshold的Y值，執行事件
        // /// </summary>
        // public UnityEvent onFrozenDistanceThreshold;

        /// <summary>
        /// 極限值的最小值
        /// </summary>
        [Header("旋轉值極限值的最小值")] [Tooltip("如果Limited為true，則指定下限，否則未使用值")][HideInInspector]
        public float minAngle;

        /// <summary>
        /// 在最小值時的事件
        /// </summary>
        [Tooltip("如果Limited，則在到達minAngle時調用事件")][HideInInspector]
        public UnityEvent onMinAngle;

        /// <summary>
        /// 極限值的最大值
        /// </summary>
        [Header("旋轉極限值的最大值")] [Tooltip("如果limited為true，則指定上限，否則未使用值")][HideInInspector]
        public float maxAngle;

        /// <summary>
        /// 在最大值時事件
        /// </summary>
        [Tooltip("如果Limited，則在達到maxAngle時調用事件")][HideInInspector]
        public UnityEvent onMaxAngle;


        /// <summary>
        /// 是否要開啟角度事件，不可與Limit共用
        /// </summary>
        [Header("特定角度的事件")] [Tooltip("是否要開啟角度事件，不可與Limit共用")] [SerializeField]
        public bool isAngleEvent;

        /// <summary>
        /// 執行事件的角度值
        /// </summary>
        [Tooltip("執行事件的角度值")][HideInInspector] public float eventAngle;

        /// <summary>
        /// 當角度為eventAngle值時，執行事件
        /// </summary>
        [SerializeField][HideInInspector] private UnityEvent angleEvent;

        /// <summary>
        /// 在limited的情況中，是否要強制設定初始值
        /// </summary>
        [Header("強制改變初始值")] [Tooltip("如果limited為true，則強制將起始角度設為startAngle，並固定為[minAngle，maxAngle]")]
        public bool forceStart;

        /// <summary>
        /// 強制設定的初始旋轉值
        /// </summary>
        [Tooltip("如果limited為true且forceStart為true，則起始角度將為此角度，並固定為[minAngle，maxAngle]")][HideInInspector][SerializeField]
        private float startAngle;


        [Header("控制三維的值")] public bool isUnhiddenHiddenVector3;
        [HideInInspector] [SerializeField] private Quaternion start;
        [HideInInspector] [SerializeField] private Vector3 worldPlaneNormal;
        [HideInInspector] [SerializeField] private Vector3 localPlaneNormal;
        [HideInInspector] [SerializeField] private Vector3 lastHandProjected;

        /// <summary>
        /// 如果驅動器被限制為最小/最大，則大於此角度的角度將被忽略
        /// </summary>
        [HideInInspector] [SerializeField] private float minMaxAngularThreshold = 1.0f;


        private void Awake()
        {
            grabbable = GetComponent<Grabbable>();
        }

        private void Start()
        {
            grabbable.OnBeforeGrabEvent += OnBeforeGrab;
            grabbable.OnGrabEvent += OnGrab;
            grabbable.OnReleaseEvent += OnRelease;



            if (childCollider == null)
            {
                childCollider = GetComponentInChildren<Collider>();
            }

            if (linearMapping == null && !GetComponent<Interact_LinearMapping>())
            {
                linearMapping = gameObject.AddComponent<Interact_LinearMapping>();
                linearMapping = GetComponent<Interact_LinearMapping>();
            }
            else if (linearMapping == null && GetComponent<Interact_LinearMapping>())
            {
                linearMapping = GetComponent<Interact_LinearMapping>();
            }



            worldPlaneNormal = new Vector3(0.0f, 0.0f, 0.0f);
            worldPlaneNormal[(int) axisOfRotation] = 0.005f;


            localPlaneNormal = worldPlaneNormal;

            if (transform.parent)
            {
                worldPlaneNormal = transform.parent.localToWorldMatrix.MultiplyVector(worldPlaneNormal).normalized;
            }

            if (limited)
            {
                start = Quaternion.identity;
                outAngle = transform.localEulerAngles[(int) axisOfRotation];

                if (forceStart)
                {
                    outAngle = Mathf.Clamp(startAngle, minAngle, maxAngle);
                }
            }
            else
            {
                start = Quaternion.AngleAxis(transform.localEulerAngles[(int) axisOfRotation], localPlaneNormal);
                outAngle = 0.0f;
            }

            UpdateAll();
        }

        private void Update()
        {

        }

        void OnDisable()
        {
            if (grabbedHand)
            {
                grabbedHand = null;
            }
        }


        void OnBeforeGrab(Hand hand, Grabbable grabbable)
        {
            // GetComponent<Rigidbody>().isKinematic = true;
        }

        void OnGrab(Hand hand, Grabbable grabbable)
        {
            // print($"{hand.name} grabbed");
            // lastHandProjected = ComputeToTransformProjected(hand.transform);

            grabbedHand = hand;

            // driving = true;

            // UpdateLinearMapping();
            // ComputeAngle(hand);
            // UpdateAll();

            // if (driving && !hand.IsGrabbing())
            // {
            //     ComputeAngle(hand);
            //     UpdateAll();
            // }
        }

        void OnRelease(Hand hand, Grabbable grabbable)
        {
            print($"{hand.name} released");

            StartCoroutine(HapticPulses(hand, 1.0f, 10));

            // GetComponent<Rigidbody>().isKinematic = false;
            driving = false;
            grabbedHand = null;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<Hand>())
            {

                if (grabbedHand != null)
                {
                    print($"{grabbedHand.IsGrabbing()}");
                    // if (driving)
                    // {
                    //     // print($"IsGrabbing : {grabbedHand.IsGrabbing()}");
                    //     lastHandProjected = ComputeToTransformProjected(grabbedHand.transform);
                    //     grabbable.body.isKinematic = false;
                    //     UpdateLinearMapping();
                    //     ComputeAngle(grabbedHand);
                    //     UpdateAll();
                    // }
                    // else
                    // {
                    //     // print($"IsGrabbing : {grabbedHand.IsGrabbing()}");
                    //     grabbable.body.isKinematic = true;
                    // }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Hand>())
            {
                driving = false;
                // grabbedHand = null;
            }
        }


        #region 計算的方法

        private IEnumerator HapticPulses(Hand hand, float flMagnitude, int nCount)
        {
            if (hand != null)
            {
                int nRangeMax = (int) AutoHand_Util.RemapNumberClamped(flMagnitude, 0.0f, 1.0f, 100.0f, 900.0f);
                nCount = Mathf.Clamp(nCount, 1, 10);

                //float hapticDuration = nRangeMax * nCount;

                //hand.TriggerHapticPulse(hapticDuration, nRangeMax, flMagnitude);

                for (ushort i = 0; i < nCount; ++i)
                {
                    // ushort duration = (ushort) Random.Range(100, nRangeMax);
                    // hand.TriggerHapticPulse(duration);
                    yield return new WaitForSeconds(.01f);
                }
            }
        }

        /// <summary>
        ///計算旋轉值
        /// </summary>
        /// <param name="xForm"></param>
        /// <returns></returns>
        [SerializeField]
        private Vector3 ComputeToTransformProjected(Transform xForm)
        {
            Vector3 toTransform = (xForm.position - transform.position).normalized;
            Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

            // Need a non-zero distance from the hand to the center of the CircularDrive
            if (toTransform.sqrMagnitude > 0.0f)
            {
                toTransformProjected = Vector3.ProjectOnPlane(toTransform, worldPlaneNormal).normalized;
            }
            else
            {
                Debug.LogFormat(
                    "<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}",
                    gameObject.ToString());
                Debug.Assert(false,
                    string.Format(
                        "<b>[SteamVR Interaction]</b> The collider needs to be a minimum distance away from the CircularDrive GameObject {0}",
                        gameObject.ToString()));
            }

            // if (debugPath && dbgPathLimit > 0)
            // {
            //     DrawDebugPath(xForm, toTransformProjected);
            // }

            return toTransformProjected;
        }


        /// <summary>
        /// 從角度更新LinearMapping值
        /// </summary>
        private void UpdateLinearMapping()
        {
            if (limited)
            {
                // Map it to a [0, 1] value
                linearMapping.value = (outAngle - minAngle) / (maxAngle - minAngle);
            }
            else
            {
                // Normalize to [0, 1] based on 360 degree windings
                float flTmp = outAngle / 360.0f;
                linearMapping.value = flTmp - Mathf.Floor(flTmp);
            }

            // UpdateDebugText();
        }


        /// <summary>
        /// 持續更新物件的旋轉值
        /// </summary>
        private void UpdateGameObject()
        {
            if (rotateGameObject)
            {
                // print($"localPlaneNormal : {localPlaneNormal}");
                transform.localRotation = start * Quaternion.AngleAxis(outAngle, localPlaneNormal);
            }
        }


        /// <summary>
        /// 使用線性映射值和角度更新
        /// </summary>
        private void UpdateAll()
        {
            UpdateLinearMapping();
            UpdateGameObject();
            // UpdateDebugText();
        }


        /// <summary>
        /// 根據變換的變化計算旋轉遊戲對象的角度
        /// </summary>
        /// <param name="hand"></param>
        private void ComputeAngle(Hand hand)
        {
            Vector3 toHandProjected;

            toHandProjected = ComputeToTransformProjected(hand.follow);
            // print($"toHandProjected : {toHandProjected} ; lastHandProjected : {lastHandProjected}");

            if (!toHandProjected.Equals(lastHandProjected))
            {
                float absAngleDelta = Vector3.Angle(lastHandProjected, toHandProjected);
                // print($"absAngleDelta : {absAngleDelta}");

                if (absAngleDelta > 0.0f)
                {
                    Vector3 cross = Vector3.Cross(lastHandProjected, toHandProjected).normalized;
                    float dot = Vector3.Dot(worldPlaneNormal, cross);

                    float signedAngleDelta = absAngleDelta;

                    if (dot < 0.0f)
                    {
                        signedAngleDelta = -signedAngleDelta;
                    }

                    //如果設定存在極限值
                    if (limited)
                    {
                        float angleTmp = Mathf.Clamp(outAngle + signedAngleDelta, minAngle, maxAngle);

                        if (outAngle == minAngle)
                        {
                            if (angleTmp > minAngle && absAngleDelta < minMaxAngularThreshold)
                            {
                                outAngle = angleTmp;
                                lastHandProjected = toHandProjected;
                            }
                        }
                        else if (outAngle == maxAngle)
                        {
                            if (angleTmp < maxAngle && absAngleDelta < minMaxAngularThreshold)
                            {
                                outAngle = angleTmp;
                                lastHandProjected = toHandProjected;
                            }
                        }
                        else if (angleTmp == minAngle)
                        {
                            outAngle = angleTmp;
                            lastHandProjected = toHandProjected;
                            onMinAngle.Invoke();
                        }
                        else if (angleTmp == maxAngle)
                        {
                            outAngle = angleTmp;
                            lastHandProjected = toHandProjected;
                            onMaxAngle.Invoke();
                        }
                        else
                        {
                            outAngle = angleTmp;
                            lastHandProjected = toHandProjected;
                        }
                    }
                    else
                    {
                        outAngle += signedAngleDelta;
                        if (isAngleEvent && outAngle >= eventAngle)
                        {
                            angleEvent.Invoke();
                        }

                        lastHandProjected = toHandProjected;
                    }
                }
            }
        }

        #endregion
    }
}