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




        /// <summary>
        /// 是否處於施作的狀態
        /// </summary>
        [Header("狀態")]
        [SerializeField] bool isPushed;

        /// <summary>
        /// 是否以觸發語音詢問情況
        /// </summary>
        [SerializeField] private bool isSpeechRecognize;


        private void Start()
        {
            player = FindObjectOfType<Player>().gameObject;
            isPushed = false;
        }

        private void Update()
        {
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


        public void SetPushed(bool pushed)
        {
            isPushed = pushed;
        }
    }
}