using Godot;

namespace Wayblazer;

public partial class BiomeRange : Resource
{
	[Export] public BiomeType Biome;
	[Export] public float MinimumHeight;
	[Export] public float MaximumHeight;
	[Export] public float MinimumEquatorValue;
	[Export] public float MaximumEquatorValue;

	public BiomeRange(BiomeType biome, float minimumHeight, float maximumHeight, float minimumEquatorValue = 0.0f, float maximumEquatorValue = 1.0f)
	{
		Biome = biome;
		MinimumHeight = minimumHeight;
		MaximumHeight = maximumHeight;
		MinimumEquatorValue = minimumEquatorValue;
		MaximumEquatorValue = maximumEquatorValue;
	}
}
