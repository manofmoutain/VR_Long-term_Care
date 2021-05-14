using System;
using System.Collections.Generic;
using GlobalSystem;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEngine;

namespace Manager
{
    public class ScoreManager : Monosingleton<ScoreManager>, IScoreSystem
    {
        #region Public Variables

        public Mod GameMod => gameMod;
        public bool isCounting;

        #endregion

        #region Private Variables

        /// <summary>
        /// 模式
        /// </summary>
        [SerializeField] private Mod gameMod;

        [SerializeField] ScoreSystem _scoreSystem;

        public string _ScoreSystem()
        {
            var _data = JsonUtility.ToJson(_scoreSystem);
            return _data;
        }

        /// <summary>
        /// 獲得操作時間
        /// </summary>
        public float GetTime => _scoreSystem.ExamTime;

        [SerializeField] List<ExaminClass> _examinClasses;

        #endregion

        #region Public Methods

        /// <summary>
        /// 設置模式
        /// </summary>
        /// <param name="mod"></param>
        public void SetGameMod(Mod mod)
        {
            gameMod = mod;
        }

        /// <summary>
        /// 某個項目刪去剩餘操作次數
        /// </summary>
        /// <param name="index"></param>
        public void DecreaseOperateSteps(int index)
        {
            _scoreSystem.DecreasesSteps(index);
        }

        /// <summary>
        /// 某個項目進行扣分
        /// </summary>
        /// <param name="index"></param>
        public void DecreaseScore(int index)
        {
            _scoreSystem.DecreaseScore(index);
        }

        /// <summary>
        /// 取得某個步驟是否完成
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetIsDone(int index)
        {
            return _scoreSystem.GetIsDone(index);
        }

        /// <summary>
        /// 取得這個考題的考題名稱
        /// </summary>
        /// <returns></returns>
        public string GetLesson()
        {
            return _scoreSystem.GetLesson();
        }

        /// <summary>
        /// 取得這個考題需要操作的步驟數量
        /// </summary>
        /// <returns></returns>
        public int GetListCount()
        {
            return _scoreSystem.GetListCount();
        }

        /// <summary>
        /// 取得某個項目需要操作的總次數
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetOperateSteps(int index)
        {
            return _scoreSystem.GetOperateSteps(index);
        }

        public OperateTopic GetOperateTopic(int index)
        {
            return _scoreSystem.GetOperateTopic(index);
        }

        /// <summary>
        /// 取得學校名稱
        /// </summary>
        /// <returns></returns>
        public string GetSchool()
        {
            return _scoreSystem.GetSchool();
        }

        /// <summary>
        /// 取得某個項目剩餘操作的次數
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetSteps(int index)
        {
            return _scoreSystem.GetSteps(index);
        }

        /// <summary>
        /// 取得學生ID
        /// </summary>
        /// <returns></returns>
        public string GetStudentID()
        {
            return _scoreSystem.GetStudentID();
        }

        /// <summary>
        /// 取得學生姓名
        /// </summary>
        /// <returns></returns>
        public string GetStudentName()
        {
            return _scoreSystem.GetStudentName();
        }

        /// <summary>
        /// 取得某個項目是否有操作的項目內容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetToDo(int index)
        {
            return _scoreSystem.GetToDo(index);
        }

        /// <summary>
        /// 取得某個項目的總扣分
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetTopicScore(int index)
        {
            return _scoreSystem.GetTopicScore(index);
        }

        /// <summary>
        /// 取得總分
        /// </summary>
        /// <returns></returns>
        public float GetTotalScore()
        {
            return _scoreSystem.GetTotalScore();
        }

        /// <summary>
        /// 取得某個步驟需要做到的內容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetWhatToDo(int index)
        {
            return _scoreSystem.GetWhatToDo(index);
        }

        /// <summary>
        /// 某個項目增加剩餘操作次數
        /// </summary>
        /// <param name="inderx"></param>
        public void IncreaseOperateSteps(int inderx)
        {
            _scoreSystem.IncreaseSteps(inderx);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="listCount"></param>
        public void Initialize(string fileName, int listCount, int timeValue)
        {
            // _scoreSystem = new ScoreSystem(100);
            //讀取某個檔案
            _scoreSystem.ReadExcelSimplePasses(fileName, listCount, timeValue);
        }

        public void AddExamData(ExaminClass examinClass)
        {
            _examinClasses.Add(examinClass);
        }

        /// <summary>
        /// 從檔案獲取題目資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="listCount"></param>
        public void ReadExcelSimplePasses(string fileName, int listCount, int timeValue)
        {
            _scoreSystem.ReadExcelSimplePasses(fileName, listCount, timeValue);
        }

        public void ReadExamData(int index)
        {
            _scoreSystem.LoadExamData(_examinClasses[index]);

        }

        /// <summary>
        /// 設置index的作答情況
        /// </summary>
        /// <param name="index"></param>
        public void SetDone(int index)
        {
            _scoreSystem.SetIsDone(index);
        }

        /// <summary>
        /// 設置學校
        /// </summary>
        /// <param name="school"></param>
        public void SetSchool(string school)
        {
            _scoreSystem.SetSchool(school);
        }

        /// <summary>
        /// 設置學生ID
        /// </summary>
        /// <param name="text"></param>
        public void SetStudentID(string text)
        {
            _scoreSystem.SetId(text);
        }

        /// <summary>
        /// 設置學生名字
        /// </summary>
        /// <param name="text"></param>
        public void SetStudentName(string text)
        {
            _scoreSystem.SetStudentName(text);
        }

        /// <summary>
        /// 開始計算測驗時間
        /// </summary>
        public void StartCounting()
        {
            isCounting = true;
        }

        public void StopCountingTime()
        {
            isCounting = false;
        }

        /// <summary>
        /// 是否在時限內完成操作：true=超過時間，false=時限內完成
        /// </summary>
        /// <returns></returns>
        public bool IsTimeLimit()
        {
            return !(Mathf.CeilToInt(_scoreSystem.ExamTime) <= _scoreSystem.LimitTime*60.0f);
        }

        #endregion

        #region Protected Methods

        protected override void Awake()
        {
            base.Awake();
            if (FindObjectsOfType<ScoreManager>().Length > 1)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }

            _scoreSystem = new ScoreSystem(100);
        }

        private void Update()
        {
            if (isCounting)
            {
                _scoreSystem.CountingTime();
            }
        }

        #endregion
    }
}