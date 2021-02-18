using GlobalSystem;
using Manager;
using Resource.Global.Scripts.Patient;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_ANDROID && !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼
using Valve.VR.InteractionSystem;

#endif


namespace Heimlich_maneuver.Patient
{
    public class HamlickPatient : MonoBehaviour
    {
        [Header("外部物件")] [SerializeField] private GameObject player;

        [SerializeField] [Tooltip("按壓時要觸碰的最終點-線性運動的終點")]
        private GameObject triggerCount;

        [Header("生成物件")] [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject bean;

        /// <summary>
        /// 是否處於施作的狀態
        /// </summary>
        [Header("狀態")] public bool isPushed;

        /// <summary>
        /// 是否以觸發語音詢問情況
        /// </summary>
        [SerializeField] private bool isSpeechRecognize;

        /// <summary>
        /// 是否處於哽塞狀態
        /// </summary>
        [SerializeField] private bool isChoking;

        /// <summary>
        /// 施作次數
        /// </summary>
        [SerializeField] private int pushCount;

        [Header("互動物件")] [SerializeField] [Tooltip("原始位置")]
        private Transform originTransform;

        [SerializeField] [Tooltip("替換位置")] private Transform sitTransform;
        [SerializeField] [Tooltip("提示UI")] private GameObject interactHint;
        [SerializeField] [Tooltip("要按壓的位置")] private GameObject interactPoint;
        [SerializeField] private GameObject patient;


        private void Start()
        {
            player = FindObjectOfType<Player>().gameObject;
            isPushed = false;
            isChoking = true;
            if (patient == null)
            {
                patient = transform.GetChild(0).gameObject;
            }
        }

        private void Update()
        {
            interactHint.SetActive(patient.transform.parent == sitTransform );
            interactPoint.SetActive(patient.transform.parent == sitTransform);




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

        public void Spit()
        {
            if (pushCount > 5 && isChoking)
            {
                int random = Random.Range(0, 10);
                if (random >= 7)
                {
                    GameObject go = Instantiate(bean, spawnPoint.position, Quaternion.identity, transform);
                    go.AddComponent<Rigidbody>();
                    go.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 3);
                    Destroy(go, 3f);
                    isChoking = false;
                }
            }
        }

        public void Push()
        {
            // pushCount = triggerCount.GetComponent<OnTriggerEnterCount>().Count;
            pushCount++;
            Spit();
        }

        public void SetPushed(bool pushed)
        {
            isPushed = pushed;
        }
    }
}