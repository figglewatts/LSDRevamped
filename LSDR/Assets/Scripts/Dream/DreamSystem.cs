using System;
using System.Collections;
using System.Linq;
using InControl;
using LSDR.Entities;
using LSDR.Entities.Dream;
using LSDR.Entities.Original;
using LSDR.Game;
using LSDR.InputManagement;
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
        public ControlSchemeLoaderSystem Control;
        public SettingsSystem SettingsSystem;
        public AudioClip LinkSound;
        public PrefabPool LBDTilePool;
        public Dream CurrentDream { get; private set; }

        public TextureSet TextureSet
        {
            get { return _textureSet; }
            set
            {
                _textureSet = value;
                ChangeTextureSet(value);
            }
        }

        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        private TextureSet _textureSet;
        
        [NonSerialized]
        private string _forcedSpawnID;

        public void OnEnable() { LevelLoader.OnLevelLoaded += spawnPlayerInDream; }

        public void BeginDream()
        {
            // TODO: spawn in first spawn point if it's the first day
            _forcedSpawnID = null;
            
            // TODO: choose texture set randomly based on which day we're on
            TextureSet = RandUtil.RandomEnum<TextureSet>();

            // TODO: spawn in dream based on graph if not first day
            string dreamPath = true ? JournalLoader.Current.GetFirstDream() : JournalLoader.Current.GetLinkableDream();
            Dream dream = _serializer.Deserialize<Dream>(IOUtil.PathCombine(Application.streamingAssetsPath,
                dreamPath));
            BeginDream(dream);
        }

        public void BeginDream(Dream dream)
        {
            Fader.FadeIn(Color.black, 3, () => Coroutines.Instance.StartCoroutine(LoadDream(dream)));
            CurrentDream = dream;
        }

        public void EndDream()
        {
            // TODO: only set to null when fade completes
            CurrentDream = null;
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
            
            // TODO: occasionally switch texture sets when transitioning

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
            Debug.Log($"Loading dream '{dream.Name}'");

            string currentScene = SceneManager.GetActiveScene().name;
            
            // load the scene in the background and wait until it's done
            var asyncLoad = SceneManager.LoadSceneAsync(DreamScene.ScenePath, LoadSceneMode.Additive);
            //asyncLoad.allowSceneActivation = false;
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            var asyncUnload = SceneManager.UnloadSceneAsync(currentScene);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(DreamScene.ScenePath));

            // then instantiate the LBD if it has one
            if (dream.Type == DreamType.Legacy)
            {
                LBDLoader.LoadLBD(dream.LBDFolder, dream.LegacyTileMode, dream.TileWidth);
                LBDLoader.UseTIX(getTIXPathFromTextureSet(dream, TextureSet));
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

        public void ChangeTextureSet(TextureSet textureSet)
        {
            // TODO: change texture set on non LBD materials
            // TODO: glitch texture set
            if (CurrentDream != null)
            {
                LBDLoader.UseTIX(getTIXPathFromTextureSet(CurrentDream, textureSet));
            }
        }

        private string getTIXPathFromTextureSet(Dream dream, TextureSet textureSet)
        {
            if (string.IsNullOrWhiteSpace(dream.LBDFolder))
            {
                Debug.LogError("Could not get TIX path from dream, dream did not have LBD folder");
                return null;
            }
            
            switch (textureSet)
            {
                case TextureSet.Kanji:
                {
                    return PathUtil.Combine(dream.LBDFolder, "TEXB.TIX");
                }
                case TextureSet.Downer:
                {
                    return PathUtil.Combine(dream.LBDFolder, "TEXC.TIX");
                }
                case TextureSet.Upper:
                {
                    return PathUtil.Combine(dream.LBDFolder, "TEXD.TIX");
                }
                default:
                {
                    return PathUtil.Combine(dream.LBDFolder, "TEXA.TIX");
                }
            }
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
            
            // make sure we choose a spawn point that isn't a tunnel entrance
            RandUtil.RandomListElement(entities.OfType<SpawnPoint>().Where(s => !s.TunnelEntrance)).Spawn();
        }
    }
}
