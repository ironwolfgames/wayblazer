using Wayblazer.Core.Models;

namespace Wayblazer.Core.Systems;

/// <summary>
/// Verifies whether engineered materials meet portal requirements.
/// </summary>
public class PortalVerificationSystem
{
    /// <summary>
    /// Runs a simulation check against the portal requirements.
    /// Returns a detailed report of pass/fail per property.
    /// </summary>
    public SimulationResult RunSimulation(
        Dictionary<ResourcePropertyType, float> materialProperties,
        Dictionary<ResourcePropertyType, float> portalRequirements)
    {
        var propertyResults = new Dictionary<ResourcePropertyType, PropertyCheckResult>();
        bool overallPass = true;

        foreach (var requirement in portalRequirements)
        {
            var propType = requirement.Key;
            var requiredValue = requirement.Value;

            float actualValue = materialProperties.GetValueOrDefault(propType, 0f);
            bool passes = actualValue >= requiredValue;

            if (!passes)
                overallPass = false;

            float percentage = requiredValue > 0 ? (actualValue / requiredValue) * 100f : 100f;

            propertyResults[propType] = new PropertyCheckResult(
                propType, requiredValue, actualValue, passes, percentage);
        }

        return new SimulationResult(overallPass, propertyResults);
    }

    /// <summary>
    /// Quick check: does the material meet ALL portal requirements?
    /// </summary>
    public bool MeetsRequirements(
        Dictionary<ResourcePropertyType, float> materialProperties,
        Dictionary<ResourcePropertyType, float> portalRequirements)
    {
        return portalRequirements.All(req =>
            materialProperties.GetValueOrDefault(req.Key, 0f) >= req.Value);
    }
}

/// <summary>
/// Result of running a portal simulation.
/// </summary>
public class SimulationResult
{
    public bool OverallPass { get; }
    public Dictionary<ResourcePropertyType, PropertyCheckResult> PropertyResults { get; }

    public float OverallIntegrity => PropertyResults.Count > 0
        ? PropertyResults.Values.Average(r => r.Percentage)
        : 0f;

    public SimulationResult(bool overallPass, Dictionary<ResourcePropertyType, PropertyCheckResult> propertyResults)
    {
        OverallPass = overallPass;
        PropertyResults = propertyResults;
    }
}

/// <summary>
/// Result of checking a single property against its requirement.
/// </summary>
public class PropertyCheckResult
{
    public ResourcePropertyType PropertyType { get; }
    public float RequiredValue { get; }
    public float ActualValue { get; }
    public bool Passes { get; }
    public float Percentage { get; }

    public PropertyCheckResult(
        ResourcePropertyType propertyType,
        float requiredValue,
        float actualValue,
        bool passes,
        float percentage)
    {
        PropertyType = propertyType;
        RequiredValue = requiredValue;
        ActualValue = actualValue;
        Passes = passes;
        Percentage = percentage;
    }
}
