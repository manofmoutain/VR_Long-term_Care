using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    [RequireComponent(typeof(TakeEvent_ToResetPosition))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(MyInteractable))]
    [RequireComponent(typeof(Rigidbody))]
    public class TakeEvent_HandGrab : MonoBehaviour
    {
        public bool isUsingTwoHands;
        [SerializeField] private Hand rightHand;
        [SerializeField] private GameObject rightHandAttachedGameObject;
        [SerializeField] private Hand leftHand;
        [SerializeField] private GameObject leftHandAttachedGameObject;

        // [Header("模型位置參數")]
        [SerializeField] private bool isStartTrigger = true;
        [SerializeField] private bool isStartKinematic = true;
        [Tooltip("原本位置")] [SerializeField] Transform originalTransform;
        private Vector3 originPosition;
        private Vector3 originRotation;
        private Vector3 originScale;
        [Tooltip("要黏著的物件")] [SerializeField] GameObject[] UsePosition;
        [SerializeField] List<TakeEvent_SnapArea> snapZoneArea;
        [SerializeField] private GameObject takeObject;

        /// <summary>
        /// 是否已抓取物件
        /// </summary>
        // [Header("抓取狀態")]
        [Tooltip("是否已抓取此物件")] public bool snapTakeObject;

        /// <summary>
        ///Trigger放開後是否要脫離手勢
        /// </summary>
        // [Tooltip("Trigger放開後是否要脫離手勢")] [SerializeField] private bool snapReleaseGesture = true;

        public SnapFixed snapFixed;
        [SerializeField] private ThrowOutside throwOutside;

        // [Header("手勢資訊")]
        [Tooltip("SnapOnAttach=該對象應捕捉到手上指定的附著點的位置\nDetachOthers=附著在此手上的其他物體將被分離\nDetachFromOtherHand=該對象將與另一隻手分離\nVelocityMovement=對象將嘗試移動以匹配手的位置和旋\nAllowSidegrade=該對象能夠從捏握切換為抓握")]
        [SerializeField]
        private Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.SnapOnAttach |
                                                       Hand.AttachmentFlags.DetachFromOtherHand |
                                                       Hand.AttachmentFlags.VelocityMovement |
                                                       Hand.AttachmentFlags.TurnOffGravity;

        /// <summary>
        /// 由於扳機保持而不是扳機按下，該物體必須移動多快才能固定？ (-1 to disable)
        /// </summary>
        float catchingSpeedThreshold = -1;

        /// <summary>
        /// 保持時用作位置和旋轉偏移量的局部點
        /// </summary>
        Transform attachmentOffset;


        //[Header("Rigibody")]
        private Rigidbody rigidbody;
        private VelocityEstimator velocityEstimator;
        protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

        /// <summary>
        /// 放開物件後的重力模擬
        /// </summary>
        [SerializeField] ReleaseStyle releaseVelocityStyle = ReleaseStyle.AdvancedEstimation;

        /// <summary>
        /// 使用RawFromHand選項釋放對象時使用的時間偏移
        /// </summary>
        float releaseVelocityTimeOffset = -0.011f;

        /// <summary>
        /// 釋放速度幅度代表水垢釋放速度曲線的終點. (-1 to disable)
        /// </summary>
        float scaleReleaseVelocityThreshold = -1.0f;

        /// <summary>
        /// 使用此曲線可根據測得的釋放速度的大小輕鬆按比例縮放釋放速度。 這樣可以更好地區分跌落，拋擲和投擲。
        /// </summary>
        AnimationCurve scaleReleaseVelocityCurve = AnimationCurve.EaseInOut(0.0f, 0.1f, 1.0f, 1.0f);

        float scaleReleaseVelocity = 1.1f;

        /// <summary>
        /// 分離對象時，它應該返回其原始父對象嗎？
        /// </summary>
        bool restoreOriginalParent = false;

        // [Header("InteractComponent")]
        private MyInteractable interactable;
        bool attached = false;
        float attachTime;
        Vector3 attachPosition;
        Quaternion attachRotation;


        public bool isHiddenEvents = true;
        [SerializeField] private UnityEvent snapIn;
        [SerializeField] private UnityEvent snapOut;
        [SerializeField] private UnityEvent onPickUp;
        [SerializeField] private UnityEvent dropDown;
        [SerializeField] private UnityEvent onOtherHandTouch;

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            GrabGameObject();
        }


        protected virtual void OnHandHoverBegin(Hand hand)
        {
            if (!attached && catchingSpeedThreshold != -1)
            {
                float catchingThreshold = catchingSpeedThreshold *
                                          SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);

                GrabTypes bestGrabType = hand.GetBestGrabbingType();

                if (bestGrabType != GrabTypes.None)
                {
                    if (rigidbody.velocity.magnitude >= catchingThreshold)
                    {
                        hand.AttachObject(gameObject, bestGrabType, attachmentFlags);
                    }
                }
            }
        }

        protected virtual void OnHandHoverEnd(Hand hand)
        {
            snapOut.Invoke();
        }

        protected virtual void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
            {
                switch (hand.gameObject.name)
                {
                    case "RightHand":
                        rightHand = hand;
                        rightHandAttachedGameObject = gameObject;
                        break;
                    case "LeftHand":
                        leftHand = hand;
                        leftHandAttachedGameObject = gameObject;
                        break;
                }

                if (isUsingTwoHands)
                {
                    // transform.SetParent(originalTransform);
                    hand.AttachObject(gameObject, startingGrabType,Hand.AttachmentFlags.VelocityMovement,attachmentOffset);
                    if (rightHandAttachedGameObject != null && leftHandAttachedGameObject != null)
                    {
                        if (attachmentFlags!=Hand.AttachmentFlags.ParentToHand)
                        {
                            transform.SetParent(originalTransform);
                        }
                        hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffset);
                        rigidbody.isKinematic = false;
                        // print("雙手抓取");
                        snapTakeObject = true;
                        // if (snapFixed.isFixed && snapReleaseGesture)
                        // {
                            // print($"已鬆手，且物件已黏合:{snapFixed.isFixed}");
                            // print($"物件吻合:{snapFixed.isLocated}");
                            snapFixed.isFixed = false;
                            snapFixed.isLocated = false;
                            // snapOut.Invoke();
                        // }
                    }
                }
                else
                {
                    if (attachmentFlags!=Hand.AttachmentFlags.ParentToHand)
                    {
                        transform.SetParent(originalTransform);
                    }
                    hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffset);
                    rigidbody.isKinematic = false;
                    // print("單手抓取");
                    snapTakeObject = true;
                    // if (snapFixed.isFixed && snapReleaseGesture)
                    // {
                        // print($"已鬆手，且物件已黏合:{snapFixed.isFixed}");
                        // print($"物件吻合:{snapFixed.isLocated}");
                        snapFixed.isFixed = false;
                        snapFixed.isLocated = false;
                        // snapOut.Invoke();
                    // }
                }
            }
        }

        protected virtual void OnAttachedToHand(Hand hand)
        {
            hand.HoverLock(null);

            if (isUsingTwoHands)
            {
                if (rightHandAttachedGameObject == null || leftHandAttachedGameObject == null)
                {
                    rightHand = null;
                    leftHand = null;
                    hadInterpolation = this.rigidbody.interpolation;

                    attached = true;

                    onPickUp.Invoke();


                    rigidbody.interpolation = RigidbodyInterpolation.None;

                    if (velocityEstimator != null)
                        velocityEstimator.BeginEstimatingVelocity();

                    attachTime = Time.time;
                    attachPosition = transform.position;
                    attachRotation = transform.rotation;
                }
            }
            else
            {
                hadInterpolation = this.rigidbody.interpolation;

                attached = true;

                onPickUp.Invoke();


                rigidbody.interpolation = RigidbodyInterpolation.None;

                if (velocityEstimator != null)
                    velocityEstimator.BeginEstimatingVelocity();
                // transform.position = hand.transform.position;
                // transform.eulerAngles = hand.transform.eulerAngles;
                attachTime = Time.time;
                attachPosition = transform.position;
                attachRotation = transform.rotation;
            }
        }

        protected virtual void OnDetachedFromHand(Hand hand)
        {
            rightHandAttachedGameObject = null;
                leftHandAttachedGameObject = null;

            hand.changePositionByTwoHands = false;

            attached = false;

            dropDown.Invoke();

            hand.HoverUnlock(null);

            rigidbody.interpolation = hadInterpolation;

            Vector3 velocity;
            Vector3 angularVelocity;

            GetReleaseVelocities(hand, out velocity, out angularVelocity);
            GetReleaseVelocities(hand.otherHand, out velocity, out angularVelocity);


            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
            rigidbody.interpolation = hadInterpolation;
            takeObject = null;
        }

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            // print($"雙手抓取:{hand.otherHand.twoHandGrab}");
            //如果手鬆開抓取
            if (hand.IsGrabEnding(gameObject))
            {
                //如果物件已吻合於區域
                if (snapFixed.isLocated)
                {
                    //鬆開Trigger若手勢脫離的情況
                    // if (snapReleaseGesture)
                    // {
                        //強制鬆手
                        hand.DetachObject(gameObject);
                        if (isUsingTwoHands)
                        {
                            hand.otherHand.DetachObject(gameObject);
                        }
                    // }

                    //Snap in：吻合物件到指定位置
                    rigidbody.isKinematic = true;
                    snapFixed.isFixed = true;
                    // print($"正在抓取{gameObject.name}");
                }
                //如果物件未吻合於區域
                else
                {
                    // hand.otherHand.ObjectIsAttached(gameObject);
                    hand.DetachObject(gameObject);
                    if (isUsingTwoHands)
                    {
                        if (hand.otherHand.ObjectIsAttached(gameObject))
                        {
                            hand.otherHand.DetachObject(gameObject);
                        }
                    }

                    hand.HoverUnlock(interactable);

                    if (throwOutside.outside)
                    {
                        //計算物件原始位置與目前位置的距離：判斷是否有將物件移開
                        if ((transform.position - throwOutside.outsideZone.position).sqrMagnitude >
                            throwOutside.outsideRange)
                        {
                            snapFixed.isOutside = true;
                        }
                    }

                    snapFixed.isFixed = false;
                    snapTakeObject = false;
                    rigidbody.isKinematic = false;
                    // Debug.Log("放下了" + gameObject.name);
                }
            }
            else
            {
                if (rightHandAttachedGameObject != null && leftHandAttachedGameObject != null)
                {
                    // gameObject.transform.position = hand.transform.position;
                }
                if (transform.parent!=originalTransform.transform && attachmentFlags!= Hand.AttachmentFlags.ParentToHand)
                {
                    transform.SetParent(originalTransform.transform);
                }
            }


            // if (onHeldUpdate != null)
            //     onHeldUpdate.Invoke(hand);
        }

        protected virtual void OnHandFocusAcquired(Hand hand)
        {
            gameObject.SetActive(true);

            if (velocityEstimator != null)
                velocityEstimator.BeginEstimatingVelocity();
        }

        protected virtual void OnHandFocusLost(Hand hand)
        {
            gameObject.SetActive(false);

            if (velocityEstimator != null)
                velocityEstimator.FinishEstimatingVelocity();
        }


        public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (hand.noSteamVRFallbackCamera && releaseVelocityStyle != ReleaseStyle.NoChange)
                releaseVelocityStyle =
                    ReleaseStyle.ShortEstimation; // only type that works with fallback hand is short estimation.

            switch (releaseVelocityStyle)
            {
                case ReleaseStyle.ShortEstimation:
                    if (velocityEstimator != null)
                    {
                        velocityEstimator.FinishEstimatingVelocity();
                        velocity = velocityEstimator.GetVelocityEstimate();
                        angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
                    }
                    else
                    {
                        Debug.LogWarning(
                            "[SteamVR Interaction System] Throwable: No Velocity Estimator component on object but release style set to short estimation. Please add one or change the release style.");

                        velocity = rigidbody.velocity;
                        angularVelocity = rigidbody.angularVelocity;
                    }

                    break;
                case ReleaseStyle.AdvancedEstimation:
                    hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
                    break;
                case ReleaseStyle.GetFromHand:
                    velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
                    angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
                    break;
                default:
                case ReleaseStyle.NoChange:
                    velocity = rigidbody.velocity;
                    angularVelocity = rigidbody.angularVelocity;
                    break;
            }

            if (releaseVelocityStyle != ReleaseStyle.NoChange)
            {
                float scaleFactor = 1.0f;
                if (scaleReleaseVelocityThreshold > 0)
                {
                    scaleFactor =
                        Mathf.Clamp01(
                            scaleReleaseVelocityCurve.Evaluate(velocity.magnitude / scaleReleaseVelocityThreshold));
                }

                velocity *= (scaleFactor * scaleReleaseVelocity);
            }
        }

        private void Initialize()
        {
            if (originalTransform == null)
            {
                originalTransform = transform.parent;
            }

            originPosition = transform.localPosition;
            originRotation = transform.localEulerAngles;
            originScale = transform.localScale;

            snapTakeObject = false;
            snapFixed.isFixed = false;
            snapFixed.isLocated = false;
            snapFixed.isOutside = false;

            catchingSpeedThreshold = -1;
            releaseVelocityTimeOffset = -0.011f;
            scaleReleaseVelocity = 1.1f;
            scaleReleaseVelocityThreshold = -1.0f;
            scaleReleaseVelocityCurve = AnimationCurve.EaseInOut(0.0f, 0.1f, 1.0f, 1.0f);

            if (isStartTrigger)
            {
                if (GetComponent<MeshCollider>())
                {
                    if (GetComponent<MeshCollider>().convex)
                    {
                        GetComponent<MeshCollider>().isTrigger = true;
                    }
                    else
                    {
                        GetComponent<MeshCollider>().convex = true;
                        GetComponent<MeshCollider>().isTrigger = true;
                    }
                }
                else
                {
                    GetComponent<Collider>().isTrigger = true;
                }
            }
            else
            {
                if (GetComponent<MeshCollider>())
                {
                    GetComponent<MeshCollider>().convex = false;
                }
                else
                {
                    GetComponent<Collider>().isTrigger = false;
                }
            }


            rigidbody = GetComponent<Rigidbody>();
            rigidbody.maxAngularVelocity = 50.0f;
            rigidbody.isKinematic = isStartKinematic;

            velocityEstimator = GetComponent<VelocityEstimator>();
            interactable = GetComponent<MyInteractable>();


            //取得 snapZoneArea 置放區域
            for (int i = 0; i < UsePosition.Length; i++)
            {
                if (UsePosition[i].transform.GetChild(0).GetComponent<TakeEvent_SnapArea>())
                {
                    snapZoneArea.Add(UsePosition[i].transform.GetChild(0).GetComponent<TakeEvent_SnapArea>());
                }
            }

            //隱藏放置提示輪廓線
            foreach (GameObject useObject in UsePosition)
            {
                useObject.transform.Find($"{gameObject.name}_OutLine").gameObject.SetActive(false);
            }
        }

        private void GrabGameObject()
        {
            //如果以抓取物件
            if (snapTakeObject)
            {
                // rigidbody.isKinematic = false;
                //開啟黏著區物件
                // foreach (GameObject useObject in UsePosition)
                // {
                //     useObject.SetActive(true);
                // }

                //已鬆手，物件已修正於新區域
                if (snapFixed.isFixed)
                {
                    // gameObject.tag = "FixObject";

                    foreach (TakeEvent_SnapArea snapZone in snapZoneArea)
                    {
                        //判斷是否為觸發放置區域
                        if (snapZone.isSnapIn)
                        {
                            snapZone.onLocatedEvent.Invoke();
                            transform.SetParent(snapZone.transform.parent);

                            Destroy(snapZone.fadedObject);
                            // print($"{gameObject.name}已成為{snapZone.transform.parent.name}的子物件");
                            snapTakeObject = false;
                            snapZone.isSnapIn = false;
                        }
                    }


                    transform.localPosition = originPosition;
                    transform.localEulerAngles = originRotation;
                    transform.localScale = originScale;
                    snapIn.Invoke();
                }

                else
                {
                    // if (transform.parent!=OriginalPositionGameObject.transform)
                    // {
                    //     transform.SetParent(OriginalPositionGameObject.transform);
                    // }
                    onPickUp.Invoke();
                }
            }
            else
            {
                // rigidbody.isKinematic = true;
                //放下物品：參數重置
                // gameObject.tag = "DropObject";

                if (attachmentFlags==Hand.AttachmentFlags.ParentToHand)
                {
                    transform.SetParent(originalTransform.transform);
                }


                snapFixed.isLocated = false;
                snapFixed.isFixed = false;
                //snapFixed.isThrowed = false;

                //隱藏放置提示輪廓線
                // foreach (GameObject useObject in UsePosition)
                // {
                //     // useObject.transform.GetChild(0).GetComponent<SnapZoneArea>().isSnapIn = true;
                //     useObject.SetActive(false);
                // }

                // if (playerHand_L != null & playerHand_R != null)
                // {
                if (snapFixed.isOutside)
                {
                    // snapZoneArea[0].gameObject.tag = "AreaZone";

                    snapFixed.isFixed = true;
                    snapTakeObject = true;
                }
                else
                {
                    dropDown.Invoke();
                }

                // }
            }
        }
    }

    [System.Serializable]
    public class SnapFixed
    {
        /// <summary>
        /// 定位判斷－抓取的物件若吻合於黏貼區(未鬆手)
        /// </summary>
        public bool isLocated;

        /// <summary>
        /// 吻合判斷－抓取的物件若吻合於黏貼區(已鬆手，物件已修正於黏貼區)
        /// </summary>
        public bool isFixed;

        /// <summary>
        /// 定點判斷
        /// </summary>
        public bool isOutside;
    }

    [System.Serializable]
    public class ThrowOutside
    {
        public bool outside;
        public float outsideRange;
        public Transform outsideZone;
    }
}