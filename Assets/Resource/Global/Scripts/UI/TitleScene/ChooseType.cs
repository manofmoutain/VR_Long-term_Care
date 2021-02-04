using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Manager;
using Random = UnityEngine.Random;

namespace TitleUIScripts
{
    public class ChooseType : MonoBehaviour
    {
        [SerializeField] private Button practiceBtn;
        [SerializeField] private Button examBtn;
        [SerializeField] private Button backBtn;

        private void Start()
        {
            practiceBtn.onClick.AddListener(delegate { UI_Manager.Instance.OpenPanel(2, 0, false); });
            examBtn.onClick.AddListener(delegate
            {
                int randomExam = Random.Range(3, 7);
                switch (randomExam)
                {
                    case 3:
                        print($"你抽到的是模擬考A");
                        break;
                    case 4:
                        print($"你抽到的是模擬考B");
                        break;
                    case 5:
                        print($"你抽到的是模擬考C-1");
                        break;
                    case 6:
                        print($"你抽到的是模擬考C-2");
                        break;
                }

                UI_Manager.Instance.OpenPanel(randomExam, 0, false);
            });
            backBtn.onClick.AddListener(delegate { UI_Manager.Instance.Back(); });
        }
    }
}