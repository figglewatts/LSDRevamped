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
            Vector2Int areaContribution = evaluateContributionList(_areaGraphContributions);
            Vector2Int entityContribution = evaluateContributionList(_entityGraphContributions);
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

            Vector2Int averageContribution = new Vector2Int(
                (int)contributions.Average(c => c.Dynamic),
                (int)contributions.Average(c => c.Upper));
            Vector2Int lastContributionEvaluation = evaluateContribution(contributions.Last());
            return averageContribution + lastContributionEvaluation;
        }

        protected Vector2Int evaluateContribution(GraphContribution contribution)
        {
            return new Vector2Int(evaluateContributionComponent(contribution.Dynamic),
                evaluateContributionComponent(contribution.Upper));
        }

        protected int evaluateContributionComponent(int contributionValue)
        {
            if (contributionValue <= -6) return -2;
            if (contributionValue <= -3) return -1;
            if (contributionValue <= 2) return 0;
            if (contributionValue <= 5) return 1;
            return 2;
        }
    }
}
