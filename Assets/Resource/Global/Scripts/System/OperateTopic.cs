using System;
using System.Reflection.Emit;

namespace GlobalSystem
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
        /// 項目類型
        /// </summary>
        public string operateType;

        /// <summary>
        /// 要操作什麼
        /// </summary>
        public string whatToDo;

        /// <summary>
        /// 扣多少分
        /// </summary>
        public float score;

        /// <summary>
        /// 同一項目中須達到數種操作的數量
        /// </summary>
        public int operateSteps;

        /// <summary>
        /// 是否有操作這個項目
        /// </summary>
        public string isToDo;
        #endregion
    }

    // [Serializable]
    // public class LoginData
    // {
    //     public string user_id;
    //     public string user_password;
    //     public string lesson_id="110-V13";
    // }
}