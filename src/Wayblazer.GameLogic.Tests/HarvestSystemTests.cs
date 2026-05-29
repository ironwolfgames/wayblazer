using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Tests for the harvest method gating system.
/// Validates that resources require specific tools and that locked methods prevent harvesting.
/// </summary>
public class HarvestSystemTests
{
    private static RawResource CreateOre() => new(
        "Iron Ore", ResourceKind.Ore,
        new Dictionary<ResourcePropertyType, ResourceProperty>
        {
            [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 3.0f)
        },
        HarvestMethod.KineticMining);

    private static RawResource CreateGas() => new(
        "Xenon Gas", ResourceKind.Gas,
        new Dictionary<ResourcePropertyType, ResourceProperty>
        {
            [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 8.0f)
        },
        HarvestMethod.GasSiphoning);

    private static RawResource CreateCrystal() => new(
        "Energy Crystal", ResourceKind.Ore,
        new Dictionary<ResourcePropertyType, ResourceProperty>
        {
            [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 9.0f)
        },
        HarvestMethod.ResonanceShattering);

    [Fact]
    public void InitialState_OnlyKineticMiningAvailable()
    {
        var harvest = new HarvestSystem();

        Assert.Contains(HarvestMethod.KineticMining, harvest.UnlockedMethods);
        Assert.Single(harvest.UnlockedMethods);
    }

    [Fact]
    public void CanHarvest_WithCorrectMethod_ReturnsTrue()
    {
        var harvest = new HarvestSystem();
        var ore = CreateOre();

        Assert.True(harvest.CanHarvest(ore));
    }

    [Fact]
    public void CanHarvest_WithLockedMethod_ReturnsFalse()
    {
        var harvest = new HarvestSystem();
        var gas = CreateGas();

        Assert.False(harvest.CanHarvest(gas));
    }

    [Fact]
    public void UnlockMethod_EnablesHarvesting()
    {
        var harvest = new HarvestSystem();
        var gas = CreateGas();

        Assert.False(harvest.CanHarvest(gas));

        harvest.UnlockMethod(HarvestMethod.GasSiphoning);

        Assert.True(harvest.CanHarvest(gas));
    }

    [Fact]
    public void TryHarvest_Success_AddsToInventory()
    {
        var harvest = new HarvestSystem();
        var inventory = new InventorySystem();
        var ore = CreateOre();

        int amount = harvest.TryHarvest(ore, inventory, 3);

        Assert.Equal(3, amount);
        Assert.Equal(3, inventory.GetItemCount("Iron Ore"));
    }

    [Fact]
    public void TryHarvest_LockedMethod_FailsAndDoesNotAddToInventory()
    {
        var harvest = new HarvestSystem();
        var inventory = new InventorySystem();
        var gas = CreateGas();

        int amount = harvest.TryHarvest(gas, inventory, 1);

        Assert.Equal(0, amount);
        Assert.Equal(0, inventory.GetItemCount("Xenon Gas"));
    }

    [Fact]
    public void TryHarvest_AfterUnlock_Succeeds()
    {
        var harvest = new HarvestSystem();
        var inventory = new InventorySystem();
        var crystal = CreateCrystal();

        // Fail first
        Assert.Equal(0, harvest.TryHarvest(crystal, inventory, 1));

        // Unlock method
        harvest.UnlockMethod(HarvestMethod.ResonanceShattering);

        // Succeed now
        Assert.Equal(1, harvest.TryHarvest(crystal, inventory, 1));
        Assert.Equal(1, inventory.GetItemCount("Energy Crystal"));
    }

    [Fact]
    public void MultipleUnlocks_AllWork()
    {
        var harvest = new HarvestSystem();

        harvest.UnlockMethod(HarvestMethod.GasSiphoning);
        harvest.UnlockMethod(HarvestMethod.ThermalLancing);
        harvest.UnlockMethod(HarvestMethod.ResonanceShattering);

        Assert.Contains(HarvestMethod.KineticMining, harvest.UnlockedMethods);
        Assert.Contains(HarvestMethod.GasSiphoning, harvest.UnlockedMethods);
        Assert.Contains(HarvestMethod.ThermalLancing, harvest.UnlockedMethods);
        Assert.Contains(HarvestMethod.ResonanceShattering, harvest.UnlockedMethods);
    }

    [Fact]
    public void GetDefaultMethodForKind_ReturnsExpectedMethods()
    {
        Assert.Equal(HarvestMethod.KineticMining, HarvestSystem.GetDefaultMethodForKind(ResourceKind.Ore));
        Assert.Equal(HarvestMethod.KineticMining, HarvestSystem.GetDefaultMethodForKind(ResourceKind.Wood));
        Assert.Equal(HarvestMethod.GasSiphoning, HarvestSystem.GetDefaultMethodForKind(ResourceKind.Gas));
    }

    [Fact]
    public void DuplicateUnlock_DoesNotThrow()
    {
        var harvest = new HarvestSystem();

        harvest.UnlockMethod(HarvestMethod.GasSiphoning);
        harvest.UnlockMethod(HarvestMethod.GasSiphoning); // Duplicate

        Assert.Contains(HarvestMethod.GasSiphoning, harvest.UnlockedMethods);
    }
}
