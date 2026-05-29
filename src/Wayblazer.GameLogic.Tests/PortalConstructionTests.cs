using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Tests for the multi-component portal construction system.
/// Validates that portal requires multiple materials assigned to different components.
/// </summary>
public class PortalConstructionTests
{
    private static PortalConstructionSystem CreateDefaultPortal()
    {
        var portal = new PortalConstructionSystem();
        portal.InitializeFromPlanet(PlanetaryConstants.Default);
        return portal;
    }

    [Fact]
    public void InitializeFromPlanet_CreatesThreeComponents()
    {
        var portal = CreateDefaultPortal();

        Assert.Equal(3, portal.Requirements.Count);
        Assert.Contains(PortalComponentType.Foundation, portal.Requirements.Keys);
        Assert.Contains(PortalComponentType.Gate, portal.Requirements.Keys);
        Assert.Contains(PortalComponentType.EnergyCore, portal.Requirements.Keys);
    }

    [Fact]
    public void Foundation_RequiresStrengthAndToughness()
    {
        var portal = CreateDefaultPortal();

        var foundationReqs = portal.Requirements[PortalComponentType.Foundation].RequiredProperties;
        Assert.Contains(ResourcePropertyType.Strength, foundationReqs.Keys);
        Assert.Contains(ResourcePropertyType.Toughness, foundationReqs.Keys);
        Assert.Equal(8.0f, foundationReqs[ResourcePropertyType.Strength]); // 3.2 × 2.5
    }

    [Fact]
    public void Gate_RequiresResistanceAndConductivity()
    {
        var portal = CreateDefaultPortal();

        var gateReqs = portal.Requirements[PortalComponentType.Gate].RequiredProperties;
        Assert.Contains(ResourcePropertyType.Resistance, gateReqs.Keys);
        Assert.Contains(ResourcePropertyType.Conductivity, gateReqs.Keys);
    }

    [Fact]
    public void EnergyCore_RequiresReactivityAndConductivity()
    {
        var portal = CreateDefaultPortal();

        var coreReqs = portal.Requirements[PortalComponentType.EnergyCore].RequiredProperties;
        Assert.Contains(ResourcePropertyType.Reactivity, coreReqs.Keys);
        Assert.Contains(ResourcePropertyType.Conductivity, coreReqs.Keys);
    }

    [Fact]
    public void AssignMaterial_MeetsRequirements_ReturnsTrue()
    {
        var portal = CreateDefaultPortal();

        var strongMaterial = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        };

        bool meetsReqs = portal.AssignMaterial(PortalComponentType.Foundation, "Super Alloy", strongMaterial);

        Assert.True(meetsReqs);
    }

    [Fact]
    public void AssignMaterial_DoesNotMeetRequirements_ReturnsFalse()
    {
        var portal = CreateDefaultPortal();

        var weakMaterial = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 2.0f,
            [ResourcePropertyType.Toughness] = 1.0f
        };

        bool meetsReqs = portal.AssignMaterial(PortalComponentType.Foundation, "Weak Alloy", weakMaterial);

        Assert.False(meetsReqs);
    }

    [Fact]
    public void IsPortalComplete_AllComponentsMet_ReturnsTrue()
    {
        var portal = CreateDefaultPortal();

        // Foundation needs Strength >= 8.0, Toughness >= 6.5
        portal.AssignMaterial(PortalComponentType.Foundation, "Foundation Alloy", new()
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        });

        // Gate needs Resistance >= 4.5, Conductivity >= 6.56
        portal.AssignMaterial(PortalComponentType.Gate, "Gate Alloy", new()
        {
            [ResourcePropertyType.Resistance] = 5.0f,
            [ResourcePropertyType.Conductivity] = 7.0f
        });

        // Energy Core needs Reactivity >= 3.75, Conductivity >= 6.56
        portal.AssignMaterial(PortalComponentType.EnergyCore, "Core Alloy", new()
        {
            [ResourcePropertyType.Reactivity] = 4.0f,
            [ResourcePropertyType.Conductivity] = 7.0f
        });

        Assert.True(portal.IsPortalComplete());
    }

    [Fact]
    public void IsPortalComplete_MissingComponent_ReturnsFalse()
    {
        var portal = CreateDefaultPortal();

        // Only assign Foundation
        portal.AssignMaterial(PortalComponentType.Foundation, "Alloy", new()
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        });

        Assert.False(portal.IsPortalComplete());
    }

    [Fact]
    public void IsPortalComplete_OneComponentFails_ReturnsFalse()
    {
        var portal = CreateDefaultPortal();

        portal.AssignMaterial(PortalComponentType.Foundation, "Good Alloy", new()
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        });

        portal.AssignMaterial(PortalComponentType.Gate, "Weak Gate", new()
        {
            [ResourcePropertyType.Resistance] = 1.0f, // Too low!
            [ResourcePropertyType.Conductivity] = 7.0f
        });

        portal.AssignMaterial(PortalComponentType.EnergyCore, "Core", new()
        {
            [ResourcePropertyType.Reactivity] = 4.0f,
            [ResourcePropertyType.Conductivity] = 7.0f
        });

        Assert.False(portal.IsPortalComplete());
    }

    [Fact]
    public void RemoveMaterial_ClearsAssignment()
    {
        var portal = CreateDefaultPortal();

        portal.AssignMaterial(PortalComponentType.Foundation, "Alloy", new()
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        });

        Assert.True(portal.MeetsComponentRequirements(PortalComponentType.Foundation));

        portal.RemoveMaterial(PortalComponentType.Foundation);

        Assert.False(portal.MeetsComponentRequirements(PortalComponentType.Foundation));
    }

    [Fact]
    public void GetStatus_ReportsCorrectState()
    {
        var portal = CreateDefaultPortal();

        portal.AssignMaterial(PortalComponentType.Foundation, "Good Alloy", new()
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        });

        var status = portal.GetStatus();

        Assert.True(status[PortalComponentType.Foundation].HasMaterial);
        Assert.True(status[PortalComponentType.Foundation].MeetsRequirements);
        Assert.Equal("Good Alloy", status[PortalComponentType.Foundation].AssignedMaterialName);

        Assert.False(status[PortalComponentType.Gate].HasMaterial);
        Assert.False(status[PortalComponentType.Gate].MeetsRequirements);
    }

    [Fact]
    public void GetCompletionProgress_TracksCorrectly()
    {
        var portal = CreateDefaultPortal();

        var (completed, total) = portal.GetCompletionProgress();
        Assert.Equal(0, completed);
        Assert.Equal(3, total);

        portal.AssignMaterial(PortalComponentType.Foundation, "Alloy", new()
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        });

        (completed, total) = portal.GetCompletionProgress();
        Assert.Equal(1, completed);
        Assert.Equal(3, total);
    }

    [Fact]
    public void ReassignMaterial_OverwritesPrevious()
    {
        var portal = CreateDefaultPortal();

        // Assign weak material
        portal.AssignMaterial(PortalComponentType.Foundation, "Weak", new()
        {
            [ResourcePropertyType.Strength] = 2.0f,
            [ResourcePropertyType.Toughness] = 1.0f
        });
        Assert.False(portal.MeetsComponentRequirements(PortalComponentType.Foundation));

        // Reassign strong material
        portal.AssignMaterial(PortalComponentType.Foundation, "Strong", new()
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        });
        Assert.True(portal.MeetsComponentRequirements(PortalComponentType.Foundation));
    }

    [Fact]
    public void DifferentPlanets_ProduceDifferentRequirements()
    {
        var portal1 = new PortalConstructionSystem();
        portal1.InitializeFromPlanet(PlanetaryConstants.Default);

        var portal2 = new PortalConstructionSystem();
        portal2.InitializeFromPlanet(new PlanetaryConstants(9.0f, 8.0f, 2.0f, 3.0f, -50f, 80f));

        var strength1 = portal1.Requirements[PortalComponentType.Foundation].RequiredProperties[ResourcePropertyType.Strength];
        var strength2 = portal2.Requirements[PortalComponentType.Foundation].RequiredProperties[ResourcePropertyType.Strength];

        Assert.True(strength2 > strength1, "Higher gravity planet should require more strength");
    }
}
