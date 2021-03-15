using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_ANDROID  &&  !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼
using Valve.VR.InteractionSystem;
#endif

namespace InteractableObject
{
    /// <summary>
    /// 要黏著於其他物件的物件
    /// </summary>
    [RequireComponent( typeof( Interactable ) )]
    public class TakeEvent_SingleHandSnapPutZone : MonoBehaviour
    {
        /// <summary>
        /// 是否已抓取物件
        /// </summary>
        [Header("狀態")] [Tooltip("是否已抓取此物件")] [SerializeField]
        public bool snapTakeObject;

        /// <summary>
        ///Trigger放開後是否要脫離手勢
        /// </summary>
        [Tooltip("Trigger放開後是否要脫離手勢")] public bool snapReleaseGesture;

        [System.Serializable]
        public class SnapFixed
        {
            [Tooltip("定位判斷－抓取的物件若吻合於黏貼區(未鬆手) ")] public bool isLocated;

            [Tooltip("吻合判斷－抓取的物件若吻合於黏貼區(已鬆手，物件已修正於黏貼區) ")]
            public bool isFixed;

            [Tooltip("定點判斷 ")] public bool isOutside;
        }

        public SnapFixed snapFixed;

        [System.Serializable]
        public class ThrowOutside
        {
            public bool outside;
            public float outsideRange;
            public Transform outsideZone;
        }

        public ThrowOutside throwOutside;

        [Header("物理性")] [SerializeField] private float catchingSpeedThreshold;
        [SerializeField] private float releaseVelocityTimeOffset;
        [SerializeField] private float scaleReleaseVelocity;
        [SerializeField] private float scaleReleaseVelocityThreshold;

        [Header("本身的Component")] [SerializeField]
        private Rigidbody rigidbody;

        [SerializeField] private RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;
        [SerializeField] private VelocityEstimator velocityEstimator;
        [SerializeField] private AnimationCurve scaleReleaseVelocityCurve;
        [SerializeField] private Interactable interactable;

        [Header("外部物件")] [SerializeField] private Hand sanpCurrentHand;
        [Tooltip("原本位置")] public GameObject OriginalPosition;
        [Tooltip("要黏著的物件")] public GameObject[] UsePosition;
        public List<TakeEvent_SnapArea> snapZoneArea;
        [SerializeField] private GameObject takeObject;

        [Header("事件")] public UnityEvent snapIn;
        public UnityEvent snapOut;
        public UnityEvent pickUp;
        public UnityEvent dropDown;


        void Start()
        {
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
            interactable = gameObject.GetComponent<Interactable>();


            //取得 snapZoneArea 置放區域
            for (int i = 0; i < UsePosition.Length; i++)
            {
                snapZoneArea.Add(UsePosition[i].transform.GetChild(0).GetComponent<TakeEvent_SnapArea>());
            }

            //隱藏放置提示輪廓線
            foreach (GameObject useObject in UsePosition)
            {
                useObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (snapTakeObject)
            {
                // gameObject.tag = "SnapObject";

                //開啟黏著區物件
                foreach (GameObject useObject in UsePosition)
                {
                    useObject.SetActive(true);
                }

                //放置在新的transform之下
                if (snapFixed.isFixed)
                {
                    // gameObject.tag = "FixObject";

                    foreach (TakeEvent_SnapArea snapZone in snapZoneArea)
                    {
                        //判斷是否為觸發放置區域
                        if (snapZone.isSnapIn)
                        {
                            transform.SetParent(snapZone.transform.parent);
                            print($"{gameObject.name}已成為{snapZone.transform.parent.gameObject.name}的子物件");
                        }
                    }


                    transform.localPosition = new Vector3();
                    transform.localEulerAngles = new Vector3();
                    transform.localScale = new Vector3(1, 1, 1);

                    // snapIn.Invoke();
                }

                // else
                // {
                //     pickUp.Invoke();
                // }
            }
            else
            {
                //放下物品：參數重置
                // gameObject.tag = "DropObject";
                transform.SetParent(OriginalPosition.transform);

                snapFixed.isLocated = false;
                snapFixed.isFixed = false;
                //snapFixed.isThrowed = false;

                //隱藏放置提示輪廓線
                foreach (GameObject useObject in UsePosition)
                {
                    // useObject.transform.GetChild(0).GetComponent<SnapZoneArea>().isSnapIn = true;
                    useObject.SetActive(false);
                }


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
            }
        }

        private void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (hand.noSteamVRFallbackCamera)
            {
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
            }
            else
            {
                velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
                angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
            }


            float scaleFactor = 1.0f;
            if (scaleReleaseVelocityThreshold > 0)
            {
                scaleFactor =
                    Mathf.Clamp01(
                        scaleReleaseVelocityCurve.Evaluate(velocity.magnitude / scaleReleaseVelocityThreshold));
            }

            velocity *= (scaleFactor * scaleReleaseVelocity);
        }


        private void OnHandHoverBegin(Hand hand)
        {
            //snapFixed.isHover = true;
            // hand.ShowGrabHint();
            snapIn.Invoke();
        }


        private void OnHandHoverEnd(Hand hand)
        {
            //snapFixed.isHover = false;
            // hand.HideGrabHint();
            snapOut.Invoke();
        }


        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes grabTypes = hand.GetGrabStarting();

            //GRAB THE OBJECT
            if (grabTypes != GrabTypes.None)
            {
                if (interactable.attachedToHand == null)
                {
                    hand.AttachObject(gameObject, grabTypes);
                    hand.HoverLock(interactable);
                    // hand.HideGrabHint();

                    //手勢脫離物件
                    if (snapFixed.isFixed && snapReleaseGesture)
                    {
                        snapFixed.isFixed = false;
                        snapFixed.isLocated = false;
                        // snapOut.Invoke();
                    }

                    //拿起指定的物件
                    snapTakeObject = true;

                    //開啟置放區的觸發
                    for (int i = 0; i < UsePosition.Length; i++)
                    {
                        snapZoneArea[i].sphereCollider.isTrigger = true;
                    }

                    Debug.Log($"抓住了{gameObject.name}");
                }
                else
                {
                    //判斷觸發把手是否相同：拾起 / 放下 須皆為同隻手把
                    if (sanpCurrentHand == hand)
                    {
                        //Snap out：釋放吻合物件
                        if (snapFixed.isFixed && !snapReleaseGesture)
                        {
                            snapFixed.isFixed = false;
                            snapFixed.isLocated = false;

                            snapOut.Invoke();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 抓取物件的瞬間事件
        /// </summary>
        /// <param name="hand"></param>
        private void OnAttachedToHand(Hand hand)
        {
            pickUp.Invoke();


            hadInterpolation = rigidbody.interpolation;
            rigidbody.interpolation = RigidbodyInterpolation.None;

            if (velocityEstimator != null)
            {
                velocityEstimator.BeginEstimatingVelocity();
            }


            //拾起物件判斷
            takeObject = hand.currentAttachedObject;
        }

        /// <summary>
        /// 物件被抓取時持續偵測
        /// </summary>
        /// <param name="hand"></param>
        private void HandAttachedUpdate(Hand hand)
        {
            //放下物品
            bool isGrabEnding = hand.IsGrabEnding(gameObject);
            if (isGrabEnding)
            {

                if (snapFixed.isLocated)
                {
                    //鬆開Trigger若手勢脫離的情況
                    if (snapReleaseGesture)
                    {
                        hand.DetachObject(gameObject);
                    }

                    //Snap in：吻合物件到指定位置
                    snapFixed.isFixed = true;
                    sanpCurrentHand = hand;
                    print($"正在抓取{gameObject.name}");
                }
                else
                {
                    hand.DetachObject(gameObject);
                    hand.HoverUnlock(interactable);
                    //Debug.Log(gameObject.name + ": " + (transform.position - throwOutside.entryZone.position).sqrMagnitude);

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


                //關閉置放區的trigger
                for (int i = 0; i < UsePosition.Length; i++)
                {
                    snapZoneArea[i].sphereCollider.isTrigger = false;
                }
            }
        }

        /// <summary>
        /// 鬆開物件的瞬間事件
        /// </summary>
        /// <param name="hand"></param>
        private void OnDetachedFromHand(Hand hand)
        {
            dropDown.Invoke();

            Vector3 velocity;
            Vector3 angularVelocity;
            GetReleaseVelocities(hand, out velocity, out angularVelocity);

            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
            rigidbody.interpolation = hadInterpolation;

            // if (isReleaseGesture)
            // {
            //     snapReleaseGesture = true;
            // }
            // snapReleaseGesture = false;

            takeObject = null;
            sanpCurrentHand = null;
        }
    }
}