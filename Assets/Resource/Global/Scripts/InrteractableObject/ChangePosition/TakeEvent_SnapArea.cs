using System;
using AutoHandInteract;
using UnityEngine;

namespace InteractableObject
{
    /// <summary>
    /// 要黏貼的區域
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TakeEvent_SnapArea : MonoBehaviour
    {
        /// <summary>
        /// 是否已被黏著
        /// </summary>
        public bool isSnapIn;

        public GameObject fadedObject;

        /// <summary>
        /// 要黏貼物件的外框線預置體
        /// </summary>
        [Tooltip("要黏貼物件的外框線預置體")] [SerializeField]
        GameObject fadedPrefab; // used to preview insubstantial inputObject

        /// <summary>
        /// 被偵測的區域碰撞體
        /// </summary>
        // public Collider sphereCollider;

        /// <summary>
        /// 要黏貼的物件(必須要有snapTakeDropZone腳本)
        /// </summary>
        [SerializeField] TakeEvent_HandGrab takeEventHandGrab;

        [SerializeField] private AutoHand_HandGrab autoHandGrab;


        void Start()
        {
            // if (sphereCollider == null)
            // {
            //     sphereCollider = GetComponent<Collider>();
            // // }
            //
            // sphereCollider.isTrigger = true;
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            //要黏著的物件進入黏著區時，且黏著區尚未啟動已黏著
            if (other.name==takeEventHandGrab.name && takeEventHandGrab!=null)
            {
                // print($"{other.name}");
                if (!isSnapIn)
                {
                    // print($"{other.gameObject.name}進入{transform.parent.name}");
                    //產生外框線
                    fadedObject = Instantiate(fadedPrefab, transform.position, Quaternion.identity, transform);
                    // fadedObject.transform.SetParent(transform);
                    // fadedObject.transform.localPosition = Vector3.zero;
                    // fadedObject.transform.localScale = Vector3.one;
                    fadedObject.transform.localRotation = Quaternion.identity;

                    //要黏著的物體已黏著與此區域
                    isSnapIn = true;


                    if (takeEventHandGrab != null)
                    {
                        takeEventHandGrab.snapFixed.isLocated = true;
                    }

                    if (autoHandGrab != null)
                    {
                        // print($"{autoHandGrab.name}.snapFixed.isLocated : {autoHandGrab.snapFixed.isLocated}");
                        autoHandGrab.snapFixed.isLocated = true;
                    }
                    //Debug.Log("Snap Object Correct！");
                }
            }
        }


        private void OnTriggerStay(Collider other)
        {
            if (other.name==takeEventHandGrab.name && takeEventHandGrab!=null)
            {
                if (isSnapIn)
                {
                    // print($"{other.gameObject.name}停留{transform.parent.name}");

                    //要黏著的物體已黏著與此區域

                    if (takeEventHandGrab != null)
                    {
                        takeEventHandGrab.snapFixed.isLocated = true;
                    }

                    if (autoHandGrab != null)
                    {
                        // print($"{autoHandGrab.name}.snapFixed.isLocated : {autoHandGrab.snapFixed.isLocated}");
                        autoHandGrab.snapFixed.isLocated = true;
                    }

                    //Debug.Log("Snap Object Correct！");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // SteamVR代碼
            if (other.name==takeEventHandGrab.name && takeEventHandGrab!=null)
            {
                if (isSnapIn)
                {
                    if (fadedObject != null)
                    {
                        Destroy(fadedObject);
                    }

                    isSnapIn = false;

                    if (takeEventHandGrab != null)
                    {
                        if (takeEventHandGrab.snapFixed.isLocated &&
                            !takeEventHandGrab.snapFixed.isFixed)
                        {
                            takeEventHandGrab.snapFixed.isLocated = false;
                        }
                    }

                    if (autoHandGrab != null)
                    {
                        if (autoHandGrab.snapFixed.isLocated && !autoHandGrab.snapFixed.isFixed)
                        {
                            autoHandGrab.snapFixed.isLocated = false;
                        }
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            //要黏著的物件進入黏著區時，且黏著區尚未啟動已黏著
            if (other.gameObject.name==takeEventHandGrab.name && takeEventHandGrab!=null)
            {
                if (!isSnapIn)
                {
                    // print($"{other.gameObject.name}進入{transform.parent.name}");
                    //產生外框線
                    fadedObject = Instantiate(fadedPrefab, transform.position, Quaternion.identity, transform);
                    // fadedObject.transform.SetParent(transform);
                    // fadedObject.transform.localPosition = Vector3.zero;
                    // fadedObject.transform.localScale = Vector3.one;
                    fadedObject.transform.localRotation = Quaternion.identity;

                    //要黏著的物體已黏著與此區域
                    isSnapIn = true;

                    if (takeEventHandGrab != null)
                    {
                        takeEventHandGrab.snapFixed.isLocated = true;
                    }

                    if (autoHandGrab != null)
                    {
                        // print($"{autoHandGrab.name}.snapFixed.isLocated : {autoHandGrab.snapFixed.isLocated}");
                        autoHandGrab.snapFixed.isLocated = true;
                    }
                    //Debug.Log("Snap Object Correct！");
                }
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.name==takeEventHandGrab.name && takeEventHandGrab!=null)
            {
                if (isSnapIn)
                {
                    // print($"{other.gameObject.name}停留{transform.parent.name}");

                    //要黏著的物體已黏著與此區域

                    if (takeEventHandGrab != null)
                    {
                        takeEventHandGrab.snapFixed.isLocated = true;
                    }

                    if (autoHandGrab != null)
                    {
                        // print($"{autoHandGrab.name}.snapFixed.isLocated : {autoHandGrab.snapFixed.isLocated}");
                        autoHandGrab.snapFixed.isLocated = true;
                    }

                    //Debug.Log("Snap Object Correct！");
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.name==takeEventHandGrab.name && takeEventHandGrab!=null)
            {
                if (isSnapIn)
                {
                    if (fadedObject != null)
                    {
                        Destroy(fadedObject);
                    }

                    isSnapIn = false;


                    if (takeEventHandGrab != null)
                    {
                        if (takeEventHandGrab.snapFixed.isLocated &&
                            !takeEventHandGrab.snapFixed.isFixed)
                        {
                            takeEventHandGrab.snapFixed.isLocated = false;
                        }
                    }

                    if (autoHandGrab != null)
                    {
                        if (autoHandGrab.snapFixed.isLocated && !autoHandGrab.snapFixed.isFixed)
                        {
                            autoHandGrab.snapFixed.isLocated = false;
                        }
                    }
                }
            }
        }
    }
}