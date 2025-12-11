public class ResourceProperty
{
	public ResourcePropertyType Type;
	public float Value;
	public string VagueDescription;

	public ResourceProperty(ResourcePropertyType type, float value)
	{
		Type = type;
		Value = value;
		VagueDescription = string.Empty;
		SetVagueDescription();
	}

	private void SetVagueDescription()
	{
		if (Value > 7.0f)
		{
			VagueDescription = "High";
		}
		else if (Value < 3.0f)
		{
			VagueDescription = "Low";
		}
		else
		{
			VagueDescription = "Medium";
		}
	}
}
