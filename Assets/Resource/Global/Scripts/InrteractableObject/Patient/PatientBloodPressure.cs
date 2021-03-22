using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Global.Pateint
{
    public class PatientBloodPressure : MonoBehaviour
    {
        [SerializeField] private int[] pressures;
        [SerializeField] private int bloodPressure;

        public int BloodPressure => bloodPressure;

        void Start()
        {
            int randomPressure = Random.Range(0, pressures.Length);
            bloodPressure = pressures[randomPressure];
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

