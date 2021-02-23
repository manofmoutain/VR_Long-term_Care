using GlobalSystem;
using Manager;
using Resource.Global.Scripts.Patient;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_ANDROID && !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼
using InrteractableObject;
using Valve.VR.InteractionSystem;

#endif


namespace Heimlich_maneuver.Patient
{
    public class HamlickPatient : MonoBehaviour
    {
        [Header("外部物件")] [SerializeField] private GameObject player;
        [SerializeField] private GameObject patient;
        [SerializeField] private Vector3 playerVectorToPatient;
        [SerializeField] private float distaceToPlayer;
        [SerializeField] private Vector3 patientDirection;
        public bool isPlayerBehindPatient;


        /// <summary>
        /// 是否處於施作的狀態
        /// </summary>
        [Header("狀態")]
        [SerializeField] bool isPushed;

        [SerializeField] private bool canHug;

        /// <summary>
        /// 是否以觸發語音詢問情況
        /// </summary>
        [SerializeField] private bool isSpeechRecognize;



        private void Start()
        {
            if (patient==null)
            {
                patient = transform.GetChild(0).gameObject;
            }
            player = FindObjectOfType<Player>().gameObject;
            isPushed = false;
            canHug = false;
            isPlayerBehindPatient = false;
        }

        private void Update()
        {
            patientDirection = GetPatientDirection();
            //player位於病患背後
            canHug = patientDirection.z > 0;
            isPlayerBehindPatient = canHug;

#if UNITY_ANDROID && !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼
            // if (patient.transform.parent == sitTransform)
            // {
            //     patient.GetComponent<SnapTakeDropZone>().enabled = false;
            // }
            // else
            // {
            //     patient.GetComponent<SnapTakeDropZone>().enabled = true;
            // }


#endif
        }

        public Vector3 GetPatientDirection()
        {
            playerVectorToPatient = player.transform.position - patient.transform.parent.position;
            distaceToPlayer = Vector3.Distance(player.transform.position, patient.transform.parent.position);
            //得到位於病患的左右方向
            Vector3 patientDirection = playerVectorToPatient / distaceToPlayer;
            return patientDirection;
        }

        public void HugPatientFromBack()
        {
            if (canHug)
            {
                //項目二：立於後側將案主移到無手扶手椅上
                ScoreManager.Instance.DecreaseOperateSteps(1);
                ScoreManager.Instance.SetDone(1);
                print($"{player.name}在{gameObject.name}後面");
            }
        }

        public void SetPushed(bool pushed)
        {
            isPushed = pushed;
        }

        public void Fallen()
        {

            //項目十：案主掉落
            ScoreManager.Instance.IncreaseOperateSteps(10);
            ScoreManager.Instance.SetDone(10);
        }

        public void WashHandBeforeOperate()
        {
            if (patient.transform.parent==transform)
            {
                //項目十二：於事前進行洗手
                ScoreManager.Instance.IncreaseOperateSteps(11);
                ScoreManager.Instance.SetDone(11);
            }
        }

    }
}