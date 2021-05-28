using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    public class SetControllerEvent : MonoBehaviour
    {
        public bool showController;
        private bool setLaserPointerPosition;


        private int releaseValveCount;
        private bool startHoldValveTimer;

        //public GameObject hand;
        // public GameObject entryFloorZone;

        public SteamVR_LaserPointer rightLaserPointer;
        public SteamVR_LaserPointer leftLaserPointer;

        public SteamVR_Action_Boolean DeviceTracked_Trigger;
        public SteamVR_Action_Boolean DeviceTracked_Teleport;
        public SteamVR_Action_Boolean DeviceTracked_Grab;
        public SteamVR_Action_Vector2 TouchPadInput;


        public SteamVR_Input_Sources hand_LeftType;
        public SteamVR_Input_Sources hand_RightType;


        // Start is called before the first frame update
        void Start()
        {
            showController = false;

            releaseValveCount = 0;

            //雷射
            //setLaserPointerPosition = true;
            rightLaserPointer.PointerIn += OnPointerIn;
            rightLaserPointer.PointerOut += OnPointerOut;
            rightLaserPointer.PointerClick += OnPointerClick;
            rightLaserPointer.gameObject.SetActive(false);

            leftLaserPointer.PointerIn += OnPointerIn;
            leftLaserPointer.PointerOut += OnPointerOut;
            leftLaserPointer.PointerClick += OnPointerClick;
            leftLaserPointer.gameObject.SetActive(false);


            //設定 SteamVR Input -> Actions
            DeviceTracked_Trigger = SteamVR_Actions.default_InteractUI;
            DeviceTracked_Teleport = SteamVR_Actions.default_Teleport;
            DeviceTracked_Grab = SteamVR_Actions.default_GrabGrip;
            TouchPadInput = SteamVR_Actions.default_TouchpadPosition;


            //抓取左右手把控制器
            hand_LeftType = SteamVR_Input_Sources.LeftHand;
            hand_RightType = SteamVR_Input_Sources.RightHand;

            //宣告監聽左右手把控制器狀態事件
            DeviceTracked_Trigger.AddOnStateDownListener(LeftTriggerDown, hand_LeftType);
            DeviceTracked_Trigger.AddOnStateUpListener(LeftTriggerUp, hand_LeftType);

            DeviceTracked_Trigger.AddOnStateDownListener(RightTriggerDown, hand_RightType);
            DeviceTracked_Trigger.AddOnStateUpListener(RightTriggerUp, hand_RightType);

            DeviceTracked_Teleport.AddOnStateDownListener(RightTouchPadDown, hand_RightType);
            DeviceTracked_Teleport.AddOnStateUpListener(RightTouchPadUp, hand_RightType);

            DeviceTracked_Teleport.AddOnStateDownListener(LeftTouchPadDown, hand_LeftType);
            DeviceTracked_Teleport.AddOnStateUpListener(LeftTouchPadUp, hand_LeftType);

            DeviceTracked_Grab.AddOnStateDownListener(RightGrabDown, hand_RightType);
            DeviceTracked_Grab.AddOnStateUpListener(RightGrabUp, hand_RightType);

            DeviceTracked_Grab.AddOnStateDownListener(LeftGrabDown, hand_LeftType);
            DeviceTracked_Grab.AddOnStateUpListener(LeftGrabUp, hand_LeftType);
        }

        private void OnPointerClick(object sender, PointerEventArgs e)
        {
            IPointerClickHandler clickHandler = e.target.GetComponent<IPointerClickHandler>();
            if (clickHandler == null)
            {
                return;
            }

            clickHandler.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        private void OnPointerOut(object sender, PointerEventArgs e)
        {
            IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
            if (pointerExitHandler == null)
            {
                return;
            }

            pointerExitHandler.OnPointerExit(new PointerEventData(EventSystem.current));
        }

        private void OnPointerIn(object sender, PointerEventArgs e)
        {
            IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();
            if (pointerEnterHandler == null)
            {
                return;
            }

            pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
        }


        // Update is called once per frame
        void Update()
        {
            foreach (var hand in Player.instance.hands)
            {
                if (showController)
                {
                    //顯示控制器
                    hand.ShowController();

                    //Controller Match 手部骨架
                    hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithController);
                }
                else
                {
                    //隱藏控制器
                    hand.HideController();

                    //Controller Match Out 手部骨架
                    hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithoutController);
                }
            }


            if (startHoldValveTimer)
            {

            }

            ////重置雷射位置：在食指指端
            //if (GameObject.FindGameObjectWithTag("pointCheck") != null && setLaserPointerPosition)
            //{
            //    steamVRLaserPointer.transform.parent = GameObject.FindGameObjectWithTag("pointCheck").transform;

            //    steamVRLaserPointer.transform.localPosition = Vector3.zero;
            //    steamVRLaserPointer.transform.localEulerAngles = new Vector3(-6, 90, 8);
            //    steamVRLaserPointer.transform.localScale = new Vector3(1, 1, 1);

            //    StartCoroutine(ReplyLaserPointerPosition());
            //    setLaserPointerPosition = false;
            //}
        }


        //IEnumerator ReplyLaserPointerPosition()
        //{
        //    yield return new WaitForSeconds(0.8f);
        //    steamVRLaserPointer.transform.parent = hand.transform;
        //    steamVRLaserPointer.gameObject.SetActive(false);
        //}


        public void LeftTriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Left Controller Trigger is Released");
        }


        public void LeftTriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Left Controller Trigger is Press");
        }


        public void RightTriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Right Controller Trigger is Released");
        }


        public void RightTriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Right Controller Trigger is Press");
        }


        public void LeftTouchPadDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
         // Debug.Log("Left Controller TouchPad is Press");
        }

        public void LeftTouchPadUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Left Controller TouchPad is Released");
        }


        public void RightTouchPadDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            //Debug.Log("Right Controller TouchPad is Press");
        }

        public void RightTouchPadUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            //Debug.Log("Right Controller TouchPad is Released");
        }


        public void LeftGrabUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Left Controller Grab is Released");
        }


        public void LeftGrabDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Left Controller Grab is Press");
        }


        public void RightGrabUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Right Controller Grab is Released");
        }


        public void RightGrabDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Debug.Log("Right Controller Grab is Press");
        }
    }
}