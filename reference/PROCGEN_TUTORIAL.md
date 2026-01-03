# 🌍 Procedural World Generation Tutorial for Wayblazer

This tutorial provides a step-by-step guide to complete the procedural generation system for Wayblazer. It's designed for developers who are new to Godot and game development, building upon the existing foundation that's already in place.

## What's Already Implemented ✅

Before we begin, let's understand what's already been built:

### Core Systems

- ✅ **NoiseService**: Generates Perlin noise maps using `FastNoiseLite`
- ✅ **NoiseLayerConfig**: Configuration resource for controlling noise generation
- ✅ **BiomeType**: Enum defining different biome types (Ocean, Plains, Desert, etc.)
- ✅ **BiomeRange**: Resource that maps height and equator values to biomes
- ✅ **GlobalRandom**: Centralized seeded random number generator
- ✅ **WorldGenerator**: Main world generation class (partially implemented)
- ✅ **ResourceNode**: Node for harvestable resources in the world
- ✅ **RawResource**: Resource data structure with properties
- ✅ **EnvironmentalDecorationType**: Enum for decoration types
- ✅ **EnvironmentalDecorationPlacementConfig**: Configuration for placing decorations

### What Works Now

The current system can:

- Generate noise-based height maps
- Assign biomes based on height and latitude (distance from equator)
- Basic WFC (Wave Function Collapse) integration for tile placement
- Define resources with properties (Strength, Conductivity, etc.)

---

## Tutorial Overview: What We'll Build

This tutorial will guide you through completing the procedural generation system by:

1. **Implementing Resource & Decoration Scattering** - Place trees, rocks, and ores based on biome rules
2. **Improving WFC Integration** - Make the tile system respect biome boundaries
3. **Creating a Planet Palette System** - Generate color schemes from world seeds
4. **Building the Visual Assets** - Guidelines for creating tileset and object sprites
5. **Testing & Refinement** - Making the world feel natural and playable

---

## Phase 1: Resource & Decoration Scattering

### Tutorial 1.1: Understanding the Scattering System

**What is scattering?** Scattering is the process of placing objects (trees, rocks, ore deposits) across the world in a natural-looking way. Instead of placing them randomly everywhere, we use noise maps to create clusters and patterns.

**How it works:**

1. Generate a separate noise map for each decoration type
2. Check if the noise value at each position exceeds a threshold
3. Check if the biome at that position is valid for that decoration
4. If both conditions are met, place the decoration

### Tutorial 1.2: Implementing Decoration Placement

The `WorldGenerator` already has `_environmentalDecorationPlacementConfigs` defined but doesn't use them yet. Let's implement the placement logic.

**Step 1:** Add noise map generation for each decoration type

In `WorldGenerator.cs`, add this method after `GenerateWorldData()`:

```csharp
private Dictionary<EnvironmentalDecorationType, float[,]> GenerateDecorationNoiseMaps()
{
    var noiseMaps = new Dictionary<EnvironmentalDecorationType, float[,]>();

    foreach (var config in _environmentalDecorationPlacementConfigs!)
    {
        var noiseMap = NoiseService.GenerateNoiseMap(
            WORLD_SIZE,
            WORLD_SIZE,
            GlobalRandom.Seed + (int)config.DecorationType, // Different seed per decoration
            config.NoiseConfig
        );
        noiseMaps[config.DecorationType] = noiseMap;
    }

    return noiseMaps;
}
```

**Why different seeds?** Using `GlobalRandom.Seed + (int)config.DecorationType` ensures each decoration type has its own unique but deterministic pattern.

**Step 2:** Create a method to check if decoration should be placed

```csharp
private bool ShouldPlaceDecoration(
    Vector2I position,
    EnvironmentalDecorationPlacementConfig config,
    float noiseValue)
{
    var biomeType = _worldMapBiomeTypes[position.X, position.Y];

    // Check if noise value is in the valid range
    if (noiseValue < config.MinimumValue || noiseValue > config.MaximumValue)
        return false;

    // Check if the current biome is valid for this decoration
    return config.ValidBiomes.Contains(biomeType);
}
```

**Step 3:** Update `RenderWorld()` to place decorations

Find the commented-out section in `RenderWorld()` that starts with `/* for (int x = 0; x < WORLD_SIZE; x++)` and replace it with:

```csharp
// Generate noise maps for all decoration types
var decorationNoiseMaps = GenerateDecorationNoiseMaps();

// Create a lookup for scene paths (you'll need to add these to Constants.Scenes)
var decorationScenes = new Dictionary<EnvironmentalDecorationType, PackedScene>
{
    { EnvironmentalDecorationType.Tree, _pineTreeScene! },
    { EnvironmentalDecorationType.OreDeposit, _goldOreScene! },
    // Add more as you create the scenes
};

// Iterate through the world and place decorations
for (int x = 0; x < WORLD_SIZE; x++)
{
    for (int y = 0; y < WORLD_SIZE; y++)
    {
        var position = new Vector2I(x, y);

        // Try to place each decoration type
        foreach (var config in _environmentalDecorationPlacementConfigs!)
        {
            // Skip if we don't have a scene for this decoration type yet
            if (!decorationNoiseMaps.ContainsKey(config.DecorationType) ||
                !decorationScenes.ContainsKey(config.DecorationType))
                continue;

            var noiseValue = decorationNoiseMaps[config.DecorationType][x, y];

            if (ShouldPlaceDecoration(position, config, noiseValue))
            {
                var scene = decorationScenes[config.DecorationType];
                var instance = scene.Instantiate<ResourceNode>();

                // Set resource data based on decoration type
                if (config.DecorationType == EnvironmentalDecorationType.OreDeposit)
                {
                    instance.ResourceData = _resources![ResourceKind.Ore][
                        GlobalRandom.Next(0, _resources[ResourceKind.Ore].Count)
                    ].Duplicate() as RawResource;
                }
                else if (config.DecorationType == EnvironmentalDecorationType.Tree)
                {
                    instance.ResourceData = _resources![ResourceKind.Wood][
                        GlobalRandom.Next(0, _resources[ResourceKind.Wood].Count)
                    ].Duplicate() as RawResource;
                }

                // Convert grid position to world position
                instance.Position = new Vector2(
                    (x + 0.5f) * TileSet.TileSize.X,
                    (y + 0.5f) * TileSet.TileSize.Y
                );
                instance.ZIndex = (int)Math.Round(instance.Position.Y);

                AddChild(instance);

                // Only place one decoration per tile
                break;
            }
        }
    }
}
```

**What this code does:**

- Generates unique noise patterns for each decoration type
- Checks each tile to see if it should have a decoration
- Only places one decoration per tile (first match wins)
- Sets the correct resource data based on decoration type
- Positions objects at the center of their tile

---

## Phase 2: Improving WFC Integration

### Tutorial 2.1: Understanding the Current WFC Setup

The current WFC (Wave Function Collapse) implementation uses hardcoded tiles:

- Tile 0: Plains
- Tile 1: Water

The WFC is currently **not connected** to the biome system. It generates independently.

### Tutorial 2.2: Connecting WFC to Biomes

We need to modify the WFC to respect our biome map. Here's how:

**Step 1:** Create biome-to-tile mappings

Add this class to `WorldGenerator.cs`:

```csharp
private class BiomeToTileMapping
{
    public Dictionary<BiomeType, int> BiomeToProtoTileIndex = new()
    {
        { BiomeType.Ocean, 1 },
        { BiomeType.Beach, 2 },
        { BiomeType.Plains, 0 },
        { BiomeType.Desert, 3 },
        { BiomeType.ForestDeciduous, 4 },
        { BiomeType.ForestConiferous, 5 },
        { BiomeType.Tundra, 6 },
        { BiomeType.Mountain, 7 },
        { BiomeType.Swamp, 8 },
        { BiomeType.Jungle, 9 },
    };
}
```

**Step 2:** Add more ProtoTiles

Expand the `protoTiles` list in `RenderWorld()` to include all biome types:

```csharp
List<ProtoTile> protoTiles =
[
    new() // 0 - Plains
    {
        Id = "plains",
        Weight = 10,
        NeighborIndices =
        [
            [ 0, 2, 3, 4 ], // up - can neighbor plains, beach, desert, deciduous forest
            [ 0, 2, 3, 4 ], // right
            [ 0, 2, 3, 4 ], // down
            [ 0, 2, 3, 4 ]  // left
        ]
    },
    new() // 1 - Water (Ocean)
    {
        Id = "ocean",
        Weight = 5,
        NeighborIndices =
        [
            [ 1, 2 ], // up - can only neighbor ocean or beach
            [ 1, 2 ], // right
            [ 1, 2 ], // down
            [ 1, 2 ], // left
        ]
    },
    new() // 2 - Beach (transition between water and land)
    {
        Id = "beach",
        Weight = 3,
        NeighborIndices =
        [
            [ 0, 1, 2, 3 ], // up - can neighbor most tiles
            [ 0, 1, 2, 3 ], // right
            [ 0, 1, 2, 3 ], // down
            [ 0, 1, 2, 3 ]  // left
        ]
    },
    new() // 3 - Desert
    {
        Id = "desert",
        Weight = 4,
        NeighborIndices =
        [
            [ 0, 2, 3 ], // up
            [ 0, 2, 3 ], // right
            [ 0, 2, 3 ], // down
            [ 0, 2, 3 ]  // left
        ]
    },
    // Add more biome tiles following the same pattern...
];
```

**Step 3:** Constrain WFC based on biome map

Modify the `getInitialValidProtoTilesForPosition` lambda in the `Output` constructor:

```csharp
var biomeMapping = new BiomeToTileMapping();

var output = new Output(configuration, width: WORLD_SIZE, height: WORLD_SIZE, depth: 1,
    getInitialValidProtoTilesForPosition: (x, y, z) =>
    {
        // Get the biome type at this position
        var biomeType = _worldMapBiomeTypes[x, y];

        // Only allow the proto tile that matches this biome
        if (biomeMapping.BiomeToProtoTileIndex.TryGetValue(biomeType, out int tileIndex))
        {
            // Return only the tile that matches this biome
            return new List<ProtoTile> { protoTiles[tileIndex] };
        }

        // Fallback to plains if biome not mapped
        return new List<ProtoTile> { protoTiles[0] };
    });
```

**What this achieves:** The WFC will now only place tiles that match the underlying biome, ensuring visual consistency with the logical biome map.

---

## Phase 3: Planet Palette System

### Tutorial 3.1: Creating the PlanetPalette Class

Colors are crucial for making each generated planet feel unique. Let's create a system that generates cohesive color schemes.

**Step 1:** Create `PlanetPalette.cs` in the Scripts folder

```csharp
using Godot;

namespace Wayblazer;

[GlobalClass]
public partial class PlanetPalette : Resource
{
    [Export] public Color PrimaryGround { get; set; }
    [Export] public Color SecondaryGround { get; set; }
    [Export] public Color WaterDeep { get; set; }
    [Export] public Color WaterShallow { get; set; }
    [Export] public Color FoliagePrimary { get; set; }
    [Export] public Color FoliageSecondary { get; set; }
    [Export] public Color MountainPrimary { get; set; }
    [Export] public Color DesertPrimary { get; set; }

    public PlanetPalette()
    {
        // Default Earth-like colors
        PrimaryGround = new Color(0.4f, 0.7f, 0.3f);
        SecondaryGround = new Color(0.6f, 0.5f, 0.4f);
        WaterDeep = new Color(0.1f, 0.3f, 0.8f);
        WaterShallow = new Color(0.3f, 0.6f, 0.9f);
        FoliagePrimary = new Color(0.2f, 0.6f, 0.2f);
        FoliageSecondary = new Color(0.4f, 0.3f, 0.2f);
        MountainPrimary = new Color(0.5f, 0.5f, 0.5f);
        DesertPrimary = new Color(0.9f, 0.8f, 0.5f);
    }
}
```

### Tutorial 3.2: Generating Palettes from Seeds

**Step 2:** Add palette generation to `WorldGenerator.cs`

```csharp
private PlanetPalette GeneratePlanetPalette()
{
    var palette = new PlanetPalette();

    // Use the world seed to generate a consistent base hue
    float baseHue = (GlobalRandom.Seed % 360) / 360.0f;

    // Generate complementary colors using HSV color theory
    // Primary ground color
    palette.PrimaryGround = Color.FromHsv(baseHue, 0.6f, 0.7f);

    // Secondary ground is slightly shifted hue
    palette.SecondaryGround = Color.FromHsv((baseHue + 0.1f) % 1.0f, 0.5f, 0.6f);

    // Water is opposite on color wheel (complementary)
    palette.WaterDeep = Color.FromHsv((baseHue + 0.5f) % 1.0f, 0.7f, 0.4f);
    palette.WaterShallow = Color.FromHsv((baseHue + 0.5f) % 1.0f, 0.5f, 0.7f);

    // Foliage is analogous to ground
    palette.FoliagePrimary = Color.FromHsv((baseHue + 0.2f) % 1.0f, 0.8f, 0.5f);
    palette.FoliageSecondary = Color.FromHsv((baseHue + 0.15f) % 1.0f, 0.6f, 0.4f);

    // Mountains are desaturated versions
    palette.MountainPrimary = Color.FromHsv(baseHue, 0.2f, 0.6f);

    // Desert is warm-shifted
    palette.DesertPrimary = Color.FromHsv((baseHue + 0.08f) % 1.0f, 0.4f, 0.8f);

    return palette;
}
```

**Understanding HSV:**

- **Hue (H)**: The color itself (0-1, representing 0-360 degrees on color wheel)
- **Saturation (S)**: How vivid the color is (0 = gray, 1 = pure color)
- **Value (V)**: How bright the color is (0 = black, 1 = bright)

**Step 3:** Apply the palette to tiles

Add this method to apply colors to your TileMapLayer:

```csharp
private void ApplyPalette(PlanetPalette palette)
{
    // You can modulate the entire TileMapLayer
    Modulate = palette.PrimaryGround;

    // Or create separate layers for different biome types and color them individually
    // This requires restructuring to use multiple TileMapLayer nodes
}
```

**Note:** For full palette support, you'll want to create separate TileMapLayer nodes for each biome type in your scene, then apply the appropriate color to each layer.

---

## Phase 4: Visual Asset Guidelines

### Tutorial 4.1: Tile Asset Creation

Since you'll be using procedural coloring, create your tiles in **grayscale** (white/gray/black only).

**What you need to create:**

**Base Terrain Tiles (48x48 pixels isometric diamonds):**

1. **Water/Ocean** - Animated if possible (2-4 frames)
2. **Beach/Sand** - Transition texture
3. **Plains/Grass** - Base ground tile
4. **Desert** - Sandy texture
5. **Forest Floor** - Dirt/leaf litter
6. **Tundra** - Snow/ice texture
7. **Mountain** - Rocky texture
8. **Swamp** - Muddy/wet texture

**Edge/Transition Tiles:**

- Water → Beach (8 variations: 4 edges + 4 corners)
- Beach → Plains (8 variations)
- Plains → Forest (8 variations)
- Plains → Mountain (8 variations)

**Why grayscale?** When you apply `Modulate` or `SelfModulate` with a color in Godot, it multiplies with the texture. White (1,1,1) becomes the modulate color, black (0,0,0) stays black, gray becomes a darker shade of the modulate color.

### Tutorial 4.2: Object/Decoration Assets

**Environmental Decorations:**

1. **Trees** (draw in grayscale on separate layers)
   - Trunk layer (will be colored with `FoliageSecondary`)
   - Leaves layer (will be colored with `FoliagePrimary`)
   - Create 3-5 variations

2. **Rocks** (32x32 sprites)
   - Small rock cluster
   - Medium boulder
   - Large rock formation
   - Create 3-4 variations

3. **Resource Nodes** (32x32 animated sprites)
   - Ore deposits (metallic look, 2-4 frames)
   - Gas vents (wispy, 4-8 frames)
   - Special flora (glowing, 4-6 frames)

**Naming Convention:**

```
tree_oak_01.png
tree_pine_01.png
rock_small_01.png
rock_large_02.png
ore_crystal_01.png
```

---

## Phase 5: Creating Scene Files

### Tutorial 5.1: Setting Up Resource Node Scenes

Currently, you have `pine_tree.tscn` and `gold_ore.tscn`. Let's create a template for more.

**Step 1:** Open Godot and create a new scene

1. Click "+" or Scene → New Scene
2. Select "Other Node" and choose `ResourceNode` (your custom class)
3. Add an `AnimatedSprite2D` as a child
4. Name it "AnimatedSprite2D" (the code looks for this name)

**Step 2:** Configure the AnimatedSprite2D

1. Select the AnimatedSprite2D
2. In Inspector, create a new `SpriteFrames` resource
3. Add your animation frames:
   - Default animation: idle state
   - Optional: harvest animation

**Step 3:** Set up the root ResourceNode

1. Select the root node (ResourceNode)
2. In Inspector, you can set a default `ResourceData` for testing
3. Make sure the script is attached (`ResourceNode.cs`)

**Step 4:** Save the scene

Save as `Scenes/Objects/oak_tree.tscn`, etc.

### Tutorial 5.2: Adding Scene References to Constants

Update `Constants.cs` to include your new scenes:

```csharp
public static class Scenes
{
    public const string PINE_TREE = "res://Scenes/Objects/pine_tree.tscn";
    public const string OAK_TREE = "res://Scenes/Objects/oak_tree.tscn";
    public const string BIRCH_TREE = "res://Scenes/Objects/birch_tree.tscn";
    public const string FIR_TREE = "res://Scenes/Objects/fir_tree.tscn";
    public const string GOLD_ORE = "res://Scenes/Objects/gold_ore.tscn";
    public const string IRON_ORE = "res://Scenes/Objects/iron_ore.tscn";
    public const string COPPER_ORE = "res://Scenes/Objects/copper_ore.tscn";
    public const string ROCK_SMALL = "res://Scenes/Objects/rock_small.tscn";
    public const string ROCK_LARGE = "res://Scenes/Objects/rock_large.tscn";
    public const string BUSH_01 = "res://Scenes/Objects/bush_01.tscn";
    public const string GRASS_TUFT = "res://Scenes/Objects/grass_tuft.tscn";
    public const string GAS_VENT = "res://Scenes/Objects/gas_vent.tscn";
}
```

---

## Phase 6: Advanced Features

### Tutorial 6.1: Adding Biome-Specific Resources

Right now, all `ResourceKind.Wood` resources can spawn with any tree. Let's make it biome-specific.

**Step 1:** Add biome data to resources

Modify `RawResource.cs` to include preferred biomes:

```csharp
[Export]
public Array<BiomeType> PreferredBiomes { get; set; } = new();
```

**Step 2:** Update resource definitions in `WorldGenerator.cs`

```csharp
{ ResourceKind.Wood, new Array<RawResource>()
    {
        new RawResource("Pine", ResourceKind.Wood, new Dictionary<ResourcePropertyType, ResourceProperty>()
        {
            // ... properties ...
        })
        {
            PreferredBiomes = new Array<BiomeType>()
            {
                BiomeType.ForestConiferous,
                BiomeType.Tundra
            }
        },
        new RawResource("Oak", ResourceKind.Wood, new Dictionary<ResourcePropertyType, ResourceProperty>()
        {
            // ... properties ...
        })
        {
            PreferredBiomes = new Array<BiomeType>()
            {
                BiomeType.ForestDeciduous,
                BiomeType.Plains
            }
        }
    }
}
```

**Step 3:** Filter resources by biome when placing

Modify the decoration placement code:

```csharp
if (config.DecorationType == EnvironmentalDecorationType.Tree)
{
    var biome = _worldMapBiomeTypes[x, y];
    var validWoods = _resources![ResourceKind.Wood]
        .Where(r => ((RawResource)r).PreferredBiomes.Contains(biome))
        .ToArray();

    if (validWoods.Length > 0)
    {
        instance.ResourceData = validWoods[
            GlobalRandom.Next(0, validWoods.Length)
        ].Duplicate() as RawResource;
    }
}
```

### Tutorial 6.2: Adding Resource Variety Within Biomes

To make each resource spawn point unique:

**Step 1:** Add quantity variation to resources

```csharp
[Export]
public int MinQuantity { get; set; } = 1;

[Export]
public int MaxQuantity { get; set; } = 5;
```

**Step 2:** Randomize quantity when spawning

```csharp
var resource = validWoods[GlobalRandom.Next(0, validWoods.Length)]
    .Duplicate() as RawResource;

resource.Quantity = GlobalRandom.Next(resource.MinQuantity, resource.MaxQuantity + 1);
instance.ResourceData = resource;
```

---

## Phase 7: Testing & Balancing

### Tutorial 7.1: Creating a Test Scene

**Step 1:** Create a simple test world

```csharp
// Add this method to WorldGenerator for testing
public void RegenerateWorld()
{
    // Clear existing tiles and children
    Clear();
    foreach (Node child in GetChildren())
    {
        child.QueueFree();
    }

    // Generate new world with new seed
    GlobalRandom.InitializeWithSeed((int)GD.Randi());
    GenerateWorldData();
    RenderWorld();

    GD.Print($"World regenerated with seed: {GlobalRandom.Seed}");
}
```

**Step 2:** Add a debug key binding

In your `GameManager` or a test script:

```csharp
public override void _Input(InputEvent @event)
{
    if (@event is InputEventKey keyEvent && keyEvent.Pressed)
    {
        if (keyEvent.Keycode == Key.F5)
        {
            var worldGen = GetNode<WorldGenerator>("WorldGenerator");
            worldGen.RegenerateWorld();
        }
    }
}
```

Now you can press F5 to regenerate the world and see different results!

### Tutorial 7.2: Balancing Resource Density

If you have too many or too few resources:

**Adjust the noise threshold:**

```csharp
new EnvironmentalDecorationPlacementConfig(
    EnvironmentalDecorationType.Tree,
    new NoiseLayerConfig() { Frequency = 0.1f, Octaves = 3 },
    minimumValue: 0.3f,  // Increase this = fewer trees
    maximumValue: 0.7f,  // Decrease this = fewer trees
    validBiomes: new Array<BiomeType>() { BiomeType.ForestDeciduous }
)
```

**Adjust the noise frequency:**

- Higher frequency = more, smaller clusters
- Lower frequency = fewer, larger clusters

---

## Summary: What You've Built

By following this tutorial, you've created:

1. ✅ **Multi-layered noise generation** for terrain and decorations
2. ✅ **Biome system** that respects elevation and latitude
3. ✅ **Scatter system** that places decorations based on biome rules
4. ✅ **WFC integration** that generates tiles matching biome types
5. ✅ **Planet palette system** for unique color schemes per world
6. ✅ **Resource system** with properties and biome preferences
7. ✅ **Scene templates** for easy content creation
8. ✅ **Testing tools** for rapid iteration

---

## Next Steps

### Immediate Enhancements

1. **Create more visual assets** - Expand your tileset and decorations
2. **Add more biome types** - Implement unique rules for each
3. **Improve transitions** - Add more edge tiles for smooth biome blending
4. **Add animation** - Water, grass, and gas vents should animate

### Advanced Features (Future)

1. **Chunk loading** - Generate only visible areas for larger worlds
2. **Caves & dungeons** - Use 3D noise for underground generation
3. **Rivers & lakes** - Special algorithms for water bodies
4. **Structures** - Integrate pre-made buildings/ruins
5. **Erosion simulation** - Make terrain more realistic
6. **Biome temperature/moisture** - More sophisticated climate model

---

## Troubleshooting Common Issues

### Issue: Resources overlapping

**Solution:** Check only one decoration per tile (the code already does this with `break`)

### Issue: Biomes look blocky

**Solution:** Add more transition tiles and reduce noise frequency

### Issue: Colors look wrong

**Solution:** Ensure your sprites are grayscale; check `Modulate` vs `SelfModulate`

### Issue: WFC fails/errors

**Solution:** Ensure all biomes have valid ProtoTile mappings and neighbor rules

### Issue: World generation is slow

**Solution:** Profile with Godot's profiler; consider generating in chunks or using threading

---

## Resources for Learning More

- **Godot Docs**: https://docs.godotengine.org/
- **Perlin Noise**: Understanding procedural generation
- **Wave Function Collapse**: Look up "WFC algorithm explained"
- **Color Theory**: HSV vs RGB for procedural colors
- **Isometric Art**: Creating tiles for isometric games

---

This tutorial should give you a solid foundation for procedural generation in Godot. Remember: start simple, test often, and iterate on what works. The beauty of procedural generation is that small changes can have big effects!

Happy generating! 🌍✨
