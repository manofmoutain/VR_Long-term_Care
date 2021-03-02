using Manager;
using UnityEngine;
using Global.Pateint;


namespace Heimlich_maneuver
{
    public class HamlickPatient : MonoBehaviour
    {
        /// <summary>
        /// 是否處於施作的狀態
        /// </summary>
        [Header("狀態")]
        [SerializeField] bool isPushed;

        public bool canHug;





        private void Start()
        {
            isPushed = false;
            canHug = false;

        }

        private void Update()
        {
            //player位於病患背後
            canHug = GetComponent<Patient>().patientDirection.z > 0;
        }


        /// <summary>
        /// 從案主背後抱起，掛在子物件的TakeEvent_SingleHandSnapPutZone的PickUp上
        /// </summary>
        /// <param name="index">考題編號</param>
        public void HugPatientFromBack(int index)
        {
            if (canHug)
            {
                //項目二：立於後側將案主移到無手扶手椅上
                ScoreManager.Instance.DecreaseOperateSteps(index);
                ScoreManager.Instance.SetDone(index);
                print($"施測者在{gameObject.name}後面");
            }
        }

        public void SetPushed(bool pushed)
        {
            isPushed = pushed;
        }

        /// <summary>
        /// 掉落事件，掛在ResetPositionZone的TakeEvent_ResetZone的OnPatientEnter上
        /// </summary>
        /// <param name="index">考題編號</param>
        public void Fallen(int index)
        {
            GetComponent<PatientSFX>().PlaySFX(0);
            //項目十：案主掉落
            ScoreManager.Instance.IncreaseOperateSteps(index);
            ScoreManager.Instance.SetDone(index);
        }

        /// <summary>
        /// 洗手，掛在DryCleaner的InteractableHoverEvents的OnHandHoverBegin上
        /// </summary>
        /// <param name="index">考題編號</param>
        public void WashHandBeforeOperate(int index)
        {
            if (GetComponent<Patient>().GetPatientTransform.parent==transform)
            {
                //項目十二：於事前進行洗手
                ScoreManager.Instance.IncreaseOperateSteps(index);
                ScoreManager.Instance.SetDone(index);
            }
        }
    }
}