using LSDR.Dream;
using LSDR.SDK;
using LSDR.SDK.Visual;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua.Proxies
{
    public class DreamSystemProxy : AbstractLuaProxy<DreamSystem>
    {
        [MoonSharpHidden]
        public DreamSystemProxy(DreamSystem target) : base(target) { }

        public SDK.Data.Dream CurrentDream => _target.CurrentDream;
        public int CurrentDreamIndex => _target.SettingsSystem.CurrentJournal.GetDreamIndex(CurrentDream);

        public int DayNumber => _target.GameSave.CurrentJournalSave.DayNumber;

        protected static Color? _transitionColor;
        protected static bool _transitionSound = true;
        protected static string _transitionSpawnID;
        protected static SDK.Data.Dream _transitionDream;

        public void SetNextTransitionColor(Color? color)
        {
            _transitionColor = color;
        }

        public void SetTransitionSounds(bool sounds)
        {
            _transitionSound = sounds;
        }

        public void SetNextTransitionSpawnID(string spawnID)
        {
            _transitionSpawnID = spawnID;
        }

        public void SetNextTransitionDream(SDK.Data.Dream dream)
        {
            _transitionDream = dream;
        }

        public void LogGraphContributionFromEntity(int dynamicness, int upperness)
        {
            _target.LogGraphContributionFromEntity(dynamicness, upperness);
        }

        public void TransitionToDream()
        {
            if (_transitionColor == null)
            {
                _target.Transition(_transitionDream, _transitionSound, lockControls: false, _transitionSpawnID);
            }
            else
            {
                _target.Transition(_transitionColor.Value, _transitionDream, _transitionSound, lockControls: false,
                    _transitionSpawnID);
            }
            _transitionColor = null;
            _transitionSpawnID = null;
            _transitionDream = null;
        }

        public void EndDream()
        {
            _transitionColor = null;
            _transitionSpawnID = null;
            _transitionDream = null;
            _target.EndDream();
        }

        public void SetTextureSet(TextureSet set)
        {
            TextureSetter.Instance.TextureSet = set;
        }
    }
}
