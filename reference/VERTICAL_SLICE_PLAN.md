# Vertical Slice Plan

## Status Overview

| Status | Sprint | Estimated Time | Actual Time |
| --- | --- | --- | ---- |
| ✅ | Sprint 1: Project Setup & Data Core #2 | 16hrs | 2hrs |
| ❌ | Sprint 2: Grid, Player Movement & Voxel Data #13 | 16hrs | |
| ❌ | Sprint 3: Resource Engine & ProcGen V1 #21 | 18hrs | |
| ❌ | Sprint 4: Interaction & Scanner UI #20 | 16hrs | |
| ❌ | Sprint 5: Field Lab & Analysis Logic #14 | 16hrs | |
| ❌ | Sprint 6: Planetary Analysis & Deduction Input #15 | 16hrs | |
| ❌ | Sprint 7: Basic Crafting & Inventory #17 | 16hrs | |
| ❌ | Sprint 8: Advanced Refining (Composition) #18 | 16hrs | |
| ❌ | Sprint 9: ProcGen Tech Tree V2 & Unlocks #19 | 16hrs | |
| ❌ | Sprint 10: Portal Construction & UI #16 | 16hrs | |
| ❌ | Sprint 11: Simulation Core & Win State #12 | 16hrs | |
| ❌ | Sprint 12: Aesthetic Polish & Juiciness #11 | 16hrs | |

# ⚙️ Sprint 1: Project Setup & Data Core (16 Hours)

## Summary

This sprint establishes the C\# data architecture for the game's core deduction system. We will create the foundational classes for resources, planetary properties, and portal requirements. This data layer is the "Brain" of the game and must be logically sound before visual elements are built.

## 🎯 Goal

A set of fully defined, functional C\# classes (not necessarily Godot nodes yet) that can generate a unique world puzzle and the corresponding material solution.

## 💻 Tech Stack Focus

  * **Godot Engine:** Project creation and configuration.
  * **C\# / .NET:** All core logic and data structures.

#### Time Log

**1: Project Setup and Godot Configuration:** 120 mins (Actual: 10 mins)
**2: Data Structure - Resource Property:** 120 mins (Actual: 10 mins)
**3: Data Structure - Raw Resource:** 120 mins (Actual: 10 mins)
**4: Data Structure - Composite Material:** 120 mins (Actual: 15 mins)
**5: Data Structure - Planetary Constants:** 120 mins (Actual: 5 mins)
**6: Acquire Placeholder Tileset and Sprite Sheets:** 120 mins (Actual: 15 mins)
**7: Data Structure - Portal Requirement:** 120 mins (Actual: 20 mins)
**8: Simple Unit Test and Review:** 120 mins (Actual: 35 mins)
**Total:** 16 hrs (Actual: 2 hrs)

-----

## Task Breakdown (16 Hours)

### Task 1: Project Setup and Godot Configuration (2 Hours)

#### Create and Configure Godot Project (0h - 1h)

1. Create a new Godot 4 project using the **C#** template.
2. Set the renderer to **2D** or **Compatibility** (since we are using 2D for the VS).
3. Create a top-level folder named `Scripts` to hold all C# files.
4. Create a main scene (`World.tscn`) and save it.

#### Editor Workflow Check (1h - 2h)

1. Open the project in your IDE (e.g., Visual Studio Code or Rider).
2. Verify that the Godot C# solution file (`.sln`) is loaded correctly and that C# scripts can be added and compiled without error. *Troubleshoot any .NET SDK path issues now.*

### Task 2: Data Structure - Resource Property (2 Hours)

This class defines a single measurable quality (e.g., "Strength") and its value.

#### Create Enumerator and Base Class (2h - 3h)

1. In the `Scripts` folder, create `Enums.cs`. Define `enum ResourcePropertyType { Strength, Resistance, Toughness, Conductivity, Reactivity }`.
2. Create `ResourceProperty.cs`. Define a C# `struct` named `ResourceProperty` (use `struct` for performance and value semantics).

#### Define Fields and Helper Methods (3h - 4h)

1. Add the following fields to `ResourceProperty`:
    - `public ResourcePropertyType Type;`
    - `public float Value;`
    - `public string VagueDescription;` (e.g., "High Integrity")
2. Implement a constructor `public ResourceProperty(ResourcePropertyType type, float value)` that sets `Type` and `Value`.
3. Implement a private method `SetVagueDescription()` inside the constructor: `if (Value > 7.0f) { VagueDescription = "High"; } else if (Value < 3.0f) { VagueDescription = "Low"; } else { VagueDescription = "Medium"; }`

### Task 3: Data Structure - Raw Resource (2 Hours)

This is the data object for all gatherable materials in the game.

#### Create RawResource Class (4h - 5h)

1. Create `RawResource.cs`. Define a C# class `RawResource`.
2. Add core descriptive fields:
    - `public string Name;` (e.g., "Dull Grey Ore")
    - `public string Description;`
    - `public int BaseHarvestDifficulty;` (1 for VS).

#### Implement Property Storage (5h - 6h)

1. Add the crucial storage field: `public Dictionary<ResourcePropertyType, ResourceProperty> Properties = new();`
2. Implement a simple stub method `public void GenerateProperties(int seed)`:
    - For the VS, hardcode the properties of one "Low Strength Ore" and one "High Strength Ore" using a simple random or fixed seed logic. (e.g., Low Strength Ore: Strength=2.0, Resistance=5.0. High Strength Ore: Strength=8.0, Resistance=2.0).

### Task 4: Data Structure - Composite Material (2 Hours)

This handles the combining of materials, the core of the engineering puzzle.

#### Composite Class Definition (6h - 7h)

1. Create `CompositeMaterial.cs`. This class should inherit from `RawResource`.
2. Add tracking fields for its inputs:
    - `public RawResource PrimaryIngredient;`
    - `public RawResource ModifierIngredient;`
    - `public float StrengthModifier;` (to track the effect of gas/additive).

#### Implement Calculation Logic (7h - 8h)

1. Implement a method `public void CalculateProperties()`.
2. **VS Logic:** The Composite's final Strength will be the sum of its ingredients' strength, plus a multiplier from a gas.
   - `float baseStrength = PrimaryIngredient.Properties[Strength].Value + ModifierIngredient.Properties[Strength].Value;`
   - `float finalStrength = baseStrength * StrengthModifier;`
3. Set the Composite's Strength property using this calculated value.

### Task 5: Data Structure - Planetary Constants (2 Hours)

This defines the unique puzzle for the current run (the "Puzzle Frame").

#### Planetary Constants Class (8h - 9h)

1. Create `PlanetaryConstants.cs`. Define a C# class `PlanetaryConstants`.
2. Add float fields for the 3 key VS properties (from the GDD):
    - `public float GravimetricShear;` (Range 0.5 - 5.0)
    - `public float CorrosiveIndex;` (Range 0.0 - 14.0)
    - `public float TectonicVolatility;` (Range 0.0 - 9.0)

#### World Generation Logic (9h - 10h)

1. Implement `public void GenerateWorld(int complexityLevel)`.
2. For the VS, hardcode the Level 1 values for the first playthrough:
    - `GravimetricShear = 3.2f;` (High but manageable)
    - `CorrosiveIndex = 2.0f;` (Low)
    - `TectonicVolatility = 1.0f;` (Low)
3. This ensures every test run uses the same baseline puzzle.

### Task 6: Acquire Placeholder Tileset and Sprite Sheets (2 Hours)

Begin the visual design process to guide future art sprints.

#### Placeholder Tileset (10h - 11h)

1. Find tileset and add it to Godot project.

#### Placeholder Sprite Sheets (11h - 12h)

1. Find sprite sheet(s) for the player character (and other moving things?) and add it/them to Godot project.

### Task 7: Data Structure - Portal Requirement (2 Hours)

This defines the ultimate goal and the mathematical deduction.

#### Portal Requirement Class (12h - 13h)

1. Create `PortalRequirement.cs`. Define a C# class `PortalRequirement`.
2. Add a dictionary to store the requirements: `public Dictionary<ResourcePropertyType, float> RequiredStats = new();`
3. Add fields to store the World Constants (for reference): `public PlanetaryConstants WorldContext;`

#### Deduction Formula Implementation (13h - 14h)

1. Implement `public void SetRequirements(PlanetaryConstants constants)`:
2. **VS Logic:** Implement the primary deduction formula from the GDD:
    - **Foundation Strength:** `RequiredStats[Strength] = constants.GravimetricShear * 2.5f;` (Target: 8.0)
    - **Gate Resistance:** `RequiredStats[Resistance] = constants.CorrosiveIndex * 1.5f;` (Target: 3.0)
3. This ensures the requirements are generated *from* the world properties.

### Task 8: Simple Unit Test and Review (2 Hours)

Verify the core deduction math works outside of the Godot environment.

#### Unit Test Project (C#/.NET) (14h - 15h)

1. Create a separate C# Console Application project in the solution (e.g., `Wayblazer.Tests`).
2. Reference the main Godot C# assembly (`Wayblazer`).
3. Write a single function `TestPortalDeductionMath()`:

a. Instantiate `PlanetaryConstants` and call `GenerateWorld()`.
b. Instantiate `PortalRequirement` and call `SetRequirements()`.
c. Instantiate a `CompositeMaterial` and manually set its properties to a known "passing" value (e.g., Strength 9.0) and a "failing" value (e.g., Strength 7.0).
d. Use a simple `if` check (`if (composite.Properties[Strength].Value > required.RequiredStats[Strength])`) to confirm the pass/fail logic is mathematically correct.

#### Code Review and Cleanup (15h - 16h)

1. Review all C# code for consistent naming conventions and commenting.
2. Delete the temporary Console Test project if it's no longer needed, or commit it as a verification tool.
3. **Commit Code:** Commit all changes to the Version Control System (VCS) with the message: "Sprint 1 Complete: Initial Data Core Architecture."

-----

# 🗺️ Sprint 2: Grid, Player Movement & Voxel Data (16 Hours)

## Summary

This sprint focuses on establishing the physical presence of the game. We will set up the Godot `TileMap` for our 2.5D world, implement the player character, and link the procedural data created in Sprint 1 to the visual representation on the map.

## 🎯 Goal

A playable scene where the character can move around a basic, generated world composed of different tile types (Ground, Ore, Wood) and visually identify the resources they are standing on.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** `TileMap`, `CharacterBody2D`, and basic scene setup.
  * **C\# / .NET:** World generation logic and data linking.

#### Time Log

**1: Godot TileMap and Tileset Setup:** 180 mins (Actual: 20 mins)
**2: World Generation and Rendering:** 180 mins (Actual: 20 mins)
**3: Player Controller and Scene Setup:** 240 mins (Actual: -)
**4: Resource Node Linking:** 180 mins (Actual: -)
**5: Aesthetic & Review:** 180 mins (Actual: -)
**Total:** 16 hrs (Actual: 40 mins)

-----

## Task Breakdown (16 Hours)

### Task 1: Godot TileMap and Tileset Setup (3 Hours)

#### TileMap Node Setup (0h - 1h)

1. In `World.tscn`, add a new `TileMap` node. Rename it to `WorldMap`.
2. In the `TileMap` inspector, set the **Cell Size** to a square size appropriate for isometric/2D grid (e.g., **64x64** or **32x32**).
3. Set the **Y Sort Enabled** property to **true** in the `WorldMap` node's transform settings. (Crucial for 2.5D depth illusion).

#### Create Basic Tileset (1h - 2h)

1. In the `TileMap` inspector, click the **Tile Set** property to create a **New TileSet** resource.
2. Open the TileSet editor.
3. Add **4 new Atlas sources** (use simple colored squares or the concept art from Sprint 1):
    - `ID 0`: **Ground** (e.g., Green/Brown square).
    - `ID 1`: **Ore Node** (e.g., Grey square).
    - `ID 2`: **Wood Node** (e.g., Dark Green/Brown square).
    - `ID 3`: **Water** (e.g., Blue square).

#### Tilemap Rendering Script Stub (2h - 3h)

1. Create a new C# script `WorldGenerator.cs` (Godot script attached to `WorldMap` node).
2. Add a reference to the TileMap node: `private TileMap _tileMap;`
3. In `_Ready()`, get the node: `_tileMap = GetNode<TileMap>(".");`
4. Define a constant for world size: `const int WorldSize = 64;` (64x64 tiles for VS).

### Task 2: World Generation and Rendering (3 Hours)

#### ProcGen V1: Simple World Array (3h - 4h)

1. In `WorldGenerator.cs`, define a private 2D array: `private int[,] _worldData = new int[WorldSize, WorldSize];`
2. Implement a stub method `GenerateInitialWorld()`:
    - Fill the entire array with `ID 0` (Ground).
    - Use `GD.RandRange(0, 100)` to randomly place `ID 1` (Ore) and `ID 2` (Wood) nodes at a density of about 5% each.

#### Render World to TileMap (4h - 5h)

1. Implement a method `RenderWorld()`:
    - Loop through `_worldData` from `x = 0` to `WorldSize` and `y = 0` to `WorldSize`.
    - Use `_tileMap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(_worldData[x, y], 0));` (Note: Ensure the source IDs match your TileSet setup).
2. Call `GenerateInitialWorld()` and then `RenderWorld()` in `_Ready()`.

#### World Context Link (5h - 6h)

1. Create a `GameManager.cs` C# class (make it a singleton via `[GlobalClass]` for easy access).
2. In `GameManager.cs`, create and initialize the C# data objects from Sprint 1: `public PlanetaryConstants Constants = new();` and `public PortalRequirement Required = new();`
3. In `_Ready()` of `GameManager.cs`, call `Constants.GenerateWorld(1)` and `Required.SetRequirements(Constants)` to load the VS puzzle.

### Task 3: Player Controller and Scene Setup (4 Hours)

#### Player Scene Setup (6h - 7h)

1. In the Godot editor, click **Scene > New Scene** from the menu.
2. Click **Other Node** in the Create Root Node dialog, search for **CharacterBody2D**, and select it.
3. With the root node selected, click the **Rename** icon (or press F2) and rename it to **Player**.
4. Click **Scene > Save Scene** and save as `Player.tscn` in the `Scenes` folder.
5. Right-click the **Player** node and select **Add Child Node**. Search for and add a **Sprite2D** node.
6. In the Sprite2D's Inspector, drag a placeholder player texture (from Sprint 1) into the **Texture** property.
7. Right-click the **Player** node again and add a **CollisionShape2D** child node.
8. In the CollisionShape2D's Inspector, click the **Shape** property dropdown and select **New RectangleShape2D**.
9. Click the newly created RectangleShape2D to expand its properties, and adjust the **Size** to roughly match your character sprite (e.g., Vector2(32, 48) for a 32x48 pixel character).
10. Open `World.tscn`, right-click the root node, select **Instantiate Child Scene**, and choose `Player.tscn`. Position it in the center of the visible area.
#### Basic Movement Script (7h - 8h)

1. Select the **Player** root node in `Player.tscn`.
2. In the Inspector, scroll to the top and click the **Attach Script** icon (looks like a scroll with a + sign).
3. In the Attach Node Script dialog, ensure the **Language** is set to **C#**, and the **Path** is set to `Scripts/PlayerController.cs`. Click **Create**.
4. In your IDE (VS Code/Rider), open the newly created `PlayerController.cs` file.
5. Replace the default code with this beginner-friendly movement template:

```csharp
using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
    // Movement speed in pixels per second
    private const float Speed = 200.0f;

    // Called every physics frame (60 times per second by default)
    public override void _PhysicsProcess(double delta)
    {
        // Get input direction using Godot's built-in UI actions
        // This returns a normalized Vector2 (-1 to 1 on each axis)
        Vector2 inputDirection = Input.GetVector(
            "ui_left",   // A key or Left Arrow
            "ui_right",  // D key or Right Arrow
            "ui_up",     // W key or Up Arrow
            "ui_down"    // S key or Down Arrow
        );

        // Set the velocity based on input and speed
        Velocity = inputDirection * Speed;

        // Apply the movement and handle collisions
        MoveAndSlide();
    }
}
```

6. Save the file. Build the project by pressing **Build** in Godot's top menu (or press F6).
7. Fix any compilation errors before proceeding.
#### Movement Testing and Collision (8h - 9h)

1. Press **F5** to run the game (or click the Play button). You should see your player character.
2. Test movement using WASD or arrow keys. The character should move smoothly in all four directions.
3. **Diagonal Movement Check:** Press two keys at once (e.g., W+D). The character should move diagonally. If the movement feels too fast diagonally, add this code after the `inputDirection` line:

```csharp
// Normalize diagonal movement to prevent faster diagonal speed
inputDirection = inputDirection.Normalized();
```

4. **Setting Up TileMap Collisions** (if not done in Task 1):
    - Open `World.tscn` and select the **WorldMap** TileMap node.
    - Click the **TileSet** property in the Inspector to open the TileSet editor at the bottom.
    - Select your Ground tile. In the middle panel, click the **Physics** tab.
    - Click **Add Physics Layer** if no physics layer exists.
    - Use the polygon tool to draw a collision shape around the tile (usually a rectangle for ground tiles).
    - Save and test. The player should not be able to walk through tiles with collision enabled.
#### Camera and Y-Sort Setup (9h - 10h)

1. In `Player.tscn`, right-click the **Player** root node and select **Add Child Node**.
2. Search for and add a **Camera2D** node.
3. With the Camera2D selected, check these properties in the Inspector:
    - **Enabled:** ON (checked)
    - **Position Smoothing > Enabled:** ON (for smooth camera tracking)
    - **Position Smoothing > Speed:** 5.0 (adjust for desired smoothness)
4. **Implementing Y-Sort for 2.5D Depth:**
    - In `PlayerController.cs`, add this line inside `_PhysicsProcess`, after `MoveAndSlide()`:

```csharp
// Update Z-index based on Y position for proper 2.5D layering
// Objects lower on screen (higher Y value) should draw on top
ZIndex = (int)Position.Y;
```

5. **Alternative Y-Sort Method** (for Godot 4.x with Y-Sort enabled on nodes):
    - Select the **WorldMap** TileMap node in `World.tscn`.
    - In the Inspector, under **Ordering**, set **Y Sort Enabled** to **ON**.
    - Do the same for the **Player** node.
    - This automatically handles Y-sorting without manual Z-index updates.
6. **Testing Y-Sort:**
    - Place a test sprite or tree in the world with a higher Y position than the player.
    - Walk the player behind it (move up). The player should be drawn behind the tree.
    - Walk the player in front of it (move down). The player should be drawn in front.
7. Your complete `PlayerController.cs` should now look like this:

```csharp
using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
    private const float Speed = 200.0f;

    public override void _PhysicsProcess(double delta)
    {
        // Get input direction from keyboard/gamepad
        Vector2 inputDirection = Input.GetVector(
            "ui_left", "ui_right", "ui_up", "ui_down"
        );

        // Set velocity based on input
        Velocity = inputDirection * Speed;

        // Move the character and handle collisions
        MoveAndSlide();

        // Update Z-index for proper 2.5D depth sorting
        ZIndex = (int)Position.Y;
    }
}
```

### Task 4: Resource Node Linking (3 Hours)

#### Resource Node Scene Creation (10h - 11h)

1. Click **Scene > New Scene** in Godot.
2. Select **Other Node** and search for **Area2D**. This will be our interactive resource node.
3. Rename the root node to **ResourceNode** and save the scene as `ResourceNode.tscn` in the `Scenes` folder.
4. Add a **Sprite2D** child node to display the resource visual.
5. Add a **CollisionShape2D** child node. Set its **Shape** to **New CircleShape2D** (for a circular interaction area).
6. Click the CircleShape2D to expand it, and set **Radius** to about 32 pixels.
7. **Create the C# Script:**
    - Select the **ResourceNode** root node.
    - Click the **Attach Script** icon.
    - Set the path to `Scripts/ResourceNode.cs` and click **Create**.
8. In your IDE, open `ResourceNode.cs` and replace the default code:

```csharp
using Godot;
using System;

public partial class ResourceNode : Area2D
{
    // Reference to the resource data (from Sprint 1)
    // This will hold all the properties of this specific resource
    public RawResource ResourceData { get; set; }

    // Visual representation of the resource
    private Sprite2D _sprite;

    public override void _Ready()
    {
        // Get reference to the Sprite2D child
        _sprite = GetNode<Sprite2D>("Sprite2D");

        // Add this node to the "Resources" group for easy finding
        AddToGroup("Resources");

        // Optional: Set sprite based on resource type
        if (ResourceData != null)
        {
            // You can add logic here to change sprite based on
            // ResourceData.Name (e.g., different ore types)
            GD.Print($"Resource node created: {ResourceData.Name}");
        }
    }

    // This method will be called when the player interacts with this node
    public void Harvest()
    {
        GD.Print($"Harvesting {ResourceData.Name}");
        // Play sound effect, spawn particles, etc.
        QueueFree(); // Remove this node from the scene
    }
}
```

9. Build the project (F6) to ensure it compiles.
#### Instantiate Resource Nodes During World Generation (11h - 12h)

1. Open `WorldGenerator.cs` in your IDE.
2. At the top of the class, add a reference to the ResourceNode scene:

```csharp
// Scene to instantiate for resources
private PackedScene _resourceNodeScene;
```

3. In the `_Ready()` method, load the scene:

```csharp
public override void _Ready()
{
    _tileMap = GetNode<TileMap>(".");
    // Load the ResourceNode scene for instantiation
    _resourceNodeScene = GD.Load<PackedScene>("res://Scenes/ResourceNode.tscn");

    GenerateInitialWorld();
    RenderWorld();
}
```

4. Modify the `RenderWorld()` method to instantiate ResourceNode objects:

```csharp
private void RenderWorld()
{
    for (int x = 0; x < WorldSize; x++)
    {
        for (int y = 0; y < WorldSize; y++)
        {
            int tileId = _worldData[x, y];

            // Place the tile
            _tileMap.SetCell(0, new Vector2I(x, y), 0,
                new Vector2I(tileId, 0));

            // If this is an Ore (ID 1) or Wood (ID 2) tile,
            // spawn a ResourceNode
            if (tileId == 1 || tileId == 2)
            {
                // Instantiate the ResourceNode
                ResourceNode node = _resourceNodeScene.Instantiate<ResourceNode>();

                // Convert grid position to world position
                // Multiply by tile size (adjust if your tiles are different size)
                Vector2 worldPos = new Vector2(x * 64, y * 64);
                node.Position = worldPos;

                // Create and assign resource data
                RawResource resource = new RawResource();
                if (tileId == 1) // Ore
                {
                    resource.Name = "Base Ore";
                    resource.GenerateProperties(x + y); // Use position as seed
                }
                else // Wood
                {
                    resource.Name = "Soft Wood";
                    resource.GenerateProperties(x * y);
                }

                node.ResourceData = resource;

                // Add to scene tree
                AddChild(node);
            }
        }
    }
}
```

5. Save and build the project. Run it to see resource nodes spawning on Ore and Wood tiles.
#### Player Resource Detection and Interaction (12h - 13h)

1. Open `PlayerController.cs` in your IDE.
2. Add a field to track the nearby resource:

```csharp
// Track the resource node the player is currently near
private ResourceNode _nearbyResource = null;
```

3. Add a method to check for nearby resources:

```csharp
// Check if player is near any resource nodes
private void CheckForResourceNode()
{
    // Clear previous nearby resource
    _nearbyResource = null;

    // Get all nodes in the "Resources" group
    var resources = GetTree().GetNodesInGroup("Resources");

    // Check each resource to see if player is close enough
    foreach (Node node in resources)
    {
        if (node is ResourceNode resourceNode)
        {
            // Calculate distance between player and resource
            float distance = Position.DistanceTo(resourceNode.Position);

            // If within interaction range (50 pixels)
            if (distance < 50.0f)
            {
                _nearbyResource = resourceNode;
                break; // Found a nearby resource, stop searching
            }
        }
    }
}
```

4. Add input handling for harvesting:

```csharp
// Handle harvesting input
private void HandleHarvestInput()
{
    // Check if player presses the interact key (E) or left mouse button
    if (Input.IsActionJustPressed("ui_accept") ||
        Input.IsActionJustPressed("ui_select"))
    {
        if (_nearbyResource != null)
        {
            // Harvest the resource
            _nearbyResource.Harvest();
            GD.Print($"Harvested: {_nearbyResource.ResourceData.Name}");
        }
    }
}
```

5. Call these methods in `_PhysicsProcess`:

```csharp
public override void _PhysicsProcess(double delta)
{
    // Existing movement code...
    Vector2 inputDirection = Input.GetVector(
        "ui_left", "ui_right", "ui_up", "ui_down"
    );
    Velocity = inputDirection * Speed;
    MoveAndSlide();
    ZIndex = (int)Position.Y;

    // New resource interaction code
    CheckForResourceNode();
    HandleHarvestInput();
}
```

6. **Optional:** Add visual feedback by displaying a prompt. Add this at the end of `_PhysicsProcess`:

```csharp
// Display interaction prompt (you'll create the UI in Sprint 3)
if (_nearbyResource != null)
{
    // For now, just print to console
    GD.Print($"Near: {_nearbyResource.ResourceData.Name} - Press E to harvest");
}
```

7. Test the game: Walk near resource nodes and press E (or Enter) to harvest them. They should disappear and print messages to the console.

### Task 5: Aesthetic & Review (3 Hours)

#### Art/Sound: Final Asset Integration (13h - 14h)

1. Replace the placeholder colored squares for the Player, Ground, Ore, and Wood tiles with the final art assets developed in Sprint 1/2 concept tasks.
2. Add simple **footstep sound effects** to the PlayerController (e.g., play a sound when velocity is non-zero).

#### Debugging and Visual Check (14h - 15h)

1. Run the game. Verify the 64x64 map loads.
2. Check Y-Sorting: If the player walks *behind* a tree (a static sprite placed on a higher Y-coordinate), the player must be visually occluded by it. If they walk in front of it (lower Y-coordinate), they must be visible.
3. Verify the debug message from Task 4 (e.g., "Over: Dull Grey Ore") correctly displays the *specific* resource linked to the instantiated node.

#### Review and Commit (15h - 16h)

1. Review all scripts for clean C# syntax and Godot C# best practices.
2. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 2 Complete: Grid World, Player Movement, and Resource Voxel Linking."

# ⛏️ Sprint 3: Resource Engine & ProcGen V1 (16 Hours)

## Summary

This sprint fully integrates the procedural data architecture (Sprint 1) with the physical world (Sprint 2). We will finalize the resource generation, implement the core harvesting mechanic, and build the foundational inventory manager and HUD necessary for the primary gameplay loop.

## 🎯 Goal

The player can successfully land, harvest one type of wood and two types of ore, see the resources added to a working inventory, and get the first hints of the deduction puzzle via the Hand Scanner.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Singleton management, Input handling, UI integration.
  * **C\# / .NET:** Random number generation for resource properties, Inventory Management class.

#### Time Log

**1: Singleton and Resource Initialization:** 180 mins (Actual: -)
**2: Inventory Manager Implementation:** 180 mins (Actual: -)
**3: Basic HUD and Inventory UI:** 240 mins (Actual: -)
**4: Hand Scanner and Vague Deduction:** 180 mins (Actual: -)
**5: Save/Load System:** 180 mins (Actual: -)
**6: Aesthetic & Review:** 120 mins (Actual: -)
**Total:** 18 hrs (Actual: -)

-----

## Task Breakdown (16 Hours)

### Task 1: Singleton and Resource Initialization (3 Hours)

#### GameManager Singleton Setup (Autoload) (0h - 1h)

1. Create a new C# script file called `GameManager.cs` in the `Scripts` folder.
2. Implement the singleton pattern:

```csharp
using Godot;
using System;

// Autoload singleton that manages game state
public partial class GameManager : Node
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Core game data
    public PlanetaryConstants Constants { get; private set; }
    public PortalRequirement Required { get; private set; }

    // Static accessors for easy access from anywhere
    public static PlanetaryConstants WorldConstants => Instance.Constants;
    public static PortalRequirement PortalGoal => Instance.Required;

    public override void _EnterTree()
    {
        // Set up singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Prevent duplicate singletons
            QueueFree();
        }
    }

    public override void _Ready()
    {
        // Initialize world data
        Constants = new PlanetaryConstants();
        Constants.GenerateWorld(1); // Complexity level 1 for VS

        Required = new PortalRequirement();
        Required.SetRequirements(Constants);

        GD.Print($"World Generated: Gravity={Constants.GravimetricShear}, " +
                 $"Corrosion={Constants.CorrosiveIndex}");
        GD.Print($"Portal Requires: Strength={Required.RequiredStats[ResourcePropertyType.Strength]}");
    }
}
```

3. **Register as Autoload:**
    - In Godot, go to **Project > Project Settings**.
    - Click the **Autoload** tab.
    - Click the folder icon next to **Path** and navigate to `Scripts/GameManager.cs`.
    - Set **Node Name** to "GameManager".
    - Check **Enable** and click **Add**.
    - Close Project Settings.
4. Build the project (F6) and run it. You should see the initialization messages in the Output console.
#### Enhanced Resource Property Generation (1h - 2h)

1. Open `RawResource.cs` (from Sprint 1) in your IDE.
2. Refine the `GenerateProperties` method to use proper randomization:

```csharp
// Generate random properties for this resource using a seed
public void GenerateProperties(int seed)
{
    // Create a random number generator with the seed
    // This ensures the same seed always produces the same properties
    RandomNumberGenerator rng = new RandomNumberGenerator();
    rng.Seed = (ulong)seed;

    // Generate a value for each property type
    foreach (ResourcePropertyType propType in Enum.GetValues(typeof(ResourcePropertyType)))
    {
        // Generate a random value between 1.0 and 10.0
        float value = rng.RandfRange(1.0f, 10.0f);

        // Create a ResourceProperty and add it to the dictionary
        ResourceProperty property = new ResourceProperty(propType, value);
        Properties[propType] = property;
    }

    GD.Print($"Generated {Name}: Strength={Properties[ResourcePropertyType.Strength].Value:F1}");
}
```

3. **Add Harvest Method Tracking:**
    - According to MECHANICS_DETAILS.md, different resources require different acquisition methods.
    - Add a new field to `RawResource.cs`:

```csharp
// The method required to harvest this resource (from MECHANICS_DETAILS.md)
public string RequiredHarvestMethod { get; set; } = "Kinetic Mining";
```

4. Build and test. Run the game and verify that resources generate random but consistent properties.

#### Create Hardcoded VS Resource Presets (2h - 3h)

1. Create a new file `VSResourcePresets.cs` in the `Scripts` folder:

```csharp
using Godot;
using System;

// Hardcoded resource definitions for the Vertical Slice
// These ensure consistent, predictable gameplay for testing
public static class VSResourcePresets
{
    // Base Ore: Low strength, not enough for portal
    public static RawResource CreateBaseOre()
    {
        RawResource ore = new RawResource
        {
            Name = "Base Ore",
            Description = "A dull grey metallic ore with low structural integrity.",
            BaseHarvestDifficulty = 1,
            RequiredHarvestMethod = "Kinetic Mining" // Standard mining
        };

        // Manually set properties for predictable VS gameplay
        ore.Properties[ResourcePropertyType.Strength] =
            new ResourceProperty(ResourcePropertyType.Strength, 2.0f);
        ore.Properties[ResourcePropertyType.Resistance] =
            new ResourceProperty(ResourcePropertyType.Resistance, 5.0f);
        ore.Properties[ResourcePropertyType.Toughness] =
            new ResourceProperty(ResourcePropertyType.Toughness, 3.0f);
        ore.Properties[ResourcePropertyType.Conductivity] =
            new ResourceProperty(ResourcePropertyType.Conductivity, 4.0f);
        ore.Properties[ResourcePropertyType.Reactivity] =
            new ResourceProperty(ResourcePropertyType.Reactivity, 6.0f);

        return ore;
    }

    // Catalyst Ore: Medium strength, adds modifier
    public static RawResource CreateCatalystOre()
    {
        RawResource ore = new RawResource
        {
            Name = "Catalyst Ore",
            Description = "A shimmering purple ore with reactive properties.",
            BaseHarvestDifficulty = 2,
            RequiredHarvestMethod = "Gas Siphoning" // Requires advanced tool
        };

        // Higher strength - will provide +4.0 boost when used in compositing
        ore.Properties[ResourcePropertyType.Strength] =
            new ResourceProperty(ResourcePropertyType.Strength, 4.0f);
        ore.Properties[ResourcePropertyType.Resistance] =
            new ResourceProperty(ResourcePropertyType.Resistance, 2.0f);
        ore.Properties[ResourcePropertyType.Toughness] =
            new ResourceProperty(ResourcePropertyType.Toughness, 2.5f);
        ore.Properties[ResourcePropertyType.Conductivity] =
            new ResourceProperty(ResourcePropertyType.Conductivity, 8.0f);
        ore.Properties[ResourcePropertyType.Reactivity] =
            new ResourceProperty(ResourcePropertyType.Reactivity, 9.0f);

        return ore;
    }

    // Soft Wood: Low difficulty, used as fuel
    public static RawResource CreateSoftWood()
    {
        RawResource wood = new RawResource
        {
            Name = "Soft Wood",
            Description = "Lightweight timber suitable for burning.",
            BaseHarvestDifficulty = 1,
            RequiredHarvestMethod = "Kinetic Mining" // Can be chopped normally
        };

        wood.Properties[ResourcePropertyType.Strength] =
            new ResourceProperty(ResourcePropertyType.Strength, 1.0f);
        wood.Properties[ResourcePropertyType.Resistance] =
            new ResourceProperty(ResourcePropertyType.Resistance, 1.5f);
        wood.Properties[ResourcePropertyType.Toughness] =
            new ResourceProperty(ResourcePropertyType.Toughness, 2.0f);
        wood.Properties[ResourcePropertyType.Conductivity] =
            new ResourceProperty(ResourcePropertyType.Conductivity, 0.5f);
        wood.Properties[ResourcePropertyType.Reactivity] =
            new ResourceProperty(ResourcePropertyType.Reactivity, 7.0f); // Burns well

        return wood;
    }
}
```

2. **Update WorldGenerator to Use Presets:**
    - Open `WorldGenerator.cs` and modify the `RenderWorld()` method.
    - Replace the resource creation code from Sprint 2 with:

```csharp
// In RenderWorld(), where resources are spawned:
if (tileId == 1) // Ore
{
    resource = VSResourcePresets.CreateBaseOre();
}
else if (tileId == 2) // Wood
{
    resource = VSResourcePresets.CreateSoftWood();
}
```

3. Build and test. Resources should now have consistent, predictable properties. Verify by scanning them with the debug console.

### Task 2: Inventory Manager Implementation (3 Hours)

#### Create Inventory Manager Singleton (3h - 4h)

1. Create a new file `InventoryManager.cs` in the `Scripts` folder.
2. Implement the singleton autoload class:

```csharp
using Godot;
using System;
using System.Collections.Generic;

// Manages the player's inventory of resources
public partial class InventoryManager : Node
{
    // Singleton instance
    public static InventoryManager Instance { get; private set; }

    // Storage: Key = resource name, Value = quantity
    public Dictionary<string, int> ResourceCounts { get; private set; }

    // Event fired whenever inventory changes (for UI updates)
    public event Action OnInventoryUpdated;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
            ResourceCounts = new Dictionary<string, int>();
        }
        else
        {
            QueueFree();
        }
    }

    public override void _Ready()
    {
        GD.Print("InventoryManager initialized");
    }
}
```

3. **Register as Autoload:**
    - Go to **Project > Project Settings > Autoload**.
    - Add `Scripts/InventoryManager.cs` with name "InventoryManager".
    - Click **Add** and close.
4. Build the project (F6) to verify compilation. |
| **4h - 5h** | **Implement Add and Remove Methods**
1. Open `InventoryManager.cs` and add these methods after `_Ready()`:

```csharp
/// <summary>
/// Adds an item to the inventory
/// </summary>
/// <param name="resourceName">Name of the resource to add</param>
/// <param name="amount">Quantity to add (default: 1)</param>
public void AddItem(string resourceName, int amount = 1)
{
    // Input validation
    if (string.IsNullOrEmpty(resourceName))
    {
        GD.PrintErr("Cannot add item: resource name is null or empty");
        return;
    }

    if (amount <= 0)
    {
        GD.PrintErr($"Cannot add {amount} of {resourceName}: amount must be positive");
        return;
    }

    // Check if resource already exists in inventory
    if (ResourceCounts.ContainsKey(resourceName))
    {
        // Increment existing count
        ResourceCounts[resourceName] += amount;
    }
    else
    {
        // Add new entry
        ResourceCounts[resourceName] = amount;
    }

    // Log for debugging
    GD.Print($"Added {amount}x {resourceName}. Total: {ResourceCounts[resourceName]}");

    // Notify UI listeners that inventory has changed
    OnInventoryUpdated?.Invoke();
}

/// <summary>
/// Attempts to remove an item from inventory
/// </summary>
/// <param name="resourceName">Name of the resource to remove</param>
/// <param name="amount">Quantity to remove</param>
/// <returns>True if successful, false if insufficient quantity</returns>
public bool TryRemoveItem(string resourceName, int amount = 1)
{
    // Check if resource exists
    if (!ResourceCounts.ContainsKey(resourceName))
    {
        GD.PrintErr($"Cannot remove {resourceName}: not in inventory");
        return false;
    }

    // Check if we have enough
    if (ResourceCounts[resourceName] < amount)
    {
        GD.PrintErr($"Cannot remove {amount}x {resourceName}: " +
                   $"only have {ResourceCounts[resourceName]}");
        return false;
    }

    // Deduct the amount
    ResourceCounts[resourceName] -= amount;

    // Remove entry if count reaches zero
    if (ResourceCounts[resourceName] <= 0)
    {
        ResourceCounts.Remove(resourceName);
    }

    GD.Print($"Removed {amount}x {resourceName}. " +
            $"Remaining: {ResourceCounts.GetValueOrDefault(resourceName, 0)}");

    // Notify UI
    OnInventoryUpdated?.Invoke();
    return true;
}

/// <summary>
/// Gets the count of a specific resource
/// </summary>
public int GetItemCount(string resourceName)
{
    return ResourceCounts.GetValueOrDefault(resourceName, 0);
}

/// <summary>
/// Checks if the player has at least the specified amount
/// </summary>
public bool HasItem(string resourceName, int amount = 1)
{
    return GetItemCount(resourceName) >= amount;
}
```

2. Build and test that the code compiles without errors. |
**5h - 6h: Integrate Inventory with Resource Harvesting**

1. Open `PlayerController.cs` from Sprint 2.
2. Modify the `HandleHarvestInput()` method to add resources to inventory:

```csharp
// Handle harvesting input (updated version)
private void HandleHarvestInput()
{
    // Check if player presses the interact key
    if (Input.IsActionJustPressed("ui_accept") ||
        Input.IsActionJustPressed("ui_select"))
    {
        if (_nearbyResource != null)
        {
            // Get resource data before harvesting
            string resourceName = _nearbyResource.ResourceData.Name;

            // Add to inventory
            InventoryManager.Instance.AddItem(resourceName, 1);

            // Harvest (destroys) the resource node
            _nearbyResource.Harvest();

            // Clear the reference since node is being destroyed
            _nearbyResource = null;
        }
    }
}
```

3. **Testing the Inventory:**
    - Run the game (F5).
    - Walk near resource nodes and press E to harvest them.
    - Watch the Output console for messages like "Added 1x Base Ore. Total: 1".
    - Harvest multiple of the same resource and verify the count increments.
4. **Optional Debug Command:**
    - Add a debug method to InventoryManager to print all items:

```csharp
// Debug method to print entire inventory
public void PrintInventory()
{
    GD.Print("=== INVENTORY ===");
    if (ResourceCounts.Count == 0)
    {
        GD.Print("  (empty)");
    }
    else
    {
        foreach (var kvp in ResourceCounts)
        {
            GD.Print($"  {kvp.Key}: {kvp.Value}");
        }
    }
    GD.Print("=================");
}
```

5. Call this from PlayerController by pressing a debug key (e.g., F1):

```csharp
// In _PhysicsProcess, add:
if (Input.IsActionJustPressed("ui_home")) // F1 key
{
    InventoryManager.Instance.PrintInventory();
}
```

6. Test: Harvest some resources, press F1, and verify the inventory list appears in the console.

### Task 3: Basic HUD and Inventory UI (4 Hours)

**6h - 7h: HUD Scene Setup**

1. Create a new `CanvasLayer` scene named `HUD.tscn`. Add a main `Control` node for layout.
2. Create a small panel in the bottom-right for the **Inventory Display**. Use 3 `Label` nodes for the VS key resources (Base Ore, Catalyst Ore, Soft Wood).

**7h - 8h: Inventory UI Scripting**

1. Create a C# script `InventoryUI.cs` and attach it to the HUD's main panel.
2. In `_Ready()`, subscribe to the inventory update event: `InventoryManager.Instance.OnInventoryUpdated += UpdateResourceDisplay;`
3. Implement `UpdateResourceDisplay()` to read the counts from `InventoryManager.ResourceCounts` and update the 3 `Label.Text` properties.

**8h - 9h: Hardcoded Target Display**

1. Add a `Label` to the top-right of the HUD for the Portal Goal.
2. In `InventoryUI.cs`, grab the hardcoded target Strength (8.0f) from `GameManager.PortalGoal`.
3. Set the label text: **"Portal Foundation: Strength ???. (Req: $>8.0$)"**. *The actual required value is shown to the player, but they don't know the material property values yet.*

**9h - 10h: Art/Sound: UI Assets**

1. Create final icons for the 3 VS resources (Base Ore, Catalyst Ore, Soft Wood).
2. Create a clean, sci-fi font and theme for the HUD.

### Task 4: Hand Scanner and Vague Deduction (3 Hours)

#### Hand Scanner Input Logic (10h - 11h)

1. In `PlayerController.cs`, implement the **Right-Click** input action (e.g., using `Input.IsActionJustPressed("RightClick")`).
2. When right-click is pressed, check for an overlapping `ResourceNode` (re-use the logic from Sprint 2).

#### Hand Scanner UI Feedback (11h - 12h)

1. Create a small overlay UI panel (e.g., top-center) that will only show when the player is scanning.
2. When a node is scanned, call `resourceNode.ResourceData.Properties[Strength].VagueDescription;` (e.g., "High Integrity").
3. Display the **Vague Description** and **Name** of the resource on the overlay panel for 2 seconds, then hide it.

#### Vague Deduction Test (12h - 13h)

1. Run the game and scan the Base Ore (Strength $\approx 2.0$, shows "Low Integrity").
2. Scan the Catalyst Ore (Strength $\approx 4.0$, shows "Medium Integrity").
3. This confirms the initial deduction loop: *Player observes vague properties and knows "Low" isn't enough for the required "\>8.0".*

### Task 5: Aesthetic and Review (3 Hours)

#### Art/Sound: Harvesting FX (13h - 14h)

1. Create a simple **"mining" sound effect** (e.g., a metallic impact) to play when the Left-Click/Harvest action is successful.
2. Create a simple **VFX** (e.g., small dust particles) to spawn and disappear where the resource node was harvested.
3. Create a short, subtle **"scan successful" sound** for the Hand Scanner action.

#### Code Review and Tuning (14h - 15h)

1. Review the C\# singleton implementations to ensure no memory leaks or improper initialization order.
2. Tune the player's harvesting animation speed and the time it takes to destroy a resource node to feel satisfying (e.g., a 0.5-second hold before the resource is harvested).

#### Final Playtest and Commit (15h - 16h)

1. Play the entire loop: Land $\rightarrow$ Scan (get vague data) $\rightarrow$ Harvest Wood $\rightarrow$ Harvest both Ores $\rightarrow$ Check Inventory and HUD.
2. Confirm the hardcoded portal target is visible.
3. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 3 Complete: Resource Engine, Inventory, and Hand Scanner Deduction."

# 🖱️ Sprint 4: Interaction & Scanner UI (16 Hours)

## Summary

In this sprint, we enhance the player's primary interaction tool—the **Hand Scanner**—and implement the structure for displaying detailed environmental information. We move the scanning feedback from a temporary debug message (Sprint 3) to a dedicated, persistent UI element, laying the groundwork for the more advanced analysis machines in future sprints.

## 🎯 Goal

The player can effectively use the Hand Scanner (Right Click) to display the **Vague Descriptions** of a resource and the **Planetary Constants** (though still hardcoded to the VS values) in a clean, dedicated UI panel.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** UI design, AnimationPlayer for UI transitions, and input binding.
  * **C\# / .NET:** Data formatting for display, constant access.

#### Time Log

**1: Dedicated Scanner UI Panel:** 180 mins (Actual: -)
**2: Scanner UI Manager Script:** 180 mins (Actual: -)
**3: Player Controller Integration:** 180 mins (Actual: -)
**4: UI Polish and Animations:** 180 mins (Actual: -)
**5: Review and Testing:** 240 mins (Actual: -)
**Total:** 16 hrs (Actual: -)

-----

## Task Breakdown (16 Hours)

### Task 1: Dedicated Scanner UI Panel (3 Hours)

#### Scanner UI Scene Setup (0h - 1h)

1. Create a new `Control` scene named `ScannerUI.tscn`. Add this as a child of the `HUD.tscn` (from Sprint 3).
2. Position the panel in a convenient, non-obtrusive location (e.g., top-center of the screen).
3. Add a title label: **"SCANNER ANALYSIS"**.
4. Add two main sections (sub-`Control` nodes): **Resource Data** and **Planetary Data**.

#### Resource Data Display (1h - 2h)

1. In the **Resource Data** section, add five `Label` nodes to display the Vague Descriptions for the 5 core properties:
    - `Label_Strength`, `Label_Resistance`, `Label_Toughness`, `Label_Conductivity`, `Label_Reactivity`.
2. Give them prefixes: "Strength:", "Resistance:", etc. (The values will be set dynamically).

#### Planetary Data Display (2h - 3h)

1. In the **Planetary Data** section, add three `Label` nodes for the VS Planetary Constants (from Sprint 1):
    - `Label_Gravity`, `Label_Corrosion`, `Label_Tectonics`.
2. Initialize these labels with the current hardcoded VS value (e.g., "Gravity: 3.2 g"). *This acts as a placeholder until the Gravimeter is built.*

### Task 2: Scanner UI Manager Script (3 Hours)

#### ScannerUIManager Script (3h - 4h)

1. Create a C\# script `ScannerUIManager.cs` and attach it to the `ScannerUI` root node.
2. Define public methods: `ShowScanPanel()` and `HideScanPanel()`.
3. In `_Ready()`, hide the panel initially: `Visible = false;`.

#### Update Resource Data Method (4h - 5h)

1. Implement a public method `UpdateResourceData(RawResource resource)`.
2. Inside this method, iterate through the `resource.Properties` dictionary.
3. For each property, find the corresponding `Label` (e.g., find `Label_Strength`) and update its text to display the `VagueDescription` (e.g., "Strength: Low").

#### Update Planetary Data Method (5h - 6h)

1. Implement `UpdatePlanetaryData(PlanetaryConstants constants)`.
2. Get the current constants from `GameManager.WorldConstants` (even if the `constants` argument isn't used yet).
3. Update the three Planetary Data labels using the hardcoded VS values and their units (e.g., `Label_Gravity.Text = $"Gravity: {constants.GravimetricShear:F1} g";`).

### Task 3: Player Controller Integration (3 Hours)

#### Player Input Refinement (6h - 7h)

1. In Godot's Input Map, ensure the **"RightClick"** action is defined.
2. In `PlayerController.cs`, modify the Right-Click logic (from Sprint 3) to be **hold-based** rather than **press-based**.
3. When `Input.IsActionPressed("RightClick")` is true, perform the scan and keep the panel visible.

#### Scanner Activation Logic (7h - 8h)

1. Create a public variable `ScannerUIManager _scannerUI;` and initialize it in `_Ready()`.
2. In the scanning section of `_PhysicsProcess(double delta)`:
    - If the player is holding Right-Click AND is overlapping a `ResourceNode`:
        - Call `_scannerUI.UpdateResourceData(overlappingResource);`
        - Call `_scannerUI.UpdatePlanetaryData(...)`
        - Call `_scannerUI.ShowScanPanel();`
    - ELSE IF the player is NOT holding Right-Click, call `_scannerUI.HideScanPanel();`.

#### Scan Range Check (8h - 9h)

1. Refine the logic to ensure the player must be within a short distance of the resource node (e.g., within 20 pixels) to scan it.
2. Provide clear visual feedback (e.g., a momentary green glow/highlight) on the resource node that is successfully being scanned.

### Task 4: UI Polish and Animations (4 Hours)

#### Aesthetics: Sci-Fi Styling (9h - 10h)

1. Apply the UI theme (from Sprint 3) to the `ScannerUI` panel.
2. Use subtle background colors, dividers, and fixed-width fonts (like a terminal font) for the data values to give it a scientific/engineering feel.

#### Panel Animation (10h - 11h)

1. Add an `AnimationPlayer` node to `ScannerUI.tscn`.
2. Create two short (0.3 second) animation tracks: **"Show"** (fades opacity from 0 to 1, or slides the panel in) and **"Hide"** (does the reverse).
3. In `ScannerUIManager.cs`, replace `Visible = true/false` with `_animationPlayer.Play("Show")` or `_animationPlayer.Play("Hide")`.

#### Code Documentation (11h - 12h)

1. Add XML documentation comments (`/// <summary>`) to all public methods and properties in `ScannerUIManager.cs` and `PlayerController.cs`.
2. Ensure the logic linking the player's input to the UI update is clean and easy to follow.

### Task 5: Review and Testing (3 Hours)

#### Gameplay Flow Test (12h - 13h)

1. Run the game. Right-click and hold near a **Base Ore** node. Confirm the UI panel appears and shows **"Strength: Low"** for the Resource Data.
2. Confirm the Planetary Data section shows the static values (e.g., **"Gravity: 3.2 g"**).
3. Move away from the resource node while holding Right-Click. The Resource Data should disappear/zero out, and the Planetary Data should remain (as it's global). Release Right-Click, and the entire panel should disappear smoothly.

#### Y-Sort and Z-Ordering Check (13h - 14h)

1. Place a resource node near the player's collision shape.
2. Verify that the `ScannerUI` panel (a `CanvasLayer`) is drawn **above** the game world, player, and all sprites. The UI should never be clipped by the game world geometry.

#### Final Code Review and Cleanup (14h - 15h)

1. Check for any unnecessary `GD.Print()` calls used during debugging.
2. Ensure the C\# code adheres to naming conventions (PascalCase for methods, camelCase for local variables).

#### Commit Code (15h - 16h)

1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 4 Complete: Dedicated Scanner UI, Hold-to-Scan, and Planetary Data Display."

# 🔬 Sprint 5: Field Lab & Analysis Logic (16 Hours)

## Summary

This sprint implements the **Field Lab**, the player's first major technological advancement. The Field Lab allows the player to submit a raw resource and receive its **exact numerical property values**. This is the leap from "Low/Medium/High" (Scanner) to the hard numbers required for engineering the Portal solution.

## 🎯 Goal

  * Design, implement, and integrate the **Field Lab** machine.
  * Implement the core "Analysis" logic: spending one resource to gain *Knowledge* and the *exact numerical data* for that resource type.
  * Update the Scanner UI to display this precise data once unlocked.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Static/Global Machine interaction, State management (Resource Input/Output).
  * **C\# / .NET:** Data linking, Knowledge tracking (for future Tech Tree unlocks).

#### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Field Lab Scene and Interaction | 180 | |
| 2: Analysis Logic and Knowledge Tracking | 240 | |
| 3: Displaying Precise Data | 240 | |
| 4: UI/UX Feedback and Polish | 300 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Field Lab Scene and Interaction (3 Hours)

#### Field Lab Scene Setup (0h - 1h)

1. Create a new scene `FieldLab.tscn` using an `Area2D` root node. Add a `Sprite2D` and a `CollisionShape2D`.
2. Add a `Marker2D` child node named `InputSlot` to visually mark where the player deposits the resource.
3. Add the `FieldLab.tscn` instance to the `World.tscn`.

#### FieldLabManager Script (1h - 2h)

1. Create a C\# script `FieldLabManager.cs` and attach it to the `FieldLab` node.
2. Define a state variable: `public RawResource InputResource { get; private set; }`
3. Implement an interaction method `ReceiveResource(string resourceName)` that is triggered when the player interacts with the machine (using a Left-Click while standing near it).

#### Player Interaction Logic (2h - 3h)

1. In `PlayerController.cs`, implement the interaction (Left-Click) logic for the Field Lab.
2. **VS Logic:** If the player is overlapping the Field Lab's `Area2D` and Left-Clicks:
    - Use `InventoryManager.TryRemoveItem("Base Ore", 1)`.
    - If successful, set `InputResource` in the `FieldLabManager.cs` to a new instance of the **Base Ore** data and start the analysis process.

### Task 2: Analysis Logic and Knowledge Tracking (4 Hours)

#### Create Knowledge Manager Singleton (3h - 4h)

1. Create a new file `KnowledgeManager.cs` in the `Scripts` folder.
2. Implement the knowledge tracking system:

```csharp
using Godot;
using System;
using System.Collections.Generic;

// Tracks what the player has learned about resources and the world
// This is the "brain" that remembers analyzed data
public partial class KnowledgeManager : Node
{
    public static KnowledgeManager Instance { get; private set; }

    // Track which resources have been analyzed
    // Key: Resource name, Value: true if data is unlocked
    public Dictionary<string, bool> DataUnlocked { get; private set; }

    // Track technology points earned through various activities
    // Key: Point type (Analysis, Smelting, Compositing)
    // Value: Current point total
    public Dictionary<string, int> TechPoints { get; private set; }

    // Track which planetary constants have been measured
    public Dictionary<string, bool> ConstantsMeasured { get; private set; }
    public float MeasuredGravity { get; set; } = 0.0f;

    // Track unlocked tech tree nodes
    public HashSet<string> UnlockedNodes { get; private set; }

    // Event fired when knowledge changes
    public event Action OnKnowledgeUpdated;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeData();
        }
        else
        {
            QueueFree();
        }
    }

    private void InitializeData()
    {
        DataUnlocked = new Dictionary<string, bool>();
        TechPoints = new Dictionary<string, int>
        {
            { "Analysis", 0 },
            { "Smelting", 0 },
            { "Compositing", 0 }
        };
        ConstantsMeasured = new Dictionary<string, bool>
        {
            { "Gravity", false },
            { "Corrosion", false },
            { "Tectonics", false }
        };
        UnlockedNodes = new HashSet<string>();
    }

    /// <summary>
    /// Unlocks precise data for a specific resource
    /// </summary>
    public void UnlockResourceData(string resourceName)
    {
        if (!DataUnlocked.ContainsKey(resourceName))
        {
            DataUnlocked[resourceName] = true;
            GD.Print($"[KNOWLEDGE] Unlocked data for: {resourceName}");
            OnKnowledgeUpdated?.Invoke();
        }
    }

    /// <summary>
    /// Checks if resource data is unlocked
    /// </summary>
    public bool IsResourceDataUnlocked(string resourceName)
    {
        return DataUnlocked.GetValueOrDefault(resourceName, false);
    }

    /// <summary>
    /// Awards tech points of a specific type
    /// </summary>
    public void AddTechPoints(string pointType, int amount)
    {
        if (TechPoints.ContainsKey(pointType))
        {
            TechPoints[pointType] += amount;
            GD.Print($"[KNOWLEDGE] Gained {amount} {pointType} points. " +
                    $"Total: {TechPoints[pointType]}");
            OnKnowledgeUpdated?.Invoke();
        }
    }

    /// <summary>
    /// Attempts to spend tech points for research
    /// </summary>
    public bool TrySpendPoints(Dictionary<string, int> costs)
    {
        // First check if we can afford all costs
        foreach (var cost in costs)
        {
            if (TechPoints.GetValueOrDefault(cost.Key, 0) < cost.Value)
            {
                GD.Print($"[KNOWLEDGE] Cannot afford: need {cost.Value} {cost.Key}, " +
                        $"have {TechPoints[cost.Key]}");
                return false;
            }
        }

        // Deduct all costs
        foreach (var cost in costs)
        {
            TechPoints[cost.Key] -= cost.Value;
        }

        GD.Print("[KNOWLEDGE] Research costs paid");
        OnKnowledgeUpdated?.Invoke();
        return true;
    }

    /// <summary>
    /// Checks if a tech tree node is unlocked
    /// </summary>
    public bool IsNodeUnlocked(string nodeId)
    {
        return UnlockedNodes.Contains(nodeId);
    }
}
```

3. **Register as Autoload:** Add to Project Settings > Autoload as "KnowledgeManager".
4. Build and verify compilation.

#### Implement Field Lab Analysis Process (4h - 5h)

1. Open `FieldLabManager.cs` (from Task 1).
2. Add the analysis logic:

```csharp
/// <summary>
/// Begins the analysis process on the input resource
/// </summary>
public void StartAnalysis()
{
    if (InputResource == null)
    {
        GD.PrintErr("[Field Lab] Cannot analyze: no resource loaded");
        return;
    }

    GD.Print($"[Field Lab] Analyzing {InputResource.Name}...");

    // For the Vertical Slice, analysis is instant
    // In full game, this could be a timed process
    CompleteAnalysis();
}

/// <summary>
/// Completes the analysis and grants knowledge
/// </summary>
private void CompleteAnalysis()
{
    // Unlock precise data for this resource type
    KnowledgeManager.Instance.UnlockResourceData(InputResource.Name);

    // Grant Analysis tech points
    KnowledgeManager.Instance.AddTechPoints("Analysis", 10);

    // Create result summary for display
    string resultText = $"ANALYSIS COMPLETE\n{InputResource.Name.ToUpper()}\n";
    resultText += "---Properties---\n";

    foreach (var prop in InputResource.Properties)
    {
        resultText += $"{prop.Key}: {prop.Value.Value:F1}\n";
    }

    GD.Print($"[Field Lab] {resultText}");

    // Display result on screen (add to Task 3)
    ShowResult(resultText);

    // Clear input resource (it's been consumed)
    InputResource = null;
}

// Store reference to result label
private Label _resultLabel;

public override void _Ready()
{
    base._Ready();
    _resultLabel = GetNode<Label>("ResultLabel"); // Add this Label in scene
    _resultLabel.Visible = false;
}

/// <summary>
/// Displays analysis results for a few seconds
/// </summary>
private void ShowResult(string text)
{
    _resultLabel.Text = text;
    _resultLabel.Visible = true;

    // Create a timer to hide the result after 5 seconds
    Timer timer = GetTree().CreateTimer(5.0);
    timer.Timeout += () => _resultLabel.Visible = false;
}
```

3. Test by depositing a resource and calling `StartAnalysis()`. Verify that:
    - The resource data is unlocked in KnowledgeManager
    - Analysis points increase by 10
    - Messages print to console

#### Add Visual Result Display to Field Lab Scene (5h - 6h)

1. Open `FieldLab.tscn` in Godot.
2. Add a **Label** node as a child of the FieldLab root:
    - Name it **ResultLabel**
    - Position: Above or next to the Field Lab sprite
    - **Horizontal Alignment:** Center
    - **Vertical Alignment:** Top
    - Set a good font size (e.g., 12-14)
3. Configure the label appearance:
    - Add a **LabelSettings** resource
    - Set **Font Color** to bright green (0, 255, 0) for sci-fi feel
    - Add a subtle **Outline** (black, size 1-2) for readability
4. Make sure **Visible** is unchecked by default.
5. **Testing:**
    - Run the game
    - Deposit a resource into the Field Lab
    - The analysis result should display for 5 seconds showing all properties
    - Verify the result text is readable and properly formatted

#### Add Audio Feedback and Polish (6h - 7h)

1. **Resource Deposit Sound:**
    - Find or create a short "mechanical click" or "item placement" sound effect
    - Add an **AudioStreamPlayer2D** node to `FieldLab.tscn`, name it **DepositSound**
    - Load the sound file into its **Stream** property
2. **Analysis Complete Sound:**
    - Find or create a "computer beep" or "scan complete" chime
    - Add another **AudioStreamPlayer2D** named **AnalysisSound**
3. Update `FieldLabManager.cs` to play sounds:

```csharp
private AudioStreamPlayer2D _depositSound;
private AudioStreamPlayer2D _analysisSound;

public override void _Ready()
{
    base._Ready();
    _resultLabel = GetNode<Label>("ResultLabel");
    _depositSound = GetNode<AudioStreamPlayer2D>("DepositSound");
    _analysisSound = GetNode<AudioStreamPlayer2D>("AnalysisSound");
    _resultLabel.Visible = false;
}

// In ReceiveResource method (from Task 1):
public void ReceiveResource(RawResource resource)
{
    InputResource = resource;
    _depositSound.Play(); // Play deposit sound
    GD.Print($"[Field Lab] Received: {resource.Name}");
}

// In CompleteAnalysis:
private void CompleteAnalysis()
{
    _analysisSound.Play(); // Play completion sound
    // ... rest of analysis code
}
```

4. **Visual Polish:**
    - Consider adding a simple **AnimationPlayer** to pulse or glow the Field Lab sprite during analysis
    - Add particle effects (like sparkles or data streams) when analysis completes
5. **Final Test:**
    - Harvest resources
    - Approach Field Lab and deposit
    - Verify deposit sound plays
    - Wait for analysis
    - Verify completion sound and visual result display

### Task 3: Displaying Precise Data (4 Hours)

#### Scanner UI Manager Update (7h - 8h)

1. In `ScannerUIManager.cs`, modify the `UpdateResourceData(RawResource resource)` method.
2. **New Logic:** Check if the resource's data is unlocked:
    - `bool isUnlocked = KnowledgeManager.Instance.DataUnlocked.GetValueOrDefault(resource.Name);`

#### Precise vs. Vague Display (8h - 9h)

1. Inside `UpdateResourceData`, implement the display logic:
    - IF `isUnlocked` is **true**:
        - Display the **exact float value** (e.g., "Strength: 2.1") for all properties using a format string (e.g., `:F1`).
    - ELSE IF `isUnlocked` is **false**:
        - Continue to display the **Vague Description** (e.g., "Strength: Low") as implemented in Sprint 4.

#### Visual Feedback (Color) (9h - 10h)

1. When displaying the **precise value** (unlocked data), change the text color of the data to green or white (to show certainty).
2. When displaying the **vague value** (unlocked data), keep the text color grey or yellow (to show uncertainty).

#### Deduction Confirmation (10h - 11h)

1. Run the game. Before analyzing, scan Base Ore (shows "Strength: Low").
2. Deposit Base Ore into the Field Lab and wait for analysis.
3. Scan Base Ore again (should now show "Strength: 2.1"). This proves the core deduction loop: **Investigate $\rightarrow$ Unlock Exact Data**.

### Task 4: UI/UX Feedback and Polish (5 Hours)

#### Tech Point Visualization (11h - 12h)

1. Add a small display section to the HUD for **Tech Points**.
2. Add a label: "Analysis Points: [Value]".
3. Update this label via the `KnowledgeManager` singleton whenever points are gained.

#### Player Interaction Prompt (12h - 13h)

1. Implement a small `Label` over the Field Lab that says **"[E] Submit Base Ore"** when the player is within range.
2. Hide this prompt if the player does not have the required `Base Ore` in their inventory.

#### Art/Sound: Polish (13h - 14h)

1. Add a simple **VFX** for the Field Lab's analysis (e.g., a looping energy effect during the analysis screen time).
2. Add an ambient hum sound effect that plays when the Field Lab is placed and idle.

#### Final Review and Testing (14h - 15h)

1. Test the full cycle: Harvest $\rightarrow$ Interact $\rightarrow$ Inventory decreases $\rightarrow$ Points increase $\rightarrow$ Scanner UI updates.
2. Ensure that analyzing **Base Ore** does *not* unlock the data for **Catalyst Ore** (the system must be material-specific).

#### Commit Code (15h - 16h)

1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 5 Complete: Field Lab Tier 1 Tech, Precise Data Analysis, and Knowledge Tracking."

# 🔭 Sprint 6: Planetary Analysis & Deduction Input (16 Hours)

## Summary

This sprint implements the **Planetary Observatory** (our stand-in for the Gravimeter/Planetary Analyzer). This machine is the player's primary tool for solving the **Portal Equation**. Building it allows the player to measure the hardcoded **Gravimetric Shear** value, which is then used to calculate the exact **Portal Strength Requirement**.

## 🎯 Goal

  * Design, implement, and integrate the **Planetary Observatory** machine.
  * Implement the core Planetary Analysis logic: spending Tech Points to gain the exact numerical value of **Gravimetric Shear**.
  * Update the Scanner UI's Planetary Data section to display this precise, measured value, completing the first half of the deduction loop.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Machine placement, Tier 2 Tech Point consumption.
  * **C\# / .NET:** Deduction math execution, data storage for measured constants.

#### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Planetary Observatory Scene and Logic | 180 | |
| 2: Implementing the Deduction Input  | 240 | |
| 3: Displaying Precise Planetary Data | 240 | |
| 4: UI/UX Feedback and Polish | 300 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Planetary Observatory Scene and Logic (3 Hours)

#### Planetary Observatory Scene Setup (0h - 1h)

1. Create a new scene `PlanetaryObservatory.tscn` (Area2D root, Sprite2D, CollisionShape2D). This machine should be physically larger than the Field Lab.
2. Add a visual element (a dish or telescope) that hints at its measurement function.
3. Instance the scene in `World.tscn`.

#### ObservatoryManager Script (1h - 2h)

1. Create a C\# script `ObservatoryManager.cs` and attach it.
2. Add a `bool IsCalibrated = false;` to track the machine's state.
3. Implement a public method `StartCalibration()` that will initiate the measurement.

#### Player Placement & Tech Point Cost (2h - 3h)

1. **Cost:** Define a constant in `ObservatoryManager.cs` for the calibration cost (e.g., `const int CalibrationCost = 25;` **Analysis Tech Points**).
2. In `PlayerController.cs`, implement a simplified placement/construction logic (Left-Click near the machine).
3. When the player "builds" the machine, check if `KnowledgeManager.TechPoints["Analysis"]` $\ge 25$. If true, deduct the points and call `StartCalibration()`. If false, display an error UI message.

### Task 2: Implementing the Deduction Input (4 Hours)

#### Planetary Constant Measurement System (3h - 4h)

1. The ConstantsMeasured dictionary was already added to `KnowledgeManager.cs` in Sprint 5.
2. Add a method to calculate portal requirements dynamically:

```csharp
/// <summary>
/// Calculates the required portal foundation strength based on measured gravity
/// This implements the deduction formula from MECHANICS_DETAILS.md
/// Formula: Foundation Strength = Gravity × 2.5
/// </summary>
public float GetCalculatedPortalStrengthRequirement()
{
    if (ConstantsMeasured.GetValueOrDefault("Gravity", false))
    {
        // Player has measured gravity, can calculate precise requirement
        float requiredStrength = MeasuredGravity * 2.5f;
        GD.Print($"[DEDUCTION] Portal requires Strength: {requiredStrength:F1} " +
                $"(based on Gravity: {MeasuredGravity:F1})");
        return requiredStrength;
    }
    else
    {
        // Gravity unknown - return placeholder or "unknown" value
        // For VS, we show the hardcoded target but player can't verify it
        GD.Print("[DEDUCTION] Cannot calculate requirement: Gravity not measured");
        return 8.0f; // Hardcoded VS target (player doesn't know where this comes from)
    }
}

/// <summary>
/// Calculates required resistance based on corrosive index
/// Formula: Gate Resistance = CorrosiveIndex × 1.5
/// </summary>
public float GetCalculatedPortalResistanceRequirement()
{
    if (ConstantsMeasured.GetValueOrDefault("Corrosion", false))
    {
        // In full game, player would measure this with another device
        // For VS, we use the hardcoded value from GameManager
        float corrosiveIndex = GameManager.WorldConstants.CorrosiveIndex;
        float requiredResistance = corrosiveIndex * 1.5f;
        return requiredResistance;
    }

    // Fallback to hardcoded VS target
    return 3.0f;
}
```

3. Build and verify compilation.

#### Implement Observatory Calibration Process (4h - 5h)

1. Open `ObservatoryManager.cs` (from Task 1).
2. Implement the measurement logic:

```csharp
using Godot;
using System;

public partial class ObservatoryManager : Area2D
{
    // Calibration state
    public bool IsCalibrated { get; private set; } = false;

    // Cost to calibrate (Analysis tech points)
    private const int CalibrationCost = 25;

    // Visual and audio components
    private Label _statusLabel;
    private AudioStreamPlayer2D _calibrationSound;
    private AudioStreamPlayer2D _completeSound;

    public override void _Ready()
    {
        _statusLabel = GetNode<Label>("StatusLabel");
        _calibrationSound = GetNode<AudioStreamPlayer2D>("CalibrationSound");
        _completeSound = GetNode<AudioStreamPlayer2D>("CompleteSound");

        UpdateStatusDisplay();
    }

    /// <summary>
    /// Attempts to start the calibration process
    /// Returns true if successful, false if insufficient points
    /// </summary>
    public bool TryStartCalibration()
    {
        // Check if already calibrated
        if (IsCalibrated)
        {
            GD.Print("[Observatory] Already calibrated");
            return false;
        }

        // Check if player has enough tech points
        var costs = new System.Collections.Generic.Dictionary<string, int>
        {
            { "Analysis", CalibrationCost }
        };

        if (!KnowledgeManager.Instance.TrySpendPoints(costs))
        {
            GD.Print($"[Observatory] Need {CalibrationCost} Analysis points to calibrate");
            return false;
        }

        // Start calibration
        StartCalibration();
        return true;
    }

    /// <summary>
    /// Performs the gravity measurement
    /// This is where the "unknown" becomes "known"
    /// </summary>
    private void StartCalibration()
    {
        GD.Print("[Observatory] Starting planetary gravity measurement...");
        _calibrationSound.Play();

        // In a full game, this could be a timed process
        // For VS, it's instant
        CompleteCalibration();
    }

    /// <summary>
    /// Completes calibration and stores the measured gravity value
    /// </summary>
    private void CompleteCalibration()
    {
        // Get the TRUE gravity value from the world constants
        // This is the "measurement" - revealing the hidden variable
        float trueGravity = GameManager.WorldConstants.GravimetricShear;

        // Store in knowledge manager
        KnowledgeManager.Instance.MeasuredGravity = trueGravity;
        KnowledgeManager.Instance.ConstantsMeasured["Gravity"] = true;

        // Update state
        IsCalibrated = true;

        // Play completion sound
        _completeSound.Play();

        GD.Print($"[Observatory] MEASUREMENT COMPLETE");
        GD.Print($"[Observatory] Gravimetric Shear: {trueGravity:F2} g");
        GD.Print($"[Observatory] Calculating portal requirements...");

        // Trigger knowledge update event
        KnowledgeManager.Instance.OnKnowledgeUpdated?.Invoke();

        // Update visual display
        UpdateStatusDisplay();
    }

    /// <summary>
    /// Updates the status label based on calibration state
    /// </summary>
    private void UpdateStatusDisplay()
    {
        if (IsCalibrated)
        {
            _statusLabel.Text = $"CALIBRATED\nGravity: {KnowledgeManager.Instance.MeasuredGravity:F2} g";
            _statusLabel.Modulate = Colors.Green;
        }
        else
        {
            _statusLabel.Text = $"UNCALIBRATED\nCost: {CalibrationCost} Analysis Points";
            _statusLabel.Modulate = Colors.Yellow;
        }
    }
}
```

3. Update `PlayerController.cs` to allow activating the Observatory:

```csharp
// In _PhysicsProcess, check for Observatory interaction
private void CheckForObservatory()
{
    // Get all observatories (usually just one)
    var observatories = GetTree().GetNodesInGroup("Observatory");

    foreach (Node node in observatories)
    {
        if (node is ObservatoryManager obs)
        {
            float distance = Position.DistanceTo(obs.Position);

            if (distance < 80.0f)
            {
                // Show prompt
                if (Input.IsActionJustPressed("ui_accept"))
                {
                    obs.TryStartCalibration();
                }
            }
        }
    }
}
```

4. Don't forget to add the Observatory to the "Observatory" group in its `_Ready()` method.

#### Portal Requirement Dynamic Display (5h - 6h)

1. The portal goal display from Sprint 3 needs to update dynamically.
2. Open `InventoryUI.cs` (or create a dedicated `PortalGoalUI.cs`).
3. Add a method to update the portal requirement label:

```csharp
private Label _portalGoalLabel;

public override void _Ready()
{
    // Get reference to portal goal label
    _portalGoalLabel = GetNode<Label>("PortalGoalLabel");

    // Subscribe to knowledge updates
    KnowledgeManager.Instance.OnKnowledgeUpdated += UpdatePortalGoal;

    // Initial display
    UpdatePortalGoal();
}

/// <summary>
/// Updates the portal requirement display
/// Shows calculated value if gravity is measured, otherwise shows generic target
/// </summary>
private void UpdatePortalGoal()
{
    // Get calculated requirement (uses deduction formula)
    float requiredStrength =
        KnowledgeManager.Instance.GetCalculatedPortalStrengthRequirement();

    // Check if player knows WHERE this number comes from
    bool gravityMeasured =
        KnowledgeManager.Instance.ConstantsMeasured.GetValueOrDefault("Gravity", false);

    if (gravityMeasured)
    {
        // Player can deduce the formula: Gravity × 2.5
        float gravity = KnowledgeManager.Instance.MeasuredGravity;
        _portalGoalLabel.Text = $"Portal Foundation Requirement:\n" +
            $"Strength > {requiredStrength:F1}\n" +
            $"(Calculated: {gravity:F1}g × 2.5)";
        _portalGoalLabel.Modulate = Colors.White; // Known value
    }
    else
    {
        // Player sees the target but doesn't know why
        _portalGoalLabel.Text = $"Portal Foundation Requirement:\n" +
            $"Strength > ???\n" +
            $"(Measure planetary gravity to calculate)";
        _portalGoalLabel.Modulate = Colors.Yellow; // Unknown
    }
}
```

4. **Testing the Deduction Loop:**
    - Run the game
    - Check the Portal Goal UI - should show "???" initially
    - Gain 25 Analysis points (use Field Lab repeatedly)
    - Activate the Observatory
    - Watch console for "Gravity: 3.20 g" message
    - Check Portal Goal UI - should now show "Strength > 8.0 (3.2g × 2.5)"
    - This proves the player has successfully measured X and calculated the requirement

#### Audio and Visual Polish (6h - 7h)

1. **Create Observatory Scene Assets:**
    - Open `PlanetaryObservatory.tscn`
    - Add visual sprites (dish antenna, measurement device)
    - Add a **Label** node named "StatusLabel" positioned above the device
2. **Add Audio Players:**
    - Add **AudioStreamPlayer2D** named "CalibrationSound"
    - Add **AudioStreamPlayer2D** named "CompleteSound"
    - Find or create appropriate sci-fi sound effects:
        - Calibration: Low hum increasing in pitch
        - Complete: Confident "beep beep" confirmation tone
3. **Add Visual Feedback:**
    - Consider adding an **AnimationPlayer** to show:
        - Idle state: Slow rotation or blinking light
        - Calibrating state: Fast spinning, bright glowing
        - Calibrated state: Steady bright glow
4. **Test the Full Experience:**
    - Approach uncalibrated Observatory - see yellow "UNCALIBRATED" label
    - Press E to activate (with enough points)
    - Hear calibration sound
    - See label turn green and show "CALIBRATED\nGravity: 3.20 g"
    - Check that Portal Goal UI updates automatically
    - Verify console prints the deduction formula explanation

### Task 3: Displaying Precise Planetary Data (4 Hours)

#### Scanner UI Manager Update (Planetary) (7h - 8h)

1. In `ScannerUIManager.cs`, modify the `UpdatePlanetaryData(PlanetaryConstants constants)` method (from Sprint 4).
2. **New Logic:** Check if Gravity has been measured:
    - `bool isMeasured = KnowledgeManager.Instance.ConstantsMeasured.GetValueOrDefault("Gravity");`

#### Measured vs. Placeholder Display (8h - 9h)

1. Inside `UpdatePlanetaryData`, implement the display logic for `Label_Gravity`:
    - IF `isMeasured` is **true**:
        - Display the **measured value** from `KnowledgeManager.MeasuredGravity` (e.g., "Gravity: 3.20 g"). Color the text green/white.
    - ELSE IF `isMeasured` is **false**:
        - Display the original placeholder (e.g., "Gravity: ??? g"). Color the text yellow/grey.

#### Updating the Portal Goal UI (9h - 10h)

1. Modify the `Label` displaying the Portal Goal (from Sprint 3's HUD).
2. The text should now call the new method: **"Foundation Strength Required: $>\{KnowledgeManager.GetCalculatedPortalStrengthRequirement():F1\}$"**.
3. If the gravity is not measured, the label should still show "Req: $>8.0$" or a similar deduced target (for VS simplicity). Once measured, it shows the exact required value based on the formula: **$3.2 \times 2.5 = 8.0$** (in our hardcoded VS puzzle).

#### Deduction Confirmation Test (10h - 11h)

1. Run the game. Check the Portal Goal (e.g., "Req: $>8.0$"). Check the Scanner UI (Gravity: ???).
2. Gain enough **Analysis Tech Points** (using the Field Lab repeatedly).
3. Build and activate the **Planetary Observatory** (points decrease).
4. Check the Scanner UI (Gravity: 3.20 g). Check the Portal Goal (Still shows "Req: $>8.0$"). This confirms the logic and gives the player the required *X* value.

### Task 4: UI/UX Feedback and Polish (5 Hours)

#### Observatory VFX/State (11h - 12h)

1. Add a VFX to the Observatory. While uncalibrated, it should have a low-power, idle look.
2. When `IsCalibrated` is true, the VFX should switch to a high-power, active look (e.g., a dish spinning or lights turning blue).

#### Construction / Placement UI (12h - 13h)

1. Implement a simplified **Building Ghost** system for the Observatory: When the player intends to place it, a translucent image of the machine appears under the cursor.
2. The ghost should turn **Green** if the player has the points and is in a valid spot, and **Red** if they are missing the points or are placing it on an invalid tile (e.g., water).

#### Refining Tech Point Display (13h - 14h)

1. Update the HUD to show the cost for the next Tier 2 tech (Observatory).
2. E.g., "Next Tech (Observatory): 25/25 Analysis Points." The cost should glow green when the player can afford it.

#### Final Review and Testing (14h - 15h)

1. Verify the full loop: Field Lab (Tier 1) grants points, points unlock Observatory (Tier 2), Observatory measures gravity, gravity updates the numerical requirements on the HUD.
2. Ensure the tech point deduction happens correctly and prevents building if the points are insufficient.

#### Commit Code (15h - 16h)

1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 6 Complete: Planetary Observatory Tier 2 Tech, Gravity Measurement, and Deduction Input Implemented."

# 🔥 Sprint 7: Basic Crafting & Inventory (16 Hours)

## Summary

This sprint focuses on the first stage of resource transformation: converting mined **Ore** into **Metal Bars** using the **Basic Furnace** (Smelting). This completes the basic gathering-processing loop necessary to start engineering materials for the Portal.

## 🎯 Goal

  * Design, implement, and integrate the **Basic Smelting Furnace** machine.
  * Implement the core Smelting logic: consuming **Ore** and **Wood** (as fuel) to produce a **Metal Bar**.
  * Update the Inventory Manager to handle the resulting processed materials.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Machine interaction, simple machine state management (Idle, Heating, Smelting).
  * **C\# / .NET:** Crafting logic, resource consumption, and property inheritance.

### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Basic Smelting Furnace Scene and Interaction | 180 |  |
| 2: Smelting Logic and State Management | 240 | |
| 3: Inventory UI and Raw vs. Processed | 240 | |
| 4: Player Progression and Testing | 300 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Basic Smelting Furnace Scene and Interaction (3 Hours)

#### Furnace Scene Setup (0h - 1h)

1. Create a new scene `BasicFurnace.tscn` using an `Area2D` root node. Add a `Sprite2D` and `CollisionShape2D`.
2. Add two `Marker2D` nodes: `Input_Ore` and `Input_Fuel` to visualize input slots.
3. Instance the scene in `World.tscn` near the player's starting area.

#### FurnaceManager Script (1h - 2h)

1. Create a C# script `FurnaceManager.cs` and attach it to the `BasicFurnace` node.
2. Define state variables:
    - `public bool HasOre = false;`
    - `public int FuelCount = 0;`
    - `private bool IsSmelting = false;`

#### Player Interaction Logic (2h - 3h)

1. In `PlayerController.cs`, implement interaction (Left-Click) for the Furnace.
2. **Input Logic:** If overlapping the Furnace, check inventory:
    - If player has `Base Ore`, `InventoryManager.TryRemoveItem("Base Ore", 1)` and set `HasOre = true`.
    - If player has `Soft Wood`, `InventoryManager.TryRemoveItem("Soft Wood", 1)` and increment `FuelCount`.
3. Display appropriate interaction prompts: `[E] Deposit Ore` or `[E] Add Fuel`.

### Task 2: Smelting Logic and State Management (4 Hours)

#### Smelting Initiation (3h - 4h)

1. In `FurnaceManager.cs`, implement `CheckForSmeltStart()` method.
2. **Smelt Condition:** If `HasOre` is true AND `FuelCount` $\ge 1$, set `IsSmelting = true` and start a timer (e.g., 3.0 seconds for the VS).
3. Display a visual indicator (e.g., red glow/fire sprite) when `IsSmelting` is true.

#### Output Resource Definition (4h - 5h)

1. Define a new C# class/preset for the output: **"Base Metal Bar"**. This should be a processed version of "Base Ore."
2. **Property Inheritance:** The Metal Bar's properties (Strength, Resistance) should be derived from the Base Ore's properties, often with a slight increase (e.g., `Metal Strength = Ore Strength * 1.2`).

#### Smelting Completion (5h - 6h)

1. Implement a method `FinishSmelting()` that runs when the timer finishes.
2. Reset state: `HasOre = false`, `FuelCount -= 1`, `IsSmelting = false`.
3. **Output:** Call `InventoryManager.AddItem("Base Metal Bar", 1)` and visually spawn the Metal Bar near the furnace (using a temporary `Area2D` that the player can pick up).

#### Art/Sound: Furnace Assets (6h - 7h)

1. Create the final 2.5D sprite asset for the **Basic Furnace** (must include a clear visual state for "off" and "smelting/on").
2. Create sound effects for **"Smelting Loop"** (low furnace roar) and **"Smelt Finished"** (a metal clang).

### Task 3: Inventory UI and Raw vs. Processed (4 Hours)

#### Inventory Manager Refinement (7h - 8h)

1. Ensure `InventoryManager` can handle the new **"Base Metal Bar"** item.
2. Add a new `public event Action OnSmeltingComplete;` to the `InventoryManager` for UI updates related to processing machines.

#### HUD Update for Processed Items (8h - 9h)

1. In the `HUD.tscn` (Inventory Display from Sprint 3), add a label to track the count of **"Base Metal Bar"**.
2. In `InventoryUI.cs`, subscribe to `OnSmeltingComplete` (and `OnInventoryUpdated`). Update the Metal Bar label when a smelting action occurs.

#### Machine Status Display (9h - 10h)

1. Add a small text label above the `BasicFurnace` in its scene.
2. The `FurnaceManager.cs` should dynamically update this label based on state:
    - Idle: "Needs Ore and Fuel."
    - Ore Only: "Needs 1 Fuel."
    - Smelting: "Smelting... (3s)"
    - Finished: "Metal Bar Ready!"

#### Smelting Knowledge Gain (10h - 11h)

1. In `FurnaceManager.cs`'s `FinishSmelting()` method, grant new knowledge:
    - `KnowledgeManager.Instance.TechPoints["Smelting"] += 5;` (Introduce the new **Smelting Tech Point** category).

### Task 4: Player Progression and Testing (5 Hours)

#### Progression Barrier Check (11h - 12h)

1. Ensure the player *must* first harvest **Soft Wood** and **Base Ore** before they can even attempt to smelt.
2. The only way to get the Metal Bar needed for future progression is via the Furnace.

#### Resource Pickup Refinement (12h - 13h)

1. Create a `PickUpItem.tscn` (simple `Area2D` with a sprite for the Metal Bar).
2. When the player overlaps the `PickUpItem`, it should call `InventoryManager.AddItem("Base Metal Bar", 1)`, play a simple pickup sound, and queue itself for deletion.

#### Full Cycle Test: Raw to Processed (13h - 14h)

1. Play the game from start: Gather Wood and Ore. Scan both.
2. Use the Wood and Ore in the Furnace.
3. Wait for the timer, collect the Metal Bar.
4. Check that the Inventory HUD updates correctly: Ore/Wood decreases, Metal Bar increases. Check that Smelting points increase.

#### Art/Sound: Polish (14h - 15h)

1. Refine the sound timing for the furnace loop (must sound continuous).
2. Ensure the visual transition from `InputSlot` to `OutputBar` is clear.

#### Commit Code (15h - 16h)

1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 7 Complete: Basic Furnace, Smelting Logic, Property Inheritance, and Smelting Tech Points."

# 🧪 Sprint 8: Advanced Refining (Composition) (16 Hours)

## Summary

This sprint implements the **Gas Injector** machine. This machine is crucial for taking a basic refined material (like the **Base Metal Bar** from Sprint 7) and enhancing its properties by infusing it with a modifying element (**Catalyst Ore**), finally creating the high-strength **Composite Alloy** required to build the Portal Foundation.

## 🎯 Goal

  * Implement the **Gas Siphoning** mechanism to acquire the modifying element (**Catalyst Ore** in its raw form).
  * Design, implement, and integrate the **Gas Injector** machine (our stand-in for an alloying or compositing device).
  * Implement the core **Compositing Logic**: Base Metal + Catalyst Ore $\rightarrow$ Composite Alloy with boosted Strength.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** New resource acquisition method, advanced machine input validation.
  * **C\# / .NET:** Property calculation for composite materials, Tech Point consumption/gain.

### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Acquiring the Modifying Element | 180 |  |
| 2: Gas Injector Scene and Interaction | 180 | |
| 3: Compositing Logic | 240 | |
| 4: UI/UX Feedback and Testing | 360 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Acquiring the Modifying Element (3 Hours)

#### Catalyst Ore Harvest Logic (0h - 1h)

1. In `ResourceNode.cs`, add a new property: `public string RequiredHarvestTool;` (default is "Kinetic Mining").
2. Set the `Catalyst Ore` resource to require a different method (e.g., `Thermal Lancing` or, for VS simplicity, **"Gas Siphoning"**).
3. In `PlayerController.cs`, modify the harvest check: If the node requires **Gas Siphoning**, display a prompt: "Requires Gas Siphon."

#### Gas Siphon Tool Stub (1h - 2h)

1. Since we skip building the Siphon tool in the VS, create a temporary mechanism.
2. In `PlayerController.cs`, if the player Left-Clicks a `Catalyst Ore` node AND has sufficient "Analysis Tech Points" (e.g., $\ge 50$), allow the harvest, simulating a high-tech tool purchase/unlock.
3. Call `InventoryManager.AddItem("Catalyst Ore", 1)`.

#### Art/Sound: Catalyst Ore & Siphon (2h - 3h)

1. Create the final 2.5D sprite asset for **"Catalyst Ore"** (it should look distinct from Base Ore—e.g., bright purple or glowing).
2. Create a unique, high-pitched **"Siphon/Thermal" sound effect** for harvesting this ore, distinct from the dull thud of the Kinetic Mine.

### Task 2: Gas Injector Scene and Interaction (4 Hours)

#### Gas Injector Scene Setup (3h - 4h)

1. Create a new scene `GasInjector.tscn` (Area2D root, Sprite2D, CollisionShape2D). The sprite should suggest infusion (e.g., pipes and a pressurized tank).
2. Add two `Marker2D` input slots: `Input_Metal` and `Input_Catalyst`.
3. Instance the scene in `World.tscn`.

#### InjectorManager Script (4h - 5h)

1. Create a C# script `InjectorManager.cs` and attach it.
2. Define state variables: `public bool HasMetal = false;`, `public bool HasCatalyst = false;`, and `private bool IsCompositing = false;`.

#### Player Deposit Logic (5h - 6h)

1. In `PlayerController.cs`, implement interaction for the Injector.
2. **Input Logic:** Check inventory and deposit items:
    - If player has **Base Metal Bar**, use `TryRemoveItem()` and set `HasMetal = true`.
    - If player has **Catalyst Ore**, use `TryRemoveItem()` and set `HasCatalyst = true`.
3. Display interaction prompts: `[E] Deposit Metal` and `[E] Deposit Catalyst`.

### Task 3: Compositing Logic (4 Hours)

#### Implement Compositing Initiation Logic (6h - 7h)

1. Open `InjectorManager.cs` from Task 2.
2. Add a timer for the compositing process:

```csharp
// Compositing takes longer than smelting
private const float CompositingTime = 5.0f;
private float _compositingTimer = 0.0f;

/// <summary>
/// Checks if compositing can start and initiates the process
/// </summary>
private void CheckForCompositingStart()
{
    // Need both materials to start
    if (HasMetal && HasCatalyst && !IsCompositing)
    {
        GD.Print("[Gas Injector] Starting composition process...");
        IsCompositing = true;
        _compositingTimer = CompositingTime;

        // Play start sound (add AudioStreamPlayer2D in scene)
        GetNode<AudioStreamPlayer2D>("CompositingSound").Play();
    }
}

public override void _Process(double delta)
{
    // Always check if we can start
    CheckForCompositingStart();

    // Update compositing timer
    if (IsCompositing)
    {
        _compositingTimer -= (float)delta;

        // Update status display
        UpdateStatusLabel($"COMPOSITING... {_compositingTimer:F1}s");

        if (_compositingTimer <= 0)
        {
            FinishCompositing();
        }
    }
}
```

3. Add visual feedback while compositing:
    - Create a pulsing animation on the Gas Injector sprite
    - Add particle effects (colored gas swirls)
    - Make a status light change color (idle=yellow, active=blue)
#### Update CompositeMaterial Class with Calculation Logic (7h - 8h)

1. Open `CompositeMaterial.cs` (from Sprint 1).
2. Update with the complete compositing mathematics:

```csharp
using Godot;
using System;
using System.Collections.Generic;

// Represents a material created by combining other materials
// This is the key to solving the portal engineering puzzle
public class CompositeMaterial : RawResource
{
    // Input materials used to create this composite
    public RawResource PrimaryIngredient { get; set; }
    public RawResource ModifierIngredient { get; set; }

    // Modifiers that affect final properties
    public float StrengthModifier { get; set; } = 1.0f;

    /// <summary>
    /// Calculates the final properties of the composite material
    /// This is where the engineering magic happens
    /// </summary>
    public void CalculateProperties()
    {
        if (PrimaryIngredient == null || ModifierIngredient == null)
        {
            GD.PrintErr("[Composite] Cannot calculate: missing ingredients");
            return;
        }

        GD.Print($"[Composite] Calculating properties for {Name}...");
        GD.Print($"  Primary: {PrimaryIngredient.Name}");
        GD.Print($"  Modifier: {ModifierIngredient.Name}");

        // For each property type, calculate the composite value
        foreach (ResourcePropertyType propType in
                 Enum.GetValues(typeof(ResourcePropertyType)))
        {
            float finalValue = CalculatePropertyValue(propType);
            Properties[propType] = new ResourceProperty(propType, finalValue);
        }

        // Log final strength (most important for portal)
        GD.Print($"[Composite] Final Strength: " +
                $"{Properties[ResourcePropertyType.Strength].Value:F1}");
    }

    /// <summary>
    /// Calculates a single property value based on composition rules
    /// Different properties may combine differently
    /// </summary>
    private float CalculatePropertyValue(ResourcePropertyType propType)
    {
        // Get base values from ingredients
        float primaryValue = PrimaryIngredient.Properties[propType].Value;
        float modifierValue = ModifierIngredient.Properties[propType].Value;

        // Strength uses special additive logic for the VS
        if (propType == ResourcePropertyType.Strength)
        {
            // Base Metal Bar has strength ~4.2 (from smelting Base Ore with 1.2x multiplier)
            // Catalyst Ore provides +4.0 boost
            // Result: 4.2 + 4.0 = 8.2 (meets requirement of >8.0)
            return primaryValue + modifierValue;
        }

        // Other properties use averaging
        // (In full game, different properties could use different formulas)
        return (primaryValue + modifierValue) / 2.0f;
    }

    /// <summary>
    /// Creates a descriptive name for the composite
    /// </summary>
    public void GenerateCompositeName()
    {
        // Create a name based on ingredients
        // E.g., "Base Metal-Catalyst Alloy"
        Name = $"{PrimaryIngredient.Name}-{ModifierIngredient.Name} Composite";
    }
}
```

3. Build and verify the class compiles.
#### Implement Composite Creation in Gas Injector (8h - 9h)

1. Continue in `InjectorManager.cs`.
2. Add references to the input materials:

```csharp
// Store actual resource data, not just bools
private RawResource _inputMetal;
private RawResource _inputCatalyst;

// Updated deposit methods
public void DepositMetal(RawResource metalBar)
{
    _inputMetal = metalBar;
    HasMetal = true;
    GD.Print($"[Gas Injector] Metal deposited: {metalBar.Name}");
}

public void DepositCatalyst(RawResource catalystOre)
{
    _inputCatalyst = catalystOre;
    HasCatalyst = true;
    GD.Print($"[Gas Injector] Catalyst deposited: {catalystOre.Name}");
}
```

3. Implement the finish compositing method:

```csharp
/// <summary>
/// Completes the compositing process and creates the output material
/// </summary>
private void FinishCompositing()
{
    GD.Print("[Gas Injector] Compositing complete!");

    // Create the composite material
    CompositeMaterial composite = new CompositeMaterial
    {
        PrimaryIngredient = _inputMetal,
        ModifierIngredient = _inputCatalyst,
        Description = "A refined alloy with enhanced structural properties."
    };

    // Generate name and calculate properties
    composite.GenerateCompositeName();
    composite.CalculateProperties();

    // Verify it meets portal requirements (for testing)
    float strength = composite.Properties[ResourcePropertyType.Strength].Value;
    float required = KnowledgeManager.Instance.GetCalculatedPortalStrengthRequirement();

    GD.Print($"[Gas Injector] Created: {composite.Name}");
    GD.Print($"[Gas Injector] Strength: {strength:F1} (Required: >{required:F1})");

    if (strength >= required)
    {
        GD.Print("[Gas Injector] ✓ MEETS PORTAL REQUIREMENTS!");
    }
    else
    {
        GD.Print("[Gas Injector] ✗ Does not meet requirements");
    }

    // Add to inventory (simplified - stores just the name)
    // In full game, would store the actual composite object
    InventoryManager.Instance.AddItem(composite.Name, 1);

    // Grant compositing tech points
    KnowledgeManager.Instance.AddTechPoints("Compositing", 10);

    // Reset state
    IsCompositing = false;
    HasMetal = false;
    HasCatalyst = false;
    _inputMetal = null;
    _inputCatalyst = null;

    // Play completion sound
    GetNode<AudioStreamPlayer2D>("CompleteSound").Play();

    UpdateStatusLabel("COMPLETE! Collect composite.");
}
```

4. **Critical: Store composite data for later verification:**
    - Add a static dictionary to GameManager to store created composites:

```csharp
// In GameManager.cs:
public static Dictionary<string, CompositeMaterial> CreatedComposites { get; private set; }
    = new Dictionary<string, CompositeMaterial>();
```

5. Update the inventory add to also store in GameManager:

```csharp
// In FinishCompositing, before adding to inventory:
GameManager.CreatedComposites[composite.Name] = composite;
```

6. This allows the Portal to verify actual properties later.
#### Test the Complete Compositing Chain (9h - 10h)

1. **Preparation Test:**
    - Start the game and harvest Base Ore
    - Smelt it in the Basic Furnace to get Base Metal Bar (Strength ~4.2)
    - Use Field Lab to analyze the metal bar (verify strength)
    - Harvest Catalyst Ore (requires tech points or simplified access for VS)
    - Analyze the Catalyst Ore (should show Strength ~4.0)
2. **Compositing Test:**
    - Approach Gas Injector
    - Deposit Base Metal Bar (press E with metal in inventory)
    - Deposit Catalyst Ore (press E again with catalyst)
    - Watch status: "COMPOSITING... 5.0s"
    - Wait for timer to reach 0
3. **Verification Test:**
    - Check console output for "Created: Base Metal-Catalyst Composite"
    - Verify strength calculation: "Strength: 8.2 (Required: >8.0)"
    - Should see "✓ MEETS PORTAL REQUIREMENTS!"
    - Check inventory has the composite
    - Check Compositing points increased by 10
4. **Analysis Verification:**
    - Take the composite to the Field Lab
    - Analyze it to unlock precise data
    - Use Hand Scanner on it
    - Verify Scanner UI shows "Strength: 8.2" (precise, green text)
5. **Math Verification:**
    - Base Ore (2.0) → Smelt → Base Metal Bar (2.0 × 1.2 = 2.4)
    - Wait, we need to adjust! Let's check:
    - Base Metal needs to start at 4.2 to work with the formula
    - Adjust `VSResourcePresets.CreateBaseOre()` or smelting multiplier
    - OR adjust Catalyst boost to compensate
    - The key: Final Strength must be > 8.0
6. **Update Resource Presets if needed:**

```csharp
// In VSResourcePresets.cs, adjust Base Ore:
ore.Properties[ResourcePropertyType.Strength] =
    new ResourceProperty(ResourcePropertyType.Strength, 3.5f);
// With 1.2x smelting multiplier: 3.5 × 1.2 = 4.2
// Then: 4.2 + 4.0 (catalyst) = 8.2 ✓
```

### Task 4: UI/UX Feedback and Testing (5 Hours)

#### HUD Update for New Resources (10h - 11h)

1. Update the `HUD.tscn` to track the counts of **"Catalyst Ore"** and **"Composite Alloy"**.

#### Machine Status Display (11h - 12h)

1. Add a status label above the `GasInjector` in its scene.
2. Update the label dynamically:
    - Idle: "Needs Metal and Catalyst."
    - One Input: "Needs Catalyst/Metal."
    - Compositing: "Refining... (5s)"
    - Finished: "Composite Alloy Ready!"

#### Full Cycle Test: Composite Creation (12h - 13h)

1. Start with Base Metal Bar (from Sprint 7) and Catalyst Ore (siphoned).
2. Deposit both into the Injector. Wait for the process.
3. Collect the **Composite Alloy**. Check Inventory.

#### Deduction Verification (13h - 14h)

1. **Crucial Test:** Use the **Hand Scanner** (vague) or the **Field Lab** (precise) on the **Composite Alloy**.
2. The Scanner UI must show the calculated value: **Strength $\approx 8.2$**. This visually confirms the player has engineered a material that $\mathbf{meets}$ the Portal's $\mathbf{>8.0}$ requirement.

#### Art/Sound: Injector FX (14h - 15h)

1. Add sound effects for **"Injector Activation"** (a whoosh/hiss of gas) and **"Compositing Loop"** (pressurized bubbling/mixing sound).
2. Add a strong VFX (e.g., colored swirling gas) to the injector while compositing.

#### Commit Code (15h - 16h)

1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 8 Complete: Gas Injector, Compositing Logic, and Required Composite Alloy Created."

# 🎓 Sprint 9: ProcGen Tech Tree V2 & Unlocks (16 Hours)

## Summary

This sprint fully implements the **ProcGen Tech Tree** as a functional system. We will create a dedicated UI window where the player can view their accumulated **Tech Points** (Analysis, Smelting, Compositing) and spend them to unlock higher-tier abilities or the construction schematics for new machines (like the **Gas Injector** and future **Simulation Core**).

## 🎯 Goal

  * Create a dedicated, navigable **Research Station UI**.
  * Implement a structured data list of researchable **Tech Nodes** with prerequisites and costs.
  * Implement the core logic for spending Tech Points to unlock new abilities/machines.
  * Integrate the system to gate the **Gas Injector** and **Planetary Observatory** behind a research cost.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Pop-up window management, custom UI control structures, button logic.
  * **C\# / .NET:** Data structure for Tech Nodes, prerequisite checking.

### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Research Station UI Scene Setup | 240 |  |
| 2: Implementing the Tech Tree Data Structure | 180 | |
| 3: Research Logic and Prerequisites | 240 | |
| 4: Gating the Machines | 300 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Research Station UI Scene Setup (4 Hours)

#### Research UI Scene (0h - 1h)

1. Create a new `CanvasLayer` scene named `ResearchUI.tscn`. This will be a pop-up window.
2. Add a main `Panel` or `Control` node that covers the center of the screen when active.
3. Add a title label: **"Research & Development"**.

#### Tech Point Display (1h - 2h)

1. Create a dedicated section (e.g., a side panel) to display the player's current Tech Point totals from `KnowledgeManager.cs`.
2. Add three labels: `Label_AnalysisPoints`, `Label_SmeltingPoints`, and `Label_CompositingPoints`.
3. Implement a C# script `TechPointDisplay.cs` to dynamically pull and update these values from the `KnowledgeManager` whenever the Research UI is opened.

#### Research Grid Structure (2h - 3h)

1. Create the main research area using a `VBoxContainer` or `GridContainer` to hold the individual Tech Nodes.
2. Create a prototype scene `TechNodeUI.tscn` (a `Panel` or `Control`) that will represent one research item. This prototype needs:
    - A `Label` for the Title/Description.
    - A `Label` for the Cost (e.g., "Cost: 10 Analysis Points").
    - A `Button` named `Button_Research`.

#### UI Activation and Input (3h - 4h)

1. In Godot's Input Map, define a new action: **"ToggleResearch"** (bind to a key like **Tab** or **R**).
2. In the `GameManager.cs` or `PlayerController.cs`, implement logic to toggle the visibility of the `ResearchUI.tscn` when "ToggleResearch" is pressed.

### Task 2: Implementing the Tech Tree Data Structure (3 Hours)

#### TechNode Data Class (4h - 5h)

1. Create a C# class `TechNode.cs` (or struct) to hold the definition of one unlockable item.
2. Fields needed:
    - `public string ID;` (e.g., "Unlock_GasInjector")
    - `public string DisplayName;`
    - `public Dictionary<string, int> Costs;` (e.g., {"Analysis", 10}, {"Smelting", 5})
    - `public string PrerequisiteID;` (ID of the node that must be unlocked first)

#### VS Tech Tree Definition (5h - 6h)

1. Create a static C# class `TechTreeData.cs` to hold the entire VS structure (list of `TechNode` objects).
2. Define the four core VS nodes:
    - **Node 1: Base Analysis V2:** ID: "Unlock_PreciseAnalysis" (Cost: 15 Analysis). **Effect:** Allows Field Lab to show precise data (currently unlocked by default in S5).
    - **Node 2: Planetary Survey:** ID: "Unlock_Observatory" (Cost: 25 Analysis, Prereq: N/A).
    - **Node 3: Advanced Compositing:** ID: "Unlock_GasInjector" (Cost: 10 Compositing, 10 Smelting).
    - **Node 4: Portal Construction:** ID: "Unlock_PortalBuild" (Cost: 50 Total Points, Prereq: "Unlock_GasInjector").

#### Knowledge Manager Integration (6h - 7h)

1. In `KnowledgeManager.cs`, add a new field: `public HashSet<string> UnlockedNodes = new();`
2. Add a method `public bool IsNodeUnlocked(string id)`.

### Task 3: Research Logic and Prerequisites (5 Hours)

#### TechNodeUI Script Logic (7h - 8h)

1. Create a C# script `TechNodeUI.cs` and attach it to the `TechNodeUI.tscn` scene.
2. Add a public method `Initialize(TechNode data)` that sets the display labels.
3. Implement `CheckState()`: checks if the node is unlocked, if prerequisites are met, and if the costs can be afforded (using `KnowledgeManager`).

#### Affordability and Button State (8h - 9h)

1. In `CheckState()`, disable `Button_Research` if the node is already unlocked OR if any prerequisites are missing.
2. If prerequisites are met, check costs. If all costs are affordable, enable the button and change its color to green (to signal readiness). If not, disable the button and show "Not Enough Points."

#### The Unlock Function (9h - 10h)

1. Connect the `Button_Research.Pressed` signal to a method `OnResearchPressed()`.
2. In this method:
    - Call `KnowledgeManager.TrySpendPoints(Costs)`.
    - If successful, add the node ID to `KnowledgeManager.UnlockedNodes`.
    - Call `CheckState()` again to update the UI (now showing "Unlocked").

#### Art/Sound: UI Polish (10h - 11h)

1. Create placeholder icons for the four VS Tech Nodes (e.g., a microscope for Analysis, a furnace icon for Smelting, a blueprint for Portal).
2. Create sound effects for **"UI Open"** (soft futuristic tone), **"Unlock Success"** (chime/jingle), and **"Unlock Fail"** (a soft error buzz).

### Task 4: Gating the Machines (4 Hours)

#### Gating the Planetary Observatory (11h - 12h)

1. Go to `ObservatoryManager.cs` (from Sprint 6).
2. Modify the "construction/placement" logic to add a gate check:
    - `if (!KnowledgeManager.Instance.IsNodeUnlocked("Unlock_Observatory"))`
    - If locked, display: "Requires Planetary Survey research." The player must now spend 25 Analysis points in the new UI before building this.

#### Gating the Gas Injector (12h - 13h)

1. Go to `InjectorManager.cs` (from Sprint 8).
2. Implement a similar gate check:
    - `if (!KnowledgeManager.Instance.IsNodeUnlocked("Unlock_GasInjector"))`
    - If locked, the machine should not be placeable or usable. Display: "Requires Advanced Compositing research."

#### System Integration Test (13h - 14h)

1. Run the game. Try to place the **Observatory** and **Gas Injector** (should fail/block).
2. Open the Research UI (Tab key). Confirm the point totals are correct and the nodes are locked.
3. Farm enough points (using Field Lab, Furnace, Injector).
4. Unlock "Planetary Survey" (points decrease, button changes to "UNLOCKED").
5. Confirm the Observatory can now be built/activated.

#### Final Review and Cleanup (14h - 15h)

1. Ensure all `TechNodeUI` instances correctly refresh their state when a node is unlocked (since unlocking one might meet the prereq for another).
2. Check for potential recursive calls or infinite loops in the prerequisite checks.

#### Commit Code (15h - 16h)

1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 9 Complete: Functional ProcGen Tech Tree UI, Cost/Prerequisite Gating, and Machine Unlocks."

# 🏗️ Sprint 10: Portal Construction & UI (16 Hours)

## Summary

This sprint implements the physical construction of the **Portal Foundation**. The player must have the required **Composite Alloy** (Strength $> 8.0$) in their inventory and have unlocked the Portal construction schematic (from Sprint 9). We will implement a dedicated Construction UI for selecting the structure and the submission logic to consume the alloy.

## 🎯 Goal

  * Implement the **Portal Foundation** scene, which acts as the final construction site.
  * Implement a dedicated **Construction Placement UI** that checks for the **PortalBuild** unlock.
  * Implement the core construction logic: consuming the $\approx 8.2$ Strength **Composite Alloy**.
  * Verify that the placed material meets the **Portal Requirement** calculated in Sprint 6.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Placement system refinement (ghosting), dedicated modal UI.
  * **C\# / .NET:** Final verification of material properties against world requirements.

### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Portal Foundation Scene and Construction UI | 240 |  |
| 2: Material Submission Logic | 240 | |
| 3: Property Verification | 240 | |
| 4: Player Progression and Testing | 240 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Portal Foundation Scene and Construction UI (4 Hours)

#### Portal Foundation Scene Setup (0h - 1h)

1. Create a new scene `PortalFoundation.tscn` (Area2D root, Sprite2D, CollisionShape2D). The sprite should represent a large, partially finished structure (like a metal ring or base).
2. Instance the scene in a specific, marked location in `World.tscn` (or implement a simplified placement system). For the VS, we will use a pre-set location.

#### Construction Placement UI (Modal) (1h - 2h)

1. Create a new `CanvasLayer` scene `ConstructionUI.tscn`. This is a small, centered modal window that appears when the player activates construction mode.
2. Add a `VBoxContainer` to list available structures.
3. Add a placeholder `Button` for the Portal Foundation: `Button_Portal`. Label it "Portal Foundation."

#### Construction Mode Activation (2h - 3h)

1. In Godot's Input Map, define **"ToggleBuildMode"** (e.g., bind to **B**).
2. In `PlayerController.cs`, implement logic to toggle `ConstructionUI.Visible` when "ToggleBuildMode" is pressed.
3. **Gating:** Check if `KnowledgeManager.IsNodeUnlocked("Unlock_PortalBuild")` is true. If not, the Construction UI should display: "Requires Portal Construction research."

#### Placement Ghosting (3h - 4h)

1. When the player clicks `Button_Portal`, activate a **Placement Ghost** mode.
2. The player's cursor should follow a translucent sprite of the `PortalFoundation.tscn`.
3. Implement a simple check: the ghost turns **Green** if placed on a valid tile (e.g., solid ground, not too close to other structures) and **Red** otherwise.

### Task 2: Material Submission Logic (4 Hours)

#### PortalManager Script (4h - 5h)

1. Create a C# script `PortalManager.cs` and attach it to the `PortalFoundation` node.
2. Define state: `public bool FoundationBuilt = false;`
3. Add a list to track required materials: `private Dictionary<string, int> RequiredMaterials = new() { { "Composite Alloy", 20 } };` (The VS requires 20 units).

#### Submission Panel UI (5h - 6h)

1. Once the `PortalFoundation` is placed, pressing Left-Click while near it should open a second modal: **SubmissionUI.tscn**.
2. This UI lists the required item: "Composite Alloy: 0/20".
3. Add a `Button` labeled **"Submit Alloy"**.

#### Submission Logic (6h - 7h)

1. Connect the `Submit Alloy` button to a method in `PortalManager.cs`.
2. This method should attempt to take all available **Composite Alloy** from the player's inventory (up to the required 20 units).
3. `InventoryManager.TryRemoveItem("Composite Alloy", amount)`. Update the Submission UI count.

#### Art/Sound: Construction Assets (7h - 8h)

1. Create the final 2.5D sprite asset for the **Portal Foundation** (an industrial, heavily armored base).
2. Create sound effects for **"Construction Mode Toggle"** (a clean UI click) and **"Material Submission"** (a heavy, automated loading sound).

### Task 3: Property Verification (4 Hours)

#### Verification Method (8h - 9h)

1. In `PortalManager.cs`, implement a method `VerifyMaterialProperties()`. This runs immediately after the required 20 units of alloy have been submitted.
2. Retrieve the *actual* properties of the submitted **Composite Alloy** (e.g., Strength $\approx 8.2$). Since the alloy is singular, we check its stats.

#### Requirement Check (9h - 10h)

1. Retrieve the *required* portal properties (from Sprint 6): `float requiredStrength = KnowledgeManager.GetCalculatedPortalStrengthRequirement();` (e.g., $8.0$ in the VS).
2. Implement the critical check:
    - `if (submittedAlloy.Properties[Strength].Value >= requiredStrength)`
    - If true, the material is adequate: `FoundationBuilt = true;`. Display success message.

#### Failure State Logic (10h - 11h)

1. If the check is **false** (e.g., the player mistakenly submitted a low-strength alloy), display a large error message:
    - "FOUNDATION FAILURE: Material Strength [X] too low for Planetary Shear Requirement [Y]! Resources Lost."
2. The player must then go back and craft the correct alloy to try again. (For the VS, this forces them to understand the deduction loop).

#### Visual Status Update (11h - 12h)

1. If `FoundationBuilt` is true, change the sprite of `PortalFoundation.tscn` to a **"Construction Complete"** state (e.g., the ring is sealed, lights turn on).
2. Hide the Submission UI, as this step is finished.

### Task 4: Player Progression and Testing (4 Hours)

#### End-to-End Test (Success) (12h - 13h)

1. Start with the Composite Alloy in inventory. Build/Place the Foundation.
2. Submit the 20 Composite Alloys.
3. Verify the `VerifyMaterialProperties()` check passes (8.2 $\ge$ 8.0).
4. Confirm the Foundation sprite changes, and the `FoundationBuilt` state is true.

#### End-to-End Test (Failure Mock) (13h - 14h)

1. **Debug Injection:** Temporarily inject a **low-strength material** (e.g., Base Metal Bar) into the inventory, labeled as the "Composite Alloy."
2. Submit the low-strength material and verify the `VerifyMaterialProperties()` check fails (e.g., 4.2 $< 8.0$).
3. Confirm the failure message appears, demonstrating the deduction risk.

#### UI/UX Polish (14h - 15h)

1. Ensure the Construction UI is intuitive and doesn't conflict with the Scanner UI.
2. Add a simple **VFX** to the final built foundation (e.g., a subtle energy aura).

#### Commit Code (15h - 16h)

1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 10 Complete: Portal Foundation Placement, Material Submission, and Property Verification Logic."

# 🤖 Sprint 11: Simulation Core & Win State (16 Hours)

## Summary

This sprint implements the **Simulation Core**, the player's final piece of technology. This machine allows the player to run a "virtual test" on the constructed Portal Foundation (from Sprint 10) to verify its structural integrity against the Planet's forces *before* committing to powering it up. The successful simulation leads directly to the Vertical Slice's win state.

## 🎯 Goal

  * Design, implement, and integrate the **Simulation Core** machine.
  * Implement the core **Simulation Logic**: a full, real-time comparison of the constructed material's properties against the Planet's constants using the deduction formulas.
  * Create a clear visual and audio feedback loop for **PASS/FAIL** simulation results.
  * Define and implement the **Vertical Slice Win State**.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Complex modal UI for detailed report, Win State scene transition.
  * **C\# / .NET:** Final deduction logic, floating point tolerance checks.

### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Simulation Core Scene and Integration | 180 |  |
| 2: Implementing the Simulation Logic | 240 | |
| 3: Simulation Report UI | 240 | |
| 4: Win State Implementation | 300 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Simulation Core Scene and Integration (3 Hours)

#### Simulation Core Scene Setup (0h - 1h)

1. Create a new scene `SimulationCore.tscn` (Area2D root, Sprite2D, CollisionShape2D). The sprite should look high-tech, like a powerful computer or projector.
2. Instance the scene in `World.tscn` (ideally near the Portal Foundation).
3. **Gating:** Add a check in `PlayerController.cs` to ensure this machine can only be built/activated IF **"Unlock\_PortalBuild"** is unlocked (from Sprint 9).

#### SimulationManager Script (1h - 2h)

1. Create a C\# script `SimulationManager.cs` and attach it.
2. Add state: `public bool IsSimulating = false;` and `public bool LastSimPassed = false;`
3. Implement a public method `StartSimulation()` triggered by player interaction.

#### Player Interaction Logic (2h - 3h)

1. In `PlayerController.cs`, implement interaction (Left-Click) for the Simulation Core.
2. **Prerequisite:** The player can only start the simulation if the **Portal Foundation** has been successfully built (`PortalManager.FoundationBuilt == true`). Display an error if the foundation is missing.
3. If conditions met, call `StartSimulation()`.

### Task 2: Implementing the Simulation Logic (4 Hours)

#### Simulation Formula Check (3h - 4h)

1. In `SimulationManager.cs`, create a method `RunChecks()`.
2. **Required Strength:** Get the required strength: `float requiredS = KnowledgeManager.GetCalculatedPortalStrengthRequirement();`
3. **Actual Strength:** Get the strength of the submitted material (e.g., from a property stored in `PortalManager.cs` after submission).
4. **Primary Test:** `bool strengthPass = actualS >= requiredS - 0.01f;` (Use a small tolerance for floating point comparisons).

#### Secondary (Placeholder) Test (4h - 5h)

1. For completeness, implement a second check based on another constant (e.g., Corrosive Index vs. Material Resistance).
2. `float requiredR = GameManager.WorldConstants.CorrosiveIndex * 1.5f;` (The deduction formula).
3. `bool resistancePass = actualR >= requiredR - 0.01f;`
4. **Overall Result:** `bool finalPass = strengthPass && resistancePass;`

#### Simulation Time and Result (5h - 6h)

1. In `StartSimulation()`, set `IsSimulating = true` and start a timer (e.g., 4.0 seconds).
2. When the timer finishes, call `RunChecks()`. Set `LastSimPassed = finalPass`. Display the results via the UI (see Task 3).

#### Art/Sound: Core Assets (6h - 7h)

1. Create the final 2.5D sprite asset for the **Simulation Core** (needs a clear visual state for "idle" and "running simulation").
2. Create sound effects for **"Simulation Start"** (a dramatic power-up tone) and a looping **"Simulation Process"** sound (fast, frantic computer noise).

### Task 3: Simulation Report UI (4 Hours)

#### Report Modal Setup (7h - 8h)

1. Create a new `CanvasLayer` scene `SimulationReportUI.tscn`. This is a large, immersive modal window.
2. Add a main title: **"PORTAL INTEGRITY SIMULATION REPORT"**.
3. Add a large, central label for the **FINAL STATUS** (e.g., "SIMULATION SUCCESS" or "CATASTROPHIC FAILURE").

#### Detailed Check Display (8h - 9h)

1. Create two `Label` pairs to show the deduction results:
    - **Check 1 (Strength):** "Shear Stress Margin: [Actual] / [Required]"
    - **Check 2 (Resistance):** "Corrosion Margin: [Actual] / [Required]"
2. Color the text for each check **Green** if it passed, and **Red** if it failed.

#### Report UI Scripting (9h - 10h)

1. Create a C\# script `SimulationReportUI.cs` and attach it.
2. Implement a public method `ShowReport(bool success, float actualS, float requiredS, float actualR, float requiredR)` that populates the labels.
3. The success/fail title should be set dynamically, with distinct colors (Green for Success, Red for Failure).

#### Report Integration (10h - 11h)

1. In `SimulationManager.cs`, after `RunChecks()` completes, instance and show the `SimulationReportUI.tscn`, passing in the calculated results (actual vs. required values).

### Task 4: Win State Implementation (5 Hours)

#### Win State Check (11h - 12h)

1. In `SimulationManager.cs`, if `LastSimPassed` is true, display a final prompt over the Portal Foundation: **"SIMULATION SUCCESSFUL. [F] Activate Portal."**
2. Create a new input action **"ActivatePortal"** (e.g., bind to **F**).

#### Vertical Slice Win Scene (12h - 13h)

1. Create a minimal new scene `WinScene.tscn`. This can be a simple black screen with a large title: **"VERTICAL SLICE COMPLETE: WORLD BLAZED\!"**
2. Add a simple success message (e.g., "The Portal is stable. You can now travel home."). Add a `Button` to quit the application.

#### Activation and Transition (13h - 14h)

1. In `PlayerController.cs`, if the player presses "ActivatePortal" while near the successful Foundation:
    - Play a final, powerful **Portal Activation Sound** (a deep whoosh/energy surge).
    - Use `GetTree().ChangeSceneToFile("res://WinScene.tscn")` to transition the player out of the game world.

#### Full Cycle Final Test (14h - 15h)

1. Play the game from start: Deduced requirements $\rightarrow$ Gathered and refined $\rightarrow$ Built Foundation (Sprint 10 pass) $\rightarrow$ Start Simulation.
2. Verify the simulation passes, the Report UI is correct, and the "Activate Portal" prompt appears.
3. Press F and confirm the transition to the **Win Scene**.

#### Commit Code (15h - 16h)

1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 11 Complete: Simulation Core Tier 3 Tech, Full Deduction Verification, and VS Win State Implemented."

# ✨ Sprint 💖 12: Aesthetic Polish & Juiciness (16 Hours)

## Summary

This final sprint is dedicated to polish. We will integrate final art, audio, music, and apply visual "juice" (particles, screen shake, UI responsiveness) to the core interactions established in Sprints 1-11. The goal is to maximize the impact of the Vertical Slice demonstration.

## 🎯 Goal

  * Integrate final 2.5D art assets for all machines and the portal.
  * Implement a simple background music loop and essential soundscapes.
  * Add VFX and screen shake to high-impact actions (Smelting complete, Simulation result, Portal Activation).
  * Ensure all UI elements are fully responsive and visually clean.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** `AnimationPlayer`, `AudioStreamPlayer`, `Camera2D` (for shake).
  * **Art/Sound:** Final integration of all developed assets.

### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Art Asset Integration | 180 |  |
| 2: Audio & Music Integration | 180 | |
| 3: Visual Juice and Feedback | 240 | |
| 4: Tutorial, Bug Fixing & Final Review | 360 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Art Asset Integration (3 Hours)

#### World and Resource Art Finalization (0h - 1h)

1. Replace all placeholder sprites (colored squares, simple sketches) with the final 2.5D isometric art assets for:
    - Ground/Base Tiles.
    - **Base Ore** and **Catalyst Ore** nodes.
    - **Soft Wood** node.
2. Ensure all tile collisions and visual offsets are correctly aligned for the isometric perspective.

#### Machine Art Finalization (1h - 2h)

1. Replace the final sprites for the four main machines:
    - **Field Lab** (Tier 1)
    - **Planetary Observatory** (Tier 2)
    - **Basic Furnace** (Smelting)
    - **Gas Injector** (Compositing)
2. Verify that the visual states (e.g., Furnace "off" vs. "smelting") use the correct animated sprites.

#### Portal & Simulation Core Art (2h - 3h)

1. Replace the final sprites for the ultimate structures:
    - **Portal Foundation** (Built/Unbuilt states)
    - **Simulation Core** (Idle/Running states)
2. Ensure the Portal's "Construction Complete" state looks visually impressive and final.

### Task 2: Audio & Music Integration (3 Hours)

#### Background Music Loop (3h - 4h)

1. Add an `AudioStreamPlayer` node to the `World.tscn` (or `GameManager`).
2. Load the final 30-second **Ambient Exploration Music** (from Sprint 3) and set it to loop.
3. Set the volume to a low, non-intrusive level.

#### Final Sound Effects Integration (4h - 5h)

1. Verify and tune the volume/pitch for all major SFX developed in previous sprints:
    - **Footsteps** (PlayerController)
    - **Harvesting/Siphoning** (ResourceNode interaction)
    - **Smelting/Compositing** (Furnace/Injector completion sounds)
    - **UI Clicks** (for Research, Construction, and all buttons).

#### Machine Ambient Soundscapes (5h - 6h)

1. Add subtle, looping ambient sounds to the machines when they are idle (but built):
    - **Field Lab:** A soft, periodic "beep/scan" sound.
    - **Planetary Observatory:** A quiet, rhythmic hum.
    - **Basic Furnace:** A low, idle crackle/glow sound.

### Task 3: Visual Juice and Feedback (4 Hours)

#### Screen Shake for Impact (6h - 7h)

1. In `PlayerController.cs` (or a dedicated `CameraShake.cs` script), implement a function `ShakeCamera(float duration, float magnitude)`. This uses `Camera2D.Offset` to apply a random, high-frequency movement.
2. Apply a small shake (e.g., 0.1s, mag 3) when **Smelting/Compositing** finishes.
3. Apply a medium shake (e.g., 0.3s, mag 5) on **Simulation SUCCESS/FAIL** to emphasize the result.

#### Particle Effects (VFX) (7h - 8h)

1. Create and integrate final particle systems for key events:
    - **Resource Collection:** Small, bright particles that fly from the resource node to the player's position on harvest.
    - **Smelting:** Plumes of smoke/heat that puff out when the Furnace timer resets.

#### UI Feedback & Responsiveness (8h - 9h)

1. Implement simple color transitions/flashes for successful actions:
    - When an item is added to the inventory, the corresponding inventory label should briefly flash **Green**.
    - When a research node is unlocked, the **Tech Points** label that paid for it should briefly flash **Red** (to show points spent).

#### Machine Interaction Prompts (9h - 10h)

1. Refine all interaction prompts (`[E] Deposit Ore`, `[F] Activate Portal`) to use a clean font, have a small background panel, and fade in/out smoothly when the player enters/exits the machine's interaction range.

### Task 4: Win State Polish and Final Review (6 Hours)

#### Portal Activation Sequence (10h - 11h)

1. Enhance the `Portal Activation` transition (from Sprint 11):
    - When "Activate Portal" is pressed, the screen should immediately fade to white (using a `ColorRect` and `AnimationPlayer`).
    - Play the final, powerful sound effect (energy surge).
2. This provides a dramatic climax before the `WinScene`.

#### Simulation Report Readability (11h - 12h)

1. Review the `SimulationReportUI.tscn` (from Sprint 11). Ensure the **Red/Green** color coding is highly visible and the font size for the **FINAL STATUS** is large and commanding.

#### VS Demo Walkthrough Tuning (12h - 13h)

1. Conduct a full, timed walkthrough of the entire Vertical Slice (Start $\rightarrow$ Gather $\rightarrow$ Analyze $\rightarrow$ Engineer $\rightarrow$ Simulate $\rightarrow$ Win).
2. Adjust game pacing (e.g., harvest speeds, smelt timers, point costs) to ensure the demo takes a suitable duration (e.g., 10-15 minutes of continuous play).

#### Performance Check (13h - 14h)

1. Run the profiler in Godot. Ensure no major performance bottlenecks exist, especially with particle systems or complex UI updates. Optimize any identified slow spots.

#### Final Code and Comment Review (14h - 15h)

1. Ensure all temporary debug code, placeholder logic (like the simplified Gas Siphon), and `GD.Print` calls are removed or commented out.
2. Review all C\# scripts one last time for clarity and consistency.

#### Final Commit (15h - 16h)

1. **Tag Repository:** Create a final Git tag (e.g., `VS_1.0_Final`).
2. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 12 Complete: Aesthetic Polish, Juiciness, Final Audio/Art Integration, and Vertical Slice Finalized."
