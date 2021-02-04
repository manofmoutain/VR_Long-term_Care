using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Manager
{
    public class UI_Manager : Monosingleton<UI_Manager>
    {
        //[Header("外部面板")]
        [Header("面板控制器")]
        public List<GameObject> panels;
        public Stack<GameObject> panelStacks = new Stack<GameObject>();

        public GameObject GetCurrentPanelOBJ
        {
            get
            {
                return panelStacks.Peek();     //回傳堆疊中最頂端的面板，且不移除
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            CleanPanel();
            OpenPanel(0,0, false);
        }

        /// <summary>
        /// 打開編號面板，上一個面板是否要關閉 
        /// false為不關閉，並保留於堆疊中(往下一層)
        /// true為關閉並於堆疊中刪除上一個面板
        /// </summary>
        /// <param name="panelIndex">面板編號</param>
        /// <param name="_time">延遲秒數</param>
        /// <param name="isHidePrevious">false為不關閉，並保留於堆疊中(往下一層)，true為關閉並於堆疊中刪除上一個面板</param>
        public void OpenPanel(int panelIndex, float _time, bool isHidePrevious = false )
        {
            if (isHidePrevious)      //如果上一個面版的狀態不是隱藏的
            {
                if (panelStacks.Peek() != null)   //如果從堆疊最上層取出的面板不是空的
                {
                    StartCoroutine(InactivePanel(_time));
                }
                StartCoroutine(ActivePanel(panelIndex, _time));
            }
            else          //如果上一個面版的狀態是隱藏的(堆疊中沒有面板)
            {
                StartCoroutine(ActivePanel(panelIndex, _time));
            }
        }

        public void Back()
        {
            if (panelStacks.Peek() != null)   //如果從堆疊最上層取出的面板不是空的
            {
                GetCurrentPanelOBJ.SetActive(false);     //關閉目前面板
                panelStacks.Pop();                                 //將目前面板從堆疊中刪除
            }
            GetCurrentPanelOBJ.SetActive(true);
        }

        /// <summary>
        /// 關閉所有面板，清除堆疊
        /// </summary>
        public void CleanPanel()
        {
            foreach (GameObject item in panels)
            {
                item.SetActive(false);
            }
            panelStacks.Clear();
        }

        /// <summary>
        /// 幾秒後打開面板
        /// </summary>
        /// <param name="index">面板編號</param>
        /// <param name="_time">延遲時間</param>
        /// <returns></returns>
        IEnumerator ActivePanel(int index , float _time)
        {
            yield return new WaitForSeconds(_time);
            panelStacks.Push(panels[index]);//將編號面板加入至面版堆疊的最上層
            GetCurrentPanelOBJ.SetActive(true);//打開目前面板
        }

        /// <summary>
        /// 幾秒後關閉面板
        /// </summary>
        /// <param name="_time">延遲時間</param>
        /// <returns></returns>
        IEnumerator InactivePanel(float _time)
        {
            yield return new WaitForSeconds(_time);
            GetCurrentPanelOBJ.SetActive(false);    //關閉目前面板
            panelStacks.Pop();   //移除最上層的面板
        }
    }

}


