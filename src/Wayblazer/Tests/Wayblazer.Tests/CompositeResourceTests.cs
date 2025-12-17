using Godot.Collections;

namespace Wayblazer.Tests;

public class CompositeResourceTests
{
	[Fact]
	public void Constructor_CreatesCompositeResourceWithName()
	{
		var inputs = new Array<RawResource>
		{
			CreateTestRawResource("Iron", ResourcePropertyType.Strength, 5f),
			CreateTestRawResource("Carbon", ResourcePropertyType.Toughness, 3f)
		};

		var composite = new CompositeResource("Steel", inputs);

		Assert.Equal("Steel", composite.Name);
		Assert.Equal(inputs, composite.Inputs);
	}

	[Fact]
	public void CompositeResource_CombinesPropertiesFromMultipleInputs()
	{
		var iron = CreateTestRawResource("Iron", ResourcePropertyType.Strength, 5f);
		var carbon = CreateTestRawResource("Carbon", ResourcePropertyType.Strength, 3f);
		var inputs = new Array<RawResource> { iron, carbon };

		var composite = new CompositeResource("Steel", inputs);

		Assert.NotNull(composite.Properties);
		Assert.Contains(ResourcePropertyType.Strength, composite.Properties.Keys);
		// Strength combines by adding: 5 + 3 = 8
		Assert.Equal(8f, composite.Properties[ResourcePropertyType.Strength].Value);
	}

	[Fact]
	public void CompositeResource_ConductivityCombination_TakesMaximumWithBoost()
	{
		var copper = new RawResource("Copper", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 10f) }
		});
		var silver = new RawResource("Silver", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 8f) }
		});
		var inputs = new Array<RawResource> { copper, silver };

		var composite = new CompositeResource("Alloy", inputs);

		// Conductivity: Max(10 + 8*0.5, 8 + 10*0.5) = Max(14, 13) = 14
		Assert.Equal(14f, composite.Properties[ResourcePropertyType.Conductivity].Value);
	}

	[Fact]
	public void CompositeResource_ReactivityCombination_MultipliesAndHalves()
	{
		var resourceA = new RawResource("A", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 4f) }
		});
		var resourceB = new RawResource("B", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Reactivity, new ResourceProperty(ResourcePropertyType.Reactivity, 6f) }
		});
		var inputs = new Array<RawResource> { resourceA, resourceB };

		var composite = new CompositeResource("Composite", inputs);

		// Reactivity: 4 * 6 * 0.5 = 12
		Assert.Equal(12f, composite.Properties[ResourcePropertyType.Reactivity].Value);
	}

	[Fact]
	public void CompositeResource_ResistanceCombination_TakesMinimumWithBoost()
	{
		var resourceA = new RawResource("A", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 8f) }
		});
		var resourceB = new RawResource("B", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 5f) }
		});
		var inputs = new Array<RawResource> { resourceA, resourceB };

		var composite = new CompositeResource("Composite", inputs);

		// Resistance: Min(8, 5) * 1.75 = 5 * 1.75 = 8.75
		Assert.Equal(8.75f, composite.Properties[ResourcePropertyType.Resistance].Value);
	}

	[Fact]
	public void CompositeResource_StrengthCombination_AddsValues()
	{
		var resourceA = new RawResource("A", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 7f) }
		});
		var resourceB = new RawResource("B", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 3f) }
		});
		var inputs = new Array<RawResource> { resourceA, resourceB };

		var composite = new CompositeResource("Composite", inputs);

		// Strength: 7 + 3 = 10
		Assert.Equal(10f, composite.Properties[ResourcePropertyType.Strength].Value);
	}

	[Fact]
	public void CompositeResource_ToughnessCombination_AddsAndMultiplies()
	{
		var resourceA = new RawResource("A", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 4f) }
		});
		var resourceB = new RawResource("B", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 6f) }
		});
		var inputs = new Array<RawResource> { resourceA, resourceB };

		var composite = new CompositeResource("Composite", inputs);

		// Toughness: (4 + 6) * 0.75 = 7.5
		Assert.Equal(7.5f, composite.Properties[ResourcePropertyType.Toughness].Value);
	}

	[Fact]
	public void CompositeResource_CombinesMultiplePropertyTypes()
	{
		var resourceA = new RawResource("A", ResourceKind.Wood, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 5f) },
			{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 3f) }
		});
		var resourceB = new RawResource("B", ResourceKind.Wood, new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 4f) },
			{ ResourcePropertyType.Toughness, new ResourceProperty(ResourcePropertyType.Toughness, 6f) }
		});
		var inputs = new Array<RawResource> { resourceA, resourceB };

		var composite = new CompositeResource("Composite", inputs);

		Assert.Equal(3, composite.Properties.Count);
		Assert.Contains(ResourcePropertyType.Strength, composite.Properties.Keys);
		Assert.Contains(ResourcePropertyType.Conductivity, composite.Properties.Keys);
		Assert.Contains(ResourcePropertyType.Toughness, composite.Properties.Keys);
	}

	[Fact]
	public void CompositeResource_WithSingleInput_PreservesProperties()
	{
		var iron = CreateTestRawResource("Iron", ResourcePropertyType.Strength, 7f);
		var inputs = new Array<RawResource> { iron };

		var composite = new CompositeResource("PureIron", inputs);

		Assert.Single(composite.Properties);
		Assert.Equal(7f, composite.Properties[ResourcePropertyType.Strength].Value);
	}

	[Fact]
	public void CompositeResource_WithThreeInputs_CombinesCorrectly()
	{
		var inputs = new Array<RawResource>
		{
			CreateTestRawResource("A", ResourcePropertyType.Strength, 2f),
			CreateTestRawResource("B", ResourcePropertyType.Strength, 3f),
			CreateTestRawResource("C", ResourcePropertyType.Strength, 5f)
		};

		var composite = new CompositeResource("ABC", inputs);

		// First combines A and B: 2 + 3 = 5
		// Then combines result with C: 5 + 5 = 10
		Assert.Equal(10f, composite.Properties[ResourcePropertyType.Strength].Value);
	}

	private static RawResource CreateTestRawResource(string name, ResourcePropertyType propertyType, float value)
	{
		var properties = new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ propertyType, new ResourceProperty(propertyType, value) }
		};
		return new RawResource(name, ResourceKind.Ore, properties);
	}
}
