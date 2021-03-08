﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Manager;
using OfficeOpenXml.ConditionalFormatting;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR.InteractionSystem;

public class ShowAchievement : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private RectTransform star;
    [SerializeField] private GameObject achievementOBJ;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Button restartBtn;

    private void Start()
    {
        score.gameObject.SetActive(false);
        restartBtn.gameObject.SetActive(false);
        star.localScale = Vector3.zero;
        // restartBtn.onClick.AddListener(delegate { SceneLoader.Instance.LoadTitle(); });
        // ShowAchievementInEnd();
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
            ScoreManager.Instance.GetIsDone(i);

            if (!ScoreManager.Instance.GetIsDone(i))
            {
                yield return new WaitForSeconds(0.5f);
                GameObject go = Instantiate(achievementOBJ, spawnPoint);
                RectTransform rectTransform = go.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, (-0.11f * (a)));
                go.GetComponentInChildren<TextMeshProUGUI>().text = ScoreManager.Instance.GetToDo(i);
                a++;
            }

            score.text = ScoreManager.Instance.GetTotalScore().ToString();
        }

        score.gameObject.SetActive(true);
        restartBtn.gameObject.SetActive(true);
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
}