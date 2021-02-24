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
        public bool isEntry;

        private void Start()
        {
            isEntry = false;
        }

        /// <summary>
        /// 掛在SnapTakeDropZone腳本的DropDown上
        /// 物件掉落在特定位置後，會回到原始位置
        /// </summary>
        public void ReturnToOriginPosition()
        {
            if (isEntry)
            {
                transform.GetComponent<Rigidbody>().isKinematic = true;

                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = Vector3.zero;
                transform.localScale = Vector3.one;

                isEntry = false;
            }
        }
    }
}