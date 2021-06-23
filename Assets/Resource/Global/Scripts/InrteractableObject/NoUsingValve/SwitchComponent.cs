using System;
using Manager;
using TreeEditor;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObject
{
    public class SwitchComponent : MonoBehaviour
    {
        private Animator animator;

        /// <summary>
        /// 是否要啟動對某個項目的判定，以打開achieveCheckpointEvent事件
        /// </summary>
        public bool isUsingCheckPoint;
        /// <summary>
        /// 達成某個項目之後要觸發的事件
        /// </summary>
        [SerializeField] private UnityEvent achieveCheckPointEvent;

        /// <summary>
        /// 為達成目標時的事件
        /// </summary>
        [SerializeField] private UnityEvent notAchieveCheckPointEvent;

        private Vector3 originPosition;
        private Vector3 originRotation;
        private MySkeletonPoser skeletonPoser;

        private void Start()
        {
            originPosition = transform.localPosition;
            originRotation = transform.localEulerAngles;
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
                achieveCheckPointEvent.Invoke();
            }
            else
            {
                notAchieveCheckPointEvent.Invoke();
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

            skeletonPoser.SetBlendingBehaviourValue(skeletonPoser.blendingBehaviours[index - 1].name,
                blendPoseValue + gestureConst);
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

            skeletonPoser.SetBlendingBehaviourValue(skeletonPoser.blendingBehaviours[index + 1].name,
                gestureConst - blendPoseValue);
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

        /// <summary>
        /// 刪除自身物件
        /// </summary>
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

        /// <summary>
        /// 開關Collider組件
        /// </summary>
        /// <param name="switcher"></param>
        public void SwitchCollider(bool switcher)
        {
            GetComponent<Collider>().enabled = switcher;
        }

        /// <summary>
        /// 開關Trigger
        /// </summary>
        /// <param name="switcher"></param>
        public void SwitchTrigger(bool switcher)
        {
            GetComponent<Collider>().isTrigger = switcher;
        }

        /// <summary>
        /// 自身物件的開關
        /// </summary>
        /// <param name="switcher"></param>
        public void SwtichGameObject(bool switcher)
        {
            gameObject.SetActive(switcher);
        }

        /// <summary>
        /// 開關RigidBody的靜態屬性
        /// </summary>
        /// <param name="switcher"></param>
        public void SwitchRigidbodyKinematic(bool switcher)
        {
            GetComponent<Rigidbody>().isKinematic = switcher;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles=Vector3.zero;
        }

        /// <summary>
        /// 啟動Animator的某個Trigger
        /// </summary>
        /// <param name="parameter"></param>
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
            transform.localEulerAngles=Vector3.zero;
            // transform.localScale = Vector3.one;
        }
    }
}