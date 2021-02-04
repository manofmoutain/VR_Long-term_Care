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

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        public int GetCurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;
        public string GetCurrentSceneName => SceneManager.GetActiveScene().name;
    }
}