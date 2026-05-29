namespace Wayblazer.GameLogic.Models;

/// <summary>
/// Represents a single objective in the progression system.
/// </summary>
public class Objective
{
    public string Id { get; }
    public string Description { get; }
    public ObjectiveCondition Condition { get; }
    public string? NextObjectiveId { get; }

    public Objective(string id, string description, ObjectiveCondition condition, string? nextObjectiveId = null)
    {
        Id = id;
        Description = description;
        Condition = condition;
        NextObjectiveId = nextObjectiveId;
    }
}

/// <summary>
/// A condition that must be met for an objective to be considered complete.
/// </summary>
public class ObjectiveCondition
{
    public ObjectiveConditionType Type { get; }
    public string TargetName { get; }
    public int RequiredAmount { get; }

    public ObjectiveCondition(ObjectiveConditionType type, string targetName, int requiredAmount = 1)
    {
        Type = type;
        TargetName = targetName;
        RequiredAmount = requiredAmount;
    }
}

public enum ObjectiveConditionType
{
    HarvestResource,
    ScanResource,
    AnalyzeResource,
    CraftItem,
    UnlockTech,
    MeasureConstant,
    BuildPortalComponent,
    PassSimulation
}
