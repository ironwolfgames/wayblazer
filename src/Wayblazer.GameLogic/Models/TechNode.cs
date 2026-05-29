namespace Wayblazer.GameLogic.Models;

/// <summary>
/// A node in the technology tree that can be unlocked with knowledge points.
/// </summary>
public class TechNode
{
    public string Id { get; }
    public string Name { get; }
    public Dictionary<KnowledgeCategory, int> Cost { get; }
    public List<string> Prerequisites { get; }
    public string UnlocksDescription { get; }

    public TechNode(
        string id,
        string name,
        Dictionary<KnowledgeCategory, int> cost,
        List<string>? prerequisites = null,
        string unlocksDescription = "")
    {
        Id = id;
        Name = name;
        Cost = cost;
        Prerequisites = prerequisites ?? new List<string>();
        UnlocksDescription = unlocksDescription;
    }
}
