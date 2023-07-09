using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class DreamArea : BaseTrigger
    {
        public GraphContribution GraphContribution = new GraphContribution(9, 0);

        protected override Color _editorColour { get; } = new Color(1, 1, 1, 0.2f);

        protected override void onTrigger(Collider player)
        {
            DreamControlManager.Managed.LogGraphContributionFromArea(GraphContribution.Dynamic,
                GraphContribution.Upper);
        }
    }
}
