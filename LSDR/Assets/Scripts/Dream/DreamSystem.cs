using System;
using System.Collections;
using LSDR.Entities;
using LSDR.Entities.Dream;
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
using Torii.Util;

namespace LSDR.Dream
{
    [CreateAssetMenu(menuName="System/DreamSystem")]
    public class DreamSystem : ScriptableObject
    {
        public ScenePicker DreamScene;
        public GameObject LBDObjectPrefab;
        public Material SkyBackground;
        public JournalLoaderSystem JournalLoader;
        public LevelLoaderSystem LevelLoader;
        public SettingsSystem SettingsSystem;

        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        public void OnEnable() { LevelLoader.OnLevelLoaded += spawnPlayerInDream; }

        public void BeginDream()
        {
            // TODO: spawn in first dream if it's the first day
            
            var randomDream = JournalLoader.Current.GetLinkableDream();
            Dream dream = _serializer.Deserialize<Dream>(IOUtil.PathCombine(Application.streamingAssetsPath,
                randomDream));
            BeginDream(dream);
        }

        public void BeginDream(Dream dream)
        {
            Fader.FadeIn(Color.black, 3, () => Coroutines.Instance.StartCoroutine(LoadDream(dream)));
        }

        public void ApplyEnvironment(DreamEnvironment environment)
        {
            RenderSettings.fogColor = environment.FogColor;
            SkyBackground.SetColor("_FogColor", environment.FogColor);
            if (Camera.main != null) Camera.main.backgroundColor = environment.SkyColor;
            SkyBackground.SetColor("_SkyColor", environment.SkyColor);
            Shader.SetGlobalInt("_SubtractiveFog", environment.SubtractiveFog ? 1 : 0);
            
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

            // then instantiate the LBD if it has one
            if (dream.Type == DreamType.Legacy)
            {
                var lbdObject = Instantiate(LBDObjectPrefab);
                LBDTileMap tileMap = lbdObject.GetComponent<LBDTileMap>();
                tileMap.LBDFolder = dream.LBDFolder;
                tileMap.LBDWidth = dream.TileWidth;
                tileMap.Mode = dream.LegacyTileMode;
                tileMap.TIXFile = dream.LBDFolder + "/TEXA.TIX"; // TODO: Texture sets for LBDs
                tileMap.Spawn();
            }

            // load the manifest if it has one
            if (!string.IsNullOrEmpty(dream.Level))
            {
                string levelPath = PathUtil.Combine(Application.streamingAssetsPath, dream.Level);
                LevelLoader.LoadLevel(levelPath);
            }
            
            ApplyEnvironment(dream.RandomEnvironment());
            
            Debug.Log(Camera.main);

            Fader.FadeOut(3);
        }

        private void spawnPlayerInDream(LevelEntities entities)
        {
            // TODO: if first day, spawn in first day spawn
            RandUtil.RandomListElement(entities.OfType<SpawnPoint>()).Spawn();
        }
    }
}
