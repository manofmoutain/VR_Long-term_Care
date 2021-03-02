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

        #endregion

        #region Private Variables

        [SerializeField] private Mod gameMod;

        [SerializeField] ScoreSystem _scoreSystem;

        #endregion

        #region Public Methods

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
        /// 取得某個項目要操作的內容
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
        public int GetTopicScore(int index)
        {
            return _scoreSystem.GetTopicScore(index);
        }

        /// <summary>
        /// 取得總分
        /// </summary>
        /// <returns></returns>
        public int GetTotalScore()
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
        /// 某個項目增加生於操作次數
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
        public void Initialize(string fileName, int listCount)
        {
            // _scoreSystem = new ScoreSystem(100);
            //讀取某個檔案
            _scoreSystem.ReadExcelSimplePasses(fileName, listCount);
        }

        /// <summary>
        /// 從檔案獲取題目資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="listCount"></param>
        public void ReadExcelSimplePasses(string fileName, int listCount)
        {
            _scoreSystem.ReadExcelSimplePasses(fileName, listCount);
        }

        /// <summary>
        /// 設置index的作答情況
        /// </summary>
        /// <param name="index"></param>
        /// <param name="checkMark"></param>
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

        public void StartCounting()
        {
            _scoreSystem.StartCounting();
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

        #endregion
    }
}