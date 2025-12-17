using Godot;
using Godot.Collections;
using Wayblazer;

[GlobalClass]
public partial class GameManager : Node
{
	public PlanetaryConstants CurrentPlanetaryConstants { get; private set; } = null!;
	public Array<RawResource> RawResources { get; private set; } = null!;
	public Array<CompositeResource> CompositeResources { get; private set; } = null!;

	public override void _Ready()
	{
		InitializeWorld();
	}

	private void InitializeWorld()
	{
		CurrentPlanetaryConstants = PlanetaryConstants.Default;

		RawResources = new Array<RawResource>();
		CompositeResources = new Array<CompositeResource>();

		GD.Print($"GameManager data structures initialized:");
		GD.Print($"  - Planetary Constants: Gravity={CurrentPlanetaryConstants.Gravity}, Pressure={CurrentPlanetaryConstants.AtmosphericPressure}");
		GD.Print($"  - Raw Resources: {RawResources.Count}");
		GD.Print($"  - Composite Resources: {CompositeResources.Count}");
	}
}
