using Wayblazer.GameLogic.Models;

namespace Wayblazer.GameLogic.Systems;

/// <summary>
/// Handles resource scanning and progressive knowledge revelation.
/// Sprint 4: Vague scanning. Sprint 5: Precise analysis via Field Lab.
/// </summary>
public class ScannerSystem
{
    private readonly HashSet<string> _analyzedResources = new();

    /// <summary>
    /// Returns true if the resource has been analyzed in the Field Lab (precise data available).
    /// </summary>
    public bool IsResourceAnalyzed(string resourceName) => _analyzedResources.Contains(resourceName);

    /// <summary>
    /// Gets the set of all analyzed resource names (for save/load).
    /// </summary>
    public IReadOnlySet<string> AnalyzedResourceNames => _analyzedResources;

    /// <summary>
    /// Gets a vague scan result for a resource (Hand Scanner - always available).
    /// Returns property type → vague description ("Low"/"Medium"/"High").
    /// </summary>
    public Dictionary<ResourcePropertyType, string> GetVagueScan(RawResource resource)
    {
        return resource.Properties.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.VagueDescription
        );
    }

    /// <summary>
    /// Gets a precise scan result (Field Lab - only if analyzed).
    /// Returns null if resource hasn't been analyzed yet.
    /// </summary>
    public Dictionary<ResourcePropertyType, float>? GetPreciseScan(RawResource resource)
    {
        if (!_analyzedResources.Contains(resource.Name))
            return null;

        return resource.Properties.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Value
        );
    }

    /// <summary>
    /// Marks a resource as analyzed (Field Lab action).
    /// Returns the analysis points earned.
    /// </summary>
    public int AnalyzeResource(RawResource resource)
    {
        if (_analyzedResources.Contains(resource.Name))
            return 0; // Already analyzed, no new points

        _analyzedResources.Add(resource.Name);
        // Each new analysis earns 5 Analysis points
        return 5;
    }

    /// <summary>
    /// Gets vague planetary description (Hand Scanner level).
    /// </summary>
    public Dictionary<string, string> GetVaguePlanetaryScan(PlanetaryConstants constants)
    {
        return new Dictionary<string, string>
        {
            ["Gravity"] = GetVagueLevel(constants.Gravity, 2f, 7f),
            ["Atmospheric Pressure"] = GetVagueLevel(constants.AtmosphericPressure, 3f, 7f),
            ["Corrosion"] = GetVagueLevel(constants.AtmosphericCorrosion, 0.3f, 0.7f),
            ["Tectonic Activity"] = GetVagueLevel(constants.TectonicVolatility, 0.3f, 0.7f),
            ["Temperature Range"] = GetVagueLevel(constants.TemperatureRange, 40f, 120f)
        };
    }

    private static string GetVagueLevel(float value, float lowThreshold, float highThreshold)
    {
        if (value > highThreshold) return "High";
        if (value < lowThreshold) return "Low";
        return "Moderate";
    }

    /// <summary>
    /// Marks a resource as analyzed without going through the full analysis flow (for save/load).
    /// </summary>
    public void MarkAsAnalyzed(string resourceName)
    {
        _analyzedResources.Add(resourceName);
    }
}
