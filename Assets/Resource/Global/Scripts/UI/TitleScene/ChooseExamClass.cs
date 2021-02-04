using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace TitleUIScripts
{
    public class ChooseExamClass : MonoBehaviour
    {
        [SerializeField] private List<Button> examBtns;
        [SerializeField] private List<ExaminClass> examClass;
        [SerializeField] private Button backBtn;

        private void Start()
        {
            backBtn.onClick.AddListener(delegate { UI_Manager.Instance.Back(); });

        }

        public void EnterExam(int index)
        {
            ScoreManager.Instance.Initialize(examClass[index].fileName, examClass[index].operateCount);
            SceneLoader.Instance.LoadScene(examClass[index].examSceneIndex);
        }
    }
}