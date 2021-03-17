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

        [Header("手勢")]
        [SerializeField] private Hand.AttachmentFlags attachmentFlags;

        /// <summary>
        /// 設定抓握的方式
        /// </summary>
        [Tooltip("設定抓握的方式")][SerializeField] private GrabTypes grabbedWithType;

        [Header("旋轉角度")]
        [Tooltip("驅動器的輸出角度值（以度為單位，無限制）將無限制地增加或減少，採用360模數來查找轉數")]
        public float outAngle;

        /// <summary>
        /// 給予Animator的值(0-1)
        /// </summary>
        public float value;

        /// <summary>
        /// 是否要開啟角度事件，不可與Limit共用
        /// </summary>
        [Tooltip("是否要開啟角度事件，不可與Limit共用")][SerializeField] private bool isAngleEvent;

        /// <summary>
        /// 執行事件的角度值
        /// </summary>
        [Tooltip("執行事件的角度值")]public float eventAngle;

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

        [Header("驅動設定")]
        [Tooltip("圓形驅動器將在局部空間中繞其旋轉的軸")]
        public Axis_t axisOfRotation = Axis_t.XAxis;

        /// <summary>
        /// 是否可以轉動
        /// </summary>
        [Tooltip("是否可以轉動")]
        public bool rotateGameObject = true;

        /// <summary>
        /// 鬆手後是否要轉回原角度
        /// </summary>
        [SerializeField]private bool isDetachedToResetAngle;

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
        [Tooltip("如果limited為true，旋轉的極限值")]
        public Vector2 frozenDistanceMinMaxThreshold = new Vector2(0.1f, 0.2f);

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
        [Header("強制改變初始值")]
        [Tooltip("如果limited為true，則強制將起始角度設為startAngle，並固定為[minAngle，maxAngle]")]
        public bool forceStart = false;

        /// <summary>
        /// 強制設定的初始旋轉值
        /// </summary>
        [Tooltip("如果limited為true且forceStart為true，則起始角度將為此角度，並固定為[minAngle，maxAngle]")]
        public float startAngle = 0.0f;



        [Header("Debug")]
        [Tooltip("繪製“手”的路徑（紅色）和預計值（綠色）")]
        public bool debugPath = false;

        [Tooltip("創建以繪製路徑的GameObject的最大數量")]
        public int dbgPathLimit = 50;

        [Tooltip("顯示此圓形驅動器的線性值和角度值")]
        public TextMesh debugText = null;
        [SerializeField] private Color red = new Color(1.0f, 0.0f, 0.0f);
        [SerializeField] private Color green = new Color(0.0f, 1.0f, 0.0f);
        [SerializeField] private GameObject[] dbgHandObjects;
        [SerializeField] private GameObject[] dbgProjObjects;
        [SerializeField] private GameObject dbgObjectsParent;
        [SerializeField] private int dbgObjectCount = 0;
        [SerializeField] private int dbgObjectIndex = 0;


        [Header("隱藏的數值")]
        [SerializeField] private Quaternion start;

        [SerializeField] private Vector3 worldPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);
        [SerializeField] private Vector3 localPlaneNormal = new Vector3(1.0f, 0.0f, 0.0f);

        [SerializeField] private Vector3 lastHandProjected;




        /// <summary>
        /// 是否正在驅動
        /// </summary>
        [SerializeField] private bool driving = false;

        /// <summary>
        /// 如果驅動器被限制為最小/最大，則大於此角度的角度將被忽略
        /// </summary>
        [SerializeField] private float minMaxAngularThreshold = 1.0f;


        /// <summary>
        /// 是否要凍結(並非不轉動，而是會在frozenSqDistanceMinMaxThreshold兩個值之間抖動)
        /// </summary>
        [Tooltip("是否要凍結(並非不轉動，而是會在frozenSqDistanceMinMaxThreshold兩個值之間抖動)")]
        [SerializeField] private bool frozen = false;
        /// <summary>
        /// 凍結瞬間的角度
        /// </summary>
        [SerializeField] private float frozenAngle = 0.0f;
        /// <summary>
        /// 凍結時手的位置
        /// </summary>
        [SerializeField] private Vector3 frozenHandWorldPos = new Vector3(0.0f, 0.0f, 0.0f);
        /// <summary>
        /// 凍結時，角度的偏移值
        /// </summary>
        [SerializeField] private Vector2 frozenSqDistanceMinMaxThreshold = new Vector2(0.0f, 0.0f);

        /// <summary>
        /// 抓握的是哪一隻手
        /// </summary>
        [SerializeField] private Hand handHoverLocked = null;

        [SerializeField] private Interactable interactable;

        

        private void Awake()
        {
            interactable = this.GetComponent<Interactable>();
        }

        private void Start()
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

            if (debugText)
            {
                debugText.alignment = TextAlignment.Left;
                debugText.anchor = TextAnchor.UpperLeft;
            }

            UpdateAll();
        }

        private void Update()
        {
            // UpdateLinearMapping();
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
            handHoverLocked = null;
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
                lastHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);

                if (hoverLock)
                {
                    hand.HoverLock(interactable);
                    handHoverLocked = hand;
                }
                hand.AttachObject(gameObject, startingGrabType,attachmentFlags);
                driving = true;

                ComputeAngle(hand);
                UpdateAll();

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
                    handHoverLocked = null;
                }

                driving = false;
                grabbedWithType = GrabTypes.None;
            }

            if (driving && isGrabEnding == false && hand.hoveringInteractable == this.interactable)
            {
                ComputeAngle(hand);
                UpdateAll();
            }
        }

        void HandAttachedUpdate(Hand hand)
        {
            UpdateLinearMapping();
            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject);
            }
        }



        void OnDetachedFromHand(Hand hand)
        {

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

            if (debugPath && dbgPathLimit > 0)
            {
                DrawDebugPath(xForm, toTransformProjected);
            }

            return toTransformProjected;
        }


        /// <summary>
        ///描繪手移動的路徑(Debug用)
        /// </summary>
        /// <param name="xForm"></param>
        /// <param name="toTransformProjected"></param>
        private void DrawDebugPath(Transform xForm, Vector3 toTransformProjected)
        {
            if (dbgObjectCount == 0)
            {
                dbgObjectsParent = new GameObject("Circular Drive Debug");
                dbgHandObjects = new GameObject[dbgPathLimit];
                dbgProjObjects = new GameObject[dbgPathLimit];
                dbgObjectCount = dbgPathLimit;
                dbgObjectIndex = 0;
            }

            //Actual path
            GameObject gSphere = null;

            if (dbgHandObjects[dbgObjectIndex])
            {
                gSphere = dbgHandObjects[dbgObjectIndex];
            }
            else
            {
                gSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                gSphere.transform.SetParent(dbgObjectsParent.transform);
                dbgHandObjects[dbgObjectIndex] = gSphere;
            }

            gSphere.name = string.Format("actual_{0}", (int) ((1.0f - red.r) * 10.0f));
            gSphere.transform.position = xForm.position;
            gSphere.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            gSphere.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
            gSphere.gameObject.GetComponent<Renderer>().material.color = red;

            if (red.r > 0.1f)
            {
                red.r -= 0.1f;
            }
            else
            {
                red.r = 1.0f;
            }

            //Projected path
            gSphere = null;

            if (dbgProjObjects[dbgObjectIndex])
            {
                gSphere = dbgProjObjects[dbgObjectIndex];
            }
            else
            {
                gSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                gSphere.transform.SetParent(dbgObjectsParent.transform);
                dbgProjObjects[dbgObjectIndex] = gSphere;
            }

            gSphere.name = string.Format("projed_{0}", (int) ((1.0f - green.g) * 10.0f));
            gSphere.transform.position = transform.position + toTransformProjected * 0.25f;
            gSphere.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            gSphere.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
            gSphere.gameObject.GetComponent<Renderer>().material.color = green;

            if (green.g > 0.1f)
            {
                green.g -= 0.1f;
            }
            else
            {
                green.g = 1.0f;
            }

            dbgObjectIndex = (dbgObjectIndex + 1) % dbgObjectCount;
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

            UpdateDebugText();
        }


        /// <summary>
        /// 持續更新物件的旋轉值
        /// </summary>
        private void UpdateGameObject()
        {
            if (rotateGameObject)
            {
                transform.localRotation = start * Quaternion.AngleAxis(outAngle, localPlaneNormal);
            }
        }


        /// <summary>
        /// 使用線性映射值和角度更新Debug TextMesh
        /// </summary>
        private void UpdateDebugText()
        {
            if (debugText)
            {
                debugText.text = string.Format("Linear: {0}\nAngle:  {1}\n", linearMapping.value, outAngle);
            }
        }


        /// <summary>
        /// 使用線性映射值和角度更新Debug TextMesh
        /// </summary>
        private void UpdateAll()
        {
            UpdateLinearMapping();
            UpdateGameObject();
            UpdateDebugText();
        }


        /// <summary>
        /// 根據變換的變化計算旋轉遊戲對象的角度
        /// </summary>
        /// <param name="hand"></param>
        private void ComputeAngle(Hand hand)
        {
            Vector3 toHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);

            if (!toHandProjected.Equals(lastHandProjected))
            {
                float absAngleDelta = Vector3.Angle(lastHandProjected, toHandProjected);

                if (absAngleDelta > 0.0f)
                {
                    if (frozen)
                    {
                        float frozenSqDist = (hand.hoverSphereTransform.position - frozenHandWorldPos).sqrMagnitude;
                        if (frozenSqDist > frozenSqDistanceMinMaxThreshold.x)
                        {
                            outAngle = frozenAngle + Random.Range(-1.0f, 1.0f);

                            float magnitude = Util.RemapNumberClamped(frozenSqDist,
                                frozenSqDistanceMinMaxThreshold.x, frozenSqDistanceMinMaxThreshold.y, 0.0f, 1.0f);
                            if (magnitude > 0)
                            {
                                StartCoroutine(HapticPulses(hand, magnitude, 10));
                            }
                            else
                            {
                                StartCoroutine(HapticPulses(hand, 0.5f, 10));
                            }

                            if (frozenSqDist >= frozenSqDistanceMinMaxThreshold.y)
                            {
                                onFrozenDistanceThreshold.Invoke();
                            }
                        }
                    }
                    else
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
                                if (freezeOnMin)
                                {
                                    Freeze(hand);
                                }
                            }
                            else if (angleTmp == maxAngle)
                            {
                                outAngle = angleTmp;
                                lastHandProjected = toHandProjected;
                                onMaxAngle.Invoke();
                                if (freezeOnMax)
                                {
                                    Freeze(hand);
                                }
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
                            if (isAngleEvent && outAngle>=eventAngle)
                            {
                                angleEvent.Invoke();
                            }
                            lastHandProjected = toHandProjected;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 如果有設定凍結最小值或最大值，凍結手的位置？
        /// </summary>
        /// <param name="hand"></param>
        private void Freeze(Hand hand)
        {
            frozen = true;
            frozenAngle = outAngle;
            //鎖定手的位置
            frozenHandWorldPos = hand.hoverSphereTransform.position;
            frozenSqDistanceMinMaxThreshold.x = frozenDistanceMinMaxThreshold.x * frozenDistanceMinMaxThreshold.x;
            frozenSqDistanceMinMaxThreshold.y = frozenDistanceMinMaxThreshold.y * frozenDistanceMinMaxThreshold.y;
        }


        //-------------------------------------------------
        private void UnFreeze()
        {
            frozen = false;
            frozenHandWorldPos.Set(0.0f, 0.0f, 0.0f);
        }

        #endregion
    }
}