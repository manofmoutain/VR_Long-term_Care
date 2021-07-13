using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PrepareMeal
{
    public class Syringe : MonoBehaviour
    {
        [SerializeField] private GameObject linearSyringe;
        [SerializeField] private bool isHide;

        void Update()
        {
            if (isHide)
            {
                transform.position = linearSyringe.transform.position;
            }
        }

        public void Hide(bool hide)
        {
            GetComponent<Collider>().enabled = !hide;
            GetComponent<MeshRenderer>().enabled = !hide;
            isHide = hide;
        }
    }
}