namespace Wayblazer.Core.Models;

/// <summary>
/// A crafting recipe that transforms input resources into output resources.
/// </summary>
public class CraftingRecipe
{
    public string Id { get; }
    public string Name { get; }
    public List<RecipeIngredient> Inputs { get; }
    public string OutputName { get; }
    public ResourceKind OutputKind { get; }
    public string RequiredMachine { get; }

    public CraftingRecipe(
        string id,
        string name,
        List<RecipeIngredient> inputs,
        string outputName,
        ResourceKind outputKind,
        string requiredMachine)
    {
        Id = id;
        Name = name;
        Inputs = inputs;
        OutputName = outputName;
        OutputKind = outputKind;
        RequiredMachine = requiredMachine;
    }
}

/// <summary>
/// A single ingredient requirement for a crafting recipe.
/// </summary>
public class RecipeIngredient
{
    public string ResourceName { get; }
    public int Amount { get; }

    public RecipeIngredient(string resourceName, int amount)
    {
        ResourceName = resourceName;
        Amount = amount;
    }
}
