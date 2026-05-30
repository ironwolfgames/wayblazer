using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Tech Tree & Unlocks — deterministic acceptance tests.
/// Tests knowledge point accumulation, prerequisite gating, and machine unlocking.
/// </summary>
public class Sprint09_TechTreeTests
{
    [Fact]
    public void InitialState_NoNodesUnlocked()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        Assert.Empty(tree.UnlockedNodes);
    }

    [Fact]
    public void AddKnowledgePoints_TracksCorrectly()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 20);
        tree.AddKnowledgePoints(KnowledgeCategory.Smelting, 5);

        Assert.Equal(20, tree.KnowledgePoints[KnowledgeCategory.Analysis]);
        Assert.Equal(5, tree.KnowledgePoints[KnowledgeCategory.Smelting]);
        Assert.Equal(0, tree.KnowledgePoints[KnowledgeCategory.Compositing]);
    }

    [Fact]
    public void TotalKnowledgePoints_SumsAllCategories()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 10);
        tree.AddKnowledgePoints(KnowledgeCategory.Smelting, 5);
        tree.AddKnowledgePoints(KnowledgeCategory.Compositing, 3);

        Assert.Equal(18, tree.TotalKnowledgePoints);
    }

    [Fact]
    public void CanUnlock_FieldLab_With15AnalysisPoints()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 15);

        Assert.True(tree.CanUnlock("Unlock_FieldLab"));
    }

    [Fact]
    public void CanUnlock_FieldLab_FailsWithInsufficientPoints()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 14);

        Assert.False(tree.CanUnlock("Unlock_FieldLab"));
    }

    [Fact]
    public void TryUnlock_SpendsPoints()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 20);

        tree.TryUnlock("Unlock_FieldLab"); // Costs 15

        Assert.Equal(5, tree.KnowledgePoints[KnowledgeCategory.Analysis]);
    }

    [Fact]
    public void TryUnlock_AddsToUnlockedSet()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 15);

        bool success = tree.TryUnlock("Unlock_FieldLab");

        Assert.True(success);
        Assert.Contains("Unlock_FieldLab", tree.UnlockedNodes);
    }

    [Fact]
    public void CanUnlock_GasInjector_RequiresFieldLabPrerequisite()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        // Give plenty of points but don't unlock FieldLab
        tree.AddKnowledgePoints(KnowledgeCategory.Smelting, 50);
        tree.AddKnowledgePoints(KnowledgeCategory.Compositing, 50);

        Assert.False(tree.CanUnlock("Unlock_GasInjector"));
    }

    [Fact]
    public void CanUnlock_GasInjector_PassesWithPrerequisiteAndPoints()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        // Unlock FieldLab first
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 15);
        tree.TryUnlock("Unlock_FieldLab");

        // Now add compositing/smelting points
        tree.AddKnowledgePoints(KnowledgeCategory.Smelting, 10);
        tree.AddKnowledgePoints(KnowledgeCategory.Compositing, 10);

        Assert.True(tree.CanUnlock("Unlock_GasInjector"));
    }

    [Fact]
    public void CanUnlock_AlreadyUnlocked_ReturnsFalse()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 30);
        tree.TryUnlock("Unlock_FieldLab");

        Assert.False(tree.CanUnlock("Unlock_FieldLab"));
    }

    [Fact]
    public void CanUnlock_NonExistentNode_ReturnsFalse()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        Assert.False(tree.CanUnlock("FakeNode"));
    }

    [Fact]
    public void GetUnlockedMachines_InitiallyOnlyHandScanner()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        var machines = tree.GetUnlockedMachines();

        Assert.Contains("HandScanner", machines);
        Assert.Single(machines);
    }

    [Fact]
    public void GetUnlockedMachines_AfterFieldLabUnlock_IncludesFieldLab()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 15);
        tree.TryUnlock("Unlock_FieldLab");

        var machines = tree.GetUnlockedMachines();

        Assert.Contains("FieldLab", machines);
        Assert.Contains("HandScanner", machines);
    }

    [Fact]
    public void FullUnlockChain_VerticalSlice()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();

        // Phase 1: Scanning earns analysis points → unlock FieldLab
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 15);
        Assert.True(tree.TryUnlock("Unlock_FieldLab"));

        // Phase 2: More analysis → unlock Observatory
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 25);
        Assert.True(tree.TryUnlock("Unlock_Observatory"));

        // Phase 3: Analysis for Furnace
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 10);
        Assert.True(tree.TryUnlock("Unlock_Furnace"));

        // Phase 4: Smelting and compositing → unlock Gas Injector
        tree.AddKnowledgePoints(KnowledgeCategory.Smelting, 10);
        tree.AddKnowledgePoints(KnowledgeCategory.Compositing, 10);
        Assert.True(tree.TryUnlock("Unlock_GasInjector"));

        // Phase 5: More of everything → unlock SimCore
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 15);
        tree.AddKnowledgePoints(KnowledgeCategory.Smelting, 10);
        tree.AddKnowledgePoints(KnowledgeCategory.Compositing, 10);
        Assert.True(tree.TryUnlock("Unlock_SimCore"));

        // Phase 6: Final unlock → Portal
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 10);
        tree.AddKnowledgePoints(KnowledgeCategory.Smelting, 10);
        tree.AddKnowledgePoints(KnowledgeCategory.Compositing, 10);
        Assert.True(tree.TryUnlock("Unlock_Portal"));

        // All machines should now be available
        var machines = tree.GetUnlockedMachines();
        Assert.Contains("FieldLab", machines);
        Assert.Contains("Observatory", machines);
        Assert.Contains("Furnace", machines);
        Assert.Contains("GasInjector", machines);
        Assert.Contains("SimulationCore", machines);
        Assert.Contains("PortalFoundation", machines);
    }

    [Fact]
    public void TryUnlock_InsufficientPoints_ReturnsFalse_KeepsPoints()
    {
        var tree = TechTreeSystem.CreateVerticalSliceTree();
        tree.AddKnowledgePoints(KnowledgeCategory.Analysis, 10);

        bool success = tree.TryUnlock("Unlock_FieldLab"); // Costs 15

        Assert.False(success);
        Assert.Equal(10, tree.KnowledgePoints[KnowledgeCategory.Analysis]); // Unchanged
    }
}
