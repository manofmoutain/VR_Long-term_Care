using UnityEngine;

namespace InrteractableObject
{
    /// <summary>
    /// 要黏貼的區域
    /// </summary>
    public class TakeEvent_SnapArea : MonoBehaviour
    {
        /// <summary>
        /// 是否已被黏著
        /// </summary>
        public bool isSnapIn;

        [SerializeField] private GameObject fadedObject;

        /// <summary>
        /// 要黏貼物件的外框線預置體
        /// </summary>
        [Tooltip("要黏貼物件的外框線預置體")] public GameObject fadedPrefab; // used to preview insubstantial inputObject

        /// <summary>
        /// 被偵測的區域碰撞體
        /// </summary>
        public SphereCollider sphereCollider;

        /// <summary>
        /// 要黏貼的物件(必須要有snapTakeDropZone腳本)
        /// </summary>
        public TakeEvent_SnapPutZone takeEventSnapPutZone;


        void Start()
        {
            if (sphereCollider == null)
            {
                sphereCollider = GetComponent<SphereCollider>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
// Oculus Quest代碼
#else
            // SteamVR代碼
//要黏著的物件進入黏著區時，且黏著區尚未啟動已黏著
            if (other.GetComponent<TakeEvent_SnapPutZone>() && !isSnapIn)
            {
                print($"{other.gameObject.name}進入放置區");
                //產生外框線
                fadedObject = Instantiate(fadedPrefab, transform.position, Quaternion.identity);
                fadedObject.transform.SetParent(transform);
                fadedObject.transform.localPosition = Vector3.zero;
                fadedObject.transform.localScale = Vector3.one;
                fadedObject.transform.localRotation = Quaternion.identity;

                //要黏著的物體已黏著與此區域
                isSnapIn = true;
                takeEventSnapPutZone.snapFixed.isLocated = true;
                //Debug.Log("Snap Object Correct！");
            }
#endif
        }

        private void OnTriggerExit(Collider other)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
// Oculus Quest代碼
#else
            // SteamVR代碼
            if (other.GetComponent<TakeEvent_SnapPutZone>() && isSnapIn)
            {
                Destroy(fadedObject);
                isSnapIn = false;

                //if (snapTakeDropZone.snapFixed.isThrowed)
                //{
                //    snapTakeDropZone.snapFixed.isLocated = false;
                //    snapTakeDropZone.snapFixed.isThrowed = false;
                //}

                if (takeEventSnapPutZone.snapFixed.isLocated && !takeEventSnapPutZone.snapFixed.isFixed)
                {
                    takeEventSnapPutZone.snapFixed.isLocated = false;
                }
            }
#endif
        }
    }
}