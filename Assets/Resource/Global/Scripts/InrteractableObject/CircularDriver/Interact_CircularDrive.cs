using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

namespace InteractableObject
{
    [RequireComponent(typeof(Interactable))]
    public class Interact_CircularDrive : MonoBehaviour
    {
        public enum Axis_t
        {
            XAxis,
            YAxis,
            ZAxis
        };

        /// <summary>
        /// 抓握的是哪一隻手
        /// </summary>
        [SerializeField] private Hand grabHand;

        /// <summary>
        /// 手勢標誌
        /// </summary>
        [SerializeField] private Hand.AttachmentFlags attachmentFlags;

        /// <summary>
        /// 設定抓握的方式
        /// </summary>
        [Tooltip("設定抓握的方式")] [SerializeField] private GrabTypes grabbedWithType;

        /// <summary>
        /// 旋轉方式是否為定點旋轉(true=定點旋轉，false=環繞旋轉)
        /// </summary>
        [Tooltip("旋轉方式是否為定點旋轉(true=定點旋轉，false=環繞旋轉)")] [SerializeField]
        private bool isRoot = true;

        /// <summary>
        /// 是否正在驅動
        /// </summary>
        [SerializeField] private bool driving = false;

        [Tooltip("驅動器的輸出角度值（以度為單位，無限制）將無限制地增加或減少，採用360模數來查找轉數")]
        public float outAngle;

        /// <summary>
        /// 給予Animator的值(0-1)
        /// </summary>
        public float Value => linearMapping.value;

        [Tooltip("圓形驅動器將在局部空間中繞其旋轉的軸")] public Axis_t axisOfRotation = Axis_t.XAxis;

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

        [Tooltip("要驅動的LinearMapping組件（如果未指定）將動態添加到此GameObject中。")] [SerializeField]
        private Interact_LinearMapping linearMapping;

        /// <summary>
        /// 是否只要抓住，驅動器就持續處於操作狀態，否則控制器移出碰撞體，驅動器將停止運行
        /// </summary>
        [Tooltip("如果為true，則只要按住該按鈕，驅動器將一直處於操作狀態；如果為false，則如果控制器移出對撞機，驅動器將停止運行。")]
        public bool hoverLock = true;

        /// <summary>
        /// 是否要設定極限值
        /// </summary>
        [Tooltip("如果limited為true，則旋轉將限制為[minAngle，maxAngle]；如果為false，則旋轉將不受限制")]
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
        [Tooltip("如果Limited為true，則指定下限，否則未使用值")] [HideInInspector]
        public float minAngle;

        /// <summary>
        /// 在最小值時的事件
        /// </summary>
        [Tooltip("如果Limited，則在到達minAngle時調用事件")] [HideInInspector]
        public UnityEvent onMinAngle;

        /// <summary>
        /// 極限值的最大值
        /// </summary>
        [Tooltip("如果limited為true，則指定上限，否則未使用值")] [HideInInspector]
        public float maxAngle;

        /// <summary>
        /// 在最大值時事件
        /// </summary>
        [Tooltip("如果Limited，則在達到maxAngle時調用事件")] [HideInInspector]
        public UnityEvent onMaxAngle;

        /// <summary>
        /// 是否要開啟角度事件，不可與Limit共用
        /// </summary>
        [Tooltip("是否要開啟角度事件，不可與Limit共用")] public bool isAngleEvent;

        /// <summary>
        /// 執行事件的角度值
        /// </summary>
        [HideInInspector] [Tooltip("執行事件的角度值")]
        public float eventAngle;

        /// <summary>
        /// 當角度為eventAngle值時，執行事件
        /// </summary>
        [HideInInspector] [SerializeField] private UnityEvent angleEvent;

        /// <summary>
        /// 在limited的情況中，是否要強制設定初始值
        /// </summary>
        [Tooltip("如果limited為true，則強制將起始角度設為startAngle，並固定為[minAngle，maxAngle]")]
        public bool forceStart;

        /// <summary>
        /// 強制設定的初始旋轉值
        /// </summary>
        [Tooltip("如果limited為true且forceStart為true，則起始角度將為此角度，並固定為[minAngle，maxAngle]")]
        public float startAngle;


        [SerializeField] private Quaternion startQuaternion;
        [SerializeField] private Vector3 worldPlaneNormal;
        [SerializeField] private Vector3 localPlaneNormal;
        [SerializeField] private Vector3 lastHandProjected;

        /// <summary>
        /// 如果驅動器被限制為最小/最大，則大於此角度的角度將被忽略
        /// </summary>
        [HideInInspector] [SerializeField]private float minMaxAngularThreshold = 1.0f;

        [HideInInspector] [SerializeField]private Interactable interactable;


        private void Awake()
        {
            interactable = GetComponent<Interactable>();
        }

        private void Start()
        {
            if (childCollider == null)
            {
                childCollider = GetComponentInChildren<Collider>();
            }

            if (linearMapping == null)
            {
                linearMapping = GetComponent<Interact_LinearMapping>();
            }

            if (linearMapping == null)
            {
                linearMapping = gameObject.AddComponent<Interact_LinearMapping>();
            }

            worldPlaneNormal = new Vector3(0.0f, 0.0f, 0.0f);
            worldPlaneNormal[(int) axisOfRotation] = 1f;


            localPlaneNormal = worldPlaneNormal;

            if (transform.parent)
            {
                worldPlaneNormal = transform.parent.localToWorldMatrix.MultiplyVector(worldPlaneNormal).normalized;
            }

            if (limited)
            {
                startQuaternion = Quaternion.identity;
                outAngle = transform.localEulerAngles[(int) axisOfRotation];

                if (forceStart)
                {
                    outAngle = Mathf.Clamp(startAngle, minAngle, maxAngle);
                }
            }
            else
            {
                startQuaternion = Quaternion.AngleAxis(transform.localEulerAngles[(int) axisOfRotation], localPlaneNormal);
                outAngle = 0.0f;
            }

            UpdateLinearMapping();
            UpdateGameObject();
        }

        void OnDisable()
        {
            if (grabHand)
            {
                grabHand.HideGrabHint();
                grabHand.HoverUnlock(interactable);
                grabHand = null;
            }
        }


        private void OnHandHoverBegin(Hand hand)
        {
            // hand.ShowGrabHint();
        }


        private void OnHandHoverEnd(Hand hand)
        {
            // hand.HideGrabHint();

            if (driving && hand)
            {
                //hand.TriggerHapticPulse() //todo: fix
                StartCoroutine(HapticPulses(hand, 1.0f, 10));
            }

            driving = false;
            grabHand = null;
        }


        private void HandHoverUpdate(Hand hand)
        {
            //抓握的狀態
            GrabTypes startingGrabType = hand.GetGrabStarting();
            //是否已鬆手
            bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;

            //如果沒有執行任何的抓握
            if (grabbedWithType == GrabTypes.None && startingGrabType != GrabTypes.None)
            {
                grabbedWithType = startingGrabType;
                // Trigger was just pressed
                if (isRoot)
                {
                    lastHandProjected = ComputeToTransformProjected(hand.transform);
                }
                else
                {
                    lastHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);
                }


                if (hoverLock)
                {
                    hand.HoverLock(interactable);
                    grabHand = hand;
                }

                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
                driving = true;

                ComputeAngle(hand);
                UpdateLinearMapping();
                UpdateGameObject();

                hand.HideGrabHint();
            }
            //如果鬆手
            else if (grabbedWithType != GrabTypes.None && isGrabEnding)
            {
                // hand.DetachObject(gameObject);
                // Trigger was just released
                if (hoverLock)
                {
                    hand.HoverUnlock(interactable);
                    grabHand = null;
                }

                driving = false;
                grabbedWithType = GrabTypes.None;
            }

            if (driving && isGrabEnding == false && hand.hoveringInteractable == this.interactable)
            {
                ComputeAngle(hand);
                UpdateLinearMapping();
                UpdateGameObject();
            }
        }

        void HandAttachedUpdate(Hand hand)
        {
            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject);
            }
        }


        void OnDetachedFromHand(Hand hand)
        {
            if (hoverLock)
            {
                hand.HoverUnlock(interactable);
                grabHand = null;
            }

            driving = false;
            grabbedWithType = GrabTypes.None;
        }


        #region 計算的方法

        private IEnumerator HapticPulses(Hand hand, float flMagnitude, int nCount)
        {
            if (hand != null)
            {
                int nRangeMax = (int) Util.RemapNumberClamped(flMagnitude, 0.0f, 1.0f, 100.0f, 900.0f);
                nCount = Mathf.Clamp(nCount, 1, 10);

                //float hapticDuration = nRangeMax * nCount;

                //hand.TriggerHapticPulse(hapticDuration, nRangeMax, flMagnitude);

                for (ushort i = 0; i < nCount; ++i)
                {
                    ushort duration = (ushort) Random.Range(100, nRangeMax);
                    hand.TriggerHapticPulse(duration);
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
                transform.localRotation = startQuaternion * Quaternion.AngleAxis(outAngle, localPlaneNormal);
                // print($"localRotation : {transform.localRotation}");
            }
        }


        /// <summary>
        /// 使用線性映射值和角度更新
        /// </summary>
        private void UpdateAll()
        {
            // UpdateLinearMapping();
            // UpdateGameObject();
            // UpdateDebugText();
        }


        /// <summary>
        /// 根據變換的變化計算旋轉遊戲對象的角度
        /// </summary>
        /// <param name="hand"></param>
        private void ComputeAngle(Hand hand)
        {
            Vector3 toHandProjected;
            if (isRoot)
            {
                toHandProjected = ComputeToTransformProjected(hand.transform);
            }
            else
            {
                toHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);
            }

            // print($"lastHandProjected : {lastHandProjected} ; toHandProjected : {toHandProjected}");
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
                            // print($"angleTmp : {angleTmp} ; absAngleDelta : {absAngleDelta}");
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