using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼
using Valve.VR.InteractionSystem;
#endif

namespace InteractableObject
{
    public class Interact_LinearInteractable : MonoBehaviour
    {
        [SerializeField] private Hand otherHand;
        [SerializeField] private GameObject[] allInteractPoints;
        [SerializeField] private GameObject interactPoint;
        [SerializeField] private GameObject linearMappingGameObject;
        [SerializeField] private GameObject linearDrive;
        [SerializeField] private Animator patientAnimator;


        private void Start()
        {
            interactPoint.GetComponent<MeshRenderer>().enabled = false;

        }



#if UNITY_ANDROID && !UNITY_EDITOR
// Oculus Quest代碼
#else
// SteamVR代碼

        #region InteractMethod

        public void OnHandHoverBegin(GameObject go)
        {
            //hint
            interactPoint.GetComponent<MeshRenderer>().enabled = true;
            //what to do
            go.SetActive(true);
        }

        public void OnHandHoverEnd(GameObject go)
        {
            //hint
            interactPoint.GetComponent<MeshRenderer>().enabled = false;
            //what to do
            go.SetActive(false);
        }

        public void OnAttachToHand(GameObject go)
        {
            //hint
            interactPoint.GetComponent<MeshRenderer>().enabled = false;
            //what to do
            go.SetActive(true);
            go.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        public void OnDetachedFromHand(GameObject go)
        {
            //hint
            interactPoint.GetComponent<MeshRenderer>().enabled = false;
            //what to do
            go.SetActive(true);
            go.GetComponent<MeshRenderer>().material.color = Color.green;
            interactPoint.transform.position = linearDrive.GetComponent<LinearDrive>().startPosition.position;
            

        }

        #endregion
#endif
    }
}