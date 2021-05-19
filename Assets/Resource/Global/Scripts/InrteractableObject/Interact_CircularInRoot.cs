using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

namespace InteractableObject
{
    public class Interact_CircularInRoot : MonoBehaviour
    {
        /// <summary>
        /// 是否正在驅動
        /// </summary>
        [SerializeField] private bool driving = false;

        [Header("手勢")] [SerializeField] private Hand.AttachmentFlags attachmentFlags;

        /// <summary>
        /// 設定抓握的方式
        /// </summary>
        [Tooltip("設定抓握的方式")] [SerializeField] private GrabTypes grabbedWithType;

        [Header("旋轉角度")] [Tooltip("驅動器的輸出角度值（以度為單位，無限制）將無限制地增加或減少，採用360模數來查找轉數")]
        public float outAngle;

        /// <summary>
        /// 給予Animator的值(0-1)
        /// </summary>
        public float value;

        /// <summary>
        /// 是否要開啟角度事件，不可與Limit共用
        /// </summary>
        [Tooltip("是否要開啟角度事件，不可與Limit共用")] [SerializeField]
        private bool isAngleEvent;

        /// <summary>
        /// 執行事件的角度值
        /// </summary>
        [Tooltip("執行事件的角度值")] public float eventAngle;

        /// <summary>
        /// 當角度為eventAngle值時，執行事件
        /// </summary>
        [SerializeField] private UnityEvent angleEvent;

        public enum Axis_t
        {
            XAxis,
            YAxis,
            ZAxis
        };

        [Header("驅動設定")] [Tooltip("圓形驅動器將在局部空間中繞其旋轉的軸")]
        public Axis_t axisOfRotation = Axis_t.XAxis;

        /// <summary>
        /// 是否可以轉動
        /// </summary>
       [Tooltip("是否可以轉動")] public bool canRotateGameObject = true;

        /// <summary>
        /// 鬆手後是否要轉回原角度
        /// </summary>
        [SerializeField] private bool isDetachedToResetAngle;

        [Tooltip("具有Collider組件以啟動交互的子GameObject，僅當存在多個Collider子對象時才需要設置")]
        public Collider childCollider = null;

        [Tooltip("要驅動的LinearMapping組件（如果未指定）將動態添加到此GameObject中。")]
        public LinearMapping linearMapping;

        /// <summary>
        /// 是否只要抓住，驅動器就持續處於操作狀態，否則控制器移出碰撞體，驅動器將停止運行
        /// </summary>
        [Tooltip("如果為true，則只要按住該按鈕，驅動器將一直處於操作狀態；如果為false，則如果控制器移出對撞機，驅動器將停止運行。")]
        public bool hoverLock = false;

        /// <summary>
        /// 是否要設定極限值
        /// </summary>
        [Header("旋轉的極限值")] [Tooltip("如果limited為true，則旋轉將限制為[minAngle，maxAngle]；如果為false，則旋轉將不受限制")]
        public bool limited = false;

        /// <summary>
        /// 如果limited為true，旋轉的極限值
        /// </summary>
        [Tooltip("如果limited為true，旋轉的極限值")] public Vector2 frozenDistanceMinMaxThreshold = new Vector2(0.1f, 0.2f);

        /// <summary>
        /// 如果旋轉值>=frozenDistanceMinMaxThreshold的Y值，執行事件
        /// </summary>
        public UnityEvent onFrozenDistanceThreshold;

        /// <summary>
        /// 極限值的最小值
        /// </summary>
        [Header("旋轉值極限值的最小值")] [Tooltip("如果Limited為true，則指定下限，否則未使用值")]
        public float minAngle = -45.0f;

        /// <summary>
        /// 達到最小角度時驅動器是否凍結其角度
        /// </summary>
        [Tooltip("如果Limited，則設置達到最小角度時驅動器是否凍結其角度")]
        public bool freezeOnMin = false;

        /// <summary>
        /// 在最小值時的事件
        /// </summary>
        [Tooltip("如果Limited，則在到達minAngle時調用事件")]
        public UnityEvent onMinAngle;

        /// <summary>
        /// 極限值的最大值
        /// </summary>
        [Header("旋轉極限值的最大值")] [Tooltip("如果limited為true，則指定上限，否則未使用值")]
        public float maxAngle = 45.0f;

        /// <summary>
        /// 設置達到最大角度時驅動器是否凍結其角度
        /// </summary>
        [Tooltip("如果Limited，則設置達到最大角度時驅動器是否凍結其角度")]
        public bool freezeOnMax = false;

        /// <summary>
        /// 在最大值時事件
        /// </summary>
        [Tooltip("如果Limited，則在達到maxAngle時調用事件")]
        public UnityEvent onMaxAngle;


        /// <summary>
        /// 在limited的情況中，是否要強制設定初始值
        /// </summary>
        [Header("強制改變初始值")] [Tooltip("如果limited為true，則強制將起始角度設為startAngle，並固定為[minAngle，maxAngle]")]
        public bool forceStart = false;

        /// <summary>
        /// 強制設定的初始旋轉值
        /// </summary>
        [Tooltip("如果limited為true且forceStart為true，則起始角度將為此角度，並固定為[minAngle，maxAngle]")]
        public float startAngle = 0.0f;

        [Header("隱藏的數值")] [SerializeField] private Quaternion start;

        [SerializeField] private Vector3 worldPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);
        [SerializeField] private Vector3 localPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);

        [SerializeField] private Vector3 lastHandProjected;




        /// <summary>
        /// 如果驅動器被限制為最小/最大，則大於此角度的角度將被忽略
        /// </summary>
        [SerializeField] private float minMaxAngularThreshold = 1.0f;


        /// <summary>
        /// 抓握的是哪一隻手
        /// </summary>
        [SerializeField] private Hand handHoverLocked = null;

        [SerializeField] private Interactable interactable;


        public static bool holdValve;
        public static bool resetHandPos;
        public static bool fixedTurn;


        private float angleBuffer;
        private float blendPoseValue;
        private float gestureConst;

        public static float holdTimer;
        public static float releaseTimer;

        private Vector3 handPos;

        // private SteamVR_Skeleton_Poser skeletonPoser;

        private void Awake()
        {
            interactable = GetComponent<Interactable>();
        }

        void OnDisable()
        {
            if (handHoverLocked)
            {
                handHoverLocked.HideGrabHint();
                handHoverLocked.HoverUnlock(interactable);
                handHoverLocked = null;
            }
        }

        void Start()
        {
            if (childCollider == null)
            {
                childCollider = GetComponentInChildren<Collider>();
            }

            if (linearMapping == null)
            {
                linearMapping = GetComponent<LinearMapping>();
            }

            if (linearMapping == null)
            {
                linearMapping = gameObject.AddComponent<LinearMapping>();
            }

            worldPlaneNormal = new Vector3(0.0f, 0.0f, 0.0f);
            worldPlaneNormal[(int) axisOfRotation] = 1.0f;

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


            holdValve = false;
            resetHandPos = false;
            fixedTurn = true;

            angleBuffer = 0;
            blendPoseValue = 0;
            minMaxAngularThreshold = 1.0f; //角度臨界值
            gestureConst = 0.7f;

            holdTimer = 0;
            releaseTimer = 0;

            // skeletonPoser = gameObject.GetComponent<SteamVR_Skeleton_Poser>();
        }


        #region 互動

        private void OnHandHoverBegin(Hand hand)
        {
            hand.ShowGrabHint();
        }


        private void OnHandHoverEnd(Hand hand)
        {
            if (driving && hand)
            {
                //hand.TriggerHapticPulse() //todo: fix
                StartCoroutine(HapticPulses(hand, 1.0f, 10));
            }

            driving = false;
            handHoverLocked = null;
        }


        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes grabTypes = hand.GetGrabStarting();
            bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;

            //GRAB THE OBJECT
            if (grabbedWithType == GrabTypes.None && grabTypes != GrabTypes.None)
            {
                // Trigger was just pressed
                lastHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);

                // if (hoverLock)
                // {
                //     hand.HoverLock(interactable);
                //     handHoverLocked = hand;
                // }

                hand.AttachObject(gameObject, grabTypes, attachmentFlags);
                hand.HoverLock(interactable);

                driving = true;
                // print($"driving : {driving}");
                RotateSwitch(hand);
                UpdateGameObjectRotation();
                holdValve = true;
                Debug.Log("The Gameobject is Grabbed.");
            }
            //如果鬆手
            else if (grabbedWithType != GrabTypes.None && isGrabEnding)
            {
                // hand.DetachObject(gameObject);
                // Trigger was just released
                if (hoverLock)
                {
                    hand.HoverUnlock(interactable);
                    handHoverLocked = null;
                }

                driving = false;
                grabbedWithType = GrabTypes.None;
            }

            if (driving && isGrabEnding == false && hand.hoveringInteractable == this.interactable)
            {
                print($"driving : {driving}");
                RotateSwitch(hand);
                UpdateGameObjectRotation();
            }

        }

        private void HandAttachedUpdate(Hand hand)
        {
            var toTransform = (hand.transform.position - gameObject.transform.position).sqrMagnitude;
            if (toTransform > 0.08f)
            {
                holdValve = false;
                hand.DetachObject(gameObject);
                hand.HoverUnlock(interactable);
            }


            if (!hand.IsGrabEnding(gameObject))
            {
                // 手勢回復後，重新定位手把位置
                if (resetHandPos)
                {
                    GetHandPostion(hand);
                    resetHandPos = false;
                }


                RotateSwitch(hand);
                UpdateGameObjectRotation();
            }
            else
            {
                //Rest Gesture and other values.
                handPos = new Vector3();
                angleBuffer = outAngle; //紀錄當前旋轉角度
                hand.DetachObject(gameObject);

                // skeletonPoser.SetBlendingBehaviourValue("HoldValveRotateTurn", 0);
                // skeletonPoser.SetBlendingBehaviourValue("HoldValveRotateBack", 0);
            }


            //Release Grab
            //bool isGrabEnding = hand.IsGrabEnding(gameObject);

            //if (isGrabEnding && !holdValve)
            //{
            //    hand.HoverUnlock(interactable);
            //    hand.DetachObject(gameObject);

            //    Debug.Log("The Gameobject is Dropped.");
            //}
        }


        private void OnAttachedToHand(Hand hand)
        {
            //handRotate = hand.transform.localEulerAngles;

            //switch (hand.name)
            //{
            //    case "LeftHand":

            //        //isRightHand = false;
            //        break;


            //    case "RightHand":

            //        //isRightHand = true;
            //        break;
            //}

            //circleRound = true;
        }


        private void OnDetachedFromHand(Hand hand)
        {
            handPos = new Vector3();
            angleBuffer = outAngle; //紀錄當前旋轉角度

            // skeletonPoser.SetBlendingBehaviourValue("HoldValveRotateTurn", 0);
            // skeletonPoser.SetBlendingBehaviourValue("HoldValveRotateBack", 0);
        }

        #endregion









        #region 計算的方法
        /// <summary>
        ///
        /// </summary>
        /// <param name="hand"></param>
        private void RotateSwitch(Hand hand)
        {
            Vector3 handPos_new = ComputeToTransformProjected(hand.hoverSphereTransform);

            float absAngleDelta = Vector3.Angle(handPos_new, handPos);
            print($"absAngleDelta : {absAngleDelta}");


            if (absAngleDelta > 0.0f)
            {
                Vector3 cross = Vector3.Cross(handPos_new, handPos).normalized;
                float dot = Vector3.Dot(worldPlaneNormal, cross);

                float signedAngleDelta = absAngleDelta;

                if (dot < 0.0f)
                {
                    signedAngleDelta = -signedAngleDelta;
                }

                //outAngle += signedAngleDelta;
                //handPos_new = handPos;

                if (limited)
                {
                }

                float angleTmp = Mathf.Clamp(outAngle + signedAngleDelta, minAngle, maxAngle);

                //如果有設定極限值

                //到達最小角度
                if (outAngle == minAngle)
                {
                    if (angleTmp > minAngle && absAngleDelta < minMaxAngularThreshold)
                    {
                        outAngle = angleTmp;
                        handPos = handPos_new;
                    }
                    else
                    {
                        outAngle = minAngle + 1;
                    }
                }

                //到達最大角度
                else if (outAngle == maxAngle)
                {
                    if (angleTmp < maxAngle && absAngleDelta < minMaxAngularThreshold)
                    {
                        outAngle = angleTmp;
                        handPos = handPos_new;
                    }
                    else
                    {
                        outAngle = maxAngle - 1;
                    }
                }

                //當達到最小角度時
                else if (angleTmp == minAngle)
                {
                    outAngle = angleTmp;
                    handPos = handPos_new;
                }
                //當達到最大角度時
                else if (angleTmp == maxAngle)
                {
                    outAngle = angleTmp;
                    handPos = handPos_new;
                }

                else
                {
                    float angleDiffer = angleTmp - outAngle; //目前旋轉的角度差值
                    float angleVariable = outAngle - angleBuffer; //初始至目前旋轉的角度變化量：判斷手勢變化的臨界值


                    // 計算手勢變換遮罩值(Blend Mask)：取得 [0, 1] 的臨界值
                    blendPoseValue = (angleVariable - minAngle) / (maxAngle - minAngle);

                    //左右轉的手勢變換
                    if (angleDiffer > 0.1f)
                    {
                        // skeletonPoser.SetBlendingBehaviourValue("HoldValveRotateBack", 0);
                        // skeletonPoser.SetBlendingBehaviourValue("HoldValveRotateTurn",
                            // blendPoseValue + gestureConst);


                        //Debug.Log("向右轉: " + Mathf.Floor(angleDiffer) + ", 手轉幅度：" + blendPoseValue);
                    }
                    else if (angleDiffer < -0.1f)
                    {
                        // skeletonPoser.SetBlendingBehaviourValue("HoldValveRotateTurn", 0);
                        // skeletonPoser.SetBlendingBehaviourValue("HoldValveRotateBack",
                            // gestureConst - blendPoseValue);


                        //Debug.Log("向左轉: " + Mathf.Floor(angleDiffer) + ", 手轉幅度：" + blendPoseValue);
                    }


                    outAngle = angleTmp;
                    handPos = handPos_new;
                }

                UpdateGameObjectRotation();
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void UpdateGameObjectRotation()
        {
            if (canRotateGameObject)
            {
                transform.localRotation = Quaternion.AngleAxis(outAngle * 1.2f, localPlaneNormal);
            }
        }

        /// <summary>
        /// 《
        /// </summary>
        /// <param name="xForm"></param>
        /// <returns></returns>
        private Vector3 ComputeToTransformProjected(Transform xForm)
        {
            //normalized： 以此當前向量作為標準值(正規化)
            Vector3 toTransform = (xForm.position - transform.position).normalized;

            Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

            //magnitude：把 vertor 平方相加在開根號
            //sqrMagnitude：把vertor 平方
            //確定抓到物體，
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

            return toTransformProjected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="hand"></param>
        private void GetHandPostion(Hand hand)
        {
            handPos = ComputeToTransformProjected(hand.transform);
        }

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

        #endregion
    }
}