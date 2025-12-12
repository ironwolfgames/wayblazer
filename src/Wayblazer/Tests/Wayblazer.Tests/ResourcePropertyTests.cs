using Xunit;

namespace Wayblazer.Tests;

public class ResourcePropertyTests
{
	[Theory]
	[InlineData(ResourcePropertyType.Strength, 8.0f, "High")]
	[InlineData(ResourcePropertyType.Strength, 7.5f, "High")]
	[InlineData(ResourcePropertyType.Conductivity, 2.5f, "Low")]
	[InlineData(ResourcePropertyType.Conductivity, 2.0f, "Low")]
	[InlineData(ResourcePropertyType.Resistance, 5.0f, "Medium")]
	[InlineData(ResourcePropertyType.Toughness, 4.0f, "Medium")]
	[InlineData(ResourcePropertyType.Reactivity, 3.0f, "Medium")]
	public void Constructor_SetsVagueDescriptionCorrectly(ResourcePropertyType type, float value, string expectedDescription)
	{
		var property = new ResourceProperty(type, value);

		Assert.Equal(expectedDescription, property.VagueDescription);
	}

	[Fact]
	public void Constructor_SetsTypeAndValue()
	{
		var type = ResourcePropertyType.Strength;
		var value = 5.5f;

		var property = new ResourceProperty(type, value);

		Assert.Equal(type, property.Type);
		Assert.Equal(value, property.Value);
	}

	[Theory]
	[InlineData(7.01f, "High")]
	[InlineData(7.0f, "Medium")] // Exactly 7.0 is not > 7.0
	[InlineData(2.99f, "Low")]
	[InlineData(3.0f, "Medium")] // Exactly 3.0 is not < 3.0
	public void VagueDescription_BoundaryValues(float value, string expectedDescription)
	{
		var property = new ResourceProperty(ResourcePropertyType.Strength, value);

		Assert.Equal(expectedDescription, property.VagueDescription);
	}

	[Theory]
	[InlineData(0f)]
	[InlineData(-5f)]
	[InlineData(100f)]
	public void Constructor_HandlesEdgeCaseValues(float value)
	{
		var property = new ResourceProperty(ResourcePropertyType.Conductivity, value);

		Assert.Equal(value, property.Value);
		Assert.NotNull(property.VagueDescription);
	}
}
