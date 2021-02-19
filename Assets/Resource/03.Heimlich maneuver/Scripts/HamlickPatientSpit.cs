using UnityEngine;
using Random = UnityEngine.Random;

namespace Heimlich_maneuver.Patient
{
    public class HamlickPatientSpit : MonoBehaviour
    {
        [Header("音效")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip spitSFX;


        [Header("生成物件")] [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject bean;

        /// <summary>
        /// 是否處於哽塞狀態
        /// </summary>
        [SerializeField] private bool isChoking;

        /// <summary>
        /// 最小按壓次數
        /// </summary>
        [SerializeField]private int minPushCount;

        /// <summary>
        /// 施作次數
        /// </summary>
        [SerializeField] private int pushCount;


        private void Start()
        {
            isChoking = true;
        }

        void Spit()
        {
            if (pushCount > minPushCount && isChoking)
            {
                int random = Random.Range(0, 10);
                if (random >= 7)
                {
                    audioSource.PlayOneShot(spitSFX);
                    GameObject go = Instantiate(bean, spawnPoint.position, Quaternion.identity, transform);
                    go.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 3);
                    Destroy(go, 3f);
                    isChoking = false;
                }
            }
        }

        public void Push()
        {
            // pushCount = triggerCount.GetComponent<OnTriggerEnterCount>().Count;
            pushCount++;
            Spit();
        }
    }
}