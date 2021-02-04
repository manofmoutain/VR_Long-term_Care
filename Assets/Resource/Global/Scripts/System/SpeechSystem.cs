using System.Collections;
using System.Collections.Generic;
using Microsoft.CognitiveServices.Speech;
using UnityEngine;

namespace System
{
    [Serializable]
    public class SpeechSystem
    {
        #region Public Variables

        public bool WaitingForRecognize => waitingForRecognize;

        public string Message => message;

        public string RespondStatus => _respondStatus;

        public string SpeechMesssage => _speechMesssage;

        #endregion

        #region Private Variables

        [SerializeField] private object threadLocker;

        /// <summary>
        /// 是否授予麥克風權限
        /// </summary>
        [SerializeField] private bool micPermissionGranted;

        /// <summary>
        /// 是否等待辨識語音，true=正在等待語音接收(正在辨識中)
        /// </summary>
        [SerializeField] private bool waitingForRecognize;

        /// <summary>
        /// 語音辨識狀態，防衛機制，避免在還未辨識語音時持續查找關鍵字
        /// </summary>
        [SerializeField] private string _respondStatus;

        /// <summary>
        /// 辨識出的語音文字
        /// </summary>
        [SerializeField] private string _speechMesssage;

        /// <summary>
        /// 語音辨識給予的訊息
        /// </summary>
        [SerializeField] private string message;

        [SerializeField]private float _waitForSecondsToClearMessage;

        #endregion

        #region Public Methods

        /// <summary>
        /// 回溯文字
        /// </summary>
        public IEnumerator CleareMessageAfterRecognize()
        {
            // waitingForRecognize = false;
            if (_waitForSecondsToClearMessage==0)
            {
                _waitForSecondsToClearMessage = 2f;
            }
            _respondStatus = string.Empty;
            yield return new WaitForSeconds(_waitForSecondsToClearMessage);
            message = "按下按鈕以啟動語音辨識";
            _speechMesssage = message;

        }

        public void NewThreadLocker()
        {
            threadLocker = new object();
        }

        public void SetMessage(string data)
        {
            message = data;
        }

        public void SetMicPermission(bool permision)
        {
            micPermissionGranted = permision;
        }

        public void SetRespondStatus(string status)
        {
            _respondStatus = status;
        }

        public void SetSpeechMassage(string text)
        {
            _speechMesssage = text;
        }

        public void UpdateMessage(UnityEngine.UI.Text text)
        {
            lock (threadLocker)
            {
                //把辨識到的語音打印在UI上
                if (text != null)
                {
                    _speechMesssage = message;
                    text.text = message;
                }
            }
        }

        /// <summary>
        /// 啟動語音辨識的方法
        /// </summary>
        public async void VoiceRecognition()
        {
            // 使用指定的訂閱密鑰和服務區域創建語音配置的實例。
            // 替換為您自己的訂閱密鑰和服務區域 (e.g., "westus").
            var config = SpeechConfig.FromSubscription("0db0b99ed8a74b158fb52c7362640897", "westus2");
            //SpeechConfig.SpeechRecognitionLanguage = "zh-TW";
            config.SpeechRecognitionLanguage = "zh-TW";


            // 使用後請務必部屬識別器!
            using (var recognizer = new SpeechRecognizer(config))
            {
                lock (threadLocker)
                {
                    waitingForRecognize = true;
                }

                // 開始語音識別，並在識別出單個語音後返回. 一次講話的結束是通過聽末尾是否靜音或直到處理完最多15秒的音頻來確定的.
                // 任務返回識別文本作為結果。
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query.
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                string newMessage = string.Empty;
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    newMessage = result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    newMessage = "匹配錯誤：語音無法辨識，請確認麥克風可以接收語音。";
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    newMessage = $"取消: 理由={cancellation.Reason} 錯誤訊息={cancellation.ErrorDetails}";
                }

                lock (threadLocker)
                {
                    message = newMessage;
                    waitingForRecognize = false;
                }
            }
        }
        #endregion
    }
}