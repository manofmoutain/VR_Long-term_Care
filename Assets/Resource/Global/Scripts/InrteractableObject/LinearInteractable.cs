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
        [SerializeField] private LinearDrive linearDrive;


        private void Start()
        {
            interactPoint.GetComponent<MeshRenderer>().enabled = false;
            if (linearDrive==null)
            {
                linearDrive.GetComponent<LinearDrive>();
            }
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
                interactPoint.transform.position=linearDrive.startPosition.position;
            }

        #endregion


    }
}