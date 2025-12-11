namespace Wayblazer;

public class ResourceProperty
{
	public ResourcePropertyType Type { get; private set; }
	public float Value { get; private set; }
	public string VagueDescription { get; private set; }

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
