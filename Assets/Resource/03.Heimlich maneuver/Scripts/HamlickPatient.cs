using InrteractableObject;
using UnityEngine;
#if UNITY_ANDROID  &&  !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼
using Valve.VR.InteractionSystem;
#endif


namespace Heimlich_maneuver.Patient
{
    public class HamlickPatient : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        // [SerializeField] private LinearInteractable interactPoint;
        public bool isPushed;

        [SerializeField] private Transform originTransform;
        [SerializeField] private Transform sitTransform;

        [SerializeField] private GameObject interactHint;
        [SerializeField] private LinearInteractable interactPoint;

        [SerializeField] private GameObject patient;

        private void Start()
        {
            player = FindObjectOfType<Player>().gameObject;
            isPushed = false;
        }

        private void Update()
        {
            interactHint.SetActive(patient.transform.parent == sitTransform);
            interactPoint.gameObject.SetActive(patient.transform.parent == sitTransform);


#if UNITY_ANDROID  &&  !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼
            if (patient.transform.parent == sitTransform)
            {
                patient.GetComponent<SnapTakeDropZone>().enabled = false;
            }
            else
            {
                patient.GetComponent<SnapTakeDropZone>().enabled = true;
            }
#endif


        }


        public void SetPushed(bool pushed)
        {
            isPushed = pushed;
        }
    }
}