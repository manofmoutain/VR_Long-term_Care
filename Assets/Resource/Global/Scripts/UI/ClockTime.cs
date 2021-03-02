using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClockTime : MonoBehaviour
{
    [SerializeField] private TaiwanCalendar taiwanTime;
    [SerializeField] private int hour;
    [SerializeField] private int minute;
    [SerializeField] private int second;
    [SerializeField] private TextMeshProUGUI timeTMPro;

    void Start()
    {
        taiwanTime = new TaiwanCalendar();
    }

    // Update is called once per frame
    void Update()
    {
        hour = taiwanTime.GetHour(DateTime.Now);
        minute = taiwanTime.GetMinute(DateTime.Now);
        second = taiwanTime.GetSecond(DateTime.Now);
        timeTMPro.text = $"{hour}:{minute}";
    }
}
