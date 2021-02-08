using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace InrteractableObject
{
    /// <summary>
    /// 要黏著於其他物件的物件
    /// </summary>
    public class SnapTakeDropZone : MonoBehaviour
    {
        /// <summary>
        /// 是否已黏貼物體
        /// </summary>
        private bool snapTakeObject;

        /// <summary>
        ///是否已黏貼手勢(虛擬手)
        /// </summary>
        public bool snapReleaseGesture;

        [SerializeField] private float catchingSpeedThreshold;
        [SerializeField] private float releaseVelocityTimeOffset;
        [SerializeField] private float scaleReleaseVelocity;
        [SerializeField] private float scaleReleaseVelocityThreshold;

        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

        [SerializeField] private Hand sanpCurrentHand;
        [SerializeField] private VelocityEstimator velocityEstimator;
        [SerializeField] private AnimationCurve scaleReleaseVelocityCurve;

        [Tooltip("原本的位置")]public GameObject OriginalPosition;

        [Tooltip("要黏著的區域")]public GameObject[] UsePosition;

        [SerializeField] private GameObject takeObject;
        [SerializeField] private GameObject headTurnRoundGroup;

        [SerializeField] private Interactable interactable;
        public List<SnapZoneArea> snapZoneArea;


        [System.Serializable]
        public class SnapFixed
        {
            [Tooltip("定位判斷－抓取的物件若吻合於黏貼區(未鬆手) ")]
            public bool isLocated;

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

        public UnityEvent snapIn;
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
            for (int u = 0; u < UsePosition.Length; u++)
            {
                snapZoneArea.Add(UsePosition[u].transform.GetChild(0).GetComponent<SnapZoneArea>());
            }

            //隱藏 Sanp Objects 的放置提示輪廓線
            foreach (GameObject useObject in UsePosition)
            {
                useObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (snapTakeObject)
            {
                gameObject.tag = "SnapObject";

                //顯示 Sanp Objects 的放置提示輪廓線
                foreach (GameObject useObject in UsePosition)
                {
                    useObject.SetActive(true);
                }

                //固定位置：Snap Position.
                if (snapFixed.isFixed)
                {
                    gameObject.tag = "FixObject";

                    foreach (SnapZoneArea snapzone in snapZoneArea)
                    {
                        //判斷是否為觸發放置區域
                        if (snapzone.gameObject.tag == "AreaZone")
                        {
                            gameObject.transform.parent = snapzone.transform.parent.transform;
                        }
                    }


                    gameObject.transform.localPosition = new Vector3();
                    gameObject.transform.localEulerAngles = new Vector3();
                    gameObject.transform.localScale = new Vector3(1, 1, 1);

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
                gameObject.tag = "DropObject";
                gameObject.transform.parent = OriginalPosition.transform;

                snapFixed.isLocated = false;
                snapFixed.isFixed = false;
                //snapFixed.isThrowed = false;

                //隱藏 Sanp Objects 的放置提示輪廓線
                foreach (GameObject useObject in UsePosition)
                {
                    useObject.transform.GetChild(0).tag = "Untagged";
                    useObject.SetActive(false);
                }


                if (snapFixed.isOutside)
                {
                    snapZoneArea[0].gameObject.tag = "AreaZone";

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
            hand.ShowGrabHint();

            headTurnRoundGroup.GetComponent<SphereCollider>().enabled = false;
        }


        private void OnHandHoverEnd(Hand hand)
        {
            //snapFixed.isHover = false;
            hand.HideGrabHint();
            headTurnRoundGroup.GetComponent<SphereCollider>().enabled = true;
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
                    hand.HideGrabHint();

                    //Snap out：釋放吻合物件
                    if (snapFixed.isFixed && snapReleaseGesture)
                    {
                        snapFixed.isFixed = false;
                        snapFixed.isLocated = false;
                        snapOut.Invoke();
                    }

                    //拿起指定的物件
                    snapTakeObject = true;

                    //開啟置放區的觸發
                    for (int i = 0; i < UsePosition.Length; i++)
                    {
                        snapZoneArea[i].sphereCollider.isTrigger = true;
                    }

                    Debug.Log($"{gameObject.name} is Grabbed.");
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


        private void OnAttachedToHand(Hand hand)
        {
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
                    //吻合物件後放開手勢
                    if (snapReleaseGesture)
                    {
                        hand.DetachObject(gameObject);
                    }

                    //Snap in：吻合物件到指定位置
                    snapFixed.isFixed = true;
                    sanpCurrentHand = hand;
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


                //開啟置放區的觸發
                for (int u = 0; u < UsePosition.Length; u++)
                {
                    snapZoneArea[u].sphereCollider.isTrigger = false;
                }
            }
        }

        private void OnDetachedFromHand(Hand hand)
        {
            Vector3 velocity;
            Vector3 angularVelocity;
            GetReleaseVelocities(hand, out velocity, out angularVelocity);

            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
            rigidbody.interpolation = hadInterpolation;

            takeObject = null;
            sanpCurrentHand = null;
        }
    }
}