using System;
using System.Collections.Generic;
using GlobalSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Manager
{
    public class SpeechManager : Monosingleton<SpeechManager>
    {
        #region Public Variables

        public bool GetWaitingForRecognize => _speechStatus.WaitingForRecognize;
        public bool GetCorrectKeyWords(int index) => _situations[index].correctKeyWords;

        public List<Situations> GetSituation => _situations;

        public string GetRespondStatus => _speechStatus.RespondStatus;

        #endregion

        #region Private Variables

        private AudioSource audioSource;

        [SerializeField] private List<Situations> _situations;


        [SerializeField] private SpeechSystem _speechStatus;

        /// <summary>
        /// 辨識出的語音文字，顯示在UI上
        /// </summary>
        // [SerializeField] private TextMeshProUGUI SpeechDebugUIText;

        #endregion

        #region Unity events

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            _speechStatus.SetRespondStatus(string.Empty);
            _speechStatus = new SpeechSystem();
            _speechStatus.SetMicPermission(false);
            _speechStatus.SetMessage(string.Empty);
            _speechStatus.NewThreadLocker();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 停止語音播放系統的音效
        /// </summary>
        public void StopAudio()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// 語音辨識系統是否正在播放音效
        /// </summary>
        /// <returns></returns>
        public bool IsAudioPlaying()
        {
            return audioSource.isPlaying;
        }

        /// <summary>
        /// 清除資訊
        /// </summary>
        public void ClearMessage()
        {
            StartCoroutine(_speechStatus.Co_CleareMessageAfterRecognize());
        }

        /// <summary>
        /// 顯示文字UI
        /// </summary>
        public void DebugSpeechUI()
        {
            _speechStatus.SetMicPermission(true);
            _speechStatus.SetMessage("按下按鈕以啟動語音辨識");
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

        public void PlayAudio(int index)
        {
            if (_situations[index].respondAudio.Length!=0)
            {
                int random = UnityEngine.Random.Range(0, _situations[index].respondAudio.Length);
                audioSource.PlayOneShot(_situations[index].respondAudio[random]);
            }

        }

        /// <summary>
        /// 開始語音辨識
        /// </summary>
        public void StartRecognizeSpeech()
        {
            _speechStatus.SetRespondStatus("MicroReceive");
            _speechStatus.SetMessage("辨識語音中.....");
            _speechStatus.SetSpeechMassage(_speechStatus.Message);
            _speechStatus.VoiceRecognition();
        }

        /// <summary>
        /// 更新顯示在牆上的語音文字
        /// </summary>
        /// <param name="text"></param>
        public void UpdateDebugUI(TextMeshProUGUI text)
        {
            _speechStatus.UpdateMessage(text);
        }

        #endregion

        #region Protected Methods

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 取得某個情況所需的關鍵字
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private List<string> KeyWordsInSituation(int index) => _situations[index].keyWords;

        /// <summary>
        /// 依據索引回應內容
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="index"></param>
        void Respond(string keyword, int index)
        {
            if (StringExtensions.Contains(_speechStatus.SpeechMesssage, keyword, 0))
            {
                //兩秒後回溯文字
                ClearMessage();
                //打印回應的文字
                Debug.Log(_situations[index].respond);
                //ToDoSomething
                for (int i = 0; i < _situations.Count; i++)
                {
                    if (i==index)
                    {
                        _situations[i].correctKeyWords = true;

                        Debug.Log($"這是{_situations[index].situationName}狀況");
                        PlayAudio(index);
                        ScoreManager.Instance.DecreaseOperateSteps(_situations[i].topicIndex);
                        ScoreManager.Instance.SetDone(_situations[i].topicIndex);
                        if (_situations[i].isCorrectClearKeywords)
                        {
                            _situations[i].keyWords.Clear();
                            return;
                        }
                    }
                }
            }
        }

        void RemoveKeywords(int index)
        {
            _situations[index].keyWords.Clear();
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// 需要辨識語音的情境
        /// </summary>
        [Serializable]
        public class Situations
        {
            #region Public Variables
            public string situationName;
            public bool isCorrectClearKeywords;
            public bool correctKeyWords;
            public AudioClip[] respondAudio;
            public string respond;
            public List<string> keyWords;
            /// <summary>
            /// 語音辨識對應的考題編號
            /// </summary>
            public int topicIndex;
            #endregion
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