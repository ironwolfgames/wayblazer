using Wayblazer.Core.Models;

namespace Wayblazer.Core.Systems;

/// <summary>
/// Manages portal construction as a multi-component system.
/// Each component requires a material that meets specific property thresholds.
/// </summary>
public class PortalConstructionSystem
{
    private readonly Dictionary<PortalComponentType, PortalComponentRequirement> _requirements = new();
    private readonly Dictionary<PortalComponentType, AssignedMaterial?> _assignments = new();

    public IReadOnlyDictionary<PortalComponentType, PortalComponentRequirement> Requirements => _requirements;

    /// <summary>
    /// Initializes portal requirements from planetary constants.
    /// </summary>
    public void InitializeFromPlanet(PlanetaryConstants constants)
    {
        var fullReqs = PlanetaryAnalysisSystem.CalculatePortalRequirements(constants);

        // Foundation primarily needs Strength and Toughness
        _requirements[PortalComponentType.Foundation] = new PortalComponentRequirement(
            PortalComponentType.Foundation,
            "Portal Foundation",
            new Dictionary<ResourcePropertyType, float>
            {
                [ResourcePropertyType.Strength] = fullReqs[ResourcePropertyType.Strength],
                [ResourcePropertyType.Toughness] = fullReqs[ResourcePropertyType.Toughness]
            });

        // Gate primarily needs Resistance and Conductivity
        _requirements[PortalComponentType.Gate] = new PortalComponentRequirement(
            PortalComponentType.Gate,
            "Portal Gate",
            new Dictionary<ResourcePropertyType, float>
            {
                [ResourcePropertyType.Resistance] = fullReqs[ResourcePropertyType.Resistance],
                [ResourcePropertyType.Conductivity] = fullReqs[ResourcePropertyType.Conductivity]
            });

        // Energy Core needs Reactivity and Conductivity
        _requirements[PortalComponentType.EnergyCore] = new PortalComponentRequirement(
            PortalComponentType.EnergyCore,
            "Energy Core",
            new Dictionary<ResourcePropertyType, float>
            {
                [ResourcePropertyType.Reactivity] = fullReqs[ResourcePropertyType.Reactivity],
                [ResourcePropertyType.Conductivity] = fullReqs[ResourcePropertyType.Conductivity]
            });

        // Initialize empty assignments
        foreach (var component in _requirements.Keys)
        {
            _assignments[component] = null;
        }
    }

    /// <summary>
    /// Assigns a material to a portal component.
    /// Returns true if the material meets the component's requirements.
    /// </summary>
    public bool AssignMaterial(PortalComponentType component, string materialName,
        Dictionary<ResourcePropertyType, float> materialProperties)
    {
        if (!_requirements.ContainsKey(component))
            return false;

        _assignments[component] = new AssignedMaterial(materialName, materialProperties);
        return MeetsComponentRequirements(component);
    }

    /// <summary>
    /// Removes the material assignment from a component.
    /// </summary>
    public void RemoveMaterial(PortalComponentType component)
    {
        if (_assignments.ContainsKey(component))
            _assignments[component] = null;
    }

    /// <summary>
    /// Checks if a specific component's requirements are met by its assigned material.
    /// </summary>
    public bool MeetsComponentRequirements(PortalComponentType component)
    {
        if (!_assignments.TryGetValue(component, out var assignment) || assignment == null)
            return false;
        if (!_requirements.TryGetValue(component, out var requirement))
            return false;

        return requirement.RequiredProperties.All(req =>
            assignment.Properties.GetValueOrDefault(req.Key, 0f) >= req.Value);
    }

    /// <summary>
    /// Checks if ALL portal components are assigned and meet requirements.
    /// </summary>
    public bool IsPortalComplete()
    {
        return _requirements.Keys.All(MeetsComponentRequirements);
    }

    /// <summary>
    /// Gets a status report for all components.
    /// </summary>
    public Dictionary<PortalComponentType, ComponentStatus> GetStatus()
    {
        var result = new Dictionary<PortalComponentType, ComponentStatus>();

        foreach (var (component, requirement) in _requirements)
        {
            var assignment = _assignments.GetValueOrDefault(component);
            result[component] = new ComponentStatus(
                component,
                requirement.Name,
                assignment?.MaterialName,
                assignment != null,
                assignment != null && MeetsComponentRequirements(component));
        }

        return result;
    }

    /// <summary>
    /// Gets the number of components that have been assigned and pass requirements.
    /// </summary>
    public (int completed, int total) GetCompletionProgress()
    {
        int total = _requirements.Count;
        int completed = _requirements.Keys.Count(MeetsComponentRequirements);
        return (completed, total);
    }
}

public class PortalComponentRequirement
{
    public PortalComponentType Component { get; }
    public string Name { get; }
    public Dictionary<ResourcePropertyType, float> RequiredProperties { get; }

    public PortalComponentRequirement(PortalComponentType component, string name,
        Dictionary<ResourcePropertyType, float> requiredProperties)
    {
        Component = component;
        Name = name;
        RequiredProperties = requiredProperties;
    }
}

public class AssignedMaterial
{
    public string MaterialName { get; }
    public Dictionary<ResourcePropertyType, float> Properties { get; }

    public AssignedMaterial(string materialName, Dictionary<ResourcePropertyType, float> properties)
    {
        MaterialName = materialName;
        Properties = properties;
    }
}

public class ComponentStatus
{
    public PortalComponentType Component { get; }
    public string ComponentName { get; }
    public string? AssignedMaterialName { get; }
    public bool HasMaterial { get; }
    public bool MeetsRequirements { get; }

    public ComponentStatus(PortalComponentType component, string componentName,
        string? assignedMaterialName, bool hasMaterial, bool meetsRequirements)
    {
        Component = component;
        ComponentName = componentName;
        AssignedMaterialName = assignedMaterialName;
        HasMaterial = hasMaterial;
        MeetsRequirements = meetsRequirements;
    }
}
