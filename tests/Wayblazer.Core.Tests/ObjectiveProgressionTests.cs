using Wayblazer.Core.Models;
using Wayblazer.Core.Systems;

namespace Wayblazer.Core.Tests;

/// <summary>
/// Tests for the objective/progression system.
/// Validates objective tracking, advancement, and completion chain.
/// </summary>
public class ObjectiveProgressionTests
{
    [Fact]
    public void CreateVerticalSlice_StartsWithHarvestWood()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        Assert.Equal("harvest_wood", objectives.CurrentObjectiveId);
        Assert.Equal("Harvest 5 Soft Wood to calibrate sensors.", objectives.GetCurrentDescription());
    }

    [Fact]
    public void ReportProgress_MatchingEvent_AdvancesProgress()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 2);
        var (current, required) = objectives.GetCurrentProgress();

        Assert.Equal(2, current);
        Assert.Equal(5, required);
    }

    [Fact]
    public void ReportProgress_WrongEvent_DoesNotAdvance()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Base Ore", 3);
        var (current, _) = objectives.GetCurrentProgress();

        Assert.Equal(0, current);
    }

    [Fact]
    public void ReportProgress_WrongType_DoesNotAdvance()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        objectives.ReportProgress(ObjectiveConditionType.ScanResource, "Soft Wood", 5);
        var (current, _) = objectives.GetCurrentProgress();

        Assert.Equal(0, current);
    }

    [Fact]
    public void ReportProgress_CompletesObjective_AdvancesToNext()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        bool completed = objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 5);

        Assert.True(completed);
        Assert.Equal("scan_ore", objectives.CurrentObjectiveId);
        Assert.True(objectives.IsObjectiveCompleted("harvest_wood"));
    }

    [Fact]
    public void ReportProgress_IncrementalCompletion()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        Assert.False(objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 2));
        Assert.False(objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 2));
        Assert.True(objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 1));

        Assert.Equal("scan_ore", objectives.CurrentObjectiveId);
    }

    [Fact]
    public void DuplicateEvent_DoesNotDoubleAdvance()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        // Complete first objective
        objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 5);
        Assert.Equal("scan_ore", objectives.CurrentObjectiveId);

        // Report same event again - should not affect new objective
        objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 5);
        Assert.Equal("scan_ore", objectives.CurrentObjectiveId);
    }

    [Fact]
    public void FullChain_CompletesAllObjectives()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 5);
        Assert.Equal("scan_ore", objectives.CurrentObjectiveId);

        objectives.ReportProgress(ObjectiveConditionType.ScanResource, "Base Ore", 1);
        Assert.Equal("analyze_resources", objectives.CurrentObjectiveId);

        objectives.ReportProgress(ObjectiveConditionType.AnalyzeResource, "any", 3);
        Assert.Equal("measure_planet", objectives.CurrentObjectiveId);

        objectives.ReportProgress(ObjectiveConditionType.MeasureConstant, "Gravity", 1);
        Assert.Equal("craft_metal", objectives.CurrentObjectiveId);

        objectives.ReportProgress(ObjectiveConditionType.CraftItem, "Metal Bar", 1);
        Assert.Equal("craft_alloy", objectives.CurrentObjectiveId);

        objectives.ReportProgress(ObjectiveConditionType.CraftItem, "Composite Alloy", 1);
        Assert.Equal("build_portal", objectives.CurrentObjectiveId);

        objectives.ReportProgress(ObjectiveConditionType.BuildPortalComponent, "Foundation", 1);
        Assert.Equal("run_simulation", objectives.CurrentObjectiveId);

        objectives.ReportProgress(ObjectiveConditionType.PassSimulation, "Portal", 1);
        Assert.Null(objectives.CurrentObjectiveId); // All objectives complete
    }

    [Fact]
    public void CompletedObjectives_TrackedCorrectly()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 5);
        objectives.ReportProgress(ObjectiveConditionType.ScanResource, "Base Ore", 1);

        Assert.True(objectives.IsObjectiveCompleted("harvest_wood"));
        Assert.True(objectives.IsObjectiveCompleted("scan_ore"));
        Assert.False(objectives.IsObjectiveCompleted("analyze_resources"));
    }

    [Fact]
    public void OnObjectiveCompleted_EventFires()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();
        string? completedId = null;
        objectives.OnObjectiveCompleted += id => completedId = id;

        objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 5);

        Assert.Equal("harvest_wood", completedId);
    }

    [Fact]
    public void ProgressDoesNotExceedRequired()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 100);
        // Should complete, not overflow
        Assert.Equal("scan_ore", objectives.CurrentObjectiveId);
    }

    [Fact]
    public void SetCurrentObjective_InvalidId_Throws()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();

        Assert.Throws<ArgumentException>(() => objectives.SetCurrentObjective("nonexistent"));
    }

    [Fact]
    public void GetCurrentProgress_NoObjective_ReturnsZero()
    {
        var objectives = new ObjectiveSystem();

        var (current, required) = objectives.GetCurrentProgress();

        Assert.Equal(0, current);
        Assert.Equal(0, required);
    }
}
