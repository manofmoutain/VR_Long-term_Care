using System.Collections;
using System.Collections.Generic;
using Autohand;
using InteractableObject;
using UnityEngine;
using UnityEngine.Events;

namespace AutoHandInteract
{
    [RequireComponent(typeof(AutoHand_Grabbable), typeof(Rigidbody))]
    public class AutoHand_HandGrab : MonoBehaviour
    {
        [Header("雙手資訊")] [SerializeField] private bool isNeedTwoHands;
        [SerializeField] private GameObject rightHandAttachedGameObject;
        [SerializeField] private GameObject leftHandAttachedGameObject;

        [Header("模型位置參數")] [Tooltip("原本位置")] [SerializeField]
        GameObject OriginalPositionGameObject;

        [SerializeField] private Vector3 originPosition;
        [SerializeField] private Vector3 originRotation;
        [SerializeField] private Vector3 originScale;


        /// <summary>
        /// 是否已抓取物件
        /// </summary>
        [Header("狀態")] [Tooltip("是否已抓取此物件")] [SerializeField]
        public bool snapTakeObject;

        /// <summary>
        ///Trigger放開後是否要脫離手勢
        /// </summary>
        [Tooltip("Trigger放開後是否要脫離手勢")] public bool snapReleaseGesture = true;

        [System.Serializable]
        public class SnapFixed
        {
            /// <summary>
            /// 抓取的物件若吻合於黏貼區(未鬆手)
            /// </summary>
            [Tooltip("定位判斷－抓取的物件若吻合於黏貼區(未鬆手) ")] public bool isLocated;

            /// <summary>
            /// 抓取的物件若吻合於黏貼區(已鬆手，物件已修正於黏貼區)
            /// </summary>
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

        public bool alignPosition;
        public bool alignRotation;
        public Vector3 alignPositionOffset;
        public Vector3 alignRotationOffset;

        // [Header("物理性")]
        private float catchingSpeedThreshold;
        private float releaseVelocityTimeOffset;
        private float scaleReleaseVelocity;
        private float scaleReleaseVelocityThreshold;

        // [Header("本身的Component")]
        [SerializeField] private Rigidbody body;
        private RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;
        private VelocityEstimator velocityEstimator;
        private AnimationCurve scaleReleaseVelocityCurve;

        [Header("外部物件")] [SerializeField] private Grabbable _grabbable;

        [Header("放置")] [Tooltip("原本位置")] public GameObject OriginalPosition;
        [Tooltip("要黏著的物件")] public GameObject[] UsePosition;
        public List<TakeEvent_SnapArea> snapZoneArea;
        private GameObject takeObject;

        [Header("事件")] [SerializeField] private UnityEvent pickUp;
        [SerializeField] private UnityEvent dropDown;


        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            GrabGameObject();
        }

        private void Initialize()
        {
            if (_grabbable == null && GetComponent<Grabbable>())
            {
                _grabbable = GetComponent<Grabbable>();
            }


            _grabbable.OnBeforeGrabEvent += OnBeforeGrab;
            _grabbable.OnGrabEvent += OnGrab;
            _grabbable.OnReleaseEvent += OnRelease;


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

            body = GetComponent<Rigidbody>();
            body.maxAngularVelocity = 50.0f;
            body.isKinematic = true;
            body.useGravity = true;
            velocityEstimator = GetComponent<VelocityEstimator>();


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

                //放置在新的transform之下
                if (snapFixed.isFixed)
                {
                    foreach (TakeEvent_SnapArea snapZone in snapZoneArea)
                    {
                        //判斷是否為觸發放置區域
                        if (snapZone.isSnapIn)
                        {
                            transform.SetParent(snapZone.transform.parent);
                            // print($"{gameObject.name}已成為{snapZone.transform.parent.gameObject.name}的子物件");
                        }
                    }

                    body.isKinematic = true;
                    transform.localPosition = originPosition;
                    transform.localEulerAngles = originRotation;
                    transform.localScale = originScale;
                }
                // LogManager.Instance.UpdateLogText($"isFixed : {snapFixed.isFixed}");
            }
            else
            {
                //放下物品：參數重置
                // transform.SetParent(OriginalPosition.transform);

                snapFixed.isLocated = false;
                snapFixed.isFixed = false;

                //snapFixed.isThrowed = false;

                //隱藏放置區物件
                foreach (GameObject useObject in UsePosition)
                {
                    // useObject.transform.GetChild(0).GetComponent<SnapZoneArea>().isSnapIn = true;
                    useObject.SetActive(false);
                }
            }
        }


        private void OnBeforeGrab(Hand hand, Grabbable grabbable)
        {
            snapFixed.isFixed = false;
            snapFixed.isLocated = false;
        }


        public void OnGrab(Hand hand, Grabbable grabbable)
        {
            if (isNeedTwoHands)
            {
                if (hand.left)
                {
                    leftHandAttachedGameObject = hand.GetHeldGrabbable().gameObject;
                }
                else
                {
                    rightHandAttachedGameObject = hand.GetHeldGrabbable().gameObject;
                }


                if (leftHandAttachedGameObject != null && rightHandAttachedGameObject != null)
                {
                    body.isKinematic = false;
                }
                else
                {
                    body.isKinematic = true;
                }
            }
            else
            {
                body.isKinematic = false;
            }


            GetComponent<Collider>().isTrigger = true;
            pickUp.Invoke();


            //拿起指定的物件
            snapTakeObject = true;


            // hadInterpolation = body.interpolation;
            // // body.isKinematic = false;
            // body.interpolation = RigidbodyInterpolation.None;
            //
            // if (velocityEstimator != null)
            // {
            //     velocityEstimator.BeginEstimatingVelocity();
            // }
        }

        public void OnRelease(Hand hand, Grabbable grabbable)
        {
            if (isNeedTwoHands)
            {
                if (hand.left)
                {
                    leftHandAttachedGameObject = null;
                }
                else
                {
                    rightHandAttachedGameObject = null;
                }
            }


            // snapTakeObject = false;

            if (snapFixed.isLocated)
            {
                //Snap in：吻合物件到指定位置
                snapFixed.isFixed = true;
                // LogManager.Instance.UpdateLogText($"{hand}已鬆手且isFixed = {snapFixed.isFixed}");
            }

            GetComponent<Collider>().isTrigger = false;
            dropDown.Invoke();


            // if (snapFixed.isFixed /*&& !snapReleaseGesture*/)
            // {
            //     snapFixed.isFixed = false;
            //     snapFixed.isLocated = false;
            // }


            // else
            // {
            //     if (throwOutside.outside)
            //     {
            //         //計算物件原始位置與目前位置的距離：判斷是否有將物件移開
            //         if ((transform.position - throwOutside.outsideZone.position).sqrMagnitude >
            //             throwOutside.outsideRange)
            //         {
            //             snapFixed.isOutside = true;
            //         }
            //     }
            //
            //     snapTakeObject = false;
            //     body.isKinematic = false;
            // }
            //
            // for (int i = 0; i < UsePosition.Length; i++)
            // {
            //     snapZoneArea[i].collider.isTrigger = false;
            // }
            //
            //
            // Vector3 velocity;
            // Vector3 angularVelocity;
            // GetReleaseVelocities(hand, out velocity, out angularVelocity);
            //
            // body.velocity = velocity;
            // body.angularVelocity = angularVelocity;
            // body.interpolation = hadInterpolation;
            //
            //
            // snapReleaseGesture = false;
            //
            // takeObject = null;
        }
    }
}