using Global.Pateint;
using Manager;
using UnityEngine;

namespace Heimlich_maneuver
{
    public partial class HamlickPatient
    {
        [Tooltip("提示UI")] [SerializeField]  GameObject interactHint;
        [Tooltip("要按壓的位置")] [SerializeField]  GameObject interactPoint;

        public void ResetToOriginPosition(int index)
        {
            if (!isChoking && !GetComponent<Patient>().isAtOriginPosition )
            {
                //項目六：將案主移回原位
                ScoreManager.Instance.DecreaseOperateSteps(index);
                ScoreManager.Instance.SetDone(index);
            }
        }
    }
}

