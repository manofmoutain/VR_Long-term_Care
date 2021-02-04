using System;
using Manager;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace InrteractableObject
{
    public class LinearInteractable : MonoBehaviour
    {
        public float linearValue;
        [SerializeField] private GameObject[] allInteractPoints;
        [SerializeField] private GameObject interactPoint;


        private void Start()
        {
            for (int i = 0; i < allInteractPoints.Length; i++)
            {
                allInteractPoints[i].SetActive(false);
            }

            switch (SceneLoader.Instance.GetCurrentSceneName)
            {
                case "02.Detect Life" :

                    break;
                case "03.Heimlich maneuver":
                    break;
                case "04.CPR":
                    break;
                case "05.Prepare Meal":
                    break;
                case "06.Wash Head":
                    break;
                case "07.Wash lower body (female)":
                    break;
                case "08.Wash lower body (male)":
                    break;
                case "09.Help to get on wheelchair":
                    break;
            }

            interactPoint.GetComponent<MeshRenderer>().enabled = false;
        }


        private void Update()
        {
            linearValue = GetComponentInChildren<LinearMapping>().value;
        }

        #region InteractMethod

        public void OnHandHoverBegin(GameObject go)
        {
            //hint
            interactPoint.GetComponent<MeshRenderer>().enabled = true;
            //what to do
            go.SetActive(false);
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
            }

        #endregion


    }
}