using Global.Pateint;
using Manager;
using UnityEngine;

namespace Heimlich_maneuver
{
    public class HamlickPatientInteract : MonoBehaviour
    {
        [Tooltip("提示UI")] [SerializeField]  GameObject interactHint;
        [Tooltip("要按壓的位置")] [SerializeField]  GameObject interactPoint;


        private void Update()
        {
            interactHint.SetActive(GetComponent<PatientTransform>().isAtChangedPosition );

            interactPoint.SetActive(GetComponent<PatientTransform>().isAtChangedPosition );
        }


        public void ResetToOriginPosition(int index)
        {
            if (!GetComponent<HamlickPatientSpit>().isChoking && !GetComponent<PatientTransform>().isAtOriginPosition )
            {
                //項目六：將案主移回原位
                ScoreManager.Instance.DecreaseOperateSteps(index);
                ScoreManager.Instance.SetDone(index);
            }
        }
    }
}

