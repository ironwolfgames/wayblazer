using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Sprint 8: Gas Injector & Compositing — deterministic acceptance tests.
/// Tests the advanced material synthesis that creates high-stat alloys.
/// </summary>
public class Sprint08_CompositingTests
{
    private static readonly HashSet<string> AllMachines = new() { "Furnace", "GasInjector", "HandScanner" };

    private static CraftingSystem CreateCompositingSystem()
    {
        var system = new CraftingSystem();

        // Smelting recipe
        system.RegisterRecipe(new CraftingRecipe(
            "smelt_metal_bar",
            "Smelt Metal Bar",
            new List<RecipeIngredient> { new("Base Ore", 2), new("Soft Wood", 1) },
            "Metal Bar",
            ResourceKind.Composite,
            "Furnace"));

        // Compositing recipe: Metal Bar + Catalyst Ore → Composite Alloy
        system.RegisterRecipe(new CraftingRecipe(
            "composite_alloy",
            "Synthesize Composite Alloy",
            new List<RecipeIngredient> { new("Metal Bar", 1), new("Catalyst Ore", 1) },
            "Composite Alloy",
            ResourceKind.Composite,
            "GasInjector"));

        return system;
    }

    private static Dictionary<string, RawResource> CreateResourceDatabase()
    {
        return new Dictionary<string, RawResource>
        {
            ["Metal Bar"] = new("Metal Bar", ResourceKind.Composite,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 3.0f),
                    [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 5.0f),
                    [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 3.0f)
                }),
            ["Catalyst Ore"] = new("Catalyst Ore", ResourceKind.Ore,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 4.0f),
                    [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 2.0f),
                    [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 8.0f),
                    [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 9.0f)
                })
        };
    }

    [Fact]
    public void CompositeAlloy_CanBeCrafted_WithGasInjector()
    {
        var crafting = CreateCompositingSystem();
        var inventory = new InventorySystem();
        inventory.AddItem("Metal Bar", 1);
        inventory.AddItem("Catalyst Ore", 1);

        Assert.True(crafting.CanCraft("composite_alloy", inventory, AllMachines));
    }

    [Fact]
    public void CompositeAlloy_RequiresGasInjector()
    {
        var crafting = CreateCompositingSystem();
        var inventory = new InventorySystem();
        inventory.AddItem("Metal Bar", 1);
        inventory.AddItem("Catalyst Ore", 1);

        var furnaceOnly = new HashSet<string> { "Furnace" };
        Assert.False(crafting.CanCraft("composite_alloy", inventory, furnaceOnly));
    }

    [Fact]
    public void CompositeAlloy_CombinesStrength_Additively()
    {
        var crafting = CreateCompositingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Metal Bar", 1);
        inventory.AddItem("Catalyst Ore", 1);

        var result = crafting.Craft("composite_alloy", inventory, AllMachines, db)!;

        // Metal Bar Strength=3.0 + Catalyst Ore Strength=4.0 = 7.0
        Assert.Equal(7.0f, result.Properties[ResourcePropertyType.Strength]);
    }

    [Fact]
    public void CompositeAlloy_EarnsCompositingPoints()
    {
        var crafting = CreateCompositingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Metal Bar", 1);
        inventory.AddItem("Catalyst Ore", 1);

        var result = crafting.Craft("composite_alloy", inventory, AllMachines, db)!;

        Assert.Equal(KnowledgeCategory.Compositing, result.KnowledgeCategoryEarned);
        Assert.True(result.PointsEarned > 0);
    }

    [Fact]
    public void CompositeAlloy_ResistanceCombines_AsMinWithBoost()
    {
        var crafting = CreateCompositingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Metal Bar", 1);
        inventory.AddItem("Catalyst Ore", 1);

        var result = crafting.Craft("composite_alloy", inventory, AllMachines, db)!;

        // Resistance: Min(5.0, 2.0) × 1.75 = 2.0 × 1.75 = 3.5
        Assert.Equal(3.5f, result.Properties[ResourcePropertyType.Resistance]);
    }

    [Fact]
    public void CompositeAlloy_ProducesOutputInInventory()
    {
        var crafting = CreateCompositingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Metal Bar", 1);
        inventory.AddItem("Catalyst Ore", 1);

        crafting.Craft("composite_alloy", inventory, AllMachines, db);

        Assert.Equal(1, inventory.GetItemCount("Composite Alloy"));
        Assert.Equal(0, inventory.GetItemCount("Metal Bar"));
        Assert.Equal(0, inventory.GetItemCount("Catalyst Ore"));
    }

    [Fact]
    public void FullCraftingChain_OreToComposite()
    {
        // This tests the full vertical slice crafting chain:
        // Base Ore + Wood → Metal Bar → Metal Bar + Catalyst → Composite Alloy
        var crafting = CreateCompositingSystem();
        var inventory = new InventorySystem();
        var db = new Dictionary<string, RawResource>
        {
            ["Base Ore"] = new("Base Ore", ResourceKind.Ore,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f)
                }),
            ["Soft Wood"] = new("Soft Wood", ResourceKind.Wood,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 1.0f)
                }),
            ["Metal Bar"] = new("Metal Bar", ResourceKind.Composite,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 3.0f)
                }),
            ["Catalyst Ore"] = new("Catalyst Ore", ResourceKind.Ore,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 5.0f)
                })
        };

        // Start with raw materials
        inventory.AddItem("Base Ore", 2);
        inventory.AddItem("Soft Wood", 1);
        inventory.AddItem("Catalyst Ore", 1);

        // Step 1: Smelt
        var smeltResult = crafting.Craft("smelt_metal_bar", inventory, AllMachines, db);
        Assert.NotNull(smeltResult);
        Assert.Equal(1, inventory.GetItemCount("Metal Bar"));

        // Step 2: Composite
        var compositeResult = crafting.Craft("composite_alloy", inventory, AllMachines, db);
        Assert.NotNull(compositeResult);
        Assert.Equal(1, inventory.GetItemCount("Composite Alloy"));

        // Metal Bar strength (3.0) + Catalyst strength (5.0) = 8.0
        Assert.Equal(8.0f, compositeResult.Properties[ResourcePropertyType.Strength]);
    }
}
