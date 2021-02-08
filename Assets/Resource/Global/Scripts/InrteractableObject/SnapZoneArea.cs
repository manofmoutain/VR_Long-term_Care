using UnityEngine;

namespace InrteractableObject
{
    /// <summary>
    /// 要黏貼的區域
    /// </summary>
    public class SnapZoneArea : MonoBehaviour
    {

        [SerializeField] private GameObject fadedObject;
        /// <summary>
        /// 要黏貼物件的外框線預置體
        /// </summary>
        [Tooltip("要黏貼物件的外框線預置體")]public GameObject fadedPrefab; // used to preview insubstantial inputObject

        /// <summary>
        /// 被偵測的區域碰撞體
        /// </summary>
        public SphereCollider sphereCollider;

        /// <summary>
        /// 要黏貼的物件(必須要有snapTakeDropZone腳本)
        /// </summary>
        public SnapTakeDropZone snapTakeDropZone;


        void Start()
        {
            if (sphereCollider==null)
            {
                sphereCollider = GetComponent<SphereCollider>();
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            //要黏貼的物件被抓取後，黏貼的物件tag會改為SnapObject
            if (other.GetComponent<SnapTakeDropZone>())
            {
                //顯示外框線
                fadedObject = Instantiate(fadedPrefab, transform.position, Quaternion.identity) as GameObject;
                fadedObject.transform.parent = transform;
                fadedObject.transform.localPosition = new Vector3();
                fadedObject.transform.localScale = new Vector3(1,1,1);
                fadedObject.transform.localRotation = Quaternion.identity;


                gameObject.tag = "AreaZone";
                snapTakeDropZone.snapFixed.isLocated = true;
                //Debug.Log("Snap Object Correct！");
            }

        }

        private void OnTriggerExit(Collider other)
        {
            Destroy(fadedObject);

            //if (snapTakeDropZone.snapFixed.isThrowed)
            //{
            //    snapTakeDropZone.snapFixed.isLocated = false;
            //    snapTakeDropZone.snapFixed.isThrowed = false;
            //}

            if (snapTakeDropZone.snapFixed.isLocated && !snapTakeDropZone.snapFixed.isFixed)
            {
                snapTakeDropZone.snapFixed.isLocated = false;
            }
        }

    }
}