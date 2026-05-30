using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Basic Furnace & Smelting — deterministic acceptance tests.
/// Tests the basic crafting/smelting mechanic.
/// </summary>
public class Sprint07_SmeltingTests
{
    private static readonly HashSet<string> UnlockedMachines = new() { "Furnace", "HandScanner" };

    private static CraftingSystem CreateSmeltingSystem()
    {
        var system = new CraftingSystem();
        system.RegisterRecipe(new CraftingRecipe(
            "smelt_metal_bar",
            "Smelt Metal Bar",
            new List<RecipeIngredient>
            {
                new("Base Ore", 2),
                new("Soft Wood", 1) // Fuel
            },
            "Metal Bar",
            ResourceKind.Composite,
            "Furnace"));
        return system;
    }

    private static Dictionary<string, RawResource> CreateResourceDatabase()
    {
        return new Dictionary<string, RawResource>
        {
            ["Base Ore"] = new("Base Ore", ResourceKind.Ore,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f),
                    [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 5.0f),
                    [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 3.0f)
                }),
            ["Soft Wood"] = new("Soft Wood", ResourceKind.Wood,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 1.0f),
                    [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 1.5f)
                })
        };
    }

    [Fact]
    public void CanCraft_WithIngredients_ReturnsTrue()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        inventory.AddItem("Base Ore", 2);
        inventory.AddItem("Soft Wood", 1);

        Assert.True(crafting.CanCraft("smelt_metal_bar", inventory, UnlockedMachines));
    }

    [Fact]
    public void CanCraft_WithoutIngredients_ReturnsFalse()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        inventory.AddItem("Base Ore", 1); // Need 2

        Assert.False(crafting.CanCraft("smelt_metal_bar", inventory, UnlockedMachines));
    }

    [Fact]
    public void CanCraft_WithoutMachine_ReturnsFalse()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        inventory.AddItem("Base Ore", 2);
        inventory.AddItem("Soft Wood", 1);

        var noMachines = new HashSet<string> { "HandScanner" };

        Assert.False(crafting.CanCraft("smelt_metal_bar", inventory, noMachines));
    }

    [Fact]
    public void Craft_ConsumesInputs()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Base Ore", 5);
        inventory.AddItem("Soft Wood", 3);

        crafting.Craft("smelt_metal_bar", inventory, UnlockedMachines, db);

        Assert.Equal(3, inventory.GetItemCount("Base Ore"));   // 5 - 2
        Assert.Equal(2, inventory.GetItemCount("Soft Wood"));  // 3 - 1
    }

    [Fact]
    public void Craft_ProducesOutput()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Base Ore", 2);
        inventory.AddItem("Soft Wood", 1);

        crafting.Craft("smelt_metal_bar", inventory, UnlockedMachines, db);

        Assert.Equal(1, inventory.GetItemCount("Metal Bar"));
    }

    [Fact]
    public void Craft_ReturnsCraftResult_WithProperties()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Base Ore", 2);
        inventory.AddItem("Soft Wood", 1);

        var result = crafting.Craft("smelt_metal_bar", inventory, UnlockedMachines, db);

        Assert.NotNull(result);
        Assert.Equal("Metal Bar", result.OutputName);
        Assert.Equal(ResourceKind.Composite, result.OutputKind);
        Assert.NotEmpty(result.Properties);
    }

    [Fact]
    public void Craft_StrengthCombines_Additively()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Base Ore", 2);
        inventory.AddItem("Soft Wood", 1);

        var result = crafting.Craft("smelt_metal_bar", inventory, UnlockedMachines, db)!;

        // Base Ore Strength=2.0 + Soft Wood Strength=1.0 = 3.0
        Assert.Equal(3.0f, result.Properties[ResourcePropertyType.Strength]);
    }

    [Fact]
    public void Craft_EarnsSmeltingPoints()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Base Ore", 2);
        inventory.AddItem("Soft Wood", 1);

        var result = crafting.Craft("smelt_metal_bar", inventory, UnlockedMachines, db)!;

        Assert.Equal(KnowledgeCategory.Smelting, result.KnowledgeCategoryEarned);
        Assert.True(result.PointsEarned > 0);
    }

    [Fact]
    public void Craft_InsufficientResources_ReturnsNull()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();
        inventory.AddItem("Base Ore", 1); // Need 2

        var result = crafting.Craft("smelt_metal_bar", inventory, UnlockedMachines, db);

        Assert.Null(result);
    }

    [Fact]
    public void Craft_NonExistentRecipe_ReturnsNull()
    {
        var crafting = CreateSmeltingSystem();
        var inventory = new InventorySystem();
        var db = CreateResourceDatabase();

        var result = crafting.Craft("fake_recipe", inventory, UnlockedMachines, db);

        Assert.Null(result);
    }
}
