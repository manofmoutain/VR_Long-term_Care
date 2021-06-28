using System;
using System.Collections;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace Manager
{
    public class TCPManager : Monosingleton<TCPManager>
    {
        [Serializable]
        public class DevicePin
        {
            public string pin;
            public string device_id;
            public string login_url;
        }

        [Serializable]
        public class UserData
        {
            public string vr_session_id;
            public string user_id;
            public string user_name;
        }

        /// <summary>
        /// 裝置pin
        /// </summary>
        [SerializeField] private DevicePin devicePin;

        /// <summary>
        /// 使用者資訊
        /// </summary>
        public UserData userData;

        [SerializeField] private string getPinURI = "/api/v2/vr/session/";

        /// <summary>
        /// 教案ID
        /// </summary>
        [SerializeField] private string lesson_ID = "A99";

        /// <summary>
        /// 登入pollURL
        /// </summary>
        [SerializeField] private string devicePollingURL = "/api/v2/vr/session/poll/";

        public bool isLogin;
        public string UserID => userData.user_id;
        public string UserName => userData.user_name;

        private void Start()
        {
            devicePin = new DevicePin();
            userData = new UserData();
            isLogin = false;

            StartCoroutine(Co_GetDevicePin());
        }


        /// <summary>
        /// 獲取pin碼
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        IEnumerator Co_GetDevicePin()
        {
            string uri = $"https://nutcvr.rdto.io{getPinURI}{lesson_ID}";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                webRequest.SetRequestHeader("AUTHORIZATION",
                    "rkgvmme5CBWMAfwpM9GOZRgP+2BgvknHv0Y3raH1mKzMP23nHzrRR4b6B!9Bgs0pdON@BAB!EPfXeCod");
                webRequest.useHttpContinue = false;
                webRequest.timeout = 60;
                string[] pages = uri.Split('/');
                int page = pages.Length - 1;
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        devicePin = JsonUtility.FromJson<DevicePin>(webRequest.downloadHandler.text);
                        break;
                }

                yield return new WaitUntil(() => devicePin.pin != string.Empty);
                Application.OpenURL(devicePin.login_url);
                InvokeRepeating(nameof(LoginByPin), 1, 1);
            }
        }

        /// <summary>
        /// 透過pin碼登入
        /// </summary>
        public void LoginByPin()
        {
            StartCoroutine(Co_Device_Login_State_Polling());
        }

        /// <summary>
        /// 從pin碼取得使用者資訊
        /// </summary>
        /// <returns></returns>
        IEnumerator Co_Device_Login_State_Polling()
        {
            string url = $"https://nutcvr.rdto.io{devicePollingURL}{devicePin.pin}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.SetRequestHeader("AUTHORIZATION",
                    "rkgvmme5CBWMAfwpM9GOZRgP+2BgvknHv0Y3raH1mKzMP23nHzrRR4b6B!9Bgs0pdON@BAB!EPfXeCod");

                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + webRequest.error);
                        isLogin = false;
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        isLogin = false;
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Received: " + webRequest.downloadHandler.text);
                        userData = JsonUtility.FromJson<UserData>(webRequest.downloadHandler.text);
                        if (webRequest.downloadHandler.text != string.Empty)
                        {
                            isLogin = true;
                        }
                        break;
                }
            }
        }
    }
}