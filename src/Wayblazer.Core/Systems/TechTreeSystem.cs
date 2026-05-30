using Wayblazer.Core.Models;

namespace Wayblazer.Core.Systems;

/// <summary>
/// Manages the technology tree, knowledge points, and unlock progression.
/// </summary>
public class TechTreeSystem
{
    private readonly Dictionary<KnowledgeCategory, int> _knowledgePoints = new()
    {
        [KnowledgeCategory.Analysis] = 0,
        [KnowledgeCategory.Smelting] = 0,
        [KnowledgeCategory.Compositing] = 0
    };

    private readonly Dictionary<string, TechNode> _techNodes = new();
    private readonly HashSet<string> _unlockedNodes = new();

    public IReadOnlyDictionary<KnowledgeCategory, int> KnowledgePoints => _knowledgePoints;
    public IReadOnlySet<string> UnlockedNodes => _unlockedNodes;

    /// <summary>
    /// Registers a tech node in the tree.
    /// </summary>
    public void RegisterNode(TechNode node)
    {
        _techNodes[node.Id] = node;
    }

    /// <summary>
    /// Adds knowledge points to a category.
    /// </summary>
    public void AddKnowledgePoints(KnowledgeCategory category, int amount)
    {
        if (amount <= 0) return;
        _knowledgePoints[category] += amount;
    }

    /// <summary>
    /// Gets total knowledge points across all categories.
    /// </summary>
    public int TotalKnowledgePoints => _knowledgePoints.Values.Sum();

    /// <summary>
    /// Returns true if a tech node can be unlocked (has points and prerequisites met).
    /// </summary>
    public bool CanUnlock(string nodeId)
    {
        if (!_techNodes.ContainsKey(nodeId)) return false;
        if (_unlockedNodes.Contains(nodeId)) return false;

        var node = _techNodes[nodeId];

        // Check prerequisites
        if (node.Prerequisites.Any(prereq => !_unlockedNodes.Contains(prereq)))
            return false;

        // Check cost
        foreach (var cost in node.Cost)
        {
            if (_knowledgePoints[cost.Key] < cost.Value)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Attempts to unlock a tech node. Spends the knowledge points.
    /// Returns true if successful.
    /// </summary>
    public bool TryUnlock(string nodeId)
    {
        if (!CanUnlock(nodeId)) return false;

        var node = _techNodes[nodeId];

        // Spend points
        foreach (var cost in node.Cost)
        {
            _knowledgePoints[cost.Key] -= cost.Value;
        }

        _unlockedNodes.Add(nodeId);
        return true;
    }

    /// <summary>
    /// Forces a node to be unlocked without checking costs or prerequisites (for save/load).
    /// </summary>
    public void ForceUnlock(string nodeId)
    {
        _unlockedNodes.Add(nodeId);
    }

    /// <summary>
    /// Returns the set of machines the player has unlocked access to.
    /// </summary>
    public IReadOnlySet<string> GetUnlockedMachines()
    {
        var machines = new HashSet<string> { "HandScanner" }; // Always available

        if (_unlockedNodes.Contains("Unlock_FieldLab"))
            machines.Add("FieldLab");
        if (_unlockedNodes.Contains("Unlock_Observatory"))
            machines.Add("Observatory");
        if (_unlockedNodes.Contains("Unlock_Furnace"))
            machines.Add("Furnace");
        if (_unlockedNodes.Contains("Unlock_GasInjector"))
            machines.Add("GasInjector");
        if (_unlockedNodes.Contains("Unlock_SimCore"))
            machines.Add("SimulationCore");
        if (_unlockedNodes.Contains("Unlock_Portal"))
            machines.Add("PortalFoundation");

        return machines;
    }

    /// <summary>
    /// Creates the standard vertical slice tech tree.
    /// </summary>
    public static TechTreeSystem CreateVerticalSliceTree()
    {
        var system = new TechTreeSystem();

        system.RegisterNode(new TechNode(
            "Unlock_FieldLab", "Precise Analysis",
            new Dictionary<KnowledgeCategory, int> { [KnowledgeCategory.Analysis] = 15 },
            unlocksDescription: "Unlocks the Field Lab for precise resource analysis."));

        system.RegisterNode(new TechNode(
            "Unlock_Observatory", "Planetary Survey",
            new Dictionary<KnowledgeCategory, int> { [KnowledgeCategory.Analysis] = 25 },
            unlocksDescription: "Unlocks the Planetary Observatory."));

        system.RegisterNode(new TechNode(
            "Unlock_Furnace", "Basic Smelting",
            new Dictionary<KnowledgeCategory, int> { [KnowledgeCategory.Analysis] = 10 },
            unlocksDescription: "Unlocks the Basic Furnace for smelting ore."));

        system.RegisterNode(new TechNode(
            "Unlock_GasInjector", "Advanced Compositing",
            new Dictionary<KnowledgeCategory, int>
            {
                [KnowledgeCategory.Smelting] = 10,
                [KnowledgeCategory.Compositing] = 10
            },
            prerequisites: new List<string> { "Unlock_FieldLab" },
            unlocksDescription: "Unlocks the Gas Injector for composite material synthesis."));

        system.RegisterNode(new TechNode(
            "Unlock_SimCore", "Simulation Technology",
            new Dictionary<KnowledgeCategory, int>
            {
                [KnowledgeCategory.Analysis] = 15,
                [KnowledgeCategory.Smelting] = 10,
                [KnowledgeCategory.Compositing] = 10
            },
            prerequisites: new List<string> { "Unlock_GasInjector" },
            unlocksDescription: "Unlocks the Simulation Core for virtual portal testing."));

        system.RegisterNode(new TechNode(
            "Unlock_Portal", "Portal Construction",
            new Dictionary<KnowledgeCategory, int>
            {
                [KnowledgeCategory.Analysis] = 10,
                [KnowledgeCategory.Smelting] = 10,
                [KnowledgeCategory.Compositing] = 10
            },
            prerequisites: new List<string> { "Unlock_SimCore" },
            unlocksDescription: "Unlocks Portal Foundation construction."));

        return system;
    }
}
