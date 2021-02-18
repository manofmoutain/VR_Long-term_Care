namespace GlobalSystem
{
    public interface IScoreSystem
    {
        #region Public Methods

        void DecreaseOperateSteps(int index);

        void DecreaseScore(int index);
        bool IsDone(int index);
        string Lesson();
        int ListCount();
        int OperateSteps(int index);

        void ReadExcelSimplePasses(string fileName, int listCount);
        string School();
        int Steps(int index);
        string StudentID();
        string StudentName();

        string ToDo(int index);
        int TopicScore(int index);
        int TotalScore();
        string WhatToDo(int index);
        void SetStudentID(string text);

        #endregion
    }
}