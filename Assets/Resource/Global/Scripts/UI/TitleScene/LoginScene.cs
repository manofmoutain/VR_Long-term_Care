using System;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TitleUIScripts
{
    public class LoginScene : MonoBehaviour
    {
        [SerializeField] private GameObject manager;
        [SerializeField] private int operateListCount;

        [SerializeField] private Text lesson;
        [SerializeField] private InputField schoolInputField;
        [SerializeField] private InputField setStudentID;
        [SerializeField] private InputField setStudentName;
        [SerializeField] private Button loginBtn;
        [SerializeField] private Button setScoreBtn;

        [SerializeField] private GameObject setScene;


        private void Awake()
        {
            GameObject go = Instantiate(manager);
            go.AddComponent<ScoreManager>();
            // go.GetComponent<ScoreManager>().Initialize(operateListCount);


            setScene.SetActive(false);

            // lesson.text = ScoreManager.Instance.Lesson();
        }

        private void Start()
        {
            loginBtn.onClick.AddListener(delegate
            {
                ScoreManager.Instance.SetStudentID(setStudentID.text);
                ScoreManager.Instance.SetStudentName(setStudentName.text);
                ScoreManager.Instance.SetSchool(schoolInputField.text);
                // SceneLoader.Instance.LoadScene(1);
                UI_Manager.Instance.OpenPanel(1,0,false);
            });
            // setScoreBtn.onClick.AddListener(delegate { setScene.SetActive(true); });
            // lesson.text = ScoreManager.Instance.Lesson();
            // schoolInputField.text = ScoreManager.Instance.School();
        }

        private void Update()
        {
            if (setStudentID.text!="" && setStudentName.text!="")
            {
                loginBtn.interactable = true;
            }
            else
            {
                loginBtn.interactable = false;
            }
        }
    }
}