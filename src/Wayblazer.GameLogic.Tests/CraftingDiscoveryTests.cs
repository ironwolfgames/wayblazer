using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Tests for recipe discovery, listing, and filtering capabilities.
/// Validates that the player can enumerate, filter, and query available recipes.
/// </summary>
public class CraftingDiscoveryTests
{
    private static CraftingSystem CreateFullRecipeSystem()
    {
        var system = new CraftingSystem();

        system.RegisterRecipe(new CraftingRecipe(
            "smelt_metal_bar", "Smelt Metal Bar",
            new List<RecipeIngredient> { new("Base Ore", 2), new("Soft Wood", 1) },
            "Metal Bar", ResourceKind.Composite, "Furnace"));

        system.RegisterRecipe(new CraftingRecipe(
            "smelt_catalyst_bar", "Smelt Catalyst Bar",
            new List<RecipeIngredient> { new("Catalyst Ore", 2), new("Soft Wood", 1) },
            "Catalyst Bar", ResourceKind.Composite, "Furnace"));

        system.RegisterRecipe(new CraftingRecipe(
            "composite_alloy", "Synthesize Composite Alloy",
            new List<RecipeIngredient> { new("Metal Bar", 1), new("Catalyst Ore", 1) },
            "Composite Alloy", ResourceKind.Composite, "GasInjector"));

        system.RegisterRecipe(new CraftingRecipe(
            "advanced_alloy", "Synthesize Advanced Alloy",
            new List<RecipeIngredient> { new("Metal Bar", 2), new("Catalyst Bar", 1) },
            "Advanced Alloy", ResourceKind.Composite, "GasInjector"));

        return system;
    }

    [Fact]
    public void Recipes_ListsAllRegistered()
    {
        var system = CreateFullRecipeSystem();

        Assert.Equal(4, system.Recipes.Count);
    }

    [Fact]
    public void GetRecipesByMachine_Furnace_ReturnsFurnaceRecipes()
    {
        var system = CreateFullRecipeSystem();

        var furnaceRecipes = system.Recipes.Where(r => r.RequiredMachine == "Furnace").ToList();

        Assert.Equal(2, furnaceRecipes.Count);
        Assert.Contains(furnaceRecipes, r => r.Id == "smelt_metal_bar");
        Assert.Contains(furnaceRecipes, r => r.Id == "smelt_catalyst_bar");
    }

    [Fact]
    public void GetRecipesByMachine_GasInjector_ReturnsGasInjectorRecipes()
    {
        var system = CreateFullRecipeSystem();

        var injectorRecipes = system.Recipes.Where(r => r.RequiredMachine == "GasInjector").ToList();

        Assert.Equal(2, injectorRecipes.Count);
        Assert.Contains(injectorRecipes, r => r.Id == "composite_alloy");
        Assert.Contains(injectorRecipes, r => r.Id == "advanced_alloy");
    }

    [Fact]
    public void CanCraft_WithFullInventory_ReturnsCraftableRecipes()
    {
        var system = CreateFullRecipeSystem();
        var inventory = new InventorySystem();
        inventory.AddItem("Base Ore", 5);
        inventory.AddItem("Soft Wood", 3);
        inventory.AddItem("Catalyst Ore", 2);
        inventory.AddItem("Metal Bar", 2);

        var allMachines = new HashSet<string> { "Furnace", "GasInjector" };
        var craftable = system.Recipes.Where(r => system.CanCraft(r.Id, inventory, allMachines)).ToList();

        Assert.Contains(craftable, r => r.Id == "smelt_metal_bar");
        Assert.Contains(craftable, r => r.Id == "smelt_catalyst_bar");
        Assert.Contains(craftable, r => r.Id == "composite_alloy");
        Assert.DoesNotContain(craftable, r => r.Id == "advanced_alloy"); // Needs Metal Bar × 2 + Catalyst Bar
    }

    [Fact]
    public void CanCraft_LimitedMachines_FiltersCorrectly()
    {
        var system = CreateFullRecipeSystem();
        var inventory = new InventorySystem();
        inventory.AddItem("Base Ore", 5);
        inventory.AddItem("Soft Wood", 3);
        inventory.AddItem("Catalyst Ore", 2);

        var furnaceOnly = new HashSet<string> { "Furnace" };
        var craftable = system.Recipes.Where(r => system.CanCraft(r.Id, inventory, furnaceOnly)).ToList();

        Assert.Equal(2, craftable.Count); // Only furnace recipes
        Assert.All(craftable, r => Assert.Equal("Furnace", r.RequiredMachine));
    }

    [Fact]
    public void CanCraft_EmptyInventory_ReturnsNothing()
    {
        var system = CreateFullRecipeSystem();
        var inventory = new InventorySystem();
        var allMachines = new HashSet<string> { "Furnace", "GasInjector" };

        var craftable = system.Recipes.Where(r => system.CanCraft(r.Id, inventory, allMachines)).ToList();

        Assert.Empty(craftable);
    }

    [Fact]
    public void RecipeInputs_CanBeQueried()
    {
        var system = CreateFullRecipeSystem();

        var recipe = system.Recipes.First(r => r.Id == "smelt_metal_bar");

        Assert.Equal(2, recipe.Inputs.Count);
        Assert.Contains(recipe.Inputs, i => i.ResourceName == "Base Ore" && i.Amount == 2);
        Assert.Contains(recipe.Inputs, i => i.ResourceName == "Soft Wood" && i.Amount == 1);
    }

    [Fact]
    public void RecipeOutput_HasCorrectDetails()
    {
        var system = CreateFullRecipeSystem();

        var recipe = system.Recipes.First(r => r.Id == "composite_alloy");

        Assert.Equal("Composite Alloy", recipe.OutputName);
        Assert.Equal(ResourceKind.Composite, recipe.OutputKind);
        Assert.Equal("GasInjector", recipe.RequiredMachine);
    }

    [Fact]
    public void MultipleCrafts_InventoryDepletes_RecipeBecomesUnavailable()
    {
        var system = CreateFullRecipeSystem();
        var inventory = new InventorySystem();
        var allMachines = new HashSet<string> { "Furnace", "GasInjector" };
        var db = new Dictionary<string, RawResource>
        {
            ["Base Ore"] = new("Base Ore", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f)
            }),
            ["Soft Wood"] = new("Soft Wood", ResourceKind.Wood, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 1.0f)
            })
        };

        inventory.AddItem("Base Ore", 4);
        inventory.AddItem("Soft Wood", 2);

        // Should be able to craft twice (needs 2 ore + 1 wood each)
        Assert.True(system.CanCraft("smelt_metal_bar", inventory, allMachines));
        system.Craft("smelt_metal_bar", inventory, allMachines, db);

        Assert.True(system.CanCraft("smelt_metal_bar", inventory, allMachines));
        system.Craft("smelt_metal_bar", inventory, allMachines, db);

        // Now should be unable (0 ore, 0 wood remaining)
        Assert.False(system.CanCraft("smelt_metal_bar", inventory, allMachines));
    }

    [Fact]
    public void RegisterRecipe_DuplicateId_BothAccessible()
    {
        var system = new CraftingSystem();

        system.RegisterRecipe(new CraftingRecipe(
            "test", "Test 1",
            new List<RecipeIngredient> { new("A", 1) },
            "Out1", ResourceKind.Composite, "Furnace"));

        system.RegisterRecipe(new CraftingRecipe(
            "test2", "Test 2",
            new List<RecipeIngredient> { new("B", 1) },
            "Out2", ResourceKind.Composite, "Furnace"));

        Assert.Equal(2, system.Recipes.Count);
    }

    [Fact]
    public void FindRecipesByOutput_WorksCorrectly()
    {
        var system = CreateFullRecipeSystem();

        var metalBarRecipes = system.Recipes.Where(r => r.OutputName == "Metal Bar").ToList();

        Assert.Single(metalBarRecipes);
        Assert.Equal("smelt_metal_bar", metalBarRecipes[0].Id);
    }
}
