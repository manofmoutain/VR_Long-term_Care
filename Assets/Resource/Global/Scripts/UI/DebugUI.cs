using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugSpeechUI;
    void Start()
    {

        if (debugSpeechUI==null)
        {
            SpeechManager.Instance.NoUIToShowSpeech();
        }
        else
        {
            SpeechManager.Instance.DebugSpeechUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpeechManager.Instance.UpdateDebugUI(debugSpeechUI);
    }

}
