using Godot;
using Godot.Collections;

namespace Wayblazer;

[GlobalClass, Tool]
public partial class EnvironmentalDecorationPlacementConfig : Resource
{
	[Export]
	public EnvironmentalDecorationType DecorationType { get; set; }
	[Export]
	public NoiseLayerConfig NoiseConfig { get; set; }
	[Export]
	public float MinimumValue { get; set; }
	[Export]
	public float MaximumValue { get; set; }
	[Export]
	public Array<BiomeType> ValidBiomes { get; set; }

	public EnvironmentalDecorationPlacementConfig()
	{
		// Default constructor for Godot
		NoiseConfig = new NoiseLayerConfig();
		ValidBiomes = new Array<BiomeType>();
	}

	public EnvironmentalDecorationPlacementConfig(EnvironmentalDecorationType decorationType, NoiseLayerConfig noiseConfig,
		float minimumValue, float maximumValue, Array<BiomeType> validBiomes)
	{
		DecorationType = decorationType;
		NoiseConfig = noiseConfig;
		MinimumValue = minimumValue;
		MaximumValue = maximumValue;
		ValidBiomes = validBiomes;
	}
}
