using Wayblazer.GameLogic.Models;

namespace Wayblazer.GameLogic.Systems;

/// <summary>
/// Measures planetary constants precisely and calculates portal requirements.
/// Sprint 6: Planetary Observatory and deduction formula.
/// </summary>
public class PlanetaryAnalysisSystem
{
    private readonly HashSet<string> _measuredConstants = new();

    /// <summary>
    /// Returns true if a specific planetary property has been precisely measured.
    /// </summary>
    public bool IsConstantMeasured(string constantName) => _measuredConstants.Contains(constantName);

    /// <summary>
    /// Measures a planetary constant using the Observatory.
    /// Returns the precise value and analysis points earned.
    /// </summary>
    public (float value, int pointsEarned) MeasureConstant(string constantName, PlanetaryConstants constants)
    {
        float value = constantName switch
        {
            "Gravity" => constants.Gravity,
            "AtmosphericPressure" => constants.AtmosphericPressure,
            "AtmosphericCorrosion" => constants.AtmosphericCorrosion,
            "TectonicVolatility" => constants.TectonicVolatility,
            "TemperatureRange" => constants.TemperatureRange,
            _ => throw new ArgumentException($"Unknown constant: {constantName}")
        };

        if (_measuredConstants.Contains(constantName))
            return (value, 0); // Already measured

        _measuredConstants.Add(constantName);
        return (value, 8); // 8 Analysis points per new measurement
    }

    /// <summary>
    /// Calculates the portal requirement for a given property based on planetary constants.
    /// These are the deduction formulas the player discovers through research.
    /// </summary>
    public static Dictionary<ResourcePropertyType, float> CalculatePortalRequirements(PlanetaryConstants constants)
    {
        return new Dictionary<ResourcePropertyType, float>
        {
            // Foundation Strength = Gravity × 2.5
            [ResourcePropertyType.Strength] = constants.Gravity * 2.5f,
            // Gate Resistance = (TemperatureRange / 20) + (Corrosion × 3)
            [ResourcePropertyType.Resistance] = (constants.TemperatureRange / 20f) + (constants.AtmosphericCorrosion * 3f),
            // Toughness = TectonicVolatility × 4 + AtmosphericPressure × 0.5
            [ResourcePropertyType.Toughness] = (constants.TectonicVolatility * 4f) + (constants.AtmosphericPressure * 0.5f),
            // Conductivity = (AtmosphericPressure + Gravity) × 0.8
            [ResourcePropertyType.Conductivity] = (constants.AtmosphericPressure + constants.Gravity) * 0.8f,
            // Reactivity = (TectonicVolatility + Corrosion) × 2.5
            [ResourcePropertyType.Reactivity] = (constants.TectonicVolatility + constants.AtmosphericCorrosion) * 2.5f
        };
    }

    /// <summary>
    /// Gets the set of all measured constants.
    /// </summary>
    public IReadOnlySet<string> MeasuredConstants => _measuredConstants;

    /// <summary>
    /// Marks a constant as measured without going through the full measurement flow (for save/load).
    /// </summary>
    public void MarkAsMeasured(string constantName)
    {
        _measuredConstants.Add(constantName);
    }
}
