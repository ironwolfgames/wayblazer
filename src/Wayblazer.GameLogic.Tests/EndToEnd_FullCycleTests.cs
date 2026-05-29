using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// End-to-End Integration: Full Vertical Slice Deduction Cycle.
/// This test class verifies the entire game loop works together deterministically.
/// </summary>
public class EndToEnd_FullCycleTests
{
    /// <summary>
    /// Simulates a complete playthrough of the vertical slice:
    /// Land → Scan → Analyze → Research → Smelt → Composite → Simulate → Win
    /// </summary>
    [Fact]
    public void FullVerticalSlice_CompletePlaythrough()
    {
        // === SETUP: Initialize all systems ===
        var inventory = new InventorySystem();
        var scanner = new ScannerSystem();
        var observatory = new PlanetaryAnalysisSystem();
        var techTree = TechTreeSystem.CreateVerticalSliceTree();
        var crafting = new CraftingSystem();
        var verifier = new PortalVerificationSystem();

        // Register recipes
        crafting.RegisterRecipe(new CraftingRecipe(
            "smelt_metal_bar", "Smelt Metal Bar",
            new List<RecipeIngredient> { new("Base Ore", 2), new("Soft Wood", 1) },
            "Metal Bar", ResourceKind.Composite, "Furnace"));

        crafting.RegisterRecipe(new CraftingRecipe(
            "composite_alloy", "Synthesize Composite Alloy",
            new List<RecipeIngredient> { new("Metal Bar", 1), new("Catalyst Ore", 1) },
            "Composite Alloy", ResourceKind.Composite, "GasInjector"));

        // Resource database with known properties
        var resourceDb = new Dictionary<string, RawResource>
        {
            ["Base Ore"] = new("Base Ore", ResourceKind.Ore,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 2.0f),
                    [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 5.0f),
                    [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 3.0f),
                    [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 4.0f),
                    [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 2.0f)
                }),
            ["Soft Wood"] = new("Soft Wood", ResourceKind.Wood,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 1.0f),
                    [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 1.5f),
                    [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 2.0f),
                    [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 0.5f),
                    [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 7.0f)
                }),
            ["Catalyst Ore"] = new("Catalyst Ore", ResourceKind.Ore,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 6.0f),
                    [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 3.0f),
                    [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 4.0f),
                    [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 8.0f),
                    [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 3.0f)
                }),
            // Metal Bar gets its properties from smelting Base Ore + Soft Wood
            ["Metal Bar"] = new("Metal Bar", ResourceKind.Composite,
                new Dictionary<ResourcePropertyType, ResourceProperty>
                {
                    [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 3.0f),
                    [ResourcePropertyType.Resistance] = new(ResourcePropertyType.Resistance, 2.625f),
                    [ResourcePropertyType.Toughness] = new(ResourcePropertyType.Toughness, 3.75f),
                    [ResourcePropertyType.Conductivity] = new(ResourcePropertyType.Conductivity, 4.25f),
                    [ResourcePropertyType.Reactivity] = new(ResourcePropertyType.Reactivity, 7.0f)
                })
        };

        var planet = PlanetaryConstants.Default;

        // === STEP 1: LAND & ASSESS ===
        // Player has just landed. Harvest resources.
        inventory.AddItem("Base Ore", 10);
        inventory.AddItem("Soft Wood", 5);
        inventory.AddItem("Catalyst Ore", 3);

        // === STEP 2: SCAN (Vague) ===
        var vagueScan = scanner.GetVagueScan(resourceDb["Base Ore"]);
        Assert.Equal("Low", vagueScan[ResourcePropertyType.Strength]); // "Hmm, ore is weak..."

        var planetScan = scanner.GetVaguePlanetaryScan(planet);
        Assert.NotEmpty(planetScan); // "Gravity feels moderate..."

        // === STEP 3: EARN ANALYSIS POINTS (scanning earns points) ===
        // Simulate earning points through scanning multiple resources
        int totalAnalysisPoints = 0;
        totalAnalysisPoints += scanner.AnalyzeResource(resourceDb["Base Ore"]);    // +5
        totalAnalysisPoints += scanner.AnalyzeResource(resourceDb["Soft Wood"]);   // +5
        totalAnalysisPoints += scanner.AnalyzeResource(resourceDb["Catalyst Ore"]); // +5
        techTree.AddKnowledgePoints(KnowledgeCategory.Analysis, totalAnalysisPoints); // = 15

        // === STEP 4: UNLOCK FIELD LAB ===
        Assert.True(techTree.CanUnlock("Unlock_FieldLab"));
        techTree.TryUnlock("Unlock_FieldLab");

        // Now precise scans work
        var precise = scanner.GetPreciseScan(resourceDb["Base Ore"]);
        Assert.NotNull(precise);
        Assert.Equal(2.0f, precise[ResourcePropertyType.Strength]);

        // === STEP 5: EARN MORE POINTS, UNLOCK OBSERVATORY ===
        techTree.AddKnowledgePoints(KnowledgeCategory.Analysis, 25);
        techTree.TryUnlock("Unlock_Observatory");

        // === STEP 6: MEASURE PLANET ===
        var (gravity, obsPoints) = observatory.MeasureConstant("Gravity", planet);
        Assert.Equal(3.2f, gravity);
        techTree.AddKnowledgePoints(KnowledgeCategory.Analysis, obsPoints);

        // === STEP 7: DEDUCE PORTAL REQUIREMENTS ===
        var portalReqs = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);
        Assert.Equal(8.0f, portalReqs[ResourcePropertyType.Strength]); // Need >= 8.0 Strength

        // === STEP 8: UNLOCK FURNACE & SMELT ===
        techTree.AddKnowledgePoints(KnowledgeCategory.Analysis, 10);
        techTree.TryUnlock("Unlock_Furnace");

        var machines = techTree.GetUnlockedMachines();
        Assert.Contains("Furnace", machines);

        var smeltResult = crafting.Craft("smelt_metal_bar", inventory, machines, resourceDb);
        Assert.NotNull(smeltResult);
        techTree.AddKnowledgePoints(smeltResult.KnowledgeCategoryEarned, smeltResult.PointsEarned);

        // === STEP 9: UNLOCK GAS INJECTOR & COMPOSITE ===
        techTree.AddKnowledgePoints(KnowledgeCategory.Smelting, 10);
        techTree.AddKnowledgePoints(KnowledgeCategory.Compositing, 10);
        techTree.TryUnlock("Unlock_GasInjector");

        machines = techTree.GetUnlockedMachines();
        Assert.Contains("GasInjector", machines);

        var compositeResult = crafting.Craft("composite_alloy", inventory, machines, resourceDb);
        Assert.NotNull(compositeResult);
        techTree.AddKnowledgePoints(compositeResult.KnowledgeCategoryEarned, compositeResult.PointsEarned);

        // === STEP 10: UNLOCK SIM CORE & VERIFY ===
        techTree.AddKnowledgePoints(KnowledgeCategory.Analysis, 15);
        techTree.AddKnowledgePoints(KnowledgeCategory.Smelting, 10);
        techTree.AddKnowledgePoints(KnowledgeCategory.Compositing, 10);
        techTree.TryUnlock("Unlock_SimCore");

        // Composite alloy: Metal Bar Str(3.0) + Catalyst Str(6.0) = 9.0 >= 8.0 ✓
        var simResult = verifier.RunSimulation(compositeResult.Properties, portalReqs);

        // The composite alloy MUST pass the strength check (9.0 >= 8.0)
        Assert.True(simResult.PropertyResults[ResourcePropertyType.Strength].Passes,
            $"Strength {compositeResult.Properties.GetValueOrDefault(ResourcePropertyType.Strength)} " +
            $"should meet requirement {portalReqs[ResourcePropertyType.Strength]}");

        // === STEP 11: UNLOCK PORTAL & WIN ===
        techTree.AddKnowledgePoints(KnowledgeCategory.Analysis, 10);
        techTree.AddKnowledgePoints(KnowledgeCategory.Smelting, 10);
        techTree.AddKnowledgePoints(KnowledgeCategory.Compositing, 10);
        techTree.TryUnlock("Unlock_Portal");

        machines = techTree.GetUnlockedMachines();
        Assert.Contains("PortalFoundation", machines);

        // Final verification: the strength property passes
        Assert.True(compositeResult.Properties[ResourcePropertyType.Strength] >= portalReqs[ResourcePropertyType.Strength],
            "GAME WON: Composite alloy strength meets portal requirement!");
    }

    /// <summary>
    /// Tests that a player who skips compositing will FAIL the portal check.
    /// This validates that the deduction puzzle has teeth - you can't brute force it.
    /// </summary>
    [Fact]
    public void VerticalSlice_WithoutCompositing_PortalFails()
    {
        var planet = PlanetaryConstants.Default;
        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);
        var verifier = new PortalVerificationSystem();

        // Try using just a smelted metal bar (no catalyst compositing)
        // Metal Bar from Base Ore (2.0) + Soft Wood (1.0) = Strength 3.0
        var metalBarProperties = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 3.0f
        };

        Assert.False(verifier.MeetsRequirements(metalBarProperties, requirements),
            "Plain metal bar should NOT meet portal requirements (3.0 < 8.0)");
    }

    /// <summary>
    /// Tests that tech tree prevents skipping ahead.
    /// </summary>
    [Fact]
    public void VerticalSlice_CannotSkipTechTree()
    {
        var techTree = TechTreeSystem.CreateVerticalSliceTree();
        var crafting = new CraftingSystem();
        var inventory = new InventorySystem();

        crafting.RegisterRecipe(new CraftingRecipe(
            "composite_alloy", "Synthesize Composite Alloy",
            new List<RecipeIngredient> { new("Metal Bar", 1), new("Catalyst Ore", 1) },
            "Composite Alloy", ResourceKind.Composite, "GasInjector"));

        inventory.AddItem("Metal Bar", 1);
        inventory.AddItem("Catalyst Ore", 1);

        // Without unlocking GasInjector, crafting should fail
        var machines = techTree.GetUnlockedMachines();
        Assert.DoesNotContain("GasInjector", machines);
        Assert.False(crafting.CanCraft("composite_alloy", inventory, machines));
    }
}
