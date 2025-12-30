# 🌍 Procedural Generation Deep-Dive: Phase 3 & Beyond

**Current Status:**

* ✅ **Noise Generation:** `NoiseService` is producing valid heightmaps.
* ✅ **Biome Data:** `WorldGenerator` successfully maps height + latitude to `BiomeType`.
* ✅ **Data Models:** `RawResource`, `PlanetaryConstants`, and `EnvironmentalDecorationPlacementConfig` are ready.

**Next Objective:** We have the *data* (the "Blueprint"), now we need the *visuals* (the "Construction").

---

## Task 3: Biome-Constrained Wave Function Collapse (WFC)

**Goal:** Turn the abstract `_worldMapBiomeTypes` grid into a beautiful, seamless TileMap.

**Concept:**
Imagine a Sudoku puzzle. You can't put a "5" next to a "5". Similarly, you can't put a "Deep Ocean" tile directly next to a "Mountain Peak" tile. WFC solves this puzzle for us.

### Step 3.1: Setting up the TileSet (Godot Editor)

Before writing code, we need tiles to work with.

1. **Create the Asset:**
    * In the FileSystem, right-click `Assets/TinySwords/Terrain` -> Create New -> **Resource**.
    * Search for **TileSet**. Name it `WorldTileSet.tres`.
2. **Configure the Atlas:**
    * Double-click `WorldTileSet.tres` to open the bottom panel.
    * Drag your terrain texture (e.g., `Assets/TinySwords/Terrain/Tileset/Water.png`) into the "Tiles" panel.
    * Select "Yes" to automatically create tiles.
3. **Define Custom Data (The "Metadata"):**
    * In the Inspector (right side) for the `TileSet` resource:
    * Expand **Custom Data Layers**.
    * Click **Add Element**.
    * Name it `BiomeType`. Set Type to **Integer**.
4. **Tagging the Tiles:**
    * In the TileSet editor (bottom), click **Select**.
    * Click a specific tile (e.g., a water tile).
    * In the Inspector, find **Custom Data**. Set `BiomeType` to `0` (or whatever your `BiomeType.Ocean` integer value is).
    * *Repeat this for a few key tiles (Grass, Sand, Snow).*

### Step 3.2: The WFC Solver Script

We need a script that looks at our `_worldMapBiomeTypes` and picks the correct tile ID.

1. **Create the Script:** Create `Scripts/WfcService.cs`.
2. **The Logic:**

```csharp
using Godot;
using System.Collections.Generic;

namespace Wayblazer;

public static class WfcService
{
    // A simple "Rule" class to define what can touch what
    public class TileRule
    {
        public int TileId;
        public BiomeType Biome;
        public List<int> AllowedNeighbors; // IDs of tiles this can touch
    }

    public static void PaintWorld(TileMapLayer tileMap, BiomeType[,] biomeGrid, TileSet tileSet)
    {
        int width = biomeGrid.GetLength(0);
        int height = biomeGrid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BiomeType targetBiome = biomeGrid[x, y];

                // 1. Find a tile in the TileSet that matches this Biome
                // Note: This is a simplified lookup. Real WFC would check neighbors.
                Vector2I atlasCoords = GetAtlasCoordsForBiome(targetBiome);

                // 2. Place the cell
                // Source ID 0 usually refers to the first texture atlas you added
                tileMap.SetCell(new Vector2I(x, y), 0, atlasCoords);
            }
        }
    }

    private static Vector2I GetAtlasCoordsForBiome(BiomeType biome)
    {
        // TODO: Replace this hardcoded switch with a lookup from the TileSet Custom Data
        return biome switch
        {
            BiomeType.Ocean => new Vector2I(0, 0), // Example coordinates
            BiomeType.Plains => new Vector2I(1, 0),
            BiomeType.Mountain => new Vector2I(2, 0),
            _ => new Vector2I(3, 0) // Default/Error tile
        };
    }
}
```

### Step 3.3: Integrating into WorldGenerator

1. Open `WorldGenerator.cs`.
2. Update `RenderWorld()` to use this new service.

```csharp
public void RenderWorld()
{
    // Ensure we have a TileSet assigned to this TileMapLayer
    if (TileSet == null)
    {
        GD.PrintErr("No TileSet assigned to WorldGenerator!");
        return;
    }

    // Call our simple painter
    WfcService.PaintWorld(this, _worldMapBiomeTypes, TileSet);

    GD.Print($"World rendered: {WORLD_SIZE}x{WORLD_SIZE} tiles");
}
```

---

## Task 4: Object Spawning (The Decoration Layer)

**Goal:** Populate the empty tiles with Trees, Rocks, and Resources based on your `EnvironmentalDecorationPlacementConfig`.

### Step 4.1: The ResourceNode Scene

We need a generic object that can represent *any* resource.

1. **Create Scene:**
    * New Scene -> **Node2D**. Name it `ResourceNode`.
    * Add a child **Sprite2D**.
    * Save as `Scenes/ResourceNode.tscn`.
2. **Create Script:** Attach `Scripts/ResourceNode.cs`.

```csharp
using Godot;

namespace Wayblazer;

public partial class ResourceNode : Node2D
{
    [Export] public Sprite2D Sprite { get; set; }
    public RawResource ResourceData { get; set; }

    public void Initialize(RawResource data)
    {
        ResourceData = data;
        // TODO: Load a specific texture based on the ResourceName or Kind
        // Sprite.Texture = GD.Load<Texture2D>($"res://Assets/Resources/{data.Name}.png");

        // For now, just tint it to show it's working
        if (data.ResourceKind == ResourceKind.Ore) Modulate = Colors.Red;
        if (data.ResourceKind == ResourceKind.Wood) Modulate = Colors.Green;
    }
}
```

### Step 4.2: The Spawning Logic

Now we implement the logic to place these nodes.

1. Open `WorldGenerator.cs`.
2. Add a new method `SpawnDecorations()`.

```csharp
private void SpawnDecorations()
{
    // 1. Loop through every tile
    for (int x = 0; x < WORLD_SIZE; x++)
    {
        for (int y = 0; y < WORLD_SIZE; y++)
        {
            BiomeType currentBiome = _worldMapBiomeTypes[x, y];

            // 2. Check every configuration rule
            foreach (var config in _decorationConfigs) // You need to expose this list
            {
                // Rule A: Is this biome allowed?
                if (!config.ValidBiomes.Contains(currentBiome)) continue;

                // Rule B: Noise Check
                // We need to generate a specific noise value for this decoration type
                float noiseVal = NoiseService.GetNoiseAt(x, y, config.NoiseConfig); // Need to add GetNoiseAt helper

                // Rule C: Threshold Check
                if (noiseVal >= config.MinimumValue && noiseVal <= config.MaximumValue)
                {
                    SpawnObject(x, y, config);
                    break; // Only one object per tile?
                }
            }
        }
    }
}

private void SpawnObject(int x, int y, EnvironmentalDecorationPlacementConfig config)
{
    // Instantiate the generic node
    var instance = _resourceNodeScene.Instantiate<ResourceNode>();
    instance.Position = MapToLocal(new Vector2I(x, y));

    // If this decoration is a Resource (like a Tree), give it data
    if (config.DecorationType == EnvironmentalDecorationType.Tree)
    {
        // Pick a random wood resource
        var woodData = _resources[ResourceKind.Wood].PickRandom();
        instance.Initialize(woodData);
    }

    AddChild(instance);
}
```

---

## Task 5: Integrating Planetary Constants

**Goal:** Make the world generation react to the planet's stats (Gravity, Temperature, etc.).

### Step 5.1: Modifying Noise Generation

High tectonic volatility should create more mountains.

1. Open `WorldGenerator.cs`.
2. In `GenerateWorldData()`, modify the `NoiseLayerConfig` before generating the map.

```csharp
public void GenerateWorldData()
{
    var worldBiomeMapNoiseConfig = new NoiseLayerConfig();

    // INFLUENCE: Tectonic Volatility affects Frequency (Jaggedness)
    // Base frequency 0.01. If Volatility is 1.0 (High), freq becomes 0.02.
    worldBiomeMapNoiseConfig.Frequency = 0.01f + (PlanetaryConstants.Current.TectonicVolatility * 0.01f);

    // ... rest of generation
}
```

### Step 5.2: Modifying Biome Thresholds

A hot planet should have more Desert and less Tundra.

1. In `GenerateWorldData()`, when defining `biomeRanges`:

```csharp
float tempOffset = PlanetaryConstants.Current.HighTemperature > 40 ? 0.1f : 0.0f;

Array<BiomeRange> biomeRanges =
[
    // Desert usually 0.2-0.4 equator. On hot planet, expand to 0.1-0.5
    new BiomeRange(BiomeType.Desert, 0.3f, 0.5f, 0.2f - tempOffset, 0.4f + tempOffset),
    // ...
];
```

---

## Summary of Immediate Actions

1. **Create `WfcService.cs`** and implement the basic tile painter.
2. **Create `ResourceNode.tscn`** and its script.
3. **Update `WorldGenerator.cs`** to call `WfcService` and `SpawnDecorations`.
