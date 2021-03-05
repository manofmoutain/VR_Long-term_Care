using System.Collections;
using Manager;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Global.Pateint
{
    public class Patient : MonoBehaviour
    {
        [Header("物件")] [Tooltip("VR攝影機")]public GameObject player;
        [SerializeField] private Vector3 playerPosition;
        [SerializeField] GameObject patient;
        [SerializeField] private Vector3 patientPosition;
        public Transform GetPatientTransform => patient.transform;

        /// <summary>
        /// 是否以觸發語音詢問情況
        /// </summary>
        [Header("狀態")] public bool isSpeechRecognize;

        /// <summary>
        /// 施測者與病患之間的向量值
        /// </summary>
        [Header("位置資訊")] public Vector3 playerVectorToPatient;

        /// <summary>
        /// 施測者與病患之間的距離
        /// </summary>
        public float distaceToPlayer;

        /// <summary>
        /// 施測者是否在病患後面
        /// </summary>
        public bool isPlayerBehindPatient;

        /// <summary>
        /// 施測者是否在病患左側
        /// </summary>
        public bool isPlayerLeftToPatient;

        /// <summary>
        /// 病患(子物件)在世界座標中的實際位置資訊
        /// </summary>
        public Vector3 patientDirection;

        private void Start()
        {
            //因為player基本上都世界中心，人物移動其實是攝影機在動，因此以攝影機作為player
            if (player == null)
            {
                player = FindObjectOfType<Camera>().gameObject;
            }

            if (patient == null)
            {
                patient = transform.GetChild(0).gameObject;
            }

            // isPlayerBehindPatient = false;
        }

        private void Update()
        {
            //持續偵測語音
            if (SpeechManager.Instance.GetRespondStatus == "MicroReceive")
            {
                if (!SpeechManager.Instance.GetWaitingForRecognize)
                {
                    for (int i = 0; i < SpeechManager.Instance.GetSituation.Count; i++)
                    {
                        SpeechManager.Instance.FindKeyWords(i);
                    }
                }
            }

            patientDirection = GetPatientDirection();
            isPlayerBehindPatient = patientDirection.z < 0;
            isPlayerLeftToPatient = patientDirection.x > 0;
        }

        private Vector3 GetPatientDirection()
        {
            playerPosition = player.transform.position;
            patientPosition = patient.transform.position;

            if (patient.transform.parent!=null)
            {
                playerVectorToPatient = player.transform.position - patient.transform.parent.position;
                distaceToPlayer = Vector3.Distance(player.transform.position, patient.transform.parent.position);
            }
            else
            {
                playerVectorToPatient = player.transform.position - patient.transform.position;
                distaceToPlayer = Vector3.Distance(player.transform.position, patient.transform.position);
            }


            //得到位於病患的左右方向
            // patientDirection = playerVectorToPatient / distaceToPlayer;

            //獲得病患與施測者之間的相對位置
            patientDirection = patient.transform.InverseTransformDirection(player.transform.position);

            return patientDirection;
        }

        public void StartSpeechRecognize()
        {
            SpeechManager.Instance.StartRecognizeSpeech();
            // StartCoroutine(Co_StopCo());
            // StartCoroutine(Co_SetDone(speechIndex, topicIndex));
        }

        IEnumerator Co_SetDone(int speechIndex, int topicIndex)
        {
            yield return new WaitUntil(() => SpeechManager.Instance.GetCorrectKeyWords(speechIndex));
            if (SpeechManager.Instance.GetCorrectKeyWords(speechIndex))
            {
                ScoreManager.Instance.DecreaseOperateSteps(topicIndex);
                ScoreManager.Instance.SetDone(topicIndex);
            }
        }

        IEnumerator Co_StopCo()
        {
            yield return new WaitForSeconds(5f);
            for (int i = 0; i < SpeechManager.Instance.GetSituation.Count; i++)
            {
                if (!SpeechManager.Instance.GetSituation[i].correctKeyWords)
                {
                    StopAllCoroutines();
                }
            }
        }
    }
}