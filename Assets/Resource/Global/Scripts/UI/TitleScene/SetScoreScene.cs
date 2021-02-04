using System;
using System.Collections.Generic;
using Manager;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace TitleUIScripts
{
    public class SetScoreScene : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Button cancelBtn;
        [SerializeField] private Button confirmBtn;
        [SerializeField] private InputField setSchool;
        [SerializeField] private List<InputField> scoreList;
        [SerializeField] private List<Text> isToDo;
        [SerializeField] private List<Text> whatToDo;
        [SerializeField] RectTransform setScoreContentPanel;
        [SerializeField] private Slider slider;

        #endregion

        #region Unity events

        private void Start()
        {
            slider.value = 292;
            confirmBtn.onClick.AddListener(delegate { SaveScoreData(); });

            cancelBtn.onClick.AddListener(delegate { RecoverScoreData(); });
        }

        #endregion

        #region Private Methods

        private void InitializeScoreData()
        {
            setSchool.text = PlayerPrefs.GetString("School") != ""
                ? ScoreManager.Instance.School()
                : PlayerPrefs.GetString("School");


            for (int i = 0; i < ScoreManager.Instance.ListCount(); i++)
            {
                whatToDo[i].text = ScoreManager.Instance.WhatToDo(i);
                isToDo[i].text = ScoreManager.Instance.ToDo(i);
                scoreList[i].text = ScoreManager.Instance.TopicScore(i).ToString();
            }
        }

        private void OnEnable()
        {
            InitializeScoreData();
        }

        private void RecoverScoreData()
        {
            PlayerPrefs.DeleteAll();
            for (int i = 0; i < ScoreManager.Instance.ListCount(); i++)
            {
                scoreList[i].text = ScoreManager.Instance.TopicScore(i).ToString();
            }

            this.gameObject.SetActive(false);
        }

        private void SaveScoreData()
        {
            if (PlayerPrefs.GetString("School") != "")
            {
                ScoreManager.Instance.SetSchool(PlayerPrefs.GetString("School"));
            }
            else
            {
                ScoreManager.Instance.SetSchool(setSchool.text != "" ? ScoreManager.Instance.School() : setSchool.text);
            }

            for (int i = 0; i < ScoreManager.Instance.ListCount(); i++)
            {
                scoreList[i].text = PlayerPrefs.GetInt($"Score{i}") != ScoreManager.Instance.TopicScore(i)
                    ? ScoreManager.Instance.TopicScore(i).ToString()
                    : PlayerPrefs.GetInt($"Score{i}").ToString();
                PlayerPrefs.SetInt($"Score{i}", int.Parse(scoreList[i].text));
                this.gameObject.SetActive(false);
            }
        }

        private void ScoreNotOver99()
        {
            foreach (var inputField in scoreList)
            {
                if (inputField.text == "")
                {
                    inputField.text = "0";
                }
                else if (int.Parse(inputField.text) >= 100)
                {
                    inputField.text = "99";
                }
                else if (int.Parse(inputField.text) < 0)
                {
                    inputField.text = "0";
                }
            }
        }

        private void Update()
        {
            setScoreContentPanel.anchoredPosition = new Vector2(0, slider.value);
            ScoreNotOver99();
        }

        #endregion
    }
}