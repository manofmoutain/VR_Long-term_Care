using Manager;
using UnityEngine;
using Random = UnityEngine.Random;
using Global.Pateint;

namespace Heimlich_maneuver
{
    public class HamlickPatientSpit : MonoBehaviour
    {
        [Header("生成物件")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject bean;
        /// <summary>
        /// 給予豆子噴出的力道
        /// </summary>
        [SerializeField] private float beanSpeed;
        /// <summary>
        /// 豆子噴出的方向
        /// </summary>
        [SerializeField] private Vector3 velocityDirection;

        /// <summary>
        /// 是否處於哽塞狀態
        /// </summary>
        public bool isChoking;

        /// <summary>
        /// 最小按壓次數
        /// </summary>
        [SerializeField] private int minPushCount;

        /// <summary>
        /// 施作次數
        /// </summary>
        [SerializeField] private int pushCount;


        private void Start()
        {
            isChoking = true;

            print(ScoreManager.Instance.GetLesson());
            minPushCount = ScoreManager.Instance.GetOperateSteps(3);
        }


        /// <summary>
        /// 掛在線性運動抓取點的InteractableHoverEvents的OnAttachedToHand上
        /// </summary>
        /// <param name="index">考題編號</param>
        public void StartPush(int index)
        {
            if (GetComponent<Patient>().isPlayerBehindPatient)
            {
                //項目三：找出正確按壓位置
                ScoreManager.Instance.DecreaseOperateSteps(index);
                ScoreManager.Instance.SetDone(index);
            }
        }

        /// <summary>
        /// 掛在線性運動中的移動終點的onLinearInteractTriggerEvent上
        /// </summary>
        /// <param name="index">考題編號</param>
        public void Push(int index)
        {
            // pushCount = triggerCount.GetComponent<OnTriggerEnterCount>().Count;
            pushCount++;
            //項目四：握拳向內壓數次
            ScoreManager.Instance.DecreaseOperateSteps(index);
            ScoreManager.Instance.SetDone(index);
            Spit();
        }

        void Spit()
        {
            if (pushCount > minPushCount && isChoking)
            {
                int random = Random.Range(0, 10);
                if (random >= 7)
                {
                    //項目五：豆子擠出
                    ScoreManager.Instance.DecreaseOperateSteps(4);
                    ScoreManager.Instance.SetDone(4);
                    GetComponent<PatientSFX>().PlaySFX(0);
                    GameObject go = Instantiate(bean, spawnPoint.position, Quaternion.identity,
                        GetComponent<Patient>().GetPatientTransform);
                    go.GetComponent<Rigidbody>().velocity = velocityDirection * beanSpeed;
                    Destroy(go, 3f);
                    isChoking = false;
                }
            }
        }
    }
}