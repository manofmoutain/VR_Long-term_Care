namespace GlobalSystem
{
    public interface IScoreSystem
    {
        #region Public Methods

        void DecreaseOperateSteps(int index);

        void DecreaseScore(int index);
        bool GetIsDone(int index);
        string GetLesson();
        int GetListCount();
        int GetOperateSteps(int index);

        void ReadExcelSimplePasses(string fileName, int listCount);
        string GetSchool();
        int GetSteps(int index);
        string GetStudentID();
        string GetStudentName();

        string GetToDo(int index);
        int GetTopicScore(int index);
        int GetTotalScore();
        string GetWhatToDo(int index);
        void SetStudentID(string text);

        #endregion
    }
}