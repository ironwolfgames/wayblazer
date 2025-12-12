using System.Collections.Generic;

namespace Wayblazer;

public class RawResource(string name, Dictionary<ResourcePropertyType, ResourceProperty> properties)
{
	public string Name { get; private set; } = name;
	public Dictionary<ResourcePropertyType, ResourceProperty> Properties { get; private set; } = properties;
}
