namespace Wayblazer.GameLogic.Models;

/// <summary>
/// Defines the planetary characteristics that form the puzzle frame.
/// All values influence the portal requirements the player must meet.
/// </summary>
public class PlanetaryConstants
{
    public float Gravity { get; }
    public float AtmosphericPressure { get; }
    public float AtmosphericCorrosion { get; }
    public float TectonicVolatility { get; }
    public float LowTemperature { get; }
    public float HighTemperature { get; }

    public float TemperatureRange => HighTemperature - LowTemperature;

    public PlanetaryConstants(
        float gravity,
        float atmosphericPressure,
        float atmosphericCorrosion,
        float tectonicVolatility,
        float lowTemperature,
        float highTemperature)
    {
        Gravity = gravity;
        AtmosphericPressure = atmosphericPressure;
        AtmosphericCorrosion = atmosphericCorrosion;
        TectonicVolatility = tectonicVolatility;
        LowTemperature = lowTemperature;
        HighTemperature = highTemperature;
    }

    /// <summary>
    /// Default vertical slice planet (Complexity 1).
    /// </summary>
    public static PlanetaryConstants Default => new(
        gravity: 3.2f,
        atmosphericPressure: 5f,
        atmosphericCorrosion: 0.5f,
        tectonicVolatility: 1.0f,
        lowTemperature: -20f,
        highTemperature: 40f
    );
}
