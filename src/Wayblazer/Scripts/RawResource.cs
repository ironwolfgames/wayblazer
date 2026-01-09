using Godot;
using Godot.Collections;

namespace Wayblazer;

[GlobalClass, Tool]
public partial class RawResource(string name, ResourceKind resourceKind, Dictionary<ResourcePropertyType, ResourceProperty> properties) : Resource
{
	[Export]
	public string Name { get; set; } = name;

	[Export]
	public ResourceKind ResourceKind { get; set; } = resourceKind;

	[Export]
	public Dictionary<ResourcePropertyType, ResourceProperty> Properties { get; set; } = properties;

	public RawResource() : this("", ResourceKind.Ore, new Dictionary<ResourcePropertyType, ResourceProperty>()) { }
}
