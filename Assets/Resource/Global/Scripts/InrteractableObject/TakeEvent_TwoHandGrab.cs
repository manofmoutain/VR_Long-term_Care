using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Heimlich_maneuver;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    [RequireComponent( typeof( Interactable ) )]
    [RequireComponent( typeof( Rigidbody ) )]
    public class TakeEvent_TwoHandGrab : MonoBehaviour
    {
        [Header("雙手資訊")]
        [SerializeField] private Hand right_Hand;
        [SerializeField] private GameObject right_Hand_Grab_GameObkect;
        [SerializeField] private Hand left_Hand;
        [SerializeField] private GameObject left_Hand_Grab_GameObkect;

        [Header("模型位置參數")]
        [Tooltip("原本位置")]
        [SerializeField] GameObject OriginalPositionGameObject;
        [SerializeField] private Vector3 originPosition;
        [SerializeField] private Vector3 originRotation;
        [SerializeField] private Vector3 originScale;
        [Tooltip("要黏著的物件")] [SerializeField] GameObject[] UsePosition;
        [SerializeField] List<TakeEvent_SnapArea> snapZoneArea;
        [SerializeField] private GameObject takeObject;

        /// <summary>
        /// 是否已抓取物件
        /// </summary>
        [Header("狀態")] [Tooltip("是否已抓取此物件")] [SerializeField]
        private bool snapTakeObject;
        /// <summary>
        ///Trigger放開後是否要脫離手勢
        /// </summary>
        [Tooltip("Trigger放開後是否要脫離手勢")] [SerializeField]
        private bool snapReleaseGesture;
        public SnapFixed snapFixed;
        [SerializeField] private ThrowOutside throwOutside;

        [Header("Gesture")]
        [Tooltip("SnapOnAttach=該對象應捕捉到手上指定的附著點的位置；" +
                 "DetachOthers=附著在此手上的其他物體將被分離；" +
                 "DetachFromOtherHand=該對象將與另一隻手分離；" +
                 "VelocityMovement=對象將嘗試移動以匹配手的位置和旋轉；" +
                 "AllowSidegrade=該對象能夠從捏握切換為抓握")]
        [SerializeField] private Hand.AttachmentFlags attachmentFlags;
        [Tooltip( "由於扳機保持而不是扳機按下，該物體必須移動多快才能固定？ (-1 to disable)" )]
        public float catchingSpeedThreshold = -1;
        [Tooltip("保持時用作位置和旋轉偏移量的局部點")]
        public Transform attachmentOffset;


        [Header("Rigibody")]
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private VelocityEstimator velocityEstimator;
        [SerializeField] protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

        /// <summary>
        /// 放開物件後的重力模擬
        /// </summary>
        [Header("物理模擬")]
        public ReleaseStyle releaseVelocityStyle = ReleaseStyle.GetFromHand;
        [Tooltip("使用RawFromHand選項釋放對象時使用的時間偏移")]
        public float releaseVelocityTimeOffset = -0.011f;
        [Tooltip("釋放速度幅度代表水垢釋放速度曲線的終點. (-1 to disable)")]
        public float scaleReleaseVelocityThreshold = -1.0f;
        [Tooltip("使用此曲線可根據測得的釋放速度的大小輕鬆按比例縮放釋放速度。 這樣可以更好地區分跌落，拋擲和投擲。")]
        public AnimationCurve scaleReleaseVelocityCurve = AnimationCurve.EaseInOut(0.0f, 0.1f, 1.0f, 1.0f);
        public float scaleReleaseVelocity = 1.1f;
        [Tooltip( "分離對象時，它應該返回其原始父對象嗎？" )]
        public bool restoreOriginalParent = false;

        [Header("InteractComponent")]
        [SerializeField] private Interactable interactable;
        [SerializeField] protected bool attached = false;
        [SerializeField] protected float attachTime;
        [SerializeField] protected Vector3 attachPosition;
        [SerializeField] protected Quaternion attachRotation;


        [Header("事件")]
        [SerializeField] private UnityEvent snapIn;
        [SerializeField] UnityEvent snapOut;
        [SerializeField] private UnityEvent onPickUp;
        // [SerializeField] private HandEvent onHeldUpdate;
        [SerializeField] private UnityEvent dropDown;

        private void Awake()
        {

            Initialize();
        }

        private void Update()
        {
            GrabGameObject();
        }


        protected virtual void OnHandHoverBegin( Hand hand )
        {
            // hand.ShowGrabHint();
            snapIn.Invoke();
            // 通過按住交互按鈕而不是按下按鈕來“捕獲”可拋出對象.
            // 僅當可拋物的移動速度快於規定的閾值速度並且未與另一隻手連接時，才執行此操作
            // if ( !attached /*&& catchingSpeedThreshold != -1*/)
            // {
            //     // float catchingThreshold = catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);
            //
            //     GrabTypes bestGrabType = hand.GetBestGrabbingType();
            //
            //     if ( bestGrabType != GrabTypes.None )
            //     {
            //
            //         // if (rigidbody.velocity.magnitude >= catchingThreshold)
            //         // {
            //             print(bestGrabType);
            //             hand.AttachObject( gameObject, bestGrabType, attachmentFlags );
            //         // }
            //     }
            // }


        }

        protected virtual void OnHandHoverEnd( Hand hand )
        {
            // hand.HideGrabHint();
            snapOut.Invoke();
        }

        protected virtual void HandHoverUpdate( Hand hand )
        {

            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
            {
                hand.AttachObject( gameObject, startingGrabType, attachmentFlags, attachmentOffset );
                // print($"{hand.name}抓住了{gameObject.name}");
                // hand.HideGrabHint();
                if (!hand.otherHand.twoHandGrab && hand.twoHandGrab)
                {
                    hand.changePositionByTwoHands=true;
                     print("雙手抓取");
                    snapTakeObject = true;
                    if (snapFixed.isFixed && snapReleaseGesture)
                    {
                        print($"已鬆手，且物件已黏合:{snapFixed.isFixed}");
                        print($"物件吻合:{snapFixed.isLocated}");
                        snapFixed.isFixed = false;
                        snapFixed.isLocated = false;
                        // snapOut.Invoke();
                    }
                }
                else
                {

                }
            }
        }

        protected virtual void OnAttachedToHand( Hand hand )
        {
            hand.HoverLock( null );
            if (hand.twoHandGrab && !hand.otherHand.twoHandGrab)
            {
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

        protected virtual void OnDetachedFromHand(Hand hand)
        {
            // print($"IsGrabEnding={hand.otherHand.IsGrabEnding(gameObject)}");

            // if (!hand.otherHand.IsGrabEnding(gameObject))
            // {
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
            // }




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
                    if (snapReleaseGesture)
                    {
                        //強制鬆手
                        hand.DetachObject(gameObject);
                        hand.otherHand.DetachObject(gameObject);
                        // playerHand_L.DetachObject(gameObject);
                        // playerHand_R.DetachObject(gameObject);
                        // snapTakeObject = false;
                    }

                    //Snap in：吻合物件到指定位置
                    snapFixed.isFixed = true;
                    // print($"正在抓取{gameObject.name}");
                }
                //如果物件未吻合於區域
                else
                {
                    hand.otherHand.DetachObject(gameObject);
                    // hand.otherHand.ObjectIsAttached(gameObject);
                    hand.DetachObject(gameObject);
                    hand.otherHand.HoverUnlock(interactable);
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

                    snapTakeObject = false;
                    rigidbody.isKinematic = false;
                    Debug.Log("放下了" + gameObject.name);
                }
            }
            else
            {
                // dropDown.Invoke();
            }

            // if (onHeldUpdate != null)
            //     onHeldUpdate.Invoke(hand);
        }

        protected virtual void OnHandFocusAcquired( Hand hand )
        {
            gameObject.SetActive( true );

            if (velocityEstimator != null)
                velocityEstimator.BeginEstimatingVelocity();
        }

        protected virtual void OnHandFocusLost( Hand hand )
        {
            gameObject.SetActive( false );

            if (velocityEstimator != null)
                velocityEstimator.FinishEstimatingVelocity();
        }




        public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (hand.noSteamVRFallbackCamera && releaseVelocityStyle != ReleaseStyle.NoChange)
                releaseVelocityStyle = ReleaseStyle.ShortEstimation; // only type that works with fallback hand is short estimation.

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
                        Debug.LogWarning("[SteamVR Interaction System] Throwable: No Velocity Estimator component on object but release style set to short estimation. Please add one or change the release style.");

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
                        scaleFactor = Mathf.Clamp01(scaleReleaseVelocityCurve.Evaluate(velocity.magnitude / scaleReleaseVelocityThreshold));
                    }

                    velocity *= (scaleFactor * scaleReleaseVelocity);
            }
        }

        private void Initialize()
        {
            if (OriginalPositionGameObject==null)
            {
                OriginalPositionGameObject = transform.parent.gameObject;
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

            rigidbody = GetComponent<Rigidbody>();
            rigidbody.maxAngularVelocity = 50.0f;
            rigidbody.isKinematic = true;

            velocityEstimator = GetComponent<VelocityEstimator>();
            interactable = GetComponent<Interactable>();


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
                useObject.SetActive(false);
            }
        }

        private void GrabGameObject()
        {
            //如果以抓取物件
            if (snapTakeObject)
            {
                // rigidbody.isKinematic = false;
                //開啟黏著區物件
                foreach (GameObject useObject in UsePosition)
                {
                    useObject.SetActive(true);
                }

                //已鬆手，物件已修正於新區域
                if (snapFixed.isFixed)
                {
                    // gameObject.tag = "FixObject";

                    foreach (TakeEvent_SnapArea snapZone in snapZoneArea)
                    {
                        //判斷是否為觸發放置區域
                        if (snapZone.isSnapIn)
                        {
                            transform.SetParent(snapZone.transform.parent);
                            print($"{gameObject.name}已成為{snapZone.transform.parent.name}的子物件");
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
                    onPickUp.Invoke();
                }
            }
            else
            {
                // rigidbody.isKinematic = true;
                //放下物品：參數重置
                // gameObject.tag = "DropObject";
                transform.SetParent(OriginalPositionGameObject.transform);

                snapFixed.isLocated = false;
                snapFixed.isFixed = false;
                //snapFixed.isThrowed = false;

                //隱藏放置提示輪廓線
                foreach (GameObject useObject in UsePosition)
                {
                    // useObject.transform.GetChild(0).GetComponent<SnapZoneArea>().isSnapIn = true;
                    useObject.SetActive(false);
                }

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


}

