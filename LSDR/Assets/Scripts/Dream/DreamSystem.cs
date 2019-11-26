using System.Collections;
using LSDR.Entities.Original;
using LSDR.Game;
using LSDR.UI;
using LSDR.Util;
using Torii.Coroutine;
using Torii.Event;
using Torii.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Torii.UI;

namespace LSDR.Dream
{
    [CreateAssetMenu(menuName="System/DreamSystem")]
    public class DreamSystem : ScriptableObject
    {
        public ScenePicker DreamScene;
        public GameObject LBDObjectPrefab;
        public Material SkyBackground;
        
        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        public void BeginDream()
        {
            Dream dream = _serializer.Deserialize<Dream>(IOUtil.PathCombine(Application.streamingAssetsPath,
                "levels/Original Dreams/Bright Moon Cottage.json"));
            BeginDream(dream);
        }

        public void BeginDream(Dream dream)
        {
            Fader.FadeIn(Color.black, 3, () => Coroutines.Instance.StartCoroutine(LoadDream(dream)));
        }

        public void ApplyEnvironment(DreamEnvironment environment)
        {
            RenderSettings.fogColor = Color.red;
            SkyBackground.SetColor("_FogColor", environment.FogColor);
            if (Camera.main != null) Camera.main.backgroundColor = environment.SkyColor;
            SkyBackground.SetColor("_SkyColor", environment.SkyColor);
            GameSettings.SubtractiveFog = environment.SubtractiveFog;
            
            // TODO: apply the rest of the environment
        }

        public IEnumerator LoadDream(Dream dream)
        {
            // load the scene in the background and wait until it's done
            var asyncLoad = SceneManager.LoadSceneAsync(DreamScene.ScenePath);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // then instantiate the LBD
            var lbdObject = Instantiate(LBDObjectPrefab);
            LBDTileMap tileMap = lbdObject.GetComponent<LBDTileMap>();
            tileMap.LBDFolder = dream.Level;
            tileMap.LBDWidth = dream.TileWidth;
            tileMap.Mode = dream.LegacyTileMode;
            tileMap.TIXFile = dream.Level + "/TEXA.TIX";
            tileMap.Spawn();
            
            ApplyEnvironment(dream.Environments[0]);
            
            Fader.FadeOut(3);
        }
    }
}
