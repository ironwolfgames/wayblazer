using Wayblazer.Core.Models;
using Wayblazer.Core.Systems;

namespace Wayblazer.Core.Tests;

/// <summary>
/// Tests for game state serialization (save/load).
/// Validates round-trip fidelity, versioning, and error handling.
/// </summary>
public class SaveLoadSerializationTests
{
    private static (InventorySystem inv, TechTreeSystem tech, ScannerSystem scan, PlanetaryAnalysisSystem obs) CreatePopulatedSystems()
    {
        var inv = new InventorySystem();
        inv.AddItem("Base Ore", 10);
        inv.AddItem("Soft Wood", 5);
        inv.AddItem("Metal Bar", 2);

        var tech = TechTreeSystem.CreateVerticalSliceTree();
        tech.AddKnowledgePoints(KnowledgeCategory.Analysis, 30);
        tech.TryUnlock("Unlock_FieldLab");

        var scan = new ScannerSystem();
        scan.MarkAsAnalyzed("Base Ore");
        scan.MarkAsAnalyzed("Catalyst Ore");

        var obs = new PlanetaryAnalysisSystem();
        obs.MarkAsMeasured("Gravity");
        obs.MarkAsMeasured("AtmosphericPressure");

        return (inv, tech, scan, obs);
    }

    [Fact]
    public void CaptureState_ContainsInventory()
    {
        var (inv, tech, scan, obs) = CreatePopulatedSystems();
        var serializer = new GameStateSerializer();

        var state = serializer.CaptureState(42, 1, inv, tech, scan, obs);

        Assert.Equal(10, state.Inventory["Base Ore"]);
        Assert.Equal(5, state.Inventory["Soft Wood"]);
        Assert.Equal(2, state.Inventory["Metal Bar"]);
    }

    [Fact]
    public void CaptureState_ContainsKnowledgePoints()
    {
        var (inv, tech, scan, obs) = CreatePopulatedSystems();
        var serializer = new GameStateSerializer();

        var state = serializer.CaptureState(42, 1, inv, tech, scan, obs);

        Assert.Equal(15, state.KnowledgePoints["Analysis"]); // 30 - 15 spent
    }

    [Fact]
    public void CaptureState_ContainsUnlockedTechs()
    {
        var (inv, tech, scan, obs) = CreatePopulatedSystems();
        var serializer = new GameStateSerializer();

        var state = serializer.CaptureState(42, 1, inv, tech, scan, obs);

        Assert.Contains("Unlock_FieldLab", state.UnlockedTechs);
    }

    [Fact]
    public void CaptureState_ContainsAnalyzedResources()
    {
        var (inv, tech, scan, obs) = CreatePopulatedSystems();
        var serializer = new GameStateSerializer();

        var state = serializer.CaptureState(42, 1, inv, tech, scan, obs);

        Assert.Contains("Base Ore", state.AnalyzedResources);
        Assert.Contains("Catalyst Ore", state.AnalyzedResources);
    }

    [Fact]
    public void CaptureState_ContainsMeasuredConstants()
    {
        var (inv, tech, scan, obs) = CreatePopulatedSystems();
        var serializer = new GameStateSerializer();

        var state = serializer.CaptureState(42, 1, inv, tech, scan, obs);

        Assert.Contains("Gravity", state.MeasuredConstants);
        Assert.Contains("AtmosphericPressure", state.MeasuredConstants);
    }

    [Fact]
    public void RoundTrip_SerializeDeserialize_PreservesData()
    {
        var (inv, tech, scan, obs) = CreatePopulatedSystems();
        var serializer = new GameStateSerializer();

        var state = serializer.CaptureState(42, 1, inv, tech, scan, obs);
        string json = serializer.Serialize(state);
        var restored = serializer.Deserialize(json);

        Assert.NotNull(restored);
        Assert.Equal(state.SaveVersion, restored.SaveVersion);
        Assert.Equal(state.WorldSeed, restored.WorldSeed);
        Assert.Equal(state.Complexity, restored.Complexity);
        Assert.Equal(state.Inventory["Base Ore"], restored.Inventory["Base Ore"]);
        Assert.Equal(state.UnlockedTechs.Count, restored.UnlockedTechs.Count);
        Assert.Equal(state.AnalyzedResources.Count, restored.AnalyzedResources.Count);
        Assert.Equal(state.MeasuredConstants.Count, restored.MeasuredConstants.Count);
    }

    [Fact]
    public void RestoreState_PopulatesInventory()
    {
        var serializer = new GameStateSerializer();
        var state = new GameState
        {
            WorldSeed = 42,
            Inventory = new Dictionary<string, int> { ["Base Ore"] = 7, ["Soft Wood"] = 3 }
        };

        var inv = new InventorySystem();
        var tech = TechTreeSystem.CreateVerticalSliceTree();
        var scan = new ScannerSystem();
        var obs = new PlanetaryAnalysisSystem();

        serializer.RestoreState(state, inv, tech, scan, obs);

        Assert.Equal(7, inv.GetItemCount("Base Ore"));
        Assert.Equal(3, inv.GetItemCount("Soft Wood"));
    }

    [Fact]
    public void RestoreState_PopulatesTechTree()
    {
        var serializer = new GameStateSerializer();
        var state = new GameState
        {
            WorldSeed = 42,
            KnowledgePoints = new Dictionary<string, int> { ["Analysis"] = 10 },
            UnlockedTechs = new List<string> { "Unlock_FieldLab", "Unlock_Observatory" }
        };

        var inv = new InventorySystem();
        var tech = TechTreeSystem.CreateVerticalSliceTree();
        var scan = new ScannerSystem();
        var obs = new PlanetaryAnalysisSystem();

        serializer.RestoreState(state, inv, tech, scan, obs);

        Assert.Contains("Unlock_FieldLab", tech.UnlockedNodes);
        Assert.Contains("Unlock_Observatory", tech.UnlockedNodes);
        Assert.Equal(10, tech.KnowledgePoints[KnowledgeCategory.Analysis]);
    }

    [Fact]
    public void RestoreState_PopulatesScanner()
    {
        var serializer = new GameStateSerializer();
        var state = new GameState
        {
            WorldSeed = 42,
            AnalyzedResources = new List<string> { "Base Ore", "Catalyst Ore" }
        };

        var inv = new InventorySystem();
        var tech = TechTreeSystem.CreateVerticalSliceTree();
        var scan = new ScannerSystem();
        var obs = new PlanetaryAnalysisSystem();

        serializer.RestoreState(state, inv, tech, scan, obs);

        Assert.True(scan.IsResourceAnalyzed("Base Ore"));
        Assert.True(scan.IsResourceAnalyzed("Catalyst Ore"));
        Assert.False(scan.IsResourceAnalyzed("Soft Wood"));
    }

    [Fact]
    public void RestoreState_PopulatesObservatory()
    {
        var serializer = new GameStateSerializer();
        var state = new GameState
        {
            WorldSeed = 42,
            MeasuredConstants = new List<string> { "Gravity" }
        };

        var inv = new InventorySystem();
        var tech = TechTreeSystem.CreateVerticalSliceTree();
        var scan = new ScannerSystem();
        var obs = new PlanetaryAnalysisSystem();

        serializer.RestoreState(state, inv, tech, scan, obs);

        Assert.True(obs.IsConstantMeasured("Gravity"));
        Assert.False(obs.IsConstantMeasured("AtmosphericPressure"));
    }

    [Fact]
    public void Deserialize_InvalidJson_ReturnsNull()
    {
        var serializer = new GameStateSerializer();

        var result = serializer.Deserialize("not valid json {{{");

        Assert.Null(result);
    }

    [Fact]
    public void Deserialize_EmptyJson_ReturnsNull()
    {
        var serializer = new GameStateSerializer();

        var result = serializer.Deserialize("");

        Assert.Null(result);
    }

    [Fact]
    public void SaveVersion_Included()
    {
        var (inv, tech, scan, obs) = CreatePopulatedSystems();
        var serializer = new GameStateSerializer();

        var state = serializer.CaptureState(42, 1, inv, tech, scan, obs);

        Assert.Equal(1, state.SaveVersion);
    }

    [Fact]
    public void Serialize_ProducesValidJson()
    {
        var serializer = new GameStateSerializer();
        var state = new GameState
        {
            WorldSeed = 42,
            Complexity = 2,
            Inventory = new Dictionary<string, int> { ["Ore"] = 5 }
        };

        string json = serializer.Serialize(state);

        Assert.Contains("\"worldSeed\"", json);
        Assert.Contains("42", json);
        Assert.Contains("\"complexity\"", json);
    }

    [Fact]
    public void RestoreState_ClearsExistingInventory()
    {
        var serializer = new GameStateSerializer();
        var state = new GameState
        {
            WorldSeed = 42,
            Inventory = new Dictionary<string, int> { ["New Ore"] = 3 }
        };

        var inv = new InventorySystem();
        inv.AddItem("Old Stuff", 99); // Pre-existing

        var tech = TechTreeSystem.CreateVerticalSliceTree();
        var scan = new ScannerSystem();
        var obs = new PlanetaryAnalysisSystem();

        serializer.RestoreState(state, inv, tech, scan, obs);

        Assert.Equal(0, inv.GetItemCount("Old Stuff")); // Cleared
        Assert.Equal(3, inv.GetItemCount("New Ore")); // Restored
    }
}
