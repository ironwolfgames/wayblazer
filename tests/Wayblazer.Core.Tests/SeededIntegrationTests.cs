using Wayblazer.Core.Models;
using Wayblazer.Core.Systems;

namespace Wayblazer.Core.Tests;

/// <summary>
/// Integration tests that verify complete game scenarios using seeded generation.
/// These prove that given a seed, the entire game loop is deterministic and solvable.
/// </summary>
public class SeededIntegrationTests
{
    /// <summary>
    /// Given a specific seed, generates a full world and validates the game is completable.
    /// This is the master test proving the procedural generation + game logic integration works.
    /// </summary>
    [Theory]
    [InlineData(42)]
    [InlineData(123)]
    [InlineData(100)]
    [InlineData(500)]
    public void SeededWorld_GeneratesConsistentSolvableGame(int seed)
    {
        var gen = new ResourceGenerationSystem();

        // 1. Generate world deterministically
        var planet = gen.GeneratePlanetaryConstants(seed, complexity: 1);
        var resources = gen.GenerateResources(seed, complexity: 1);

        // 2. Calculate portal requirements
        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(planet);

        // 3. Verify planet is solvable with available resources
        Assert.True(DifficultySystem.IsPlanetSolvable(resources, requirements),
            $"Seed {seed} generated an unsolvable world");

        // 4. Verify determinism: generate again and compare
        var planet2 = gen.GeneratePlanetaryConstants(seed, complexity: 1);
        var resources2 = gen.GenerateResources(seed, complexity: 1);

        Assert.Equal(planet.Gravity, planet2.Gravity);
        Assert.Equal(resources.Count, resources2.Count);
        for (int i = 0; i < resources.Count; i++)
        {
            Assert.Equal(resources[i].Name, resources2[i].Name);
        }
    }

    /// <summary>
    /// Tests that the save/load round-trip preserves complete game state mid-playthrough.
    /// </summary>
    [Fact]
    public void SaveLoad_MidGameState_PreservesAndRestores()
    {
        // === SETUP: Play the game partway ===
        var inventory = new InventorySystem();
        var techTree = TechTreeSystem.CreateVerticalSliceTree();
        var scanner = new ScannerSystem();
        var observatory = new PlanetaryAnalysisSystem();
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();
        var serializer = new GameStateSerializer();

        // Simulate some gameplay
        inventory.AddItem("Base Ore", 10);
        inventory.AddItem("Metal Bar", 2);
        techTree.AddKnowledgePoints(KnowledgeCategory.Analysis, 40);
        techTree.TryUnlock("Unlock_FieldLab");
        scanner.MarkAsAnalyzed("Base Ore");
        scanner.MarkAsAnalyzed("Catalyst Ore");
        observatory.MarkAsMeasured("Gravity");

        // === SAVE ===
        var state = serializer.CaptureState(42, 1, inventory, techTree, scanner, observatory, objectives);
        string json = serializer.Serialize(state);

        // === RESTORE into fresh systems ===
        var newInventory = new InventorySystem();
        var newTechTree = TechTreeSystem.CreateVerticalSliceTree();
        var newScanner = new ScannerSystem();
        var newObservatory = new PlanetaryAnalysisSystem();

        var restoredState = serializer.Deserialize(json)!;
        serializer.RestoreState(restoredState, newInventory, newTechTree, newScanner, newObservatory);

        // === VERIFY ===
        Assert.Equal(10, newInventory.GetItemCount("Base Ore"));
        Assert.Equal(2, newInventory.GetItemCount("Metal Bar"));
        Assert.Contains("Unlock_FieldLab", newTechTree.UnlockedNodes);
        Assert.True(newScanner.IsResourceAnalyzed("Base Ore"));
        Assert.True(newScanner.IsResourceAnalyzed("Catalyst Ore"));
        Assert.True(newObservatory.IsConstantMeasured("Gravity"));
        Assert.Equal(42, restoredState.WorldSeed);
    }

    /// <summary>
    /// Tests the full harvest → craft → verify flow using the harvest system.
    /// </summary>
    [Fact]
    public void FullFlow_Harvest_Craft_Verify()
    {
        var harvest = new HarvestSystem();
        var inventory = new InventorySystem();
        var crafting = new CraftingSystem();
        var verifier = new PortalVerificationSystem();

        // Register recipes
        crafting.RegisterRecipe(new CraftingRecipe(
            "smelt", "Smelt",
            new List<RecipeIngredient> { new("Ore A", 2), new("Wood", 1) },
            "Metal", ResourceKind.Composite, "Furnace"));

        crafting.RegisterRecipe(new CraftingRecipe(
            "composite", "Composite",
            new List<RecipeIngredient> { new("Metal", 1), new("Ore B", 1) },
            "Alloy", ResourceKind.Composite, "GasInjector"));

        // Resources
        var oreA = new RawResource("Ore A", ResourceKind.Ore, new()
        {
            [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 3.0f)
        }, HarvestMethod.KineticMining);

        var oreB = new RawResource("Ore B", ResourceKind.Ore, new()
        {
            [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 7.0f)
        }, HarvestMethod.KineticMining);

        var wood = new RawResource("Wood", ResourceKind.Wood, new()
        {
            [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 1.0f)
        }, HarvestMethod.KineticMining);

        var db = new Dictionary<string, RawResource>
        {
            ["Ore A"] = oreA, ["Ore B"] = oreB, ["Wood"] = wood,
            ["Metal"] = new("Metal", ResourceKind.Composite, new()
            {
                [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 4.0f) // sum of 3+1
            })
        };

        var machines = new HashSet<string> { "Furnace", "GasInjector" };

        // Harvest
        Assert.Equal(2, harvest.TryHarvest(oreA, inventory, 2));
        Assert.Equal(1, harvest.TryHarvest(wood, inventory, 1));
        Assert.Equal(1, harvest.TryHarvest(oreB, inventory, 1));

        // Smelt
        var smeltResult = crafting.Craft("smelt", inventory, machines, db);
        Assert.NotNull(smeltResult);

        // Composite
        var compositeResult = crafting.Craft("composite", inventory, machines, db);
        Assert.NotNull(compositeResult);

        // Verify: combined strength = 4.0 (metal) + 7.0 (ore B) = 11.0
        var requirements = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 8.0f
        };

        var sim = verifier.RunSimulation(compositeResult.Properties, requirements);
        Assert.True(sim.OverallPass);
        Assert.True(sim.PropertyResults[ResourcePropertyType.Strength].Percentage > 100f);
    }

    /// <summary>
    /// Tests that harvest gating prevents bypassing the tech progression.
    /// </summary>
    [Fact]
    public void HarvestGating_PreventsEarlyAccess()
    {
        var harvest = new HarvestSystem();
        var inventory = new InventorySystem();

        // Gas resource requires GasSiphoning
        var rareGas = new RawResource("Xenon", ResourceKind.Gas, new()
        {
            [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 10.0f)
        }, HarvestMethod.GasSiphoning);

        // Cannot harvest without method
        Assert.Equal(0, harvest.TryHarvest(rareGas, inventory));
        Assert.Equal(0, inventory.GetItemCount("Xenon"));

        // After unlocking, can harvest
        harvest.UnlockMethod(HarvestMethod.GasSiphoning);
        Assert.Equal(1, harvest.TryHarvest(rareGas, inventory));
        Assert.Equal(1, inventory.GetItemCount("Xenon"));
    }

    /// <summary>
    /// Tests that the objective system integrates with gameplay events correctly.
    /// </summary>
    [Fact]
    public void ObjectiveSystem_IntegratesWithGameplay()
    {
        var objectives = ObjectiveSystem.CreateVerticalSliceObjectives();
        var harvest = new HarvestSystem();
        var inventory = new InventorySystem();

        var wood = new RawResource("Soft Wood", ResourceKind.Wood, new()
        {
            [ResourcePropertyType.Strength] = new(ResourcePropertyType.Strength, 1.0f)
        });

        // Harvest wood and report progress
        for (int i = 0; i < 5; i++)
        {
            harvest.TryHarvest(wood, inventory);
            objectives.ReportProgress(ObjectiveConditionType.HarvestResource, "Soft Wood", 1);
        }

        // First objective should be complete
        Assert.True(objectives.IsObjectiveCompleted("harvest_wood"));
        Assert.Equal("scan_ore", objectives.CurrentObjectiveId);
        Assert.Equal(5, inventory.GetItemCount("Soft Wood"));
    }

    /// <summary>
    /// Tests portal construction with multi-component requirements.
    /// </summary>
    [Fact]
    public void PortalConstruction_RequiresAllComponents()
    {
        var portal = new PortalConstructionSystem();
        portal.InitializeFromPlanet(PlanetaryConstants.Default);
        var verifier = new PortalVerificationSystem();

        // Create materials targeting each component
        var foundationMaterial = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Toughness] = 8.0f
        };

        var gateMaterial = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Resistance] = 6.0f,
            [ResourcePropertyType.Conductivity] = 8.0f
        };

        var coreMaterial = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Reactivity] = 5.0f,
            [ResourcePropertyType.Conductivity] = 8.0f
        };

        // Assign one by one
        portal.AssignMaterial(PortalComponentType.Foundation, "Foundation Alloy", foundationMaterial);
        Assert.False(portal.IsPortalComplete());

        portal.AssignMaterial(PortalComponentType.Gate, "Gate Alloy", gateMaterial);
        Assert.False(portal.IsPortalComplete());

        portal.AssignMaterial(PortalComponentType.EnergyCore, "Core Alloy", coreMaterial);
        Assert.True(portal.IsPortalComplete());

        // Also verify with the simulation system
        var fullMaterial = new Dictionary<ResourcePropertyType, float>
        {
            [ResourcePropertyType.Strength] = 10.0f,
            [ResourcePropertyType.Resistance] = 6.0f,
            [ResourcePropertyType.Toughness] = 8.0f,
            [ResourcePropertyType.Conductivity] = 8.0f,
            [ResourcePropertyType.Reactivity] = 5.0f
        };
        var requirements = PlanetaryAnalysisSystem.CalculatePortalRequirements(PlanetaryConstants.Default);
        var sim = verifier.RunSimulation(fullMaterial, requirements);
        Assert.True(sim.OverallPass);
    }
}
