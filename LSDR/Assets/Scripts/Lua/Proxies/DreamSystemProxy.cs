using LSDR.Dream;
using LSDR.SDK;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class DreamSystemProxy : AbstractLuaProxy<DreamSystem>
    {
        [MoonSharpHidden]
        public DreamSystemProxy(DreamSystem target) : base(target) { }

        public SDK.Data.Dream CurrentDream => _target.CurrentDream;
        public int CurrentDreamIndex => _target.SettingsSystem.CurrentJournal.GetDreamIndex(CurrentDream);

        public int DayNumber => _target.GameSave.CurrentJournalSave.DayNumber;

        // EndDream()

        // LogGraphContributionFromArea/FromEntity()

        // Transition()

        // SwitchTextures()

        public void SetTextureSet(TextureSet set)
        {
            // TODO
        }
    }
}
