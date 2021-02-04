namespace System
{
    /// <summary>
    /// 操作項目
    /// </summary>
    [Serializable]
    public class OperateTopic
    {
        #region Public Variables

        /// <summary>
        /// 是否操作正確
        /// </summary>
        public bool isDone;

        /// <summary>
        /// 同一項目中須達到數種操作的數量
        /// </summary>
        public int operateSteps;

        /// <summary>
        /// 扣多少分
        /// </summary>
        public int score;

        /// <summary>
        /// 是否有操作這個項目
        /// </summary>
        public string isToDo;

        /// <summary>
        /// 要操作什麼
        /// </summary>
        public string whatToDo;

        #endregion
    }
}