using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class SceneLoader : Monosingleton<SceneLoader>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 回到第一個場景Title
        /// </summary>
        public void LoadTitle()
        {
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// 讀取場景
        /// </summary>
        /// <param name="index"></param>
        public void LoadScene(int index)
        {
            StartCoroutine(Co_LoadScene(index));
        }

        IEnumerator Co_LoadScene(int index)
        {
            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadScene(index);
        }

        public int GetCurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;
        public string GetCurrentSceneName => SceneManager.GetActiveScene().name;
    }
}