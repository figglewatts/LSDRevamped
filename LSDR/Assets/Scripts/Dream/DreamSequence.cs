using System.Collections.Generic;
using System.Linq;
using LSDR.SDK.Data;
using LSDR.SDK.Entities;
using Newtonsoft.Json;
using UnityEngine;

namespace LSDR.Dream
{
    /// <summary>
    ///     A DreamSequence is a sequence of dreams visited in a day of play.
    /// </summary>
    [JsonObject]
    public class DreamSequence
    {
        /// <summary>
        ///     The list of visited dreams this day.
        /// </summary>
        public readonly List<VisitedDream> Visited = new List<VisitedDream>();

        [JsonProperty] protected List<GraphContribution> _areaGraphContributions = new List<GraphContribution>();
        [JsonProperty] protected List<GraphContribution> _entityGraphContributions = new List<GraphContribution>();
        [JsonProperty] protected int _dayNumber;

        [JsonIgnore] public IEnumerable<GraphContribution> EntityGraphContributions => _entityGraphContributions;

        public void LogGraphContributionFromArea(int dynamicness, int upperness)
        {
            _areaGraphContributions.Add(new GraphContribution(dynamicness, upperness));
        }

        public void LogGraphContributionFromEntity(int dynamicness, int upperness, Transform playerTransform,
            BaseEntity sourceEntity, string dream)
        {
            _entityGraphContributions.Add(new GraphContribution(dynamicness, upperness, playerTransform,
                sourceEntity, dream));
        }

        public void SetDayNumber(int dayNumber)
        {
            _dayNumber = dayNumber;
        }

        public Vector2Int EvaluateGraphPosition()
        {
            Vector2Int areaContribution = correctOutOfGraphBounds(evaluateContributionList(_areaGraphContributions));
            Vector2Int entityContribution = _entityGraphContributions.Count == 0
                ? areaContribution
                : correctOutOfGraphBounds(evaluateContributionList(_entityGraphContributions));

            Vector2Int unclampedContribution = (areaContribution + entityContribution) / 2;
            return correctOutOfGraphBounds(unclampedContribution);
        }

        protected Vector2Int correctOutOfGraphBounds(Vector2Int contribution)
        {
            int correctComponent(int component)
            {
                if (component < -9) return 9;
                if (component > 9) return -9;
                return component;
            }

            return new Vector2Int(correctComponent(contribution.x), correctComponent(contribution.y));
        }

        protected Vector2Int evaluateContributionList(List<GraphContribution> contributions)
        {
            if (contributions.Count == 0)
            {
                return Vector2Int.zero;
            }

            // the average, plus a third of the last contribution
            GraphContribution last = contributions.Last();
            return new Vector2Int(
                contributions.Sum(c => c.Dynamic) / contributions.Count + last.Dynamic / 3,
                contributions.Sum(c => c.Upper) / contributions.Count + last.Upper / 3);
        }
    }
}
