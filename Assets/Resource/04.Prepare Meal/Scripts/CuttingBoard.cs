using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrepareMeal
{
    public class CuttingBoard : MonoBehaviour
    {
        public bool isCleaned;



        public void MakeCLeaned(bool clean)
        {
            isCleaned = clean;
        }

        public void SetToRightPosition()
        {
            if (isCleaned)
            {

            }
        }
    }
}

