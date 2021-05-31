using System;
using UnityEngine;

namespace InteractableObject
{
    /// <summary>
    /// 可互動性物件的重制位置
    /// 這個腳本會和SnapPutZone配合在一起
    /// </summary>
    public class TakeEvent_ToResetPosition : MonoBehaviour
    {
        /// <summary>
        /// 是否進入重置區域
        /// </summary>
        [Tooltip("是否進入重置區域")]public bool isEntry;

        [Header("物件原本的位置資訊")]
        [SerializeField] private Transform originTransform;
        [SerializeField] private Vector3 originPosition;
        [SerializeField] private Vector3 originRotation;
        [SerializeField] private Vector3 originScale;

        private void Start()
        {
            if (originTransform==null)
            {
                originTransform = transform.parent.transform;
            }
            isEntry = false;
            originTransform = transform.parent;
            originPosition = transform.localPosition;
            originRotation = transform.localEulerAngles;
            originScale = transform.localScale;
        }

        private void Update()
        {
            ReturnToOriginPosition();
        }

        /// <summary>
        /// 掛在SnapTakeDropZone腳本的DropDown上
        /// 物件掉落在特定位置後，會回到原始位置
        /// </summary>
        public void ReturnToOriginPosition()
        {
            if (isEntry)
            {
                print($"{gameObject.name}回歸原位");
                transform.SetParent(originTransform);
                transform.GetComponent<Rigidbody>().isKinematic = true;

                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
                transform.localScale = originScale;

                isEntry = false;
            }
        }

        public void SetIsEntry()
        {
            isEntry = true;
        }
    }
}