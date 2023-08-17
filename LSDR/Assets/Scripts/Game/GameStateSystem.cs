using LSDR.SDK.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName = "System/GameStateSystem")]
    public class GameStateSystem : ScriptableObject
    {
        // scenes
        public SceneProperty ModLoadScene;
        public SceneProperty TitleScreenScene;

        // systems
        public ModLoaderSystem ModLoaderSystem;


        public void StartGame()
        {
            SceneManager.LoadScene(ModLoadScene);
        }

        public void ReturnToTitle()
        {
            SceneManager.LoadScene(TitleScreenScene);
        }
    }
}
