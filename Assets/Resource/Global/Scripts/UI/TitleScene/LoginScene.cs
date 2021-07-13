using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Sockets;
using GlobalSystem;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TitleUIScripts
{
    public class LoginScene : MonoBehaviour
    {
        [SerializeField] private GameObject managerPrefab;

        [SerializeField] private InputField schoolInputField;
        [SerializeField] private Text IDText;
        [SerializeField] private Text nameText;
        [SerializeField] private Button fbLoginBtn;
        [SerializeField] private Button loginBtn;
        [SerializeField] private Button setScoreBtn;

        [SerializeField] private GameObject setScene;

        [SerializeField] private List<ExaminClass> _examinClasses;


        private void Awake()
        {
            GameObject go = Instantiate(managerPrefab);


            setScene.SetActive(false);
        }

        private void Start()
        {
            IDText.text = string.Empty;
            nameText.text = string.Empty;
            for (int i = 0; i < _examinClasses.Count; i++)
            {
                ScoreManager.Instance.AddExamData(_examinClasses[i]);
            }
            fbLoginBtn.onClick.AddListener(delegate
            {
                TCPManager.Instance.LoginByDevice();
            });
            loginBtn.onClick.AddListener(delegate
            {
                ScoreManager.Instance.SetStudentID(IDText.text);
                ScoreManager.Instance.SetStudentName(nameText.text);
                ScoreManager.Instance.SetSchool("110-V13");
                UI_Manager.Instance.OpenPanel(1,0,false);
            });
        }

        private void Update()
        {
            if (TCPManager.Instance.isLogin)
            {
                IDText.text = TCPManager.Instance.UserID;
                nameText.text = TCPManager.Instance.UserName;
                loginBtn.gameObject.SetActive(true);
                fbLoginBtn.gameObject.SetActive(false);
            }
            else
            {
                IDText.text = string.Empty;
                nameText.text = string.Empty;
                loginBtn.gameObject.SetActive(false);
                fbLoginBtn.gameObject.SetActive(true);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}