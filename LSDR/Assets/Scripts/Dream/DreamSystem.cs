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
        public ScenePicker TitleScene;
        public GameObject LBDObjectPrefab;
        public Material SkyBackground;
        public JournalLoaderSystem JournalLoader;
        public LevelLoaderSystem LevelLoader;
        public LBDFastMeshSystem LBDLoader;
        public GameSaveSystem GameSave;
        public ControlSchemeLoaderSystem Control;
        public SettingsSystem SettingsSystem;
        public AudioClip LinkSound;
        public PrefabPool LBDTilePool;
        public Dream CurrentDream { get; private set; }
        public DreamSequence CurrentSequence { get; private set; }
        public ToriiEvent OnReturnToTitle;

        private bool _dreamIsEnding = false;
        private bool _canTransition = true;

        // one in every 6 links switches texture sets
        private const float CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING = 6;

        public TextureSet TextureSet
        {
            get { return _textureSet; }
            set
            {
                _textureSet = value;
                ApplyTextureSet(value);
            }
        }

        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        private TextureSet _textureSet;
        
        [NonSerialized]
        private string _forcedSpawnID;

        public void OnEnable() { LevelLoader.OnLevelLoaded += spawnPlayerInDream; }

        public void BeginDream()
        {
            _forcedSpawnID = null;
            _canTransition = true;
            
            TextureSet = randomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);
            
            string dreamPath = GameSave.CurrentJournalSave.DayNumber == 1
                ? JournalLoader.Current.GetFirstDream()
                : JournalLoader.Current.GetLinkableDream();
            Dream dream = _serializer.Deserialize<Dream>(IOUtil.PathCombine(Application.streamingAssetsPath,
                dreamPath));
            BeginDream(dream);
        }

        public void BeginDream(Dream dream)
        {
            Fader.FadeIn(Color.black, 3, () => Coroutines.Instance.StartCoroutine(LoadDream(dream)));
            CurrentDream = dream;
            CurrentSequence = new DreamSequence();
            CurrentSequence.Visited.Add(dream);
        }

        public void EndDream()
        {
            _dreamIsEnding = true;
            _canTransition = false;
            
            Fader.FadeIn(Color.black, 5f, () =>
            {
                CurrentDream = null;
                GameSave.CurrentJournalSave.SequenceData.Add(CurrentSequence);
                GameSave.Save();
                _dreamIsEnding = false;
                Coroutines.Instance.StartCoroutine(ReturnToTitle());
            });
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
            if (!_canTransition) return;
            
            SettingsSystem.CanControlPlayer = false;
            _forcedSpawnID = spawnPointID;
            
            // TODO: disable/reenable pausing when transitioning

            if (playSound)
            {
                Audio.Instance.PlayClip(LinkSound, "SFX");
            }

            LBDTilePool.ReturnAll();
            
            CurrentSequence.Visited.Add(dream);

            // see if we should switch texture sets
            if (RandUtil.OneIn(CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING))
            {
                TextureSet = randomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);
            }

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

        public IEnumerator ReturnToTitle()
        {
            Debug.Log("Loading title screen");

            var asyncLoad = SceneManager.LoadSceneAsync(TitleScene.ScenePath);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            OnReturnToTitle.Raise();

            yield return null;
            
            Fader.FadeOut(1F);
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

        public void ApplyTextureSet(TextureSet textureSet)
        {
            // TODO: change texture set on non LBD materials
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

        private TextureSet randomTextureSetFromDayNumber(int dayNumber)
        {
            // 4 texture sets, add a new one into the mix every 10 days -- hence the mod 41 here
            // (41 instead of 40, as day isn't zero indexed -- it begins at 1!)
            int dayNumWithinBounds = dayNumber % 41;

            if (dayNumWithinBounds <= 10)
            {
                // just normal
                return TextureSet.Normal;
            }
            else if (dayNumWithinBounds <= 20)
            {
                // add kanji in
                return RandUtil.From(TextureSet.Normal, TextureSet.Kanji);
            }
            else if (dayNumWithinBounds <= 30)
            {
                // add downer in
                return RandUtil.From(TextureSet.Normal, TextureSet.Kanji, TextureSet.Downer);
            }
            else if (dayNumWithinBounds <= 40)
            {
                // full choice!
                return RandUtil.RandomEnum<TextureSet>();
            }
            
            // TODO: randomly introduce the glitch texture set

            // shouldn't get here due to the mod 41, but we need a default return otherwise C# will moan!
            return TextureSet.Normal;
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
            
            // spawn in a first day-designated spawn if it's the first day
            if (GameSave.CurrentJournalSave.DayNumber == 1)
            {
                var firstDaySpawns = entities.OfType<SpawnPoint>().Where(s => s.DayOneSpawn);
                if (firstDaySpawns.Any())
                {
                    RandUtil.RandomListElement(firstDaySpawns).Spawn();
                    return;
                }
            }
            
            // make sure we choose a spawn point that isn't a tunnel entrance
            RandUtil.RandomListElement(entities.OfType<SpawnPoint>().Where(s => !s.TunnelEntrance)).Spawn();
        }
    }
}
