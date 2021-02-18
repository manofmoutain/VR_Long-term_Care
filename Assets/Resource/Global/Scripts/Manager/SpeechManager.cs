using System;
using System.Collections.Generic;
using GlobalSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class SpeechManager : Monosingleton<SpeechManager>
    {
        #region Private Variables

        /// <summary>
        /// 需要辨識語音的情境
        /// </summary>
        [Serializable]
        class Situations
        {
            #region Public Variables

            public AudioClip[] respondAudio;
            public string respond;
            public string[] keyWords;

            #endregion

            #region Private Variables

            [SerializeField] private string situationName;

            #endregion
        }

        [SerializeField] private List<Situations> _situations;


        [SerializeField] private SpeechSystem _speechSystem;

        /// <summary>
        /// 辨識出的語音文字，顯示在UI上
        /// </summary>
        [SerializeField] private Text SpeechDebugUIText;

        #endregion

        #region Unity events

        private void Start()
        {
            _speechSystem.SetRespondStatus(string.Empty);
            _speechSystem = new SpeechSystem();
            _speechSystem.SetMicPermission(false);
            _speechSystem.SetMessage(string.Empty);
            _speechSystem.NewThreadLocker();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 清除資訊
        /// </summary>
        public void ClearMessage()
        {
            StartCoroutine(_speechSystem.CleareMessageAfterRecognize());
        }

        /// <summary>
        /// 顯示文字UI
        /// </summary>
        public void DebugSpeechUI()
        {
            _speechSystem.SetMicPermission(true);
            _speechSystem.SetMessage("按下按鈕以啟動語音辨識");
        }

        /// <summary>
        /// 依據索引查找關鍵字
        /// </summary>
        /// <param name="index"></param>
        public void FindKeyWords(int index)
        {
            foreach (string words in KeyWordsInSituation(index))
            {
                Respond(words, index);
            }
        }

        /// <summary>
        /// 警告沒有文字UI
        /// </summary>
        public void NoUIToShowSpeech()
        {
            Debug.LogError("沒有置放辨識語音文字的UI");
        }

        /// <summary>
        /// 開始語音辨識
        /// </summary>
        public void StartRecognizeSpeech()
        {
            _speechSystem.SetRespondStatus("MicroReceive");
            _speechSystem.SetMessage("辨識語音中.....");
            _speechSystem.SetSpeechMassage(_speechSystem.Message);
            _speechSystem.VoiceRecognition();
        }


        public void UpdateDebugUI(Text text)
        {
            _speechSystem.UpdateMessage(text);
        }

        #endregion

        #region Protected Methods

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Private Methods

        private string[] KeyWordsInSituation(int index) => _situations[index].keyWords;

        /// <summary>
        /// 依據索引回應內容
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="index"></param>
        void Respond(string keyword, int index)
        {
            if (StringExtensions.Contains(_speechSystem.SpeechMesssage, keyword, 0))
            {
                //兩秒後回溯文字
                ClearMessage();
                //打印回應的文字
                Debug.Log(_situations[index].respond);
                //ToDoSomething
                switch (index)
                {
                    case 0:
                        ScoreManager.Instance.DecreaseOperateSteps(0);
                        ScoreManager.Instance.SetDone(index,true);
                        break;
                    case 1:
                        break;
                    default:
                        Debug.Log($"沒有設置回應");
                        break;
                        ;
                }
            }
        }

        private void Update()
        {
            if (_speechSystem.RespondStatus == "MicroReceive")
            {
                if (!_speechSystem.WaitingForRecognize)
                {
                    for (int i = 0; i < _situations.Count; i++)
                    {
                        FindKeyWords(i);
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 查找關鍵字
    /// </summary>
    public static class StringExtensions
    {
        #region Public Methods

        public static bool Contains(this String str, String substring, StringComparison comp)
        {
            if (substring == null)
            {
                throw new ArgumentNullException("substring", "substring cannot be null.");
            }
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
            {
                throw new ArgumentException("comp is not a member of StringComparison", "comp");
            }

            return str.IndexOf(substring, comp) >= 0;
        }

        #endregion
    }
}