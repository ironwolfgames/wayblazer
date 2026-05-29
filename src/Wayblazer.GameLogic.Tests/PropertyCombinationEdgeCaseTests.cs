using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Thorough tests for all property combination formulas.
/// Tests single-input, dual-input, triple-input, edge cases, and the
/// conductivity duplicate-max fix.
/// </summary>
public class PropertyCombinationEdgeCaseTests
{
    private static CraftingSystem CreateSystemWithRecipe(string id, List<string> inputNames, string output)
    {
        var system = new CraftingSystem();
        var inputs = inputNames.Select(n => new RecipeIngredient(n, 1)).ToList();
        system.RegisterRecipe(new CraftingRecipe(id, id, inputs, output, ResourceKind.Composite, "Furnace"));
        return system;
    }

    private static readonly HashSet<string> AllMachines = new() { "Furnace", "GasInjector" };

    // === STRENGTH (Additive: sum of all inputs) ===

    [Fact]
    public void Strength_SingleInput_ReturnsValue()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 5.0f)
            })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(5.0f, result.Properties[ResourcePropertyType.Strength]);
    }

    [Fact]
    public void Strength_ThreeInputs_SumsAll()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B", "C" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1); inv.AddItem("C", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 3.0f) }),
            ["C"] = new("C", ResourceKind.Ore, new() { [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 4.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(9.0f, result.Properties[ResourcePropertyType.Strength]); // 2+3+4
    }

    [Fact]
    public void Strength_WithZeroValue_StillSums()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 5.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 0.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(5.0f, result.Properties[ResourcePropertyType.Strength]);
    }

    // === RESISTANCE (Min × 1.75) ===

    [Fact]
    public void Resistance_SingleInput_MultipliedBy175()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 4.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(7.0f, result.Properties[ResourcePropertyType.Resistance]); // 4.0 × 1.75
    }

    [Fact]
    public void Resistance_UsesMinimum_NotAverage()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B", "C" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1); inv.AddItem("C", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 8.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 2.0f) }),
            ["C"] = new("C", ResourceKind.Ore, new() { [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 6.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(3.5f, result.Properties[ResourcePropertyType.Resistance]); // min(8,2,6)=2 × 1.75
    }

    // === TOUGHNESS (Sum × 0.75) ===

    [Fact]
    public void Toughness_SingleInput_MultipliedBy075()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 8.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(6.0f, result.Properties[ResourcePropertyType.Toughness]); // 8.0 × 0.75
    }

    [Fact]
    public void Toughness_ThreeInputs_SumsThenMultiplies()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B", "C" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1); inv.AddItem("C", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 2.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 4.0f) }),
            ["C"] = new("C", ResourceKind.Ore, new() { [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 6.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(9.0f, result.Properties[ResourcePropertyType.Toughness]); // (2+4+6)×0.75
    }

    // === CONDUCTIVITY (Max + sum(others) × 0.5) ===

    [Fact]
    public void Conductivity_SingleInput_ReturnsValue()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 7.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(7.0f, result.Properties[ResourcePropertyType.Conductivity]);
    }

    [Fact]
    public void Conductivity_TwoInputs_MaxPlusHalfOther()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 10.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 4.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(12.0f, result.Properties[ResourcePropertyType.Conductivity]); // 10 + 4×0.5
    }

    [Fact]
    public void Conductivity_DuplicateMaxValues_HandledCorrectly()
    {
        // This tests the fix for the duplicate-max bug:
        // [10, 10, 2] should give 10 + (10+2)×0.5 = 16, not 10 + 2×0.5 = 11
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B", "C" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1); inv.AddItem("C", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 10.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 10.0f) }),
            ["C"] = new("C", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 2.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        // max=10, others=[10, 2], sum(others)×0.5 = 12×0.5 = 6, total = 16
        Assert.Equal(16.0f, result.Properties[ResourcePropertyType.Conductivity]);
    }

    [Fact]
    public void Conductivity_ThreeInputs_MaxPlusSumOthersTimesHalf()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B", "C" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1); inv.AddItem("C", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 8.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 4.0f) }),
            ["C"] = new("C", ResourceKind.Ore, new() { [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 2.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        // max=8, others=[4, 2], sum(others)×0.5 = 6×0.5 = 3, total = 11
        Assert.Equal(11.0f, result.Properties[ResourcePropertyType.Conductivity]);
    }

    // === REACTIVITY (Product × 0.5) ===

    [Fact]
    public void Reactivity_SingleInput_MultipliedByHalf()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 6.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(3.0f, result.Properties[ResourcePropertyType.Reactivity]); // 6×0.5
    }

    [Fact]
    public void Reactivity_ThreeInputs_ProductTimesHalf()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B", "C" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1); inv.AddItem("C", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 2.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 3.0f) }),
            ["C"] = new("C", ResourceKind.Ore, new() { [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 4.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(12.0f, result.Properties[ResourcePropertyType.Reactivity]); // 2×3×4×0.5
    }

    [Fact]
    public void Reactivity_WithZero_ProducesZero()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new() { [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 5.0f) }),
            ["B"] = new("B", ResourceKind.Ore, new() { [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 0.0f) })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(0.0f, result.Properties[ResourcePropertyType.Reactivity]); // 5×0×0.5
    }

    // === MIXED PROPERTIES ===

    [Fact]
    public void MixedProperties_OnlyPresentPropertiesCombined()
    {
        // Resource A has Strength+Resistance, Resource B only has Strength
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 3.0f),
                [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 4.0f)
            }),
            ["B"] = new("B", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f)
                // No Resistance
            })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;
        Assert.Equal(5.0f, result.Properties[ResourcePropertyType.Strength]); // 3+2
        Assert.Equal(7.0f, result.Properties[ResourcePropertyType.Resistance]); // min(4) × 1.75 = only A has it
    }

    [Fact]
    public void AllProperties_CombinedCorrectly_TwoInputs()
    {
        var system = CreateSystemWithRecipe("r1", new List<string> { "A", "B" }, "Out");
        var inv = new InventorySystem();
        inv.AddItem("A", 1); inv.AddItem("B", 1);
        var db = new Dictionary<string, RawResource>
        {
            ["A"] = new("A", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 3.0f),
                [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 4.0f),
                [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 5.0f),
                [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 6.0f),
                [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 2.0f)
            }),
            ["B"] = new("B", ResourceKind.Ore, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f),
                [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 6.0f),
                [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 3.0f),
                [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 8.0f),
                [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 4.0f)
            })
        };

        var result = system.Craft("r1", inv, AllMachines, db)!;

        Assert.Equal(5.0f, result.Properties[ResourcePropertyType.Strength]);       // 3+2
        Assert.Equal(7.0f, result.Properties[ResourcePropertyType.Resistance]);     // min(4,6)×1.75 = 7.0
        Assert.Equal(6.0f, result.Properties[ResourcePropertyType.Toughness]);      // (5+3)×0.75
        Assert.Equal(11.0f, result.Properties[ResourcePropertyType.Conductivity]);  // max(6,8)=8 + 6×0.5 = 11
        Assert.Equal(4.0f, result.Properties[ResourcePropertyType.Reactivity]);     // 2×4×0.5
    }
}
