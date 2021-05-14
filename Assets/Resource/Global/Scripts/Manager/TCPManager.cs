using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Manager
{
    public class TCPManager : MonoBehaviour
    {
        private void Start()
        {
            // StartCoroutine(Upload());
        }

        public void UploadData()
        {
            Application.OpenURL("https://nutcvr.rdto.io/");
            StartCoroutine(Upload());
        }

        IEnumerator Upload()
        {
            var data = System.Text.Encoding.UTF8.GetBytes(ScoreManager.Instance._ScoreSystem());
            var unityWebRequest = new UnityWebRequest("https://nutcvr.rdto.io/api/v1/vr/session", "POST");

            unityWebRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
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