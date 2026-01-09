using Godot;

namespace Wayblazer;

[GlobalClass, Tool]
public partial class NoiseLayerConfig : Resource
{
	/// <summary>
	/// Controls the scale of noise features. Lower values (0.001-0.01) create large continents, higher values (0.1-1.0) create fine details.
	/// </summary>
	[Export(PropertyHint.Range, "0.001,1.0")]
	public float Frequency { get; set; } = 0.01f;

	/// <summary>
	/// Number of noise layers combined. More octaves add finer detail but increase computation cost. Range: 1-8.
	/// </summary>
	[Export(PropertyHint.Range, "1,8,1")]
	public int Octaves { get; set; } = 3;

	/// <summary>
	/// Frequency multiplier for each octave. Higher values create more contrast between noise scales. Typical: 2.0.
	/// </summary>
	[Export(PropertyHint.Range, "0.1,4.0,0.1")]
	public float Lacunarity { get; set; } = 2.0f;

	/// <summary>
	/// Amplitude multiplier for each octave. Lower values (0.3-0.4) reduce detail contribution, higher values (0.6-0.7) increase it. Typical: 0.5.
	/// </summary>
	[Export(PropertyHint.Range, "0.1,1.0,0.1")]
	public float Persistence { get; set; } = 0.5f;
}
