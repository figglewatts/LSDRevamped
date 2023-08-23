using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class DreamArea : BaseTrigger
    {
        public GraphContribution GraphContribution = new GraphContribution(dynamicness: 9, upperness: 0);

        protected override Color _editorColour { get; } = new Color(r: 1, g: 1, b: 1, a: 0.2f);

        protected override void onTrigger(Collider player)
        {
            DreamControlManager.Managed.LogGraphContributionFromArea(GraphContribution.Dynamic,
                GraphContribution.Upper);
        }

        protected override void onTriggerExit(Collider player) { }
    }
}
