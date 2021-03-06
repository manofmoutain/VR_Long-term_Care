using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

namespace InteractableObject
{
    [RequireComponent(typeof(MyInteractable), typeof(Interact_LinearMapping), typeof(Rigidbody))]
    public class Interact_CircularDrive : MonoBehaviour
    {
        private Hand rightHand;
        private Hand leftHand;

        public enum Axis_t
        {
            XAxis,
            YAxis,
            ZAxis
        };

        /// <summary>
        /// 是否要使用混合手勢
        /// </summary>
        public bool isUsingBlenderPoser;

        /// <summary>
        /// 轉開手勢
        /// </summary>
        [SerializeField] private UnityEvent switchOnPoser;

        /// <summary>
        /// 關閉手勢
        /// </summary>
        [SerializeField] private UnityEvent switchOffPoser;

        /// <summary>
        /// 抓握的是哪一隻手
        /// </summary>
        [SerializeField] private Hand grabHand;

        /// <summary>
        /// 手勢標誌
        /// </summary>
        [SerializeField] private Hand.AttachmentFlags attachmentFlags;

        /// <summary>
        /// 目前被抓握的方式
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
        /// 在isRoot的情況下，目前的旋轉值(0-360)
        /// </summary>
        Vector3 eulerRotation;

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

        [Tooltip("具有Collider組件以啟動交互的子GameObject，僅當存在多個Collider子對象時才需要設置")] [SerializeField]
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

        /// <summary>
        /// 達到極限值是否立即鬆手
        /// </summary>
        public bool isLimteToDetatch;

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

        public bool isAngleEventToDetatch;

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
        [SerializeField] private Vector3 worldPlaneNormalize;
        [SerializeField] private Vector3 localPlaneNormalize;
        [SerializeField] private Vector3 lastHandProjected;

        /// <summary>
        /// 如果驅動器被限制為最小/最大，則大於此角度的角度將被忽略
        /// </summary>
        [HideInInspector] [SerializeField] private float minMaxAngularThreshold = 1.0f;

        [HideInInspector] [SerializeField] private Interactable interactable;
        private float angleBuffer;
        public float blendPoseValue;


        private void Awake()
        {
            interactable = GetComponent<Interactable>();
        }

        private void Start()
        {
            rightHand = null;
            leftHand = null;
            if (childCollider == null)
            {
                childCollider = GetComponentInChildren<Collider>();
            }

            if (!GetComponent<Interact_LinearMapping>())
            {
                gameObject.AddComponent<Interact_LinearMapping>();
                linearMapping = GetComponent<Interact_LinearMapping>();
            }
            else
            {
                linearMapping = GetComponent<Interact_LinearMapping>();
            }

            worldPlaneNormalize = new Vector3(0.0f, 0.0f, 0.0f);
            worldPlaneNormalize[(int) axisOfRotation] = 1f;

            GetComponent<Rigidbody>().isKinematic = true;

            localPlaneNormalize = worldPlaneNormalize;

            if (transform.parent)
            {
                worldPlaneNormalize =
                    transform.parent.localToWorldMatrix.MultiplyVector(worldPlaneNormalize).normalized;
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
                startQuaternion =
                    Quaternion.AngleAxis(transform.localEulerAngles[(int) axisOfRotation], localPlaneNormalize);
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
            //取得在HoverUpdate時，"手部"的抓握狀態
            GrabTypes startingGrabType = hand.GetGrabStarting();
            //取得目前手部抓握的狀態是否等於物件目前被抓握的方式，設定為false"
            bool isGrabEnding = hand.IsGrabbingWithType(grabbedWithType) == false;

            //如果目前被抓握的方式等於none，且目前手抓握的狀態不等於none
            //也就是物件尚未被抓取，而手部開始進行抓握的狀態
            if (grabbedWithType == GrabTypes.None && startingGrabType != GrabTypes.None)
            {
                //物件目前被抓握的狀態=手部抓握的狀態
                grabbedWithType = startingGrabType;

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

                if (hand.name == "RightHand")
                {
                    rightHand = hand;
                }
                else
                {
                    leftHand = hand;
                }

                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
                driving = true;

                ComputeAngle(hand);
                UpdateLinearMapping();
                UpdateGameObject();

                hand.HideGrabHint();
            }
            //如果目前被抓握方式不等於none(已經改變)，且目前抓握的狀態為設定的抓握方式(none)
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
                rightHand = null;
                leftHand = null;
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
        private Vector3 ComputeToTransformProjected(Transform xForm)
        {
            Vector3 toTransform;
            if (isRoot)
            {
                toTransform = (xForm.eulerAngles - transform.eulerAngles).normalized;
            }
            else
            {
                toTransform = (xForm.position - transform.position).normalized;
            }

            Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

            // Need a non-zero distance from the hand to the center of the CircularDrive
            if (toTransform.sqrMagnitude > 0.0f && !isRoot)
            {
                toTransformProjected = Vector3.ProjectOnPlane(toTransform, worldPlaneNormalize).normalized;
            }
            else if (isRoot)
            {
                toTransformProjected = Vector3.ProjectOnPlane(toTransform, worldPlaneNormalize);
                // print(toTransformProjected);
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
                if (isRoot)
                {
                    switch (axisOfRotation)
                    {
                        case Axis_t.XAxis:
                            if (rightHand != null)
                            {
                                eulerRotation = new Vector3(rightHand.transform.localEulerAngles.x, 0, 0);
                                transform.rotation = Quaternion.Euler(eulerRotation);
                                // transform.localRotation = Quaternion.Euler(eulerRotation) * Quaternion.AngleAxis(outAngle, localPlaneNormalize);
                            }

                            if (leftHand != null)
                            {
                                eulerRotation = new Vector3(rightHand.transform.localEulerAngles.x, 0, 0);
                                transform.rotation = Quaternion.Euler(eulerRotation);
                                // transform.localRotation = Quaternion.Euler(eulerRotation) * Quaternion.AngleAxis(outAngle, localPlaneNormalize);
                            }

                            break;
                        case Axis_t.YAxis:
                            if (rightHand != null)
                            {
                                eulerRotation = new Vector3(0, rightHand.transform.localEulerAngles.y, 0);
                                transform.rotation = Quaternion.Euler(eulerRotation);
                                // transform.localRotation = Quaternion.Euler(eulerRotation) * Quaternion.AngleAxis(outAngle, localPlaneNormalize);
                            }

                            if (leftHand != null)
                            {
                                eulerRotation = new Vector3(0, rightHand.transform.localEulerAngles.y, 0);
                                transform.rotation = Quaternion.Euler(eulerRotation);
                                // transform.localRotation = Quaternion.Euler(eulerRotation) * Quaternion.AngleAxis(outAngle, localPlaneNormalize);
                            }

                            break;
                        case Axis_t.ZAxis:
                            if (rightHand != null)
                            {
                                eulerRotation = new Vector3(0, 0, rightHand.transform.localEulerAngles.z);

                                transform.eulerAngles = eulerRotation;

                                // transform.localRotation = Quaternion.Euler(eulerRotation) * Quaternion.AngleAxis(outAngle, localPlaneNormalize);
                            }

                            if (leftHand != null)
                            {
                                eulerRotation = new Vector3(0, 0, leftHand.transform.localEulerAngles.z);
                                transform.eulerAngles = eulerRotation;
                                // transform.localRotation = Quaternion.Euler(eulerRotation) * Quaternion.AngleAxis(outAngle, localPlaneNormalize);
                            }

                            break;
                    }
                }
                else
                {
                    transform.localRotation = startQuaternion * Quaternion.AngleAxis(outAngle, localPlaneNormalize);
                }

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
                toHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);
                if (!toHandProjected.Equals(lastHandProjected))
                {
                    float absAngleDelta = Vector3.Angle(lastHandProjected, toHandProjected);
                    if (absAngleDelta > 0.0f)
                    {
                        Vector3 cross = Vector3.Cross(lastHandProjected, toHandProjected).normalized;
                        float dot = Vector3.Dot(worldPlaneNormalize, cross);

                        float signedAngleDelta = absAngleDelta;

                        if (dot < 0.0f)
                        {
                            signedAngleDelta = -signedAngleDelta;
                        }

                        switch (axisOfRotation)
                        {
                            case Axis_t.XAxis:
                                outAngle = transform.localEulerAngles.x;
                                break;
                            case Axis_t.YAxis:
                                outAngle = transform.localEulerAngles.y;
                                break;
                            case Axis_t.ZAxis:
                                outAngle = transform.localEulerAngles.z;
                                if (transform.localEulerAngles.z > 180.0f)
                                {
                                    outAngle = -(360.0f - outAngle);
                                }

                                break;
                        }

                        if (limited)
                        {
                            if (outAngle <= minAngle + 10.0f && outAngle >= minAngle - 10.0f)
                            {
                                onMinAngle.Invoke();
                            }
                            else if (outAngle <= maxAngle + 10.0f && outAngle >= maxAngle - 10.0f)
                            {
                                onMaxAngle.Invoke();
                            }

                            // if (outAngle>=eventAngle)
                            // {
                            //     angleEvent.Invoke();
                            // }


                            // float angleTmp = Mathf.Clamp(outAngle + signedAngleDelta, minAngle, maxAngle);
                            //
                            // if (outAngle == minAngle)
                            // {
                            //     if (angleTmp > minAngle && absAngleDelta < minMaxAngularThreshold)
                            //     {
                            //         // Debug.Log("Min Angle: ");
                            //         outAngle = angleTmp;
                            //         lastHandProjected = toHandProjected;
                            //     }
                            //     else
                            //     {
                            //         outAngle = minAngle + 1;
                            //     }
                            // }
                            // else if (outAngle == maxAngle)
                            // {
                            //     if (angleTmp < maxAngle && absAngleDelta < minMaxAngularThreshold)
                            //     {
                            //         // Debug.Log("Max Angle: ");
                            //         outAngle = angleTmp;
                            //         lastHandProjected = toHandProjected;
                            //     }
                            //     else
                            //     {
                            //         outAngle = minAngle - 1;
                            //     }
                            // }
                            // else if (angleTmp == minAngle)
                            // {
                            //     outAngle = angleTmp;
                            //     lastHandProjected = toHandProjected;
                            //     if (isLimteToDetatch)
                            //     {
                            //         hand.DetachObject(gameObject);
                            //     }
                            //
                            //     onMinAngle.Invoke();
                            // }
                            // else if (angleTmp == maxAngle)
                            // {
                            //     outAngle = angleTmp;
                            //     lastHandProjected = toHandProjected;
                            //     if (isLimteToDetatch)
                            //     {
                            //         hand.DetachObject(gameObject);
                            //     }
                            //
                            //     onMaxAngle.Invoke();
                            // }
                            // else
                            // {
                            //     //當達到最小角度時
                            //     if (angleTmp == minAngle)
                            //     {
                            //         outAngle = angleTmp;
                            //         lastHandProjected = toHandProjected;
                            //         // if (isLimteToDetatch)
                            //         // {
                            //         //     hand.DetachObject(gameObject);
                            //         // }
                            //         //
                            //         // onMinAngle.Invoke();
                            //     }
                            //     //當達到最大角度時
                            //     else if (angleTmp == maxAngle)
                            //     {
                            //         outAngle = angleTmp;
                            //         lastHandProjected = toHandProjected;
                            //         // if (isLimteToDetatch)
                            //         // {
                            //         //     hand.DetachObject(gameObject);
                            //         // }
                            //         //
                            //         // onMaxAngle.Invoke();
                            //     }
                            //     else
                            //     {
                            //         float angleDiffer = angleTmp - outAngle; //目前旋轉的角度差值
                            //         float angleVariable = outAngle - angleBuffer; //初始至目前旋轉的角度變化量：判斷手勢變化的臨界值
                            //
                            //         if (isUsingBlenderPoser)
                            //         {
                            //             // 計算手勢變換遮罩值(Blend Mask)：取得 [0, 1] 的臨界值
                            //             blendPoseValue = (angleVariable - minAngle) / (maxAngle - minAngle);
                            //
                            //             //開關的手勢變換
                            //             if (angleDiffer > 0.1f)
                            //             {
                            //                 switchOnPoser.Invoke();
                            //                 // Debug.Log("轉開: " + Mathf.Floor(angleDiffer) + ", 手轉幅度：" + blendPoseValue);
                            //             }
                            //             else if (angleDiffer < -0.1f)
                            //             {
                            //                 switchOffPoser.Invoke();
                            //                 // Debug.Log("關閉: " + Mathf.Floor(angleDiffer) + ", 手轉幅度：" + blendPoseValue);
                            //             }
                            //         }
                            //
                            //         outAngle = angleTmp;
                            //         lastHandProjected = toHandProjected;
                            //     }
                            // }
                        }
                        else
                        {
                            // outAngle += signedAngleDelta;
                            // if (isAngleEvent && outAngle >= eventAngle)
                            // {
                            //     hand.DetachObject(gameObject);
                            //     angleEvent.Invoke();
                            // }
                            //
                            // lastHandProjected = toHandProjected;
                        }

                        if (isAngleEvent && outAngle >= eventAngle - 10 && outAngle <= eventAngle + 10)
                        {
                            if (isAngleEventToDetatch)
                            {
                                hand.DetachObject(gameObject);
                            }

                            angleEvent.Invoke();
                        }
                    }
                }
            }
            else
            {
                toHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);
                // print($"lastHandProjected : {lastHandProjected} ; toHandProjected : {toHandProjected}");
                if (!toHandProjected.Equals(lastHandProjected))
                {
                    float absAngleDelta = Vector3.Angle(lastHandProjected, toHandProjected);
                    // print($"absAngleDelta : {absAngleDelta}");

                    if (absAngleDelta > 0.0f)
                    {
                        Vector3 cross = Vector3.Cross(lastHandProjected, toHandProjected).normalized;
                        float dot = Vector3.Dot(worldPlaneNormalize, cross);

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
                                if (isLimteToDetatch)
                                {
                                    hand.DetachObject(gameObject);
                                }

                                onMinAngle.Invoke();
                            }
                            else if (angleTmp == maxAngle)
                            {
                                outAngle = angleTmp;
                                lastHandProjected = toHandProjected;
                                if (isLimteToDetatch)
                                {
                                    hand.DetachObject(gameObject);
                                }

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
                                if (isAngleEventToDetatch)
                                {
                                    hand.DetachObject(gameObject);
                                }

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
}