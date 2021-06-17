using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Global.Pateint
{
    public partial class Patient
    {
        public bool isUsingBloodPressure;
        [SerializeField] private float[] pressures;
        [SerializeField] private float bloodPressure;

        public float BloodPressure => bloodPressure;


        public void ShowBloodPressure(TextMeshProUGUI textMesh)
        {
            textMesh.text = BloodPressure.ToString();
        }
    }
}

