using Wayblazer.Core.Models;

namespace Wayblazer.Core.Systems;

/// <summary>
/// Validates and controls resource harvesting based on required methods and available tools.
/// </summary>
public class HarvestSystem
{
    private readonly HashSet<HarvestMethod> _unlockedMethods = new()
    {
        HarvestMethod.KineticMining // Always available from start
    };

    public IReadOnlySet<HarvestMethod> UnlockedMethods => _unlockedMethods;

    /// <summary>
    /// Unlocks a harvest method for the player.
    /// </summary>
    public void UnlockMethod(HarvestMethod method)
    {
        _unlockedMethods.Add(method);
    }

    /// <summary>
    /// Checks if the player can harvest a resource with their current tools.
    /// </summary>
    public bool CanHarvest(RawResource resource)
    {
        return _unlockedMethods.Contains(resource.RequiredHarvestMethod);
    }

    /// <summary>
    /// Attempts to harvest a resource, adding it to inventory if successful.
    /// Returns the amount harvested (0 if failed).
    /// </summary>
    public int TryHarvest(RawResource resource, InventorySystem inventory, int amount = 1)
    {
        if (!CanHarvest(resource))
            return 0;

        inventory.AddItem(resource.Name, amount);
        return amount;
    }

    /// <summary>
    /// Gets the required harvest method for a given resource kind.
    /// </summary>
    public static HarvestMethod GetDefaultMethodForKind(ResourceKind kind)
    {
        return kind switch
        {
            ResourceKind.Ore => HarvestMethod.KineticMining,
            ResourceKind.Wood => HarvestMethod.KineticMining,
            ResourceKind.Gas => HarvestMethod.GasSiphoning,
            _ => HarvestMethod.KineticMining
        };
    }
}
