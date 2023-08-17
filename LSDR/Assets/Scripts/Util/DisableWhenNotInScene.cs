using Torii.UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSDR.Util
{
    public class DisableWhenNotInScene : MonoBehaviour
    {
        public ScenePicker Scene;

        public void Start() { disableIfInScene(); }

        public void OnEnable() { disableIfInScene(); }

        private void disableIfInScene()
        {
            if (SceneManager.GetActiveScene().name != Scene)
            {
                gameObject.SetActive(value: false);
            }
        }
    }
}
