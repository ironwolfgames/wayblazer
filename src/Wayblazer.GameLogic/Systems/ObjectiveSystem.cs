using Wayblazer.GameLogic.Models;

namespace Wayblazer.GameLogic.Systems;

/// <summary>
/// Manages player objectives and progression through the game.
/// Objectives form a linear chain in the vertical slice.
/// </summary>
public class ObjectiveSystem
{
    private readonly Dictionary<string, Objective> _objectives = new();
    private readonly Dictionary<string, int> _progress = new(); // objectiveId → current progress

    public string? CurrentObjectiveId { get; private set; }
    public IReadOnlyList<string> CompletedObjectives => _completedObjectives;
    private readonly List<string> _completedObjectives = new();

    public event Action<string>? OnObjectiveCompleted;
    public event Action<string>? OnObjectiveAdvanced;

    /// <summary>
    /// Registers an objective in the system.
    /// </summary>
    public void RegisterObjective(Objective objective)
    {
        _objectives[objective.Id] = objective;
    }

    /// <summary>
    /// Sets the current active objective.
    /// </summary>
    public void SetCurrentObjective(string objectiveId)
    {
        if (!_objectives.ContainsKey(objectiveId))
            throw new ArgumentException($"Unknown objective: {objectiveId}");

        CurrentObjectiveId = objectiveId;
        if (!_progress.ContainsKey(objectiveId))
            _progress[objectiveId] = 0;

        OnObjectiveAdvanced?.Invoke(objectiveId);
    }

    /// <summary>
    /// Reports progress toward the current objective.
    /// Returns true if the objective was completed.
    /// </summary>
    public bool ReportProgress(ObjectiveConditionType eventType, string targetName, int amount = 1)
    {
        if (CurrentObjectiveId == null) return false;
        if (!_objectives.TryGetValue(CurrentObjectiveId, out var objective)) return false;

        var condition = objective.Condition;
        if (condition.Type != eventType || condition.TargetName != targetName)
            return false;

        _progress[CurrentObjectiveId] = Math.Min(
            _progress.GetValueOrDefault(CurrentObjectiveId, 0) + amount,
            condition.RequiredAmount);

        if (_progress[CurrentObjectiveId] >= condition.RequiredAmount)
        {
            CompleteCurrentObjective();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the progress of the current objective (current/required).
    /// </summary>
    public (int current, int required) GetCurrentProgress()
    {
        if (CurrentObjectiveId == null) return (0, 0);
        if (!_objectives.TryGetValue(CurrentObjectiveId, out var objective)) return (0, 0);

        return (_progress.GetValueOrDefault(CurrentObjectiveId, 0), objective.Condition.RequiredAmount);
    }

    /// <summary>
    /// Gets the description of the current objective.
    /// </summary>
    public string? GetCurrentDescription()
    {
        if (CurrentObjectiveId == null) return null;
        return _objectives.TryGetValue(CurrentObjectiveId, out var obj) ? obj.Description : null;
    }

    /// <summary>
    /// Checks if a specific objective has been completed.
    /// </summary>
    public bool IsObjectiveCompleted(string objectiveId) => _completedObjectives.Contains(objectiveId);

    private void CompleteCurrentObjective()
    {
        if (CurrentObjectiveId == null) return;

        var completedId = CurrentObjectiveId;
        _completedObjectives.Add(completedId);
        OnObjectiveCompleted?.Invoke(completedId);

        // Advance to next objective if one is defined
        if (_objectives.TryGetValue(completedId, out var objective) && objective.NextObjectiveId != null)
        {
            SetCurrentObjective(objective.NextObjectiveId);
        }
        else
        {
            CurrentObjectiveId = null;
        }
    }

    /// <summary>
    /// Creates the standard vertical slice objective chain.
    /// </summary>
    public static ObjectiveSystem CreateVerticalSliceObjectives()
    {
        var system = new ObjectiveSystem();

        system.RegisterObjective(new Objective(
            "harvest_wood", "Harvest 5 Soft Wood to calibrate sensors.",
            new ObjectiveCondition(ObjectiveConditionType.HarvestResource, "Soft Wood", 5),
            "scan_ore"));

        system.RegisterObjective(new Objective(
            "scan_ore", "Scan Base Ore with the Hand Scanner.",
            new ObjectiveCondition(ObjectiveConditionType.ScanResource, "Base Ore", 1),
            "analyze_resources"));

        system.RegisterObjective(new Objective(
            "analyze_resources", "Analyze 3 resources in the Field Lab.",
            new ObjectiveCondition(ObjectiveConditionType.AnalyzeResource, "any", 3),
            "measure_planet"));

        system.RegisterObjective(new Objective(
            "measure_planet", "Measure Gravity at the Observatory.",
            new ObjectiveCondition(ObjectiveConditionType.MeasureConstant, "Gravity", 1),
            "craft_metal"));

        system.RegisterObjective(new Objective(
            "craft_metal", "Smelt a Metal Bar at the Furnace.",
            new ObjectiveCondition(ObjectiveConditionType.CraftItem, "Metal Bar", 1),
            "craft_alloy"));

        system.RegisterObjective(new Objective(
            "craft_alloy", "Synthesize a Composite Alloy at the Gas Injector.",
            new ObjectiveCondition(ObjectiveConditionType.CraftItem, "Composite Alloy", 1),
            "build_portal"));

        system.RegisterObjective(new Objective(
            "build_portal", "Assign materials to the Portal Foundation.",
            new ObjectiveCondition(ObjectiveConditionType.BuildPortalComponent, "Foundation", 1),
            "run_simulation"));

        system.RegisterObjective(new Objective(
            "run_simulation", "Run the Portal Simulation and achieve PASS.",
            new ObjectiveCondition(ObjectiveConditionType.PassSimulation, "Portal", 1),
            null));

        system.SetCurrentObjective("harvest_wood");
        return system;
    }
}
