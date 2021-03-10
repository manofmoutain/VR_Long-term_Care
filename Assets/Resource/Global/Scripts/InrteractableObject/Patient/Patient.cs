using System.Collections;
using Manager;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Global.Pateint
{
    public class Patient : MonoBehaviour
    {
        [Header("物件")] [Tooltip("VR攝影機")] public GameObject player;
        [SerializeField] private Vector3 playerPosition;
        public GameObject patient;
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
        /// 施測者與病患之間的夾角餘弦
        /// </summary>
        public float directionToPlayer;

        /// <summary>
        /// 施測者是否在病患後面
        /// </summary>
        public bool isPlayerBehindPatient;

        /// <summary>
        /// 施測者是否在病患左側
        /// </summary>
        public bool isPlayerLeftToPatient;

        /// <summary>
        /// 病患與施測者之間的相對方位
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


            isPlayerBehindPatient = GetPatientDirection().y < 0;
            isPlayerLeftToPatient = GetPatientDirection().y < 0;
        }

        private Vector3 GetPatientDirection()
        {
            playerPosition = player.transform.position.normalized;
            patientPosition = patient.transform.position.normalized;

            // if (patient.transform.parent!=null)
            // {
            //     playerVectorToPatient = player.transform.position - patient.transform.parent.position;
            //     distaceToPlayer = Vector3.Distance(player.transform.position, patient.transform.parent.position);
            // }
            // else
            // {
            //     playerVectorToPatient = player.transform.position - patient.transform.position;
            //     distaceToPlayer = Vector3.Distance(playerPosition, patientPosition);
            // }


            //得到位於病患的左右方向
            // patientDirection = playerVectorToPatient / distaceToPlayer;

            //獲得病患與施測者之間的相對位置
            // patientDirection = player.transform.InverseTransformDirection(patientPosition);

            // 計算向量V1，V2 點乘結果
            // 即獲取V1,V2夾角餘弦cos(夾角)
            directionToPlayer = Vector3.Dot(patientPosition, playerPosition);
            // 夾角方向一般取（0 - 180 度）
            // 如果取(0 - 360 度)
            // direction >= 0 則夾角在（0 - 90] 和[270 - 360] 度之間
            // direction < 0 則夾角在（90 - 270) 度之間
            // direction 無法確定具體角度

            // 反餘弦求V1，V2 夾角的弧度
            float rad = Mathf.Acos(directionToPlayer);
            // 再將弧度轉換爲角度
            float deg = rad * Mathf.Rad2Deg;
            // 得到的deg 爲V1，V2 在（0 - 180 度的夾角）還無法確定V1，V2 的相對夾角
            // deg 還是無法確定具體角度

            // 計算向量V1， V2 的叉乘結果
            // 得到垂直於V1， V2 的向量， Vector3(0, sin(V1,V2夾角), 0)
            // 即uy = sin(V1,V2夾角)
            patientDirection = Vector3.Cross(patientPosition, playerPosition);
            // uy >= 0 則夾角在( 0 - 180] 度之間
            // uy < 0 則夾角在(180 - 360) 度之間
            // uy 依然無法確定具體角度
            if (patientDirection.y >= 0) // (0 - 180]
            {
                if (directionToPlayer >= 0)
                {
                    // (0 - 90] 度
                    // print("前面");
                }
                else
                {
                    // (90 - 180] 度
                    // print("(90 - 180] 度");
                }
            }
            else // (180 - 360]
            {
                if (directionToPlayer >= 0)
                {
                    // [270 - 360]
                    // 360 + (-1)deg
                    // print("[後面]");
                }
                else
                {
                    // (180 - 270)
                    // print("(180 - 270)");
                }
            }

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