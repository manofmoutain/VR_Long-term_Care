using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    [RequireComponent(typeof(Interactable))]
    public class TakeEvent_TwoHandSnapPuZone : MonoBehaviour
    {
        [Header("模型位置參數")] [SerializeField] private Vector3 originPosition;
        [SerializeField] private Vector3 originRotation;
        [SerializeField] private Vector3 originScale;

        [Header("雙手參數")]
        // [SerializeField] private GameObject AttachObjectLeft;
        // [SerializeField] private GameObject AttachObjectRight;
        [SerializeField] private Hand playerHand_L;
        [SerializeField] private Hand playerHand_R;
        [SerializeField] private Vector3 handPos;
        [SerializeField] private Vector3 worldPlaneNormal = new Vector3(1, 0, 0);


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

        [System.Serializable]
        public class SnapFixed
        {
            /// <summary>
            /// 定位判斷－抓取的物件若吻合於黏貼區(未鬆手)
            /// </summary>
            [Tooltip("定位判斷－抓取的物件若吻合於黏貼區(未鬆手) ")] public bool isLocated;

            /// <summary>
            /// 吻合判斷－抓取的物件若吻合於黏貼區(已鬆手，物件已修正於黏貼區)
            /// </summary>
            [Tooltip("吻合判斷－抓取的物件若吻合於黏貼區(已鬆手，物件已修正於黏貼區) ")]
            public bool isFixed;

            /// <summary>
            /// 定點判斷
            /// </summary>
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

        [SerializeField] private ThrowOutside throwOutside;

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
        [Tooltip("原本位置")] [SerializeField] GameObject OriginalPosition;
        [Tooltip("要黏著的物件")] [SerializeField] GameObject[] UsePosition;
        [SerializeField] List<TakeEvent_SnapArea> snapZoneArea;
        [SerializeField] private GameObject takeObject;

        /// <summary>
        /// 雙手碰觸到物件的事件
        /// </summary>
        [Header("事件")] [SerializeField] UnityEvent snapIn;

        /// <summary>
        /// 雙手離開物件的事件
        /// </summary>
        [SerializeField] UnityEvent snapOut;

        public UnityEvent attachUpdate;

        /// <summary>
        /// 雙手抓取物件的事件
        /// </summary>
        [SerializeField] public UnityEvent pickUp;

        /// <summary>
        /// 雙手鬆開物件但尚未離開物件的事件
        /// </summary>
        [SerializeField] public UnityEvent dropDown;


        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
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

        private void Update()
        {
            GrabGameObject();
        }

        private void GrabGameObject()
        {
            //如果以抓取物件
            if (snapTakeObject)
            {
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
                        }
                    }


                    transform.localPosition = originPosition;
                    transform.localEulerAngles = originRotation;
                    transform.localScale = originScale;

                    snapIn.Invoke();
                }

                else
                {
                    pickUp.Invoke();
                }
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


        private void OnHandHoverBegin(Hand hand)
        {
            //snapFixed.isHover = true;
            hand.ShowGrabHint();
            snapIn.Invoke();
        }


        private void OnHandHoverEnd(Hand hand)
        {
            switch (hand.name)
            {
                case "LeftHand":

                    playerHand_L = null;
                    // AttachObjectLeft = playerHand_L.currentAttachedObject.gameObject;
                    break;


                case "RightHand":

                    playerHand_R = null;
                    // AttachObjectRight = playerHand_R.currentAttachedObject.gameObject;
                    // handPos = ComputeToTransformProjected(playerHand_R.transform);
                    break;
            }

            hand.HideGrabHint();
            snapOut.Invoke();
        }

        /// <summary>
        /// 碰觸物件時持續更新的事件
        /// </summary>
        /// <param name="hand"></param>
        private void HandHoverUpdate(Hand hand)
        {
            switch (hand.name)
            {
                case "LeftHand":

                    playerHand_L = hand;
                    // AttachObjectLeft = playerHand_L.currentAttachedObject.gameObject;
                    break;


                case "RightHand":

                    playerHand_R = hand;
                    // AttachObjectRight = playerHand_R.currentAttachedObject.gameObject;
                    // handPos = ComputeToTransformProjected(playerHand_R.transform);
                    break;
            }

            GrabTypes grabTypes = hand.GetGrabStarting();
            //GRAB THE OBJECT
            if (grabTypes != GrabTypes.None)
            {
                // print(grabTypes);
                if (interactable.attachedToHand == null)
                {
                    // print($"目前沒有任何一隻手附著到此物件");
                    //如果雙手同時附著到此物件
                    if (playerHand_L != null && playerHand_R != null)
                    {

                        hand.AttachObject(gameObject, grabTypes);
                        hand.HoverLock(interactable);
                        hand.HideGrabHint();
                        // playerHand_L.AttachObject(gameObject, grabTypes);
                        // playerHand_L.HoverLock(interactable);
                        // playerHand_L.HideGrabHint();
                        // playerHand_R.AttachObject(gameObject,grabTypes);
                        // playerHand_R.HoverLock(interactable);
                        // playerHand_R.HideGrabHint();

                        //如果設定Trigger鬆開後手勢脫離物件
                        if (snapFixed.isFixed && snapReleaseGesture)
                        {
                            snapFixed.isFixed = false;
                            print($"已鬆手:{snapFixed.isFixed}");
                            snapFixed.isLocated = false;
                            print($"物件吻合:{snapFixed.isLocated}");
                            // snapOut.Invoke();
                        }


                        //拿起指定的物件
                        snapTakeObject = true;
                        Debug.Log($"抓住了{gameObject.name}");


                        //開啟置放區的觸發
                        // for (int i = 0; i < UsePosition.Length; i++)
                        // {
                        //     snapZoneArea[i].sphereCollider.isTrigger = true;
                        // }
                    }
                }
                else
                {
                    print($"附著到手上的是{interactable.attachedToHand}");
                    //判斷觸發把手是否相同：拾起 / 放下 須皆為同隻手把
                    if (sanpCurrentHand == hand)
                    {
                        if (playerHand_L!=null && playerHand_R!=null)
                        {
                            //Snap out：釋放吻合物件
                            if (snapFixed.isFixed && !snapReleaseGesture)
                            {
                                snapFixed.isFixed = false;
                                snapFixed.isLocated = false;

                                snapOut.Invoke();
                            }
                        }
                        else
                        {
                            hand.DetachObject(gameObject);
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
            // print($"isGrabEnding is {hand.IsGrabEnding(gameObject)}");
            //處於鬆開Trigger的狀態
            if (isGrabEnding)
            {
                //如果未鬆手
                if (snapFixed.isLocated)
                {
                    //鬆開Trigger若手勢脫離的情況
                    if (snapReleaseGesture)
                    {
                        //強制鬆手
                        hand.DetachObject(gameObject);
                        // playerHand_L.DetachObject(gameObject);
                        // playerHand_R.DetachObject(gameObject);
                    }

                    //Snap in：吻合物件到指定位置
                    snapFixed.isFixed = true;
                    sanpCurrentHand = hand;
                    // print($"正在抓取{gameObject.name}");
                }
                //如果已鬆手(正常情況)
                else
                {
                    hand.DetachObject(gameObject);
                    // playerHand_L.DetachObject(gameObject);
                    // playerHand_R.DetachObject(gameObject);
                    hand.HoverUnlock(interactable);
                    // playerHand_L.HoverUnlock(interactable);
                    // playerHand_R.HoverUnlock(interactable);
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
                // for (int i = 0; i < UsePosition.Length; i++)
                // {
                //     snapZoneArea[i].sphereCollider.isTrigger = false;
                // }
            }
            else
            {
                if (playerHand_L==null || playerHand_R==null)
                {
                    print($"物件脫離手勢");
                    // hand.DetachObject(gameObject);
                    // hand.HoverUnlock(interactable);
                    //
                    // if (throwOutside.outside)
                    // {
                    //     //計算物件原始位置與目前位置的距離：判斷是否有將物件移開
                    //     if ((transform.position - throwOutside.outsideZone.position).sqrMagnitude >
                    //         throwOutside.outsideRange)
                    //     {
                    //         snapFixed.isOutside = true;
                    //     }
                    // }
                    //
                    // snapTakeObject = false;
                    // rigidbody.isKinematic = false;

                }
                dropDown.Invoke();
            }
        }

        /// <summary>
        /// 鬆開物件的瞬間事件
        /// </summary>
        /// <param name="hand"></param>
        private void OnDetachedFromHand(Hand hand)
        {
            switch (hand.name)
            {
                case "LeftHand":
                    // AttachObjectLeft = null;
                    playerHand_L = null;
                    break;


                case "RightHand":

                    // AttachObjectRight = null;
                    playerHand_R = null;
                    break;
            }

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


        private Vector3 ComputeToTransformProjected(Transform xForm)
        {
            //normalized： 以此當前向量作為標準值(正規化)
            Vector3 toTransform = (xForm.position - gameObject.transform.position).normalized;

            Vector3 toTransformProjected = new Vector3(0.0f, 0.0f, 0.0f);

            //magnitude：把 vertor 平方相加在開根號
            //sqrMagnitude：把vertor 平方
            //確定抓到物體，
            if (toTransform.sqrMagnitude > 0.0f)
            {
                toTransformProjected = Vector3.ProjectOnPlane(toTransform, worldPlaneNormal).normalized;
            }

            return toTransformProjected;
        }

        /// <summary>
        /// 鬆手之後給予物件的速度值
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="velocity"></param>
        /// <param name="angularVelocity"></param>
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
    }
}