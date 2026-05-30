namespace Wayblazer.Core.Models;

/// <summary>
/// Serializable game state for save/load persistence.
/// </summary>
public class GameState
{
    public int SaveVersion { get; set; } = 1;
    public int WorldSeed { get; set; }
    public int Complexity { get; set; } = 1;

    // Inventory: resource name → count
    public Dictionary<string, int> Inventory { get; set; } = new();

    // Knowledge points per category
    public Dictionary<string, int> KnowledgePoints { get; set; } = new();

    // Unlocked tech node IDs
    public List<string> UnlockedTechs { get; set; } = new();

    // Analyzed resource names
    public List<string> AnalyzedResources { get; set; } = new();

    // Measured planetary constant names
    public List<string> MeasuredConstants { get; set; } = new();

    // Current objective ID
    public string? CurrentObjectiveId { get; set; }

    // Portal components: component type → assigned material name
    public Dictionary<string, string> PortalComponents { get; set; } = new();
}
