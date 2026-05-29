namespace Wayblazer.GameLogic.Models;

/// <summary>
/// A single measurable property of a resource (e.g., Strength: 4.2).
/// Also provides vague descriptions for the Hand Scanner.
/// </summary>
public class ResourceProperty
{
    public ResourcePropertyType Type { get; }
    public float Value { get; }
    public string VagueDescription { get; }

    public ResourceProperty(ResourcePropertyType type, float value)
    {
        Type = type;
        Value = value;
        VagueDescription = GetVagueDescription(value);
    }

    private static string GetVagueDescription(float value)
    {
        if (value > 7.0f) return "High";
        if (value < 3.0f) return "Low";
        return "Medium";
    }
}
