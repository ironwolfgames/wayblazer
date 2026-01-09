using Godot;

namespace Wayblazer;

[GlobalClass, Tool]
public partial class ResourceProperty : Resource
{
	[Export]
	public ResourcePropertyType Type { get; set; }

	[Export]
	public float Value { get; set; }

	[Export]
	public string VagueDescription { get; set; }

	public ResourceProperty() : this(ResourcePropertyType.Strength, 0.0f) { }

	public ResourceProperty(ResourcePropertyType type, float value)
	{
		Type = type;
		Value = value;

		VagueDescription =
			Value switch
			{
				> 7.0f => "High",
				< 3.0f => "Low",
				_ => "Medium"
			};
	}
}
