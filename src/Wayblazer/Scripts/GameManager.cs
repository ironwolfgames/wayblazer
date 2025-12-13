using Godot;
using System.Collections.Generic;
using Wayblazer;

[GlobalClass]
public partial class GameManager : Node
{
	public PlanetaryConstants CurrentPlanetaryConstants { get; private set; } = null!;
	public List<RawResource> RawResources { get; private set; } = null!;
	public List<CompositeResource> CompositeResources { get; private set; } = null!;

	public override void _Ready()
	{
		InitializeWorld();
	}

	private void InitializeWorld()
	{
		CurrentPlanetaryConstants = PlanetaryConstants.Default;

		RawResources = new List<RawResource>
		{
			new RawResource("Iron", new Dictionary<ResourcePropertyType, ResourceProperty>
			{
				{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 8.0f) },
				{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 7.5f) },
				{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 6.0f) }
			}),
			new RawResource("Copper", new Dictionary<ResourcePropertyType, ResourceProperty>
			{
				{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 9.0f) },
				{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 4.0f) }
			}),
			new RawResource("Stone", new Dictionary<ResourcePropertyType, ResourceProperty>
			{
				{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 8.5f) },
				{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 9.0f) }
			}),
			new RawResource("Wood", new Dictionary<ResourcePropertyType, ResourceProperty>
			{
				{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 3.5f) },
				{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 7.0f) }
			}),
			new RawResource("Crystal", new Dictionary<ResourcePropertyType, ResourceProperty>
			{
				{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 8.5f) },
				{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 6.0f) }
			})
		};

		CompositeResources = new List<CompositeResource>();

		GD.Print($"GameManager data structures initialized:");
		GD.Print($"  - Planetary Constants: Gravity={CurrentPlanetaryConstants.Gravity}, Pressure={CurrentPlanetaryConstants.AtmosphericPressure}");
		GD.Print($"  - Raw Resources: {RawResources.Count}");
		GD.Print($"  - Composite Resources: {CompositeResources.Count}");
	}
}
