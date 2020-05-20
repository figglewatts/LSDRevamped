using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using LSDR.Audio;
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
using ResourceManager = Torii.Resource.ResourceManager;

namespace LSDR.Dream
{
    [CreateAssetMenu(menuName="System/DreamSystem")]
    public class DreamSystem : ScriptableObject
    {
        public ScenePicker DreamScene;
        public ScenePicker TitleScene;
        public Material SkyBackground;
        public JournalLoaderSystem JournalLoader;
        public LevelLoaderSystem LevelLoader;
        public LBDFastMeshSystem LBDLoader;
        public GameSaveSystem GameSave;
        public ControlSchemeLoaderSystem Control;
        public SettingsSystem SettingsSystem;
        public MusicSystem MusicSystem;
        public PauseSystem PauseSystem;
        public AudioClip LinkSound;
        public Dream CurrentDream { get; private set; }
        public DreamSequence CurrentSequence { get; private set; }
        public ToriiEvent OnReturnToTitle;
        public ToriiEvent OnLevelLoad;
        public ToriiEvent OnSongChange;
        [NonSerialized] public AudioSource MusicSource;
        [NonSerialized] public Transform Player;
        
        [NonSerialized] private bool _dreamIsEnding = false;
        [NonSerialized] private bool _canTransition = true;
        [NonSerialized] private bool _currentlyTransitioning = false;
        [NonSerialized] private Coroutine _endDreamTimer;
        [NonSerialized] private string _currentDreamPath;

        // one in every 6 links switches texture sets
        private const float CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING = 6;
        private const float MIN_SECONDS_IN_DREAM = 300;
        private const float MAX_SECONDS_IN_DREAM = 600;
        private const int FALLING_UPPER_PENALTY = -3;
        private const float FADE_OUT_SECS_REGULAR = 5;
        private const float FADE_OUT_SECS_FALL = 2.5f;
        private const float FADE_OUT_SECS_FORCE = 1;

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
            TextureSet = randomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);

            string dreamPath = GameSave.CurrentJournalSave.DayNumber == 1
                ? JournalLoader.Current.GetFirstDream()
                : JournalLoader.Current.GetDreamFromGraph(GameSave.CurrentJournalSave.LastGraphX,
                    GameSave.CurrentJournalSave.LastGraphY);
            _currentDreamPath = dreamPath;
            Dream dream = _serializer.Deserialize<Dream>(PathUtil.Combine(Application.streamingAssetsPath,
                dreamPath));
            BeginDream(dream);
        }

        public void BeginDream(Dream dream)
        {
            _forcedSpawnID = null;
            _canTransition = true;

            // start a timer to end the dream
            float secondsInDream = RandUtil.Float(MIN_SECONDS_IN_DREAM, MAX_SECONDS_IN_DREAM);
            _endDreamTimer = Coroutines.Instance.StartCoroutine(EndDreamAfterSeconds(secondsInDream));
            
            ToriiFader.Instance.FadeIn(Color.black, 3, () => Coroutines.Instance.StartCoroutine(LoadDream(dream)));
            CurrentSequence = new DreamSequence();
            CurrentSequence.Visited.Add(dream);
        }

        public void EndDream(bool fromFall = false)
        {
            if (_dreamIsEnding) return;
            
            commonEndDream();

            // penalise upper score if ending dream from falling
            if (fromFall)
            {
                CurrentSequence.UpperModifier += FALLING_UPPER_PENALTY;
                SettingsSystem.CanControlPlayer = false;
            }

            ToriiFader.Instance.FadeIn(Color.black, fromFall ? FADE_OUT_SECS_FALL : FADE_OUT_SECS_REGULAR, () =>
            {
                CurrentDream = null;
                _currentDreamPath = null;
                GameSave.CurrentJournalSave.SequenceData.Add(CurrentSequence);
                GameSave.Save();
                _dreamIsEnding = false;
                Coroutines.Instance.StartCoroutine(ReturnToTitle());
            });
        }

        /// <summary>
        /// End dream without advancing the day number or adding progress.
        /// </summary>
        public void ForceEndDream()
        {
            if (_dreamIsEnding) return;

            commonEndDream();
            
            ToriiFader.Instance.FadeIn(Color.black, FADE_OUT_SECS_FORCE, () =>
            {
                CurrentDream = null;
                _currentDreamPath = null;
                _dreamIsEnding = false;
                Coroutines.Instance.StartCoroutine(ReturnToTitle());
            });
        }

        public IEnumerator EndDreamAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            // don't end the dream if we happen to be linking when the timer expires
            while (_currentlyTransitioning)
            {
                yield return null;
            }

            // if the dream is already ending (perhaps we fell) then don't end it again
            if (_dreamIsEnding)
            {
                yield break;
            }
            
            EndDream();
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
            
            Debug.Log($"Linking to {dream.Name}");

            _currentlyTransitioning = true;
            
            SettingsSystem.CanControlPlayer = false;
            _forcedSpawnID = spawnPointID;

            // disable pausing to prevent throwing off timers etc
            PauseSystem.CanPause = false;

            if (playSound)
            {
                AudioPlayer.Instance.PlayClip(LinkSound, false, "SFX");
            }

            CurrentSequence.Visited.Add(dream);

            ToriiFader.Instance.FadeIn(fadeCol, 1F, () =>
            {
                // see if we should switch texture sets
                if (RandUtil.OneIn(CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING))
                {
                    TextureSet = randomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);
                }
                
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
                // choose a random dream that isn't the current dream
                dreamPath = RandUtil.RandomListElement(
                    JournalLoader.Current.LinkableDreams.Where(d => !d.Equals(_currentDreamPath)));
                Debug.Log($"Dream path: {dreamPath}, Current dream path: {_currentDreamPath}");
            }

            _currentDreamPath = dreamPath;
            Dream dream =
                _serializer.Deserialize<Dream>(PathUtil.Combine(Application.streamingAssetsPath, dreamPath));
            Transition(fadeCol, dream, playSound, spawnPointID);
        }

        public IEnumerator ReturnToTitle()
        {
            Debug.Log("Loading title screen");
            
            if (MusicSource != null && MusicSource.isPlaying)
            {
                MusicSource.Stop();
            }

            var asyncLoad = SceneManager.LoadSceneAsync(TitleScene.ScenePath);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            ResourceManager.ClearLifespan("scene");
            
            OnReturnToTitle.Raise();

            yield return null;

            ToriiCursor.Show();
            
            ToriiFader.Instance.FadeOut(1F);
        }

        public IEnumerator LoadDream(Dream dream)
        {
            Debug.Log($"Loading dream '{dream.Name}'");

            if (MusicSource != null && MusicSource.isPlaying)
            {
                MusicSource.Stop();
            }

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
            
            ResourceManager.ClearLifespan("scene");

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(DreamScene.ScenePath));

            CurrentDream = dream;
            
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
            SettingsSystem.CanMouseLook = true;
            
            OnLevelLoad.Raise();

            MusicSource = MusicSystem.PlayRandomSongFromDirectory(PathUtil.Combine(Application.streamingAssetsPath,
                JournalLoader.Current.MusicFolder));
            OnSongChange.Raise();

            // reenable pausing
            PauseSystem.CanPause = true;
            
            ToriiCursor.Hide();

            ToriiFader.Instance.FadeOut(1F, () => _currentlyTransitioning = false);
        }

        public void ApplyTextureSet(TextureSet textureSet)
        {
            // TODO: change texture set on non LBD materials
            if (CurrentDream != null)
            {
                LBDLoader.UseTIX(getTIXPathFromTextureSet(CurrentDream, textureSet));
            }
        }

        public void SkipSong()
        {
            MusicSystem.PlayRandomSongFromDirectory(MusicSource,
                PathUtil.Combine(Application.streamingAssetsPath, JournalLoader.Current.MusicFolder));
            OnSongChange.Raise();
        }

        private void commonEndDream()
        {
            _dreamIsEnding = true;
            _canTransition = false;
            
            // disable pausing to prevent throwing off timers etc
            PauseSystem.CanPause = false;

            Debug.Log("Ending dream");

            // make sure the dream end timer stops
            if (_endDreamTimer != null)
            {
                Coroutines.Instance.StopCoroutine(_endDreamTimer);
                _endDreamTimer = null;
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
                default:
                {
                    return PathUtil.Combine(dream.LBDFolder, "TEXA.TIX");
                }
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
            var allSpawns = entities.OfType<SpawnPoint>();
            if (!allSpawns.Any())
            {
                Debug.LogError("No spawn points in dream! Unable to spawn player.");
                return;
            }
            
            // handle a designated SpawnPoint being used
            if (!string.IsNullOrEmpty(_forcedSpawnID))
            {
                try
                {
                    SpawnPoint spawn = allSpawns.First(e => e.EntityID == _forcedSpawnID);
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
                var firstDaySpawns = allSpawns.Where(s => s.DayOneSpawn).ToList();
                if (firstDaySpawns.Any())
                {
                    RandUtil.RandomListElement(firstDaySpawns).Spawn();
                    return;
                }
            }

            // make sure we choose a spawn point that isn't a tunnel entrance
            var spawnPoints = allSpawns.Where(s => !s.TunnelEntrance).ToList();
            if (spawnPoints.Any())
            {
                RandUtil.RandomListElement(spawnPoints).Spawn();
                return;
            }
            
            // otherwise we'll have to spawn on a tunnel entrance... this shouldn't happen so warn the player
            Debug.LogWarning(
                "Unable to find a spawn point that isn't a tunnel entrance -- using a tunnel entrance instead.");
            RandUtil.RandomListElement(allSpawns).Spawn();
        }
    }
}
