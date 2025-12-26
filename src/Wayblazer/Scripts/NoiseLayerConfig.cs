using Godot;

public partial class NoiseLayerConfig : Resource
{
	[Export] public float Frequency = 0.01f;
	[Export] public int Octaves = 3;
	[Export] public float Lacunarity = 2.0f;
	[Export] public float Persistence = 0.5f;
}
