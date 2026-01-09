using Godot;

namespace Wayblazer;

[GlobalClass, Tool]
public partial class BiomeRange : Resource
{
	[Export] public BiomeType Biome { get; set; }
	[Export] public float MinimumHeight { get; set; }
	[Export] public float MaximumHeight { get; set; }
	[Export] public float MinimumEquatorValue { get; set; }
	[Export] public float MaximumEquatorValue { get; set; }

	public BiomeRange()
	{
		// Default constructor for Godot
	}

	public BiomeRange(BiomeType biome, float minimumHeight, float maximumHeight, float minimumEquatorValue = 0.0f, float maximumEquatorValue = 1.0f)
	{
		Biome = biome;
		MinimumHeight = minimumHeight;
		MaximumHeight = maximumHeight;
		MinimumEquatorValue = minimumEquatorValue;
		MaximumEquatorValue = maximumEquatorValue;
	}
}
