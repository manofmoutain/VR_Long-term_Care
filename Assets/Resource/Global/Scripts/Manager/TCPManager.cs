using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Manager
{
    public class TCPManager : Monosingleton<TCPManager>
    {
        private void Start()
        {

        }

        public void UploadData(string data)
        {
            Application.OpenURL("https://nutcvr.rdto.io/");
            StartCoroutine(Upload(data));
        }

        IEnumerator Upload(string data)
        {
            var myData = new byte[2048];
            myData = System.Text.Encoding.UTF8.GetBytes(data);
            UnityWebRequest unityWebRequest = new UnityWebRequest("https://nutcvr.rdto.io/api/v1/vr/session", "POST");

            unityWebRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(myData);
            unityWebRequest.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("AUTHORIZATION",
                "rkgvmme5CBWMAfwpM9GOZRgP+2BgvknHv0Y3raH1mKzMP23nHzrRR4b6B!9Bgs0pdON@BAB!EPfXeCod");

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                print(unityWebRequest.error);
            }
            else
            {
                print("Upload complete");
            }
        }
    }
}