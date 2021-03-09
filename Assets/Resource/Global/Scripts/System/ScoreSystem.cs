using UnityEngine;
using System;
using System.Collections.Generic;
using Excel;
using System.Data;
using System.IO;
using JetBrains.Annotations;
using TMPro.Examples;

namespace GlobalSystem
{
    [System.Serializable]
    public class ScoreSystem
    {
        #region Private Variables

        private int columns;
        private int rows;
        [SerializeField] private int topicCount;

        /// <summary>
        /// 總分
        /// </summary>
        [SerializeField] private int totalScore;

        /// <summary>
        /// 操作項目列表
        /// </summary>
        [SerializeField] private List<OperateTopic> _operateTopics;

        [SerializeField] private string excelFileName;

        /// <summary>
        /// 受試者編號
        /// </summary>
        [SerializeField] private string m_ID;


        /// <summary>
        /// 測驗題目
        /// </summary>
        [SerializeField] private string m_Lesson;

        /// <summary>
        /// 學校名稱
        /// </summary>
        [SerializeField] private string m_School;

        /// <summary>
        /// 受試者名稱
        /// </summary>
        [SerializeField] private string m_StudentName;

        /// <summary>
        /// 時間限制
        /// </summary>
        [SerializeField] private int timeLimite;

        /// <summary>
        /// 操作時間
        /// </summary>
        [SerializeField] private float timing;

        /// <summary>
        /// 操作時間
        /// </summary>
        public float ExamTime => timing;

        #endregion

        #region Public Methods

        /// <summary>
        /// 此測驗的時間限制
        /// </summary>
        public int LimitTime => timeLimite;

        /// <summary>
        /// 計算時間
        /// </summary>
        public void CountingTime()
        {
            timing += Time.deltaTime;
        }

        /// <summary>
        /// 依項目扣分
        /// </summary>
        /// <param name="index">第幾項</param>
        public void DecreaseScore(int index)
        {
            totalScore -= _operateTopics[index].score * GetSteps(index);
            Debug.Log(
                $"第{index}題：{_operateTopics[index].whatToDo}操作失敗，扣{_operateTopics[index].score}分，總分剩{totalScore}分");
        }

        /// <summary>
        /// 增加操作步驟
        /// </summary>
        /// <param name="index"></param>
        public void IncreaseSteps(int index)
        {
            _operateTopics[index].operateSteps++;

            Debug.Log($"第{index}項操作失誤，增加為{_operateTopics[index].operateSteps}個操作步驟");
        }

        /// <summary>
        /// 刪減操作步驟
        /// </summary>
        /// <param name="index"></param>
        public void DecreasesSteps(int index)
        {
            _operateTopics[index].operateSteps--;
            if (_operateTopics[index].operateSteps < 0)
            {
                _operateTopics[index].operateSteps = 0;
            }

            Debug.Log($"第{index}項操作剩餘{_operateTopics[index].operateSteps}個步驟未操作");
        }

        /// <summary>
        /// 取得操作List
        /// </summary>
        /// <returns></returns>
        public List<OperateTopic> GetAllOperateTopics()
        {
            return _operateTopics;
        }

        /// <summary>
        /// 回傳是否已操作完成
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetIsDone(int index)
        {
            if (_operateTopics[index].operateSteps==0)
            {
                _operateTopics[index].isDone = true;
            }
            Debug.Log(_operateTopics[index].isDone ? $"第{index}項已操作完成" : $"第{index}項操作未完成");
            return _operateTopics[index].isDone;
        }

        /// <summary>
        /// 回傳測試題目
        /// </summary>
        /// <returns></returns>
        public string GetLesson()
        {
            Debug.Log($"測試題目為{m_Lesson}");
            return m_Lesson;
        }

        /// <summary>
        /// 獲取操作項目的數量
        /// </summary>
        /// <returns></returns>
        public int GetListCount()
        {
            Debug.Log($"操作項目共有{_operateTopics.Count}項");
            return _operateTopics.Count;
        }

        /// <summary>
        /// 取得目前某操作項目的剩餘操作步驟
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetOperateSteps(int index)
        {
            Debug.Log($"第{index}項需要{_operateTopics[index].operateSteps}個操作步驟");
            return _operateTopics[index].operateSteps;
        }

        /// <summary>
        /// 從檔案獲取資料
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="listCount">操作項目數量</param>
        public void ReadExcelSimplePasses(string fileName, int listCount , int timeValue)
        {
            excelFileName = fileName;
            string excelName = excelFileName + ".xlsx";
            string sheetName = "sheet1";
            DataRowCollection collect = ExcelReader(excelName, sheetName, 4);

            SetLesson(collect[0][2].ToString());
            SetSchool(collect[1][2].ToString());
            NewOperateList();
            for (int i = 2; i < listCount + 2; i++)
            {
                var newTopic = new OperateTopic
                {
                    whatToDo = collect[i][1].ToString(),
                    isToDo = collect[i][2].ToString(),
                    score = int.Parse(collect[i][3].ToString()),
                    operateSteps = int.Parse(collect[i][4].ToString())
                };
                AddOperateTopics(newTopic);
            }

            timeLimite = timeValue;
        }

        public string GetSchool()
        {
            Debug.Log($"受試者學校為：{m_School}");
            return m_School;
        }

        /// <summary>
        /// 設定受試者編號
        /// </summary>
        /// <param name="id"></param>
        public void SetId(string id)
        {
            m_ID = id;
            Debug.Log($"受試者編號：{id}");
        }

        /// <summary>
        /// 設置是否已操作結束
        /// </summary>
        /// <param name="index"></param>
        /// <param name="checkMark"></param>
        public void SetIsDone(int index)
        {
            _operateTopics[index].isDone = _operateTopics[index].operateSteps <= 0 ? true : false;
            Debug.Log($"第{index}題操作結果：{_operateTopics[index].isDone}");
        }


        /// <summary>
        /// 設定測試題目
        /// </summary>
        /// <param name="lessonData"></param>
        public void SetLesson(string lessonData)
        {
            m_Lesson = lessonData;
            Debug.Log($"設置測驗題目：{lessonData}");
        }

        /// <summary>
        /// 設定學校
        /// </summary>
        /// <param name="schoolData"></param>
        public void SetSchool(string schoolData)
        {
            m_School = schoolData;
            Debug.Log($"設置學校：{schoolData}");
        }

        /// <summary>
        /// 設定受試者名稱
        /// </summary>
        /// <param name="_name"></param>
        public void SetStudentName(string _name)
        {
            m_StudentName = _name;
            Debug.Log($"設置受試者名稱：{_name}");
        }

        /// <summary>
        /// 設置某個操作項目的分數
        /// </summary>
        /// <param name="index">操作項目</param>
        /// <param name="score">分數</param>
        public void SetTopicScore(int index, int score)
        {
            _operateTopics[index].score = score;
            Debug.Log($"設置第{index}題分數為{score}");
        }

        /// <summary>
        /// 回傳操作項目剩餘的操作步驟
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetSteps(int index)
        {
            Debug.Log($"第{index}項操作剩餘{_operateTopics[index].operateSteps}個步驟未操作");
            return _operateTopics[index].operateSteps;
        }

        /// <summary>
        /// 回傳受試者編號
        /// </summary>
        /// <returns></returns>
        public string GetStudentID()
        {
            Debug.Log($"受試者編號為：{m_ID}");
            return m_ID;
        }

        /// <summary>
        /// 回傳受試者姓名
        /// </summary>
        /// <returns></returns>
        public string GetStudentName()
        {
            Debug.Log($"受試者姓名為：{m_StudentName}");
            return m_StudentName;
        }

        /// <summary>
        /// 獲得是否操作項目的標題
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetToDo(int index)
        {
            Debug.Log($"第{index}項的目標標題為{_operateTopics[index].isToDo}");
            return _operateTopics[index].isToDo;
        }

        /// <summary>
        /// 獲得某個項目的分數
        /// </summary>
        /// <param name="index">操作項目編號</param>
        /// <returns></returns>
        public int GetTopicScore(int index)
        {
            Debug.Log($"第{index}題的分數為{_operateTopics[index].score}");
            return _operateTopics[index].score;
        }

        /// <summary>
        /// 獲得總成績
        /// </summary>
        /// <returns></returns>
        public int GetTotalScore()
        {
            Debug.Log($"總分為{totalScore}");
            return totalScore < 0 ? totalScore = 0 : totalScore;
        }

        /// <summary>
        /// 獲得操作標題
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetWhatToDo(int index)
        {
            Debug.Log($"第{index}項的標題是{_operateTopics[index].whatToDo}");
            return _operateTopics[index].whatToDo;
        }

        public OperateTopic GetOperateTopic(int index)
        {
            return _operateTopics[index];
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 增加一個新的操作欄位
        /// </summary>
        /// <param name="newTopic"></param>
        private void AddOperateTopics(OperateTopic newTopic)
        {
            _operateTopics.Add(newTopic);
        }

        /// <summary>
        /// 回傳檔案內部資料
        /// </summary>
        /// <param name="excelName">檔案名稱</param>
        /// <param name="sheetName">第幾項資料</param>
        /// <param name="columnsIndex">總共幾個欄位</param>
        /// <returns></returns>
        private DataRowCollection ExcelReader(string excelName, string sheetName, int columnsIndex)
        {
            string path = Application.dataPath + "/外部修改檔案/" + excelName;
            // Debug.Log(path);
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet();
            //columns = result.Tables[0].Columns.Count;
            columns = columnsIndex;
            rows = result.Tables[0].Rows.Count;

            //tables可以按照sheet名获取，也可以按照sheet索引获取
            //return result.Tables[0].Rows;
            // Debug.Log(result.Tables[sheetName].Rows);
            return result.Tables[sheetName].Rows;
        }

        /// <summary>
        /// 實例化操作列表
        /// </summary>
        private void NewOperateList()
        {
            _operateTopics = new List<OperateTopic>();
        }

        #endregion

        /// <summary>
        /// 實例化分數系統
        /// </summary>
        /// <param name="score"></param>
        public ScoreSystem(int score)
        {
            totalScore = score;
        }
    }
}