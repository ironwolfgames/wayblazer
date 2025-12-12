
namespace Wayblazer.Tests;

public class RawResourceTests
{
	[Fact]
	public void Constructor_SetsNameAndProperties()
	{
		var name = "Iron";
		var properties = new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 7f) }
		};

		var resource = new RawResource(name, properties);

		Assert.Equal(name, resource.Name);
		Assert.Equal(properties, resource.Properties);
	}

	[Fact]
	public void Constructor_WithMultipleProperties()
	{
		var name = "ComplexOre";
		var properties = new Dictionary<ResourcePropertyType, ResourceProperty>
		{
			{ ResourcePropertyType.Strength, new ResourceProperty(ResourcePropertyType.Strength, 5f) },
			{ ResourcePropertyType.Conductivity, new ResourceProperty(ResourcePropertyType.Conductivity, 8f) },
			{ ResourcePropertyType.Resistance, new ResourceProperty(ResourcePropertyType.Resistance, 3f) }
		};

		var resource = new RawResource(name, properties);

		Assert.Equal(name, resource.Name);
		Assert.Equal(3, resource.Properties.Count);
		Assert.Equal(5f, resource.Properties[ResourcePropertyType.Strength].Value);
		Assert.Equal(8f, resource.Properties[ResourcePropertyType.Conductivity].Value);
		Assert.Equal(3f, resource.Properties[ResourcePropertyType.Resistance].Value);
	}

	[Fact]
	public void Constructor_WithEmptyProperties()
	{
		var name = "EmptyResource";
		var properties = new Dictionary<ResourcePropertyType, ResourceProperty>();

		var resource = new RawResource(name, properties);

		Assert.Equal(name, resource.Name);
		Assert.Empty(resource.Properties);
	}
}
