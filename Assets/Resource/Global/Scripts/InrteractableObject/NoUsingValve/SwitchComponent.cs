using System;
using Manager;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class SwitchComponent : MonoBehaviour
    {
        private Animator animator;
        private MySkeletonPoser skeletonPoser;

        public bool isUsingCheckPoint;
        /// <summary>
        /// 達成某個項目之後要觸發的事件
        /// </summary>
        [SerializeField] private UnityEvent achieveCheckpointEvent;

        private void Start()
        {
            if (GetComponent<Animator>())
            {
                animator = GetComponent<Animator>();
            }

            if (GetComponent<MySkeletonPoser>())
            {
                skeletonPoser = GetComponent<MySkeletonPoser>();
            }
        }

        /// <summary>
        /// 確認是否達成某個項目之後觸發事件(achieveCheckpointEvent內放置Patient裡的CheckPoint方法)
        /// 這個方法要掛在其他腳本上
        /// </summary>
        /// <param name="index">確認是否完成的項目編號</param>
        public void CheckPointInEvent(int index)
        {
            if (ScoreManager.Instance.GetIsDone(index))
            {
                achieveCheckpointEvent.Invoke();
            }
        }

        /// <summary>
        /// 轉開時使用的混合手勢
        /// </summary>
        /// <param name="index"></param>
        public void BlenderSwitchOnPoser(int index)
        {
            float blendPoseValue = 0;
            float gestureConst = 0.7f;
            if (GetComponent<Interact_CircularDrive>())
            {
                blendPoseValue = GetComponent<Interact_CircularDrive>().blendPoseValue;
            }

            // print(skeletonPoser.blendingBehaviours[0].name);
            skeletonPoser.SetBlendingBehaviourValue(skeletonPoser.blendingBehaviours[index].name, 0);

            skeletonPoser.SetBlendingBehaviourValue(skeletonPoser.blendingBehaviours[index - 1].name, blendPoseValue + gestureConst);
        }

        /// <summary>
        /// 關閉時使用的混合手勢
        /// </summary>
        /// <param name="index"></param>
        public void BlenderSwitchOffPoser(int index)
        {
            float blendPoseValue = 0;
            float gestureConst = 0.7f;
            if (GetComponent<Interact_CircularDrive>())
            {
                blendPoseValue = GetComponent<Interact_CircularDrive>().blendPoseValue;
            }

            skeletonPoser.SetBlendingBehaviourValue(skeletonPoser.blendingBehaviours[index].name, 0);

            skeletonPoser.SetBlendingBehaviourValue(skeletonPoser.blendingBehaviours[index + 1].name, gestureConst - blendPoseValue);
        }

        /// <summary>
        /// 使某個物件打開
        /// </summary>
        /// <param name="go"></param>
        public void ActiveGameObject(GameObject go)
        {
            go.SetActive(true);
        }

        /// <summary>
        /// 使某個物件關閉
        /// </summary>
        /// <param name="go"></param>
        public void DeactiveGameObject(GameObject go)
        {
            go.SetActive(false);
        }

        public void DestroyGmaobject(GameObject go)
        {
            Destroy(go);
        }

        public void DestroyThisGameObject()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 開關物件的MeshRemder
        /// </summary>
        /// <param name="switcher"></param>
        public void SwitchMeshRender(bool switcher)
        {
            GetComponent<MeshRenderer>().enabled = switcher;
        }

        public void SwitchCollider(bool switcher)
        {
            GetComponent<Collider>().enabled = switcher;
        }

        public void SwitchTrigger(bool switcher)
        {
            GetComponent<Collider>().isTrigger = switcher;
        }

        public void SwtichGameObject(bool switcher)
        {
            gameObject.SetActive(switcher);
        }

        public void SwitchRigidbodyKinematic(bool switcher)
        {
            GetComponent<Rigidbody>().isKinematic = switcher;
        }

        public void AnimatorSetTrigger(string parameter)
        {
            animator.SetTrigger(parameter);
        }

        public void AnimatorSetBool(string parameter)
        {

            animator.SetBool(parameter, !animator.GetBool(parameter));
        }

        public void ChangeParent(Transform parent)
        {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }
}