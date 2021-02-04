using System;
using UnityEngine;

namespace Manager
{
    public class InputManager : Monosingleton<InputManager>
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpeechManager.Instance.StartRecognizeSpeech();
                // SpeechManager.Instance.ClearMessage();
            }
        }
        void III()
        {

        }
    }


}