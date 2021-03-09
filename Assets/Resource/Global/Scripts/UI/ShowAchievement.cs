using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Manager;
using OfficeOpenXml.ConditionalFormatting;
using TMPro;
using TMPro.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR.InteractionSystem;

public class ShowAchievement : MonoBehaviour
{
    [Header("")]
    [SerializeField] private RectTransform star;

    [Header("生成未完成的項目物件")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject achievementOBJ;
    [SerializeField] private TextMeshProUGUI score;

    [Header("重新開始按鈕")]
    [SerializeField] private Button restartBtn;
    [SerializeField] private Sprite normalRestartSprite;
    [SerializeField] private Sprite tochedRestartSprite;

    [Header("操作時間")] [SerializeField] private GameObject timePanel;
    [SerializeField] private TextMeshProUGUI timeText;

    private void Start()
    {
        restartBtn.GetComponent<Image>().sprite = normalRestartSprite;
        score.gameObject.SetActive(false);
        restartBtn.gameObject.SetActive(false);
        star.localScale = Vector3.zero;
        timePanel.SetActive(false);
        // ShowAchievementInEnd();
    }

    public void StopExam(int index)
    {
        ScoreManager.Instance.StopCountingTime();
        if (!ScoreManager.Instance.IsTimeLimit())
        {
            ScoreManager.Instance.DecreaseOperateSteps(index);
        }
    }

    /// <summary>
    /// 顯示未完成的項目
    /// </summary>
    public void ShowAchievementInEnd()
    {
        InvokeRepeating(nameof(ShowStar), 0.01f, 0.01f);
        StartCoroutine(Co_SpawnAchievement());
    }

    /// <summary>
    /// 動態顯示星星
    /// </summary>
    void ShowStar()
    {
        if (star.localScale.x < 1)
        {
            star.localScale = new Vector3(star.localScale.x + 0.01f, star.localScale.y + 0.01f, 1);
        }

        if (star.localScale.x > 1)
        {
            CancelInvoke(nameof(ShowStar));
        }
    }

    /// <summary>
    /// 執行緒_生成未完成項目
    /// </summary>
    /// <returns></returns>
    IEnumerator Co_SpawnAchievement()
    {
        yield return new WaitForSeconds(0.5f);
        int a = 0;
        for (int i = 0; i < ScoreManager.Instance.GetListCount(); i++)
        {
            if (i==ScoreManager.Instance.GetListCount()-1)
            {
                if (!ScoreManager.Instance.IsTimeLimit())
                {
                    ScoreManager.Instance.DecreaseOperateSteps(i);
                }
            }
            ScoreManager.Instance.GetIsDone(i);

            if (!ScoreManager.Instance.GetIsDone(i))
            {
                yield return new WaitForSeconds(0.5f);
                GameObject go = Instantiate(achievementOBJ, spawnPoint);
                RectTransform rectTransform = go.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, (-0.11f * a));
                go.GetComponentInChildren<TextMeshProUGUI>().text = ScoreManager.Instance.GetToDo(i);
                a++;
                ScoreManager.Instance.DecreaseScore(i);
            }

            score.text = ScoreManager.Instance.GetTotalScore().ToString();
        }

        score.gameObject.SetActive(true);
        restartBtn.gameObject.SetActive(true);
        timePanel.SetActive(true);
        timeText.text = $"{Mathf.CeilToInt(ScoreManager.Instance.GetTime / 60)}分{Mathf.CeilToInt(ScoreManager.Instance.GetTime % 60)}秒";
    }

    /// <summary>
    /// 重新開始
    /// </summary>
    public void Restart()
    {
        Destroy(FindObjectOfType<ScoreManager>().gameObject);
        Destroy(FindObjectOfType<Player>().gameObject);
        SceneLoader.Instance.LoadTitle();
    }

    /// <summary>
    /// 觸碰到按鈕換圖片
    /// </summary>
    public void TouchBtn()
    {
        restartBtn.GetComponent<Image>().sprite = tochedRestartSprite;
    }

    /// <summary>
    /// 離開按鈕還原圖片
    /// </summary>
    public void LeaveBtn()
    {
        restartBtn.GetComponent<Image>().sprite = normalRestartSprite;
    }
}