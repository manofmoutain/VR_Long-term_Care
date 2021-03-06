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

        void ReadExcelSimplePasses(string fileName, int listCount , int timeValue);
        string GetSchool();
        int GetSteps(int index);
        string GetStudentID();
        string GetStudentName();

        string GetToDo(int index);
        float GetTopicScore(int index);
        float GetTotalScore();
        string GetWhatToDo(int index);
        void SetStudentID(string text);

        #endregion
    }
}