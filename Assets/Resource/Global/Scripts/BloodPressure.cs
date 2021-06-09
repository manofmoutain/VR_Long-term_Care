using System.Collections;
using UnityEngine;

namespace InteractableObject
{
    /// <summary>
    /// 血壓計的帶子
    /// </summary>
    public class BloodPressure : MonoBehaviour
    {
        [SerializeField] private bool binding;

        public GameObject Belt;
        public GameObject DeflateBelt;

        public Animator beltAnimator;


        // Start is called before the first frame update
        void Start()
        {
            Belt.GetComponent<SkinnedMeshRenderer>().enabled = true;
            DeflateBelt.GetComponent<MeshRenderer>().enabled = false;

            beltAnimator = gameObject.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

            //取得目前動畫狀態的資訊
            AnimatorStateInfo currentState = beltAnimator.GetCurrentAnimatorStateInfo(0);

            //然後直接比對名稱就好，但是狀態資訊中並不是用string來存，所以把名稱轉Hash來比對，格式[Layer name].[state name]
            if (currentState.IsName("Expansion") && currentState.normalizedTime > 0f)
            {
                StartCoroutine(VentAnimation());
            }
            else if (currentState.IsName("Vent") && currentState.normalizedTime > 1.0f)
            {
                //顯示數值

            }
        }



        public void PutBeltTie()
        {
            Belt.GetComponent<SkinnedMeshRenderer>().enabled = true;
            DeflateBelt.GetComponent<MeshRenderer>().enabled = false;
        }

        public void PickBeltLoose()
        {
            Belt.GetComponent<SkinnedMeshRenderer>().enabled = false;
            DeflateBelt.GetComponent<MeshRenderer>().enabled = true;
        }

        public void BindBelt()
        {
            binding = true;
        }

        public void ReleaseBelt()
        {
            binding = false;
        }


        public void PreesureHR()
        {
            if(binding)
            {
                //膨脹動畫
                beltAnimator.SetBool("active", true);
            }

        }


        IEnumerator VentAnimation()
        {
            yield return new WaitForSeconds(2.0f);

            //洩氣動畫
            beltAnimator.SetBool("active", false);
        }

    }
}

