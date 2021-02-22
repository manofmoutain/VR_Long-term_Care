using Manager;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatientSpit : MonoBehaviour
    {
        [Header("音效")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip spitSFX;


        [Header("生成物件")] [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject bean;
        [SerializeField] private float beanSpeed;
        [SerializeField] private Vector3 velocityDirection;

        /// <summary>
        /// 是否處於哽塞狀態
        /// </summary>
        [SerializeField] private bool isChoking;

        /// <summary>
        /// 最小按壓次數
        /// </summary>
        [SerializeField]private int minPushCount;

        /// <summary>
        /// 施作次數
        /// </summary>
        [SerializeField] private int pushCount;

        [SerializeField] private Transform patient;


        private void Start()
        {
            if (patient==null)
            {
                patient = transform.GetChild(0).transform;
            }
            isChoking = true;
            minPushCount = ScoreManager.Instance.GetOperateSteps(3);
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
                    audioSource.PlayOneShot(spitSFX);
                    GameObject go = Instantiate(bean, spawnPoint.position, Quaternion.identity, patient);
                    Vector3 patientDirect = GetComponent<HamlickPatient>().GetPatientDirection();
                    go.GetComponent<Rigidbody>().velocity = velocityDirection* beanSpeed;
                    Destroy(go, 3f);
                    // isChoking = false;
                }
            }
        }

        public void StartPush()
        {
            if (GetComponent<HamlickPatient>().isPlayerBehindPatient)
            {
                //項目三：找出正確按壓位置
                ScoreManager.Instance.DecreaseOperateSteps(2);
                ScoreManager.Instance.SetDone(2);
            }
        }

        public void Push()
        {
            // pushCount = triggerCount.GetComponent<OnTriggerEnterCount>().Count;
            pushCount++;
            //項目四：握拳向內壓數次
            ScoreManager.Instance.DecreaseOperateSteps(3);
            ScoreManager.Instance.SetDone(3);
            Spit();
        }
    }
}