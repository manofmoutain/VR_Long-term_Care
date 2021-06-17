using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class WatchTime : MonoBehaviour
{
    [SerializeField] private TaiwanCalendar taiwanTime;
    [SerializeField] private string hour;
    [SerializeField] private string minute;
    [SerializeField] private int second;
    [SerializeField] private TextMeshProUGUI timeTMPro;

    void Start()
    {
        taiwanTime = new TaiwanCalendar();
    }

    // Update is called once per frame
    void Update()
    {
        // transform.localPosition = new Vector3(-0.55f, 0, 0.637f);
        // transform.localRotation = Quaternion.identity;


        if (taiwanTime.GetHour(DateTime.Now)<10)
        {
            hour = $"0{taiwanTime.GetHour(DateTime.Now)}";
        }
        else
        {
            hour = $"{taiwanTime.GetHour(DateTime.Now)}";
        }

        if (taiwanTime.GetMinute(DateTime.Now)<10)
        {
            minute = $"0{taiwanTime.GetMinute(DateTime.Now)}";
        }
        else
        {
            minute = $"{taiwanTime.GetMinute(DateTime.Now)}";
        }

        second = taiwanTime.GetSecond(DateTime.Now);
        timeTMPro.text = $"{hour}\n{minute}";

    }
}
