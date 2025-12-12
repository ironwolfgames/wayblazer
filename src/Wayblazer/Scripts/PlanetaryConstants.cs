namespace Wayblazer;

public class PlanetaryConstants(float gravity, float atmosphericPressure, float atmosphericCorrosion, float tectonicVolatility, float lowTemperature, float highTemperature)
{
	/// <summary>
	/// Gravity of the planet, measured on a scale from 0 (no gravity) to 10 (very high gravity).
	/// </summary>
	public float Gravity { get; private set; } = gravity;
	/// <summary>
	/// Atmospheric pressure of the planet, measured on a scale from 0 (no atmosphere) to 10 (very high pressure).
	/// </summary>
	public float AtmosphericPressure { get; private set; } = atmosphericPressure;
	/// <summary>
	/// Atmospheric corrosion factor of the planet, measured on a scale from 0 (no corrosion) to 1 (very high corrosion).
	/// </summary>
	public float AtmosphericCorrosion { get; private set; } = atmosphericCorrosion;
	/// <summary>
	/// Tectonic volatility of the planet, measured on a scale from 0 (geologically stable) to 1 (very volatile).
	/// </summary>
	public float TectonicVolatility { get; private set; } = tectonicVolatility;
	/// <summary>
	/// Low temperature limit of the planet in degrees Celsius.
	/// </summary>
	public float LowTemperature { get; private set; } = lowTemperature;
	/// <summary>
	/// High temperature limit of the planet in degrees Celsius.
	/// </summary>
	public float HighTemperature { get; private set; } = highTemperature;

	public static PlanetaryConstants Default => new PlanetaryConstants(5f, 5f, 1f, 3f, -50.0f, 50.0f);
}
