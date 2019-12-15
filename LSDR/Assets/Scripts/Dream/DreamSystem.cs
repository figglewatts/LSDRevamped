using System;
using System.Collections;
using System.Linq;
using LSDR.Entities;
using LSDR.Entities.Dream;
using LSDR.Entities.Original;
using LSDR.Game;
using LSDR.IO;
using LSDR.UI;
using LSDR.Util;
using Torii.Audio;
using Torii.Coroutine;
using Torii.Event;
using Torii.Pooling;
using Torii.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Torii.UI;
using Torii.UnityEditor;
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
        public LBDFastMeshSystem LBDLoader;
        public SettingsSystem SettingsSystem;
        public AudioClip LinkSound;
        public PrefabPool LBDTilePool;

        private readonly ToriiSerializer _serializer = new ToriiSerializer();
        private string _forcedSpawnID;

        public void OnEnable() { LevelLoader.OnLevelLoaded += spawnPlayerInDream; }

        public void BeginDream()
        {
            // TODO: spawn in first dream if it's the first day
            // TODO: spawn in first spawn point if it's the first day
            
            //var randomDream = JournalLoader.Current.GetLinkableDream();
            Dream dream = _serializer.Deserialize<Dream>(IOUtil.PathCombine(Application.streamingAssetsPath,
                "levels/Original Dreams/Kyoto.json"));
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

        public void Transition(Color fadeCol, Dream dream, bool playSound = true, string spawnPointID = null)
        {
            SettingsSystem.CanControlPlayer = false;
            _forcedSpawnID = spawnPointID;
            
            // TODO: disable/reenable pausing when transitioning

            if (playSound)
            {
                Audio.Instance.PlayClip(LinkSound, "SFX");
            }

            LBDTilePool.ReturnAll();

            Fader.FadeIn(fadeCol, 1F, () =>
            {
                Coroutines.Instance.StartCoroutine(LoadDream(dream));
            });
        }

        public void Transition(Color fadeCol,
            string dreamPath = null,
            bool playSound = true,
            string spawnPointID = null)
        {
            if (string.IsNullOrEmpty(dreamPath))
            {
                dreamPath = JournalLoader.Current.GetLinkableDream();
            }

            Dream dream =
                _serializer.Deserialize<Dream>(IOUtil.PathCombine(Application.streamingAssetsPath, dreamPath));
            Transition(fadeCol, dream, playSound, spawnPointID);
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
                LBDLoader.LoadLBD(dream.LBDFolder, dream.LegacyTileMode, dream.TileWidth);
                LBDLoader.UseTIX(dream.LBDFolder + "/TEXA.TIX"); // TODO: Texture sets for LBDs
            }

            // load the manifest if it has one
            if (!string.IsNullOrEmpty(dream.Level))
            {
                string levelPath = PathUtil.Combine(Application.streamingAssetsPath, dream.Level);
                LevelLoader.LoadLevel(levelPath);
            }
            
            ApplyEnvironment(dream.RandomEnvironment());

            SettingsSystem.CanControlPlayer = true;
            
            // TODO: disable/reenable pausing when transitioning

            Fader.FadeOut(1F);
        }

        private void spawnPlayerInDream(LevelEntities entities)
        {
            // handle a designated SpawnPoint being used
            if (!string.IsNullOrEmpty(_forcedSpawnID))
            {
                try
                {
                    SpawnPoint spawn = entities.OfType<SpawnPoint>().First(e => e.EntityID == _forcedSpawnID);
                    spawn.Spawn();
                    return;
                }
                catch (InvalidOperationException e)
                {
                    Debug.LogError($"Unable to find SpawnPoint with ID '{_forcedSpawnID}'");
                    throw;
                }
            }
            
            // TODO: handle the first day spawn
            
            RandUtil.RandomListElement(entities.OfType<SpawnPoint>()).Spawn();
        }
    }
}
