using System.Text.Json;
using Wayblazer.GameLogic.Models;

namespace Wayblazer.GameLogic.Systems;

/// <summary>
/// Serializes and deserializes game state to/from JSON for save/load persistence.
/// Includes versioning for forward compatibility.
/// </summary>
public class GameStateSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Captures current game state from all systems into a serializable object.
    /// </summary>
    public GameState CaptureState(
        int worldSeed,
        int complexity,
        InventorySystem inventory,
        TechTreeSystem techTree,
        ScannerSystem scanner,
        PlanetaryAnalysisSystem observatory,
        ObjectiveSystem? objectives = null)
    {
        var state = new GameState
        {
            SaveVersion = 1,
            WorldSeed = worldSeed,
            Complexity = complexity,
            CurrentObjectiveId = objectives?.CurrentObjectiveId
        };

        // Capture inventory
        foreach (var (name, count) in inventory.Items)
        {
            state.Inventory[name] = count;
        }

        // Capture knowledge points
        foreach (var (category, points) in techTree.KnowledgePoints)
        {
            state.KnowledgePoints[category.ToString()] = points;
        }

        // Capture unlocked techs
        state.UnlockedTechs = techTree.UnlockedNodes.ToList();

        // Capture analyzed resources
        state.AnalyzedResources = scanner.AnalyzedResourceNames.ToList();

        // Capture measured constants
        state.MeasuredConstants = observatory.MeasuredConstants.ToList();

        return state;
    }

    /// <summary>
    /// Serializes a game state to JSON.
    /// </summary>
    public string Serialize(GameState state)
    {
        return JsonSerializer.Serialize(state, JsonOptions);
    }

    /// <summary>
    /// Deserializes a JSON string to a game state.
    /// Returns null if deserialization fails.
    /// </summary>
    public GameState? Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<GameState>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Restores systems from a saved game state.
    /// </summary>
    public void RestoreState(
        GameState state,
        InventorySystem inventory,
        TechTreeSystem techTree,
        ScannerSystem scanner,
        PlanetaryAnalysisSystem observatory)
    {
        // Restore inventory
        inventory.Clear();
        foreach (var (name, count) in state.Inventory)
        {
            inventory.AddItem(name, count);
        }

        // Restore knowledge points
        foreach (var (categoryStr, points) in state.KnowledgePoints)
        {
            if (Enum.TryParse<KnowledgeCategory>(categoryStr, out var category))
            {
                techTree.AddKnowledgePoints(category, points);
            }
        }

        // Restore tech unlocks (bypassing cost checks)
        foreach (var nodeId in state.UnlockedTechs)
        {
            techTree.ForceUnlock(nodeId);
        }

        // Restore analyzed resources
        foreach (var resourceName in state.AnalyzedResources)
        {
            scanner.MarkAsAnalyzed(resourceName);
        }

        // Restore measured constants
        foreach (var constantName in state.MeasuredConstants)
        {
            observatory.MarkAsMeasured(constantName);
        }
    }
}
