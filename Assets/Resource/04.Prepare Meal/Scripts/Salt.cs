using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrepareMeal
{
    public class Salt : MonoBehaviour
    {
        [SerializeField] private Transform spanPosition;
        [SerializeField] private GameObject salt;
        [SerializeField] private float timer;

        // Update is called once per frame
        void Update()
        {
            if (transform.eulerAngles.z >= 120 || transform.eulerAngles.z <= 180)
            {
                timer += Time.deltaTime;
                if (timer > 1.0f)
                {
                    timer = 0;
                    GameObject go = Instantiate(salt,spanPosition);
                }
            }
        }
    }
}