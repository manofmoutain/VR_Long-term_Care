using UnityEngine;

namespace Heimlich_maneuver
{
    public partial class HamlickPatient
    {
        [Header("模型")] [SerializeField] private GameObject hugRightHand;
        [SerializeField] private GameObject hugLeftHand;

        /// <summary>
        /// 是否處於被抱起來的狀態
        /// </summary>
        [Header("狀態")]
        [SerializeField] private bool isHuging;

        public void Hugging(bool hugging)
        {
            isHuging = hugging;
        }
    }
}

