using Wayblazer.Core.Models;

namespace Wayblazer.Core.Systems;

/// <summary>
/// Manages difficulty scaling as the player progresses to more complex worlds.
/// Higher complexity produces harder planetary conditions and requirements.
/// </summary>
public class DifficultySystem
{
    /// <summary>
    /// Gets the expected number of resources available at a given complexity.
    /// </summary>
    public static int GetResourceCount(int complexity) => 3 + complexity;

    /// <summary>
    /// Gets the minimum portal requirement multiplier for a given complexity.
    /// Higher complexity means higher requirements relative to available resources.
    /// </summary>
    public static float GetRequirementMultiplier(int complexity)
    {
        return 1.0f + (complexity - 1) * 0.3f;
    }

    /// <summary>
    /// Validates that a planet at the given complexity is solvable:
    /// at least one combination of available resources can meet portal requirements.
    /// Uses best-pair selection (since recipes combine 2 inputs at a time).
    /// </summary>
    public static bool IsPlanetSolvable(
        List<RawResource> availableResources,
        Dictionary<ResourcePropertyType, float> portalRequirements)
    {
        foreach (var (propType, required) in portalRequirements)
        {
            // Get all values for this property from available resources
            var values = availableResources
                .Where(r => r.Properties.ContainsKey(propType))
                .Select(r => r.Properties[propType].Value)
                .OrderByDescending(v => v)
                .ToList();

            if (values.Count == 0)
                return false;

            // Simulate best-case crafting using top 2 resources (since recipes pair 2 inputs)
            var topValues = values.Take(2).ToList();

            float bestAchievable = propType switch
            {
                ResourcePropertyType.Strength => topValues.Sum(),
                ResourcePropertyType.Resistance => topValues.Min() * 1.75f,
                ResourcePropertyType.Toughness => topValues.Sum() * 0.75f,
                ResourcePropertyType.Conductivity => topValues.Count > 1
                    ? topValues.Max() + topValues.Skip(1).Sum() * 0.5f
                    : topValues[0],
                ResourcePropertyType.Reactivity => topValues.Aggregate(1f, (a, b) => a * b) * 0.5f,
                _ => topValues.Max()
            };

            if (bestAchievable < required)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the number of tech nodes that should be unlockable at a given complexity.
    /// </summary>
    public static int GetTechNodeCount(int complexity) => 4 + complexity * 2;

    /// <summary>
    /// Gets a difficulty descriptor for display purposes.
    /// </summary>
    public static string GetDifficultyName(int complexity) => complexity switch
    {
        1 => "Frontier",
        2 => "Challenging",
        3 => "Hostile",
        4 => "Extreme",
        _ => complexity > 4 ? "Impossible" : "Tutorial"
    };
}
