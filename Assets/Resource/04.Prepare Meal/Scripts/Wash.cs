using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrepareMeal
{
    public class Wash : MonoBehaviour
    {
        [SerializeField] private bool isCleaned;


        public void Washed(bool cleaned)
        {
            isCleaned = cleaned;
        }
    }
}

