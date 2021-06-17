using System.Collections.Generic;
using GlobalSystem;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace TitleUIScripts
{
    public class ChooseExamClass : MonoBehaviour
    {
        [SerializeField] private bool isLoadExel;
        [SerializeField] private List<Button> examBtns;
        [SerializeField] private List<ExaminClass> examClass;
        [SerializeField] private Button backBtn;

        private void Start()
        {
            if (isLoadExel)
            {
                for (int i = 0; i < examClass.Count; i++)
                {
                    ScoreManager.Instance.Initialize(examClass[i].fileName, examClass[i].operateCount , examClass[i].limitTime);
                    examClass[i].lesson = ScoreManager.Instance.GetLesson();
                    examClass[i].operateTopics.Clear();
                    for (int j = 0; j < examClass[i].operateCount; j++)
                    {
                        examClass[i].operateTopics.Add(ScoreManager.Instance.GetOperateTopic(j));
                    }
                }
            }



            backBtn.onClick.AddListener(delegate
            {
                UI_Manager.Instance.Back();
            });

        }

        public void EnterExam(int index)
        {
            if (isLoadExel)
            {
                ScoreManager.Instance.Initialize(examClass[index].fileName, examClass[index].operateCount , examClass[index].limitTime);
            }
            else
            {
                ScoreManager.Instance.ReadExamData(index);
            }

            SceneLoader.Instance.LoadScene(examClass[index].examSceneIndex);
            ScoreManager.Instance.StartCounting();
        }
    }
}