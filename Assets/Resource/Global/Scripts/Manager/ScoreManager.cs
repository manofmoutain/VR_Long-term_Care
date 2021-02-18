using GlobalSystem;
using UnityEngine;

namespace Manager
{
    public class ScoreManager : Monosingleton<ScoreManager> , IScoreSystem
    {
        // public ScoreSystem _scoreSystem;
        [SerializeField] ScoreSystem _scoreSystem;
        private Mod gameMod;
        public Mod GameMod => gameMod;
        protected override void Awake()
        {
            base.Awake();
            if (FindObjectsOfType<ScoreManager>().Length>1)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }

            _scoreSystem = new ScoreSystem(100);
        }

        public void Initialize(string fileName , int listCount)
        {
            _scoreSystem = new ScoreSystem(100);
            _scoreSystem.ReadExcelSimplePasses(fileName, listCount);
        }

        public void DecreaseOperateSteps(int index)
        {
            _scoreSystem.DecreasesSteps(index);
        }

        public void DecreaseScore(int index)
        {
            _scoreSystem.DecreaseScore(index);
        }

        public bool IsDone(int index)
        {
            return _scoreSystem.IsDone(index);
        }

        public string Lesson()
        {
            return _scoreSystem.Lesson();
        }

        public int ListCount()
        {
            return _scoreSystem.ListCount();
        }

        public int OperateSteps(int index)
        {
            return _scoreSystem.OperateSteps(index);
        }

        public void ReadExcelSimplePasses(string fileName, int listCount)
        {
            _scoreSystem.ReadExcelSimplePasses(fileName,listCount);
        }

        public string School()
        {
            return _scoreSystem.School();
        }

        public int Steps(int index)
        {
            return _scoreSystem.Steps(index);
        }

        public string StudentID()
        {
            return _scoreSystem.StudentID();
        }

        public string StudentName()
        {
            return _scoreSystem.StudentName();
        }

        public string ToDo(int index)
        {
            return _scoreSystem.ToDo(index);
        }

        public int TotalScore()
        {
            return _scoreSystem.TotalScore();
        }

        public string WhatToDo(int index)
        {
            return _scoreSystem.WhatToDo(index);
        }

        public int TopicScore(int index)
        {
            return _scoreSystem.TopicScore(index);
        }

        public void SetStudentID(string text)
        {
            _scoreSystem.SetId(text);
        }

        public void SetStudentName(string text)
        {
            _scoreSystem.SetStudentName(text);
        }

        public void SetSchool(string school)
        {
            _scoreSystem.SetSchool(school);
        }

        public void SetDone(int index, bool checkMark)
        {
            _scoreSystem.SetIsDone(index,checkMark);
        }
    }
}