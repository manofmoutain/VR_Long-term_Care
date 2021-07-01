using System;
using System.Collections;
using System.Collections.Generic;
using Global.Pateint;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace PrepareMeal
{
    public class PatientEat : MonoBehaviour
    {
        [SerializeField] private float timer;
        [SerializeField] private bool isEating;
        [SerializeField] private float eatingTime;

        [Range(0,1)][SerializeField] private float hintFillValue;
        [SerializeField] private Image eatingHint;


        private void Update()
        {
            // eatingHint.fillAmount = hintFillValue;
            if (isEating)
            {
                timer += Time.deltaTime;
                hintFillValue = Mathf.Lerp(0,1 , timer*0.5f);
                eatingHint.fillAmount = hintFillValue;
                if (timer>=eatingTime)
                {
                    isEating = false;
                }
            }
            else
            {
                timer = 0;
                eatingHint.fillAmount = 0;
            }
        }

        public void Eating(int index)
        {
            if (isEating && timer<=eatingTime)
            {
                //嗆到
                GetComponent<Patient>().PlaySFX(0);
                ScoreManager.Instance.IncreaseOperateSteps(index);
            }
            isEating = !isEating;


        }
    }
}

