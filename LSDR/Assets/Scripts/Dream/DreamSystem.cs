using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LSDR.Audio;
using LSDR.Entities.Dream;
using LSDR.Entities.Player;
using LSDR.Game;
using LSDR.InputManagement;
using LSDR.SDK;
using LSDR.SDK.Audio;
using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using LSDR.SDK.Entities;
using LSDR.SDK.Util;
using LSDR.SDK.Visual;
using LSDR.Visual;
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
using UnityEngine.Video;

namespace LSDR.Dream
{
    [CreateAssetMenu(menuName = "System/DreamSystem")]
    public class DreamSystem : ScriptableObject, IDreamController
    {
        protected const float CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING = 2;
        protected const float MIN_SECONDS_IN_DREAM = 60;
        protected const float MAX_SECONDS_IN_DREAM = 240;
        protected const int FALLING_UPPER_PENALTY = -9;
        protected const float FADE_OUT_SECS_REGULAR = 3;
        protected const float FADE_OUT_SECS_FALL = 2.5f;
        protected const float FADE_OUT_SECS_FORCE = 1;
        protected const float MIN_SECONDS_IN_FLASHBACK = 8f;
        protected const float MAX_SECONDS_IN_FLASHBACK = 16f;

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
        public ToriiEvent OnBeginDream;
        [NonSerialized] public bool ReturningFromDream = false;

        [NonSerialized] public Action OnDreamTimeout;

        protected readonly ToriiSerializer _serializer = new ToriiSerializer();
        [NonSerialized] protected bool _canTransition = true;
        [NonSerialized] protected bool _currentlyTransitioning;

        [NonSerialized] protected bool _dreamIsEnding;
        [NonSerialized] protected Coroutine _endDreamTimer;
        [NonSerialized] protected Coroutine _flashbackCoroutine;

        [NonSerialized] protected string _forcedSpawnID;
        [NonSerialized] protected float _lastPlayerYRotation;
        [NonSerialized] protected bool _spawnedInAtLeastOnce; // true if we've been in a dream yet
        [NonSerialized] public GameObject Player;
        [NonSerialized] protected SDK.Data.Dream _nextDream = null;
        [NonSerialized] protected DreamEnvironment _currentEnvironment;
        [NonSerialized] protected TimeSince _timeInCurrentFlashback;
        [NonSerialized] protected (Vector3, float)? _flashbackEventSpawn = null;
        [NonSerialized] protected int _flashbackDisabledForDays = 0;
        [NonSerialized] protected bool _dreamStretching = false;
        [NonSerialized] protected bool _playingVideo = false;
        [NonSerialized] protected DreamEnvironmentEffects _environmentEffects;

        [field: NonSerialized] public SDK.Data.Dream CurrentDream { get; protected set; }
        [field: NonSerialized] public GameObject CurrentDreamInstance { get; protected set; }
        [field: NonSerialized] public DreamSequence CurrentSequence { get; protected set; }

        public int CurrentDay => GameSave.CurrentJournalSave.DayNumber;

        public bool InDream => CurrentDream != null;
        public bool InFlashback { get; protected set; }

        public bool CanFlashback =>
            _flashbackDisabledForDays <= 0 && GameSave.CurrentJournalSave.HasEnoughDataForFlashback();

        public void EndDream(bool fromFall = false)
        {
            if (_dreamIsEnding || _playingVideo) return;

            // penalise upper score if ending dream from falling
            if (fromFall)
            {
                var pitch = RandUtil.RandomArrayElement(new[] { 0.25f, 0.5f });
                AudioPlayer.Instance.PlayClip(LinkSound, loop: false, pitch: pitch, mixerGroup: "SFX");
                CurrentSequence?.LogGraphContributionFromArea(dynamicness: 0, FALLING_UPPER_PENALTY);
                SettingsSystem.CanControlPlayer = false;
            }
            else
                SettingsSystem.PlayerGravity = false;

            if (!InFlashback)
            {
                GameSave.CurrentJournalSave.IncrementDayNumberWithSequence(CurrentSequence);
                GameSave.Save();
                if (_flashbackDisabledForDays > 0) _flashbackDisabledForDays--;
            }

            ToriiFader.Instance.FadeIn(InFlashback ? Color.white : fadeColorFromCurrentSequence(),
                fromFall ? FADE_OUT_SECS_FALL : FADE_OUT_SECS_REGULAR, () =>
                {
                    CurrentDream = null;
                    _dreamIsEnding = false;
                    Coroutines.Instance.StartCoroutine(ReturnToTitle());
                });

            commonEndDream();
        }

        public void SetCanControlPlayer(bool state)
        {
            SettingsSystem.CanControlPlayer = state;
        }

        public DreamJournal GetCurrentJournal()
        {
            return SettingsSystem.CurrentJournal;
        }

        public List<SDK.Data.Dream> GetDreamsFromJournal()
        {
            return SettingsSystem.CurrentJournal.Dreams;
        }

        public void OverrideDreamInstance(GameObject dreamInstance)
        {
            CurrentDreamInstance = dreamInstance;
        }

        public void LogGraphContributionFromArea(int dynamicness, int upperness)
        {
            if (CurrentSequence == null)
            {
                Debug.LogWarning("Attempting to log contribution with no sequence");
                return;
            }
            CurrentSequence.LogGraphContributionFromArea(dynamicness, upperness);
        }

        public void LogGraphContributionFromEntity(int dynamicness, int upperness, BaseEntity sourceEntity)
        {
            if (CurrentSequence == null)
            {
                Debug.LogWarning("Attempting to log contribution with no sequence");
                return;
            }
            CurrentSequence.LogGraphContributionFromEntity(dynamicness, upperness, Player.transform, sourceEntity,
                CurrentDream.Name);
        }

        public void SetNextLinkDream(SDK.Data.Dream dream, string spawnPointID = null)
        {
            SetNextSpawn(spawnPointID);
            _nextDream = dream;
        }

        public void DisableFlashbackForDays(int days)
        {
            _flashbackDisabledForDays = days;
        }

        public void Transition(Color fadeCol,
            SDK.Data.Dream dream = null,
            bool playSound = true,
            bool lockControls = true,
            string spawnPointID = null)
        {
            if (!_canTransition) return;
            _canTransition = false;

            if (CurrentSequence == null) Debug.LogWarning("transitioning dreams with no sequence");

            // prefer given dream, but fall back to _nextDream if not given (and it is set)
            if (dream == null && _nextDream != null)
            {
                dream = _nextDream;

                // set _nextDream to null as we'll be transitioning out of the TriggerLink that caused this
                _nextDream = null;
            }

            if (dream == null) dream = SettingsSystem.CurrentJournal.GetLinkable(CurrentDream);

            Debug.Log($"Linking to {dream.Name}");

            _currentlyTransitioning = true;

            SettingsSystem.CanControlPlayer = false;
            OverrideNextSpawn(spawnPointID);
            if (lockControls)
            {
                Player.GetComponent<PlayerMovement>().SetInputLock();
            }

            // save the last Y rotation for the player, so we can set it back to this when we load the next dream
            _lastPlayerYRotation = Player.transform.rotation.eulerAngles.y;

            // disable pausing to prevent throwing off timers etc
            PauseSystem.CanPause = false;

            if (dream != CurrentDream) MusicSystem.StopSong();

            if (playSound)
            {
                var pitch = RandUtil.RandomArrayElement(new[] { 0.25f, 0.5f, 1, 2 });
                AudioPlayer.Instance.PlayClip(LinkSound, loop: false, pitch: pitch, mixerGroup: "SFX");
            }

            CurrentSequence?.Visited.Add(dream);

            ToriiFader.Instance.FadeIn(fadeCol, duration: 1F, () =>
            {
                // see if we should switch texture sets
                if (RandUtil.OneIn(CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING))
                {
                    TextureSetter.Instance.SetRandomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);
                }

                // if we're locking controls then we're going through a tunnel and want to update player rotation
                Coroutines.Instance.StartCoroutine(LoadDream(dream, !lockControls));
            });
        }

        /// <summary>
        ///     Transition with fade colour determined by the current graph position.
        /// </summary>
        /// <param name="dream">Dream to transition to, if null then randomly selected from pool of eligible.</param>
        /// <param name="playSound">Whether to play sound or not when transitioning. Default true.</param>
        /// <param name="lockControls">Whether to lock controls while linking. Default false.</param>
        /// <param name="spawnPointID">If non-null then should be the ID of a spawn point to spawn at in the dream.</param>
        public void Transition(SDK.Data.Dream dream = null, bool playSound = true, bool lockControls = false,
            string spawnPointID = null)
        {
            Transition(fadeColorFromCurrentSequence(), dream, playSound, lockControls, spawnPointID);
        }

        public void Initialise() { DevConsole.Register(this); }

        public void BeginFlashback()
        {
            if (CurrentSequence != null) return;

            OnBeginDream.Raise(); // controls UI buttons

            TextureSetter.Instance.SetRandomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);
            var randomEventStream = GameSave.CurrentJournalSave.SequenceData
                                            .SelectMany(sd => sd.EntityGraphContributions);
            ToriiFader.Instance.FadeIn(Color.black, duration: 3,
                () =>
                {
                    _flashbackCoroutine = Coroutines.Instance.StartCoroutine(flashback(randomEventStream));

                    // start a timer to end the dream
                    float secondsInDream = RandUtil.Float(MIN_SECONDS_IN_DREAM, MAX_SECONDS_IN_DREAM);
                    _endDreamTimer = Coroutines.Instance.StartCoroutine(EndDreamAfterSeconds(secondsInDream));
                });
        }

        protected IEnumerator flashback(IEnumerable<GraphContribution> pastEvents)
        {
            InFlashback = true;

            Debug.Log("loading first flashback");

            // transition to first area
            var firstEvent = pastEvents.OrderBy(_ => RandUtil.Int()).First();
            SDK.Data.Dream dream;
            if (firstEvent.Dream == null)
            {
                // might be old version of save format, choose random dream
                dream = RandUtil.RandomListElement(SettingsSystem.CurrentJournal.Dreams);
            }
            else
            {
                dream = SettingsSystem.CurrentJournal.Dreams.FirstOrDefault(d =>
                    d.Name.Equals(firstEvent.Dream, StringComparison.InvariantCulture));
            }
            _flashbackEventSpawn = new(firstEvent.PlayerPosition, firstEvent.PlayerYRotation);
            yield return Coroutines.Instance.StartCoroutine(LoadDream(dream, transitioning: true, onLoadFinished: () =>
            {
                EntityIndex.Instance.Get(firstEvent.EntityID).SetActive(true);
            }));

            while (true)
            {
                Debug.Log("waiting for next flashback");
                float secondsInFlashback = RandUtil.Float(MIN_SECONDS_IN_FLASHBACK, MAX_SECONDS_IN_FLASHBACK);
                _timeInCurrentFlashback = 0;
                while (_timeInCurrentFlashback < secondsInFlashback)
                {
                    yield return null;
                }

                var flashbackEvent = pastEvents.OrderBy(_ => RandUtil.Int()).First();

                // transition to flashback event
                if (flashbackEvent.Dream == null)
                {
                    // might be old version of save format, choose random dream
                    dream = RandUtil.RandomListElement(SettingsSystem.CurrentJournal.Dreams);
                }
                else
                {
                    dream = SettingsSystem.CurrentJournal.Dreams.FirstOrDefault(d =>
                        d.Name.Equals(flashbackEvent.Dream, StringComparison.InvariantCulture));
                }
                _flashbackEventSpawn = new(flashbackEvent.PlayerPosition, flashbackEvent.PlayerYRotation);
                Debug.Log(_flashbackEventSpawn);

                Debug.Log("loading next flashback");
                SetCanControlPlayer(false);
                ToriiFader.Instance.FadeIn(Color.white, duration: 2F, () =>
                {
                    // see if we should switch texture sets
                    if (RandUtil.OneIn(CHANCE_TO_SWITCH_TEXTURES_WHEN_LINKING))
                    {
                        TextureSetter.Instance.SetRandomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);
                    }

                    // if we're locking controls then we're going through a tunnel and want to update player rotation
                    Coroutines.Instance.StartCoroutine(LoadDream(dream, transitioning: true,
                        onLoadFinished: () =>
                        {
                            SetCanControlPlayer(true);
                            EntityIndex.Instance.Get(flashbackEvent.EntityID).SetActive(true);
                            _timeInCurrentFlashback = 0;
                        }));
                });
            }
        }

        public void BeginDream()
        {
            if (CurrentSequence != null) return;

            OnBeginDream.Raise();

            CurrentSequence = new DreamSequence();

            TextureSetter.Instance.SetRandomTextureSetFromDayNumber(GameSave.CurrentJournalSave.DayNumber);

            if (SettingsSystem.CurrentJournal.HasSpecialDay(GameSave.CurrentJournalSave.DayNumber,
                    out AbstractSpecialDay specialDay))
            {
                if (SettingsSystem.Settings.SpecialDaysEnabled || specialDay.Always)
                {
                    ToriiFader.Instance.FadeIn(Color.black, duration: 3, () =>
                    {
                        specialDay.HandleDay(GameSave.CurrentJournalSave.DayNumber);
                        LogGraphContributionFromArea(specialDay.Contribution.Dynamic, specialDay.Contribution.Upper);
                    });
                    return;
                }
            }

            BeginPlayableDream(getRandomDream());
        }

        public void BeginPlayableDream(SDK.Data.Dream dream)
        {
            SetNextSpawn(null); // don't force a spawn point
            _canTransition = true;

            // start a timer to end the dream
            float secondsInDream = RandUtil.Float(MIN_SECONDS_IN_DREAM, MAX_SECONDS_IN_DREAM);
            _endDreamTimer = Coroutines.Instance.StartCoroutine(EndDreamAfterSeconds(secondsInDream));

            ToriiFader.Instance.FadeIn(Color.black, duration: 3, () => Coroutines.Instance.StartCoroutine(
                LoadDream(dream, transitioning: false)));
            CurrentSequence.Visited.Add(dream);
        }

        /// <summary>
        /// Set the next spawn point ID. Doesn't check for null etc.
        /// </summary>
        /// <param name="spawnPointID"></param>
        public void SetNextSpawn(string spawnPointID)
        {
            _forcedSpawnID = spawnPointID;
        }

        /// <summary>
        /// Override the next spawn point ID. Will only set if given value is not null or empty (so won't set to null/empty).
        /// </summary>
        /// <param name="spawnPointID"></param>
        public void OverrideNextSpawn(string spawnPointID)
        {
            if (!string.IsNullOrWhiteSpace(spawnPointID)) _forcedSpawnID = spawnPointID;
        }

        public void PlayVideo(VideoClip videoClip, Color fadeInColor)
        {
            _playingVideo = true;
            MusicSystem.StopSong();

            // make sure flashback stops
            if (_flashbackCoroutine != null)
            {
                Coroutines.Instance.StopCoroutine(_flashbackCoroutine);
                _flashbackCoroutine = null;
            }

            // make sure the dream end timer stops
            if (_endDreamTimer != null)
            {
                Coroutines.Instance.StopCoroutine(_endDreamTimer);
                _endDreamTimer = null;
            }

            FadeManager.Managed.FadeIn(fadeInColor, 3f, () =>
            {
                SceneManager.LoadScene("video_dream");
                FadeManager.Managed.FadeOut(Color.black, 1f, () =>
                {
                    VideoSpecialDayControl control = FindObjectOfType<VideoSpecialDayControl>();
                    control.BeginVideoClip(videoClip);
                }, 1);
            });
        }

        public void VideoFinished()
        {
            _playingVideo = false;
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

        public void ApplyEnvironment()
        {
            if (_currentEnvironment == null) return;
            _currentEnvironment.Apply(SettingsSystem.Settings.LongDrawDistance, _environmentEffects);
        }

        public void StretchDream(float amount, float timeSeconds)
        {
            if (CurrentDreamInstance == null)
            {
                Debug.LogWarning("Cannot stretch dream when not instantiated");
            }

            if (_dreamStretching) return;

            Coroutines.Instance.StartCoroutine(stretchDreamCoroutine(amount, timeSeconds));
        }

        protected IEnumerator stretchDreamCoroutine(float amount, float timeSeconds)
        {
            _dreamStretching = true;
            float t = 0;
            float startYScale = CurrentDreamInstance.transform.localScale.y;
            float targetYScale = startYScale * amount;
            while (t < timeSeconds)
            {
                CurrentDreamInstance.transform.localScale = new Vector3(CurrentDreamInstance.transform.localScale.x,
                    Mathf.Lerp(startYScale, targetYScale, t / timeSeconds),
                    CurrentDreamInstance.transform.localScale.z);
                t += Time.deltaTime;
                yield return null;
            }
            CurrentDreamInstance.transform.localScale = new Vector3(CurrentDreamInstance.transform.localScale.x,
                targetYScale,
                CurrentDreamInstance.transform.localScale.z);
            _dreamStretching = false;
        }

        public IEnumerator EndDreamAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            // don't end the dream if we happen to be linking when the timer expires
            while (_currentlyTransitioning) yield return null;

            // if the dream is already ending (perhaps we fell) then don't end it again
            if (_dreamIsEnding) yield break;

            OnDreamTimeout?.Invoke();

            EndDream();
        }

        public IEnumerator ReturnToTitle()
        {
            Debug.Log("Loading title screen");

            TextureSetter.Instance.DeregisterAllMaterials();
            EntityIndex.Instance.DeregisterAllEntities();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(TitleScene.ScenePath);
            while (!asyncLoad.isDone) yield return null;

            ResourceManager.ClearLifespan("scene");

            OnReturnToTitle.Raise();

            yield return null;

            ToriiCursor.Show();

            ToriiFader.Instance.FadeOut(duration: 1F);
        }

        public IEnumerator LoadDream(SDK.Data.Dream dream, bool transitioning,
            [CanBeNull] Action onLoadFinished = null)
        {
            Debug.Log($"Loading dream '{dream.Name}'");

            // disable falling so we don't go through level geometry
            SettingsSystem.PlayerGravity = false;

            // unload the last scene (if it existed)
            if (CurrentDream != null)
            {
                Destroy(CurrentDreamInstance);
                _environmentEffects.ClearEffects();
            }

            _playingVideo = false;

            // unhook events
            OnDreamTimeout = null;

            TextureSetter.Instance.DeregisterAllMaterials();
            EntityIndex.Instance.DeregisterAllEntities();

            bool loadingSameDream = CurrentDream == dream;
            CurrentDream = dream;

            if (!SceneManager.GetActiveScene().name.Equals("load_mod", StringComparison.InvariantCulture))
            {
                Debug.Log("Loading dream scene...");
                AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync("load_mod");
                yield return loadSceneOp;
            }

            yield return Resources.UnloadUnusedAssets();

            // reroll song style, update song library, switch to next song based on graph position
            MusicSystem.OriginalSongLibrary.DreamNumber = SettingsSystem.CurrentJournal.GetDreamIndex(CurrentDream);
            MusicSystem.CurrentSongLibrary = CurrentDream.SongLibrary;
            int songNumber = GameSave.CurrentJournalSave.DayNumber == 1
                ? 2
                : (GameSave.CurrentJournalSave.LastGraphY + 9) * GraphSpawnMap.GRAPH_SIZE +
                  (GameSave.CurrentJournalSave.LastGraphX + 9);
            if (!loadingSameDream)
            {
                MusicSystem.SetSongStyle(
                    (SongStyle)((GameSave.CurrentJournalSave.DayNumber - 1) % (int)SongStyle.COUNT));
                MusicSystem.NextSong(songNumber);
            }

            Debug.Log("Registering entities...");
            Player = GameObject.FindWithTag("Player");
            if (Player == null) Debug.LogError("Unable to find player in scene!");
            registerCommonEntities();

            _environmentEffects = FindObjectOfType<DreamEnvironmentEffects>();

            Player.GetComponent<PlayerMovement>().StopExternalInput();
            Player.GetComponent<PlayerRotation>().StopExternalInput();

            OnLevelPreLoad.Raise();

            Debug.Log("Instantiating dream prefab...");
            var prefabRequest = SettingsSystem.CurrentMod.GetDreamPrefabAsync(CurrentDream);
            yield return prefabRequest;

            CurrentDreamInstance = Instantiate((GameObject)prefabRequest.asset);

            ResourceManager.ClearLifespan("scene");

            EntityIndex.Instance.AllRegistered();

            // if we're not transitioning (via a link) then set the orientation
            // because we're beginning the dream
            spawnPlayerInDream(transitioning == false);
            _forcedSpawnID = null; // reset forced spawn ID as we're now in different dream so last value is irrelevant
            _flashbackEventSpawn = null; // reset flashback spawn

            Debug.Log("Choosing environment...");
            if (!InFlashback)
            {
                _currentEnvironment = dream.ChooseEnvironment(GameSave.CurrentJournalSave.DayNumber);
                ApplyEnvironment();
            }
            else
            {
                _currentEnvironment = dream.RandomEnvironment();
                ApplyEnvironment();
            }

            SettingsSystem.CanControlPlayer = true;
            SettingsSystem.CanMouseLook = true;
            SettingsSystem.PlayerGravity = true;

            Debug.Log("Restoring player input etc...");
            PlayerMovement playerMovement = Player.GetComponent<PlayerMovement>();
            playerMovement.ClearInputLock();
            playerMovement.OnNewDream();
            PlayerCameraRotation playerCameraRotation = Player.GetComponent<PlayerCameraRotation>();
            playerCameraRotation.SetUsingExternalInput(false);
            PlayerRotation playerRotation = Player.GetComponent<PlayerRotation>();
            playerRotation.StopExternalInput();
            _canTransition = true;

            // tell grey man spawner we're in a new dream
            Player.GetComponent<GreymanSpawner>().OnNewDream();

            // enable flashback effect if we're in a flashback
            Player.transform.GetChild(0).GetComponent<FlashbackImageEffect>().enabled = InFlashback;

            OnLevelLoad.Raise();
            OnSongChange.Raise();

            // reenable pausing
            PauseSystem.CanPause = true;

            ToriiCursor.Hide();

            onLoadFinished?.Invoke();

            ToriiFader.Instance.FadeOut(duration: 1F, () =>
            {
                playerMovement.CanLink = true;
                _currentlyTransitioning = false;
            });
        }

        protected Color fadeColorFromCurrentSequence()
        {
            if (!InDream) return Color.black;

            if (CurrentSequence == null)
            {
                Debug.LogWarning("attempted to get fade color without current sequence");
                return RandUtil.RandColor();
            }

            Vector2Int graphPos = CurrentSequence.EvaluateGraphPosition();
            switch (graphPos)
            {
                case var p when p.x <= -4 && p.y >= 1:
                    return Color.green;
                case var p when p.x <= 3 && p.y >= 4:
                    return Color.blue;
                case var p when p.x >= 4 && p.y >= 4:
                    return Color.magenta;
                case var p when p.x >= 4 && p.y >= -3:
                    return Color.black;
                case var p when p.x <= -4 && p.y <= -4:
                    return Color.yellow;
                case var p when p.x <= 3 && p.y <= -4:
                    return Color.red;
                default:
                    return Color.white;
            }
        }

        protected SDK.Data.Dream getRandomDream()
        {
            if (GameSave.CurrentJournalSave.DayNumber == 1)
                return SettingsSystem.CurrentJournal.GetFirstDream();
            return SettingsSystem.CurrentJournal.GetDreamFromGraph(GameSave.CurrentJournalSave.LastGraphX,
                GameSave.CurrentJournalSave.LastGraphY);
        }

        protected void registerCommonEntities()
        {
            EntityIndex.Instance.Register("__player", Player, force: true);

            GameObject camera = GameObject.FindWithTag("MainCamera");
            if (camera == null) Debug.LogWarning("Unable to find MainCamera in scene");
            EntityIndex.Instance.Register("__camera", camera, force: true);
        }

        // stuff that gets called when ending a dream whether or not we are manually quitting or naturally ending
        protected void commonEndDream()
        {
            _dreamIsEnding = true;
            _canTransition = false;
            CurrentSequence = null;
            InFlashback = false;
            ReturningFromDream = true;

            // save the last Y rotation for the player, so we can set it back to this when we start the next dream
            if (Player != null) _lastPlayerYRotation = Player.transform.rotation.eulerAngles.y;

            Player = null;

            // disable pausing to prevent throwing off timers etc
            PauseSystem.CanPause = false;

            MusicSystem.StopSong();

            Debug.Log("Ending dream");

            // make sure flashback stops
            if (_flashbackCoroutine != null)
            {
                Coroutines.Instance.StopCoroutine(_flashbackCoroutine);
                _flashbackCoroutine = null;
            }

            // make sure the dream end timer stops
            if (_endDreamTimer != null)
            {
                Coroutines.Instance.StopCoroutine(_endDreamTimer);
                _endDreamTimer = null;
            }
        }

        protected void spawnPlayerInDream(bool setOrientation = false)
        {
            Debug.Log("Spawning player...");
            try
            {
                // see if we're spawning in a flashback
                if (_flashbackEventSpawn != null)
                {
                    CharacterController controller = Player.GetComponent<CharacterController>();
                    float skinWidth = 0f;
                    if (controller) skinWidth = controller.skinWidth;

                    // spawn the player facing the entity
                    var (spawnPos, spawnAngle) = _flashbackEventSpawn.Value;
                    Player.transform.position =
                        new Vector3(spawnPos.x + 0.01f, spawnPos.y + skinWidth,
                            spawnPos.z - 0.01f);
                    Player.transform.rotation = Quaternion.Euler(x: 0, spawnAngle, z: 0);

                    Debug.Log($"spawning player at pos: {spawnPos} - dream: {CurrentDream.Name}");

                    return;
                }

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
            finally
            {
                // make sure we only try to do this if we've already dreamed (i.e. we have a last Y rotation)
                // otherwise, we'll be overwriting the Y rotation of the spawn point chosen
                if (_spawnedInAtLeastOnce && !setOrientation)
                {
                    // set the last Y rotation to what it was before
                    Player.transform.rotation = Quaternion.Euler(Player.transform.rotation.x, _lastPlayerYRotation,
                        Player.transform.rotation.z);
                }
                _spawnedInAtLeastOnce = true;
            }
        }

#region Console Commands

        [Console]
        public void SkipSong()
        {
            if (CurrentDream == null) return;

            MusicSystem.NextSong(RandUtil.Int(100));
        }

        [Console]
        public void SetDay(int dayNumber)
        {
            Debug.Log($"Setting day number to: {dayNumber}");
            GameSave.CurrentJournalSave.SetDayNumber(dayNumber);
        }

        [Console]
        public void ListEntities()
        {
            Debug.Log("Listing entities:");
            int entityCount = 0;
            foreach (var entityId in EntityIndex.Instance.GetEntityIDs())
            {
                Debug.Log(entityId);
                entityCount++;
            }
            Debug.Log($"Total: {entityCount} entities");

        }

        [Console]
        public void TelePlayer(string id)
        {
            var destObj = EntityIndex.Instance.Get(id);
            Player.transform.position = destObj.transform.position;
        }

        [Console]
        public void SetSongStyle(int style)
        {
            if (CurrentDream == null) return;
            if (style < 0 || style > (int)SongStyle.COUNT)
            {
                Debug.LogError($"Invalid song style '{style}', needs to be 0-6");
            }

            SongStyle songStyle = (SongStyle)style;
            Debug.Log($"Switching song style to: {songStyle.ToString()}");
            MusicSystem.SetSongStyle(songStyle);
        }

        [Console]
        public void ListDreamEnvironments()
        {
            if (CurrentDream == null) return;
            Debug.Log($"Dream has {CurrentDream.Environments.Environments.Count} environments.");
        }

        [Console]
        public void ApplyDreamEnvironment(int idx)
        {
            if (CurrentDream == null) return;
            if (idx < 0 || idx >= CurrentDream.Environments.Environments.Count)
            {
                Debug.LogError(
                    $"Unable to apply environment '{idx}', dream only has {CurrentDream.Environments.Environments.Count}");
                return;
            }

            CurrentDream.Environments.Environments[idx]
                        .Apply(SettingsSystem.Settings.LongDrawDistance, _environmentEffects);
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
        public void ListDreams()
        {
            Debug.Log("Listing dreams:");
            foreach (var dream in SettingsSystem.CurrentJournal.Dreams)
            {
                Debug.Log(dream.Name);
            }
            Debug.Log($"Total: {SettingsSystem.CurrentJournal.Dreams.Count} dreams");
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
                BeginPlayableDream(dream);
            else
                Transition(Color.black, dream, playSound: false);
        }

        [Console]
        public void LoadDreamSpawn(string dreamName, string spawnId)
        {
            SDK.Data.Dream dream = SettingsSystem.CurrentJournal.Dreams.FirstOrDefault(d =>
                d.Name.Equals(dreamName, StringComparison.InvariantCulture));
            if (dream == null)
            {
                Debug.LogError($"Unable to load dream: no such dream '{dreamName}' in " +
                               $"journal '{SettingsSystem.CurrentJournal.Name}'");
            }

            if (CurrentDream == null)
                BeginPlayableDream(dream);
            else
                Transition(Color.black, dream, playSound: false, spawnPointID: spawnId);
        }

#endregion
    }
}
