using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LSDR.Audio;
using LSDR.Entities.Player;
using LSDR.Game;
using LSDR.InputManagement;
using LSDR.SDK;
using LSDR.SDK.DreamControl;
using LSDR.SDK.Entities;
using LSDR.SDK.Util;
using LSDR.SDK.Visual;
using Torii.Audio;
using Torii.Console;
using Torii.Coroutine;
using Torii.Event;
using Torii.Resource;
using Torii.Serialization;
using Torii.UI;
using Torii.UnityEditor;
using Torii.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSDR.Dream
{
    [CreateAssetMenu(menuName = "System/DreamSystem")]
    public class DreamSystem : ScriptableObject, IDreamController
    {
        // one in every 6 links switches texture sets
        protected const float CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING = 6;
        protected const float MIN_SECONDS_IN_DREAM = 90;
        protected const float MAX_SECONDS_IN_DREAM = 600;
        protected const int FALLING_UPPER_PENALTY = -3;
        protected const float FADE_OUT_SECS_REGULAR = 5;
        protected const float FADE_OUT_SECS_FALL = 2.5f;
        protected const float FADE_OUT_SECS_FORCE = 1;
        public ScenePicker TitleScene;
        public GameSaveSystem GameSave;
        public ControlSchemeLoaderSystem Control;
        public SettingsSystem SettingsSystem;
        public MusicSystem MusicSystem;
        public PauseSystem PauseSystem;
        public AudioClip LinkSound;
        public ToriiEvent OnReturnToTitle;
        public ToriiEvent OnLevelLoad;
        public ToriiEvent OnSongChange;
        public ToriiEvent OnPlayerSpawned;
        public ToriiEvent OnLevelPreLoad;

        protected readonly ToriiSerializer _serializer = new ToriiSerializer();
        [NonSerialized] protected bool _canTransition = true;
        [NonSerialized] protected bool _currentlyTransitioning;

        [NonSerialized] protected bool _dreamIsEnding;
        [NonSerialized] protected Coroutine _endDreamTimer;

        [NonSerialized] protected string _forcedSpawnID;
        [NonSerialized] public AudioSource MusicSource;
        [NonSerialized] public GameObject Player;
        public SDK.Data.Dream CurrentDream { get; protected set; }
        public GameObject CurrentDreamInstance { get; protected set; }
        public DreamSequence CurrentSequence { get; protected set; }

        public void EndDream(bool fromFall = false)
        {
            if (_dreamIsEnding) return;

            // penalise upper score if ending dream from falling
            if (fromFall)
            {
                CurrentSequence.UpperModifier += FALLING_UPPER_PENALTY;
                SettingsSystem.CanControlPlayer = false;
            }
            else
                SettingsSystem.PlayerGravity = false;

            ToriiFader.Instance.FadeIn(Color.black, fromFall ? FADE_OUT_SECS_FALL : FADE_OUT_SECS_REGULAR, () =>
            {
                CurrentDream = null;
                GameSave.CurrentJournalSave.SequenceData.Add(CurrentSequence);
                GameSave.Save();
                _dreamIsEnding = false;
                Coroutines.Instance.StartCoroutine(ReturnToTitle());
            });

            commonEndDream();
        }

        public void Transition(Color fadeCol,
            SDK.Data.Dream dream = null,
            bool playSound = true,
            bool lockControls = false,
            string spawnPointID = null)
        {
            if (!_canTransition) return;
            _canTransition = false;

            if (dream == null) dream = SettingsSystem.CurrentJournal.GetLinkable(CurrentDream);

            Debug.Log($"Linking to {dream.Name}");

            _currentlyTransitioning = true;

            SettingsSystem.CanControlPlayer = false;
            _forcedSpawnID = spawnPointID;
            if (lockControls) Player.GetComponent<PlayerMovement>().SetInputLock();

            // disable pausing to prevent throwing off timers etc
            PauseSystem.CanPause = false;

            if (playSound) AudioPlayer.Instance.PlayClip(LinkSound, false, "SFX");

            CurrentSequence.Visited.Add(dream);

            ToriiFader.Instance.FadeIn(fadeCol, 1F, () =>
            {
                // see if we should switch texture sets
                if (RandUtil.OneIn(CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING))
                    TextureSetter.Instance.SetRandomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);

                // if we're locking controls then we're going through a tunnel and want to update player rotation
                Coroutines.Instance.StartCoroutine(LoadDream(dream, !lockControls));
            });
        }

        public void Initialise() { DevConsole.Register(this); }

        public void BeginDream()
        {
            TextureSetter.Instance.SetRandomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);

            BeginDream(getRandomDream());
        }

        public void BeginDream(SDK.Data.Dream dream)
        {
            _forcedSpawnID = null;
            _canTransition = true;

            // start a timer to end the dream
            float secondsInDream = RandUtil.Float(MIN_SECONDS_IN_DREAM, MAX_SECONDS_IN_DREAM);
            _endDreamTimer = Coroutines.Instance.StartCoroutine(EndDreamAfterSeconds(secondsInDream));

            ToriiFader.Instance.FadeIn(Color.black, 3, () => Coroutines.Instance.StartCoroutine(
                LoadDream(dream, false)));
            CurrentSequence = new DreamSequence();
            CurrentSequence.Visited.Add(dream);
        }

        /// <summary>
        ///     End dream without advancing the day number or adding progress.
        /// </summary>
        [Console]
        public void ForceEndDream()
        {
            if (_dreamIsEnding) return;

            commonEndDream();

            ToriiFader.Instance.FadeIn(Color.black, FADE_OUT_SECS_FORCE, () =>
            {
                CurrentDream = null;
                _dreamIsEnding = false;
                Coroutines.Instance.StartCoroutine(ReturnToTitle());
            });
        }

        public IEnumerator EndDreamAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            // don't end the dream if we happen to be linking when the timer expires
            while (_currentlyTransitioning) yield return null;

            // if the dream is already ending (perhaps we fell) then don't end it again
            if (_dreamIsEnding) yield break;

            EndDream();
        }

        public IEnumerator ReturnToTitle()
        {
            Debug.Log("Loading title screen");

            if (MusicSource != null && MusicSource.isPlaying) MusicSource.Stop();

            TextureSetter.Instance.DeregisterAllMaterials();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(TitleScene.ScenePath);
            while (!asyncLoad.isDone) yield return null;

            ResourceManager.ClearLifespan("scene");

            OnReturnToTitle.Raise();

            yield return null;

            ToriiCursor.Show();

            ToriiFader.Instance.FadeOut(1F);
        }

        public IEnumerator LoadDream(SDK.Data.Dream dream, bool transitioning)
        {
            Debug.Log($"Loading dream '{dream.Name}'");

            // disable falling so we don't go through level geometry
            SettingsSystem.PlayerGravity = false;

            // unload the last scene (if it existed)
            if (CurrentDream != null)
            {
                Destroy(CurrentDreamInstance);
                TextureSetter.Instance.DeregisterAllMaterials();
                EntityIndex.Instance.DeregisterAllEntities();
            }

            CurrentDream = dream;

            if (!SceneManager.GetActiveScene().name.Equals("load_mod", StringComparison.InvariantCulture))
            {
                AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync("load_mod");
                yield return loadSceneOp;
            }

            if (MusicSource != null && MusicSource.isPlaying) MusicSource.Stop();

            OnLevelPreLoad.Raise();

            CurrentDreamInstance = Instantiate(dream.DreamPrefab);
            yield return null;

            ResourceManager.ClearLifespan("scene");

            Player = GameObject.FindWithTag("Player");
            if (Player == null) Debug.LogError("Unable to find player in scene!");
            registerCommonEntities();

            // if we're not transitioning (via a link) then set the orientation
            // because we're beginning the dream
            spawnPlayerInDream(transitioning == false);

            dream.ChooseEnvironment(GameSave.CurrentJournalSave.DayNumber).Apply();

            SettingsSystem.CanControlPlayer = true;
            SettingsSystem.CanMouseLook = true;
            SettingsSystem.PlayerGravity = true;

            PlayerMovement playerMovement = Player.GetComponent<PlayerMovement>();
            playerMovement.CanLink = true;
            playerMovement.ClearInputLock();
            _canTransition = true;

            OnLevelLoad.Raise();
            OnSongChange.Raise();

            // reenable pausing
            PauseSystem.CanPause = true;

            ToriiCursor.Hide();

            ToriiFader.Instance.FadeOut(1F, () => _currentlyTransitioning = false);
        }

        [Console]
        public void SkipSong()
        {
            if (CurrentDream == null) return;

            OnSongChange.Raise();
        }

        protected SDK.Data.Dream getRandomDream()
        {
            if (GameSave.CurrentJournalSave.DayNumber == 1)
                return SettingsSystem.CurrentJournal.GetFirstDream();
            return SettingsSystem.CurrentJournal.GetDreamFromGraph(GameSave.CurrentJournalSave.LastGraphX,
                GameSave.CurrentJournalSave.LastGraphY);
        }

        protected void registerCommonEntities() { EntityIndex.Instance.Register("__player", Player); }

        protected void commonEndDream()
        {
            _dreamIsEnding = true;
            _canTransition = false;

            Player = null;

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

        protected void spawnPlayerInDream(bool setOrientation = false)
        {
            PlayerSpawn[] allSpawns = CurrentDreamInstance.GetComponentsInChildren<PlayerSpawn>();
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
                    PlayerSpawn spawn =
                        allSpawns.First(e => e.ID.Equals(_forcedSpawnID, StringComparison.InvariantCulture));
                    spawn.Spawn(Player.transform, setOrientation);
                    return;
                }
                catch (InvalidOperationException)
                {
                    Debug.LogError($"Unable to find SpawnPoint with ID '{_forcedSpawnID}'");
                    throw;
                }
            }

            // spawn in a first day-designated spawn if it's the first day
            if (GameSave.CurrentJournalSave.DayNumber == 1)
            {
                List<PlayerSpawn> firstDaySpawns = allSpawns.Where(s => s.DayOneSpawn).ToList();
                if (firstDaySpawns.Any())
                {
                    RandUtil.RandomListElement(firstDaySpawns).Spawn(Player.transform, setOrientation);
                    return;
                }
            }

            // make sure we choose a spawn point that isn't a tunnel entrance
            List<PlayerSpawn> spawnPoints = allSpawns.Where(s => !s.TunnelEntrance).ToList();
            if (spawnPoints.Any())
            {
                RandUtil.RandomListElement(spawnPoints).Spawn(Player.transform, setOrientation);
                return;
            }

            // otherwise we'll have to spawn on a tunnel entrance... this shouldn't happen so warn the player
            Debug.LogWarning(
                "Unable to find a spawn point that isn't a tunnel entrance -- using a tunnel entrance instead.");
            RandUtil.RandomListElement(allSpawns).Spawn(Player.transform, setOrientation);
        }

#region Console Commands

        [Console]
        public void ListDreamEnvironments()
        {
            if (CurrentDream == null) return;
            Debug.Log($"Dream has {CurrentDream.Environments.Count} environments.");
        }

        [Console]
        public void ApplyDreamEnvironment(int idx)
        {
            if (CurrentDream == null) return;
            if (idx < 0 || idx >= CurrentDream.Environments.Count)
            {
                Debug.LogError(
                    $"Unable to apply environment '{idx}', dream only has {CurrentDream.Environments.Count}");
                return;
            }

            CurrentDream.Environments[idx].Apply();
        }

        [Console]
        public void SwitchTextures(int idx)
        {
            switch (idx)
            {
                case 0:
                    TextureSetter.Instance.TextureSet = TextureSet.Normal;
                    break;
                case 1:
                    TextureSetter.Instance.TextureSet = TextureSet.Kanji;
                    break;
                case 2:
                    TextureSetter.Instance.TextureSet = TextureSet.Downer;
                    break;
                case 3:
                    TextureSetter.Instance.TextureSet = TextureSet.Upper;
                    break;
            }
        }

        [Console]
        public void LoadDream(string dreamName)
        {
            SDK.Data.Dream dream = SettingsSystem.CurrentJournal.Dreams.FirstOrDefault(d =>
                d.Name.Equals(dreamName, StringComparison.InvariantCulture));
            if (dream == null)
            {
                Debug.LogError($"Unable to load dream: no such dream '{dreamName}' in " +
                               $"journal '{SettingsSystem.CurrentJournal.Name}'");
            }

            if (CurrentDream == null)
                BeginDream(dream);
            else
                Transition(Color.black, dream, false);
        }

        [Console]
        public void PlaySong(string songPath)
        {
            ToriiAudioClip clip = ResourceManager.Load<ToriiAudioClip>(PathUtil.Combine(
                Application.streamingAssetsPath,
                "music",
                songPath), "scene");
            MusicSource.Stop();
            MusicSource.clip = clip;
            MusicSource.loop = true;
            MusicSource.Play();
        }

#endregion
    }
}
