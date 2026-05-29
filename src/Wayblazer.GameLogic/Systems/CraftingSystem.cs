using Wayblazer.GameLogic.Models;

namespace Wayblazer.GameLogic.Systems;

/// <summary>
/// Handles crafting operations: smelting (Furnace) and compositing (Gas Injector).
/// Sprint 7: Basic smelting. Sprint 8: Advanced compositing.
/// </summary>
public class CraftingSystem
{
    private readonly List<CraftingRecipe> _recipes = new();

    public IReadOnlyList<CraftingRecipe> Recipes => _recipes;

    public void RegisterRecipe(CraftingRecipe recipe)
    {
        _recipes.Add(recipe);
    }

    /// <summary>
    /// Checks if a recipe can be crafted given the current inventory and unlocked machines.
    /// </summary>
    public bool CanCraft(string recipeId, InventorySystem inventory, IReadOnlySet<string> unlockedMachines)
    {
        var recipe = _recipes.FirstOrDefault(r => r.Id == recipeId);
        if (recipe == null) return false;

        // Check machine is unlocked
        if (!unlockedMachines.Contains(recipe.RequiredMachine)) return false;

        // Check all ingredients are available
        return inventory.HasItems(recipe.Inputs);
    }

    /// <summary>
    /// Executes a craft operation. Consumes inputs from inventory, produces output.
    /// Returns the crafted resource properties, or null if crafting fails.
    /// </summary>
    public CraftResult? Craft(
        string recipeId,
        InventorySystem inventory,
        IReadOnlySet<string> unlockedMachines,
        Dictionary<string, RawResource> resourceDatabase)
    {
        var recipe = _recipes.FirstOrDefault(r => r.Id == recipeId);
        if (recipe == null) return null;

        if (!CanCraft(recipeId, inventory, unlockedMachines)) return null;

        // Consume inputs
        foreach (var input in recipe.Inputs)
        {
            if (!inventory.TryRemoveItem(input.ResourceName, input.Amount))
                return null; // Should not happen if CanCraft passed
        }

        // Calculate output properties based on recipe type
        var outputProperties = CalculateOutputProperties(recipe, resourceDatabase);

        // Add output to inventory
        inventory.AddItem(recipe.OutputName, 1);

        // Determine knowledge category based on machine
        var knowledgeCategory = recipe.RequiredMachine switch
        {
            "Furnace" => KnowledgeCategory.Smelting,
            "GasInjector" => KnowledgeCategory.Compositing,
            _ => KnowledgeCategory.Smelting
        };

        return new CraftResult(recipe.OutputName, recipe.OutputKind, outputProperties, knowledgeCategory, 5);
    }

    private static Dictionary<ResourcePropertyType, float> CalculateOutputProperties(
        CraftingRecipe recipe,
        Dictionary<string, RawResource> resourceDatabase)
    {
        var result = new Dictionary<ResourcePropertyType, float>();

        // Gather all input properties
        var inputResources = recipe.Inputs
            .Where(i => resourceDatabase.ContainsKey(i.ResourceName))
            .Select(i => resourceDatabase[i.ResourceName])
            .ToList();

        if (inputResources.Count == 0)
            return result;

        // Combine properties from all inputs
        var allPropertyTypes = inputResources
            .SelectMany(r => r.Properties.Keys)
            .Distinct();

        foreach (var propType in allPropertyTypes)
        {
            var values = inputResources
                .Where(r => r.Properties.ContainsKey(propType))
                .Select(r => r.Properties[propType].Value)
                .ToList();

            if (values.Count == 0) continue;

            result[propType] = propType switch
            {
                // Strength: additive
                ResourcePropertyType.Strength => values.Sum(),
                // Resistance: min × 1.75 (weakest link, but improved by alloying)
                ResourcePropertyType.Resistance => values.Min() * 1.75f,
                // Toughness: sum × 0.75
                ResourcePropertyType.Toughness => values.Sum() * 0.75f,
                // Conductivity: max + (sum of all others × 0.5)
                ResourcePropertyType.Conductivity => CombineConductivity(values),
                // Reactivity: product × 0.5
                ResourcePropertyType.Reactivity => values.Aggregate(1f, (a, b) => a * b) * 0.5f,
                _ => values.Average()
            };
        }

        return result;
    }

    /// <summary>
    /// Combines conductivity values: takes the highest value, adds 50% of the sum of all others.
    /// Correctly handles duplicate maximums by only removing one instance.
    /// </summary>
    private static float CombineConductivity(List<float> values)
    {
        if (values.Count == 0) return 0f;
        if (values.Count == 1) return values[0];

        float max = values.Max();
        // Remove exactly one instance of the max value to get "others"
        var others = new List<float>(values);
        others.Remove(max);
        return max + others.Sum() * 0.5f;
    }
}

/// <summary>
/// Result of a successful crafting operation.
/// </summary>
public class CraftResult
{
    public string OutputName { get; }
    public ResourceKind OutputKind { get; }
    public Dictionary<ResourcePropertyType, float> Properties { get; }
    public KnowledgeCategory KnowledgeCategoryEarned { get; }
    public int PointsEarned { get; }

    public CraftResult(
        string outputName,
        ResourceKind outputKind,
        Dictionary<ResourcePropertyType, float> properties,
        KnowledgeCategory knowledgeCategoryEarned,
        int pointsEarned)
    {
        OutputName = outputName;
        OutputKind = outputKind;
        Properties = properties;
        KnowledgeCategoryEarned = knowledgeCategoryEarned;
        PointsEarned = pointsEarned;
    }
}
