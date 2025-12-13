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

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Project Setup and Godot Configuration | 120 | 10 |
| 2: Data Structure - Resource Property | 120 | 10 |
| 3: Data Structure - Raw Resource | 120 | 10 |
| 4: Data Structure - Composite Material | 120 | 15 |
| 5: Data Structure - Planetary Constants | 120 | 5 |
| 6: Acquire Placeholder Tileset and Sprite Sheets| 120 | 15 |
| 7: Data Structure - Portal Requirement | 120 | 20 |
| 8: Simple Unit Test and Review | 120 | 35 |
| **Total** | **16 hrs** | **2 hrs** |

-----

## Task Breakdown (16 Hours)

### Task 1: Project Setup and Godot Configuration (2 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Create and Configure Godot Project**<br>1. Create a new Godot 4 project using the **C\#** template.<br>2. Set the renderer to **2D** or **Compatibility** (since we are using 2D for the VS).<br>3. Create a top-level folder named `Scripts` to hold all C\# files.<br>4. Create a main scene (`World.tscn`) and save it. |
| **1h - 2h** | **Editor Workflow Check**<br>1. Open the project in your IDE (e.g., Visual Studio Code or Rider).<br>2. Verify that the Godot C\# solution file (`.sln`) is loaded correctly and that C\# scripts can be added and compiled without error. *Troubleshoot any .NET SDK path issues now.* |

### Task 2: Data Structure - Resource Property (2 Hours)

This class defines a single measurable quality (e.g., "Strength") and its value.

| Duration | Steps |
| :--- | :--- |
| **2h - 3h** | **Create Enumerator and Base Class**<br>1. In the `Scripts` folder, create `Enums.cs`. Define `enum ResourcePropertyType { Strength, Resistance, Toughness, Conductivity, Reactivity }`.<br>2. Create `ResourceProperty.cs`. Define a C\# `struct` named `ResourceProperty` (use `struct` for performance and value semantics). |
| **3h - 4h** | **Define Fields and Helper Methods**<br>1. Add the following fields to `ResourceProperty`:<br>    - `public ResourcePropertyType Type;`<br>    - `public float Value;`<br>    - `public string VagueDescription;` (e.g., "High Integrity")<br>2. Implement a constructor `public ResourceProperty(ResourcePropertyType type, float value)` that sets `Type` and `Value`.<br>3. Implement a private method `SetVagueDescription()` inside the constructor: `if (Value > 7.0f) { VagueDescription = "High"; } else if (Value < 3.0f) { VagueDescription = "Low"; } else { VagueDescription = "Medium"; }` |

### Task 3: Data Structure - Raw Resource (2 Hours)

This is the data object for all gatherable materials in the game.

| Duration | Steps |
| :--- | :--- |
| **4h - 5h** | **Create RawResource Class**<br>1. Create `RawResource.cs`. Define a C\# class `RawResource`.<br>2. Add core descriptive fields:<br>    - `public string Name;` (e.g., "Dull Grey Ore")<br>    - `public string Description;`<br>    - `public int BaseHarvestDifficulty;` (1 for VS). |
| **5h - 6h** | **Implement Property Storage**<br>1. Add the crucial storage field: `public Dictionary<ResourcePropertyType, ResourceProperty> Properties = new();`<br>2. Implement a simple stub method `public void GenerateProperties(int seed)`:<br>    - For the VS, hardcode the properties of one "Low Strength Ore" and one "High Strength Ore" using a simple random or fixed seed logic. (e.g., Low Strength Ore: Strength=2.0, Resistance=5.0. High Strength Ore: Strength=8.0, Resistance=2.0). |

### Task 4: Data Structure - Composite Material (2 Hours)

This handles the combining of materials, the core of the engineering puzzle.

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Composite Class Definition**<br>1. Create `CompositeMaterial.cs`. This class should inherit from `RawResource`.<br>2. Add tracking fields for its inputs:<br>    - `public RawResource PrimaryIngredient;`<br>    - `public RawResource ModifierIngredient;`<br>    - `public float StrengthModifier;` (to track the effect of gas/additive). |
| **7h - 8h** | **Implement Calculation Logic**<br>1. Implement a method `public void CalculateProperties()`.<br>2. **VS Logic:** The Composite's final Strength will be the sum of its ingredients' strength, plus a multiplier from a gas.<br>   - `float baseStrength = PrimaryIngredient.Properties[Strength].Value + ModifierIngredient.Properties[Strength].Value;`<br>   - `float finalStrength = baseStrength * StrengthModifier;`<br>3. Set the Composite's Strength property using this calculated value. |

### Task 5: Data Structure - Planetary Constants (2 Hours)

This defines the unique puzzle for the current run (the "Puzzle Frame").

| Duration | Steps |
| :--- | :--- |
| **8h - 9h** | **Planetary Constants Class**<br>1. Create `PlanetaryConstants.cs`. Define a C\# class `PlanetaryConstants`.<br>2. Add float fields for the 3 key VS properties (from the GDD):<br>    - `public float GravimetricShear;` (Range 0.5 - 5.0)<br>    - `public float CorrosiveIndex;` (Range 0.0 - 14.0)<br>    - `public float TectonicVolatility;` (Range 0.0 - 9.0) |
| **9h - 10h** | **World Generation Logic**<br>1. Implement `public void GenerateWorld(int complexityLevel)`.<br>2. For the VS, hardcode the Level 1 values for the first playthrough:<br>    - `GravimetricShear = 3.2f;` (High but manageable)<br>    - `CorrosiveIndex = 2.0f;` (Low)<br>    - `TectonicVolatility = 1.0f;` (Low)<br>3. This ensures every test run uses the same baseline puzzle. |

### Task 6: Acquire Placeholder Tileset and Sprite Sheets (2 Hours)

Begin the visual design process to guide future art sprints.

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **Placeholder Tileset**<br>1. Find tileset and add it to Godot project. |
| **11h - 12h**| **Placeholder Sprite Sheets**<br>1. Find sprite sheet(s) for the player character (and other moving things?) and add it/them to Godot project. |

### Task 7: Data Structure - Portal Requirement (2 Hours)

This defines the ultimate goal and the mathematical deduction.

| Duration | Steps |
| :--- | :--- |
| **12h - 13h**| **Portal Requirement Class**<br>1. Create `PortalRequirement.cs`. Define a C\# class `PortalRequirement`.<br>2. Add a dictionary to store the requirements: `public Dictionary<ResourcePropertyType, float> RequiredStats = new();`<br>3. Add fields to store the World Constants (for reference): `public PlanetaryConstants WorldContext;` |
| **13h - 14h**| **Deduction Formula Implementation**<br>1. Implement `public void SetRequirements(PlanetaryConstants constants)`:<br>2. **VS Logic:** Implement the primary deduction formula from the GDD:<br>    - **Foundation Strength:** `RequiredStats[Strength] = constants.GravimetricShear * 2.5f;` (Target: 8.0)<br>    - **Gate Resistance:** `RequiredStats[Resistance] = constants.CorrosiveIndex * 1.5f;` (Target: 3.0)<br>3. This ensures the requirements are generated *from* the world properties. |

### Task 8: Simple Unit Test and Review (2 Hours)

Verify the core deduction math works outside of the Godot environment.

| Duration | Steps |
| :--- | :--- |
| **14h - 15h**| **Unit Test Project (C\#/.NET)**<br>1. Create a separate C\# Console Application project in the solution (e.g., `Wayblazer.Tests`).<br>2. Reference the main Godot C\# assembly (`Wayblazer`).<br>3. Write a single function `TestPortalDeductionMath()`: |
| | a. Instantiate `PlanetaryConstants` and call `GenerateWorld()`.<br>b. Instantiate `PortalRequirement` and call `SetRequirements()`.<br>c. Instantiate a `CompositeMaterial` and manually set its properties to a known "passing" value (e.g., Strength 9.0) and a "failing" value (e.g., Strength 7.0).<br>d. Use a simple `if` check (`if (composite.Properties[Strength].Value > required.RequiredStats[Strength])`) to confirm the pass/fail logic is mathematically correct. |
| **15h - 16h**| **Code Review and Cleanup**<br>1. Review all C\# code for consistent naming conventions and commenting.<br>2. Delete the temporary Console Test project if it's no longer needed, or commit it as a verification tool.<br>3. **Commit Code:** Commit all changes to the Version Control System (VCS) with the message: "Sprint 1 Complete: Initial Data Core Architecture." |

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

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Godot TileMap and Tileset Setup | 180 | 20 |
| 2: World Generation and Rendering | 180 | 20 |
| 3: Player Controller and Scene Setup | 240 | |
| 4: Resource Node Linking | 180 | |
| 5: Aesthetic & Review | 180 | |
| **Total** | **16 hrs** | **40 mins** |

-----

## Task Breakdown (16 Hours)

### Task 1: Godot TileMap and Tileset Setup (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **TileMap Node Setup**<br>1. In `World.tscn`, add a new `TileMap` node. Rename it to `WorldMap`.<br>2. In the `TileMap` inspector, set the **Cell Size** to a square size appropriate for isometric/2D grid (e.g., **64x64** or **32x32**).<br>3. Set the **Y Sort Enabled** property to **true** in the `WorldMap` node's transform settings. (Crucial for 2.5D depth illusion). |
| **1h - 2h** | **Create Basic Tileset**<br>1. In the `TileMap` inspector, click the **Tile Set** property to create a **New TileSet** resource.<br>2. Open the TileSet editor.<br>3. Add **4 new Atlas sources** (use simple colored squares or the concept art from Sprint 1):<br>    - `ID 0`: **Ground** (e.g., Green/Brown square).<br>    - `ID 1`: **Ore Node** (e.g., Grey square).<br>    - `ID 2`: **Wood Node** (e.g., Dark Green/Brown square).<br>    - `ID 3`: **Water** (e.g., Blue square). |
| **2h - 3h** | **Tilemap Rendering Script Stub**<br>1. Create a new C\# script `WorldGenerator.cs` (Godot script attached to `WorldMap` node).<br>2. Add a reference to the TileMap node: `private TileMap _tileMap;`<br>3. In `_Ready()`, get the node: `_tileMap = GetNode<TileMap>(".");`<br>4. Define a constant for world size: `const int WorldSize = 64;` (64x64 tiles for VS). |

### Task 2: World Generation and Rendering (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **ProcGen V1: Simple World Array**<br>1. In `WorldGenerator.cs`, define a private 2D array: `private int[,] _worldData = new int[WorldSize, WorldSize];`<br>2. Implement a stub method `GenerateInitialWorld()`:<br>    - Fill the entire array with `ID 0` (Ground).<br>    - Use `GD.RandRange(0, 100)` to randomly place `ID 1` (Ore) and `ID 2` (Wood) nodes at a density of about 5% each. |
| **4h - 5h** | **Render World to TileMap**<br>1. Implement a method `RenderWorld()`:<br>    - Loop through `_worldData` from `x = 0` to `WorldSize` and `y = 0` to `WorldSize`.<br>    - Use `_tileMap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(_worldData[x, y], 0));` (Note: Ensure the source IDs match your TileSet setup).<br>2. Call `GenerateInitialWorld()` and then `RenderWorld()` in `_Ready()`. |
| **5h - 6h** | **World Context Link**<br>1. Create a `GameManager.cs` C\# class (make it a singleton via `[GlobalClass]` for easy access).<br>2. In `GameManager.cs`, create and initialize the C\# data objects from Sprint 1: `public PlanetaryConstants Constants = new();` and `public PortalRequirement Required = new();`<br>3. In `_Ready()` of `GameManager.cs`, call `Constants.GenerateWorld(1)` and `Required.SetRequirements(Constants)` to load the VS puzzle. |

### Task 3: Player Controller and Scene Setup (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Player Scene Setup**<br>1. In the Godot editor, click **Scene > New Scene** from the menu.<br>2. Click **Other Node** in the Create Root Node dialog, search for **CharacterBody2D**, and select it.<br>3. With the root node selected, click the **Rename** icon (or press F2) and rename it to **Player**.<br>4. Click **Scene > Save Scene** and save as `Player.tscn` in the `Scenes` folder.<br>5. Right-click the **Player** node and select **Add Child Node**. Search for and add a **Sprite2D** node.<br>6. In the Sprite2D's Inspector, drag a placeholder player texture (from Sprint 1) into the **Texture** property.<br>7. Right-click the **Player** node again and add a **CollisionShape2D** child node.<br>8. In the CollisionShape2D's Inspector, click the **Shape** property dropdown and select **New RectangleShape2D**.<br>9. Click the newly created RectangleShape2D to expand its properties, and adjust the **Size** to roughly match your character sprite (e.g., Vector2(32, 48) for a 32x48 pixel character).<br>10. Open `World.tscn`, right-click the root node, select **Instantiate Child Scene**, and choose `Player.tscn`. Position it in the center of the visible area. |
| **7h - 8h** | **Basic Movement Script**<br>1. Select the **Player** root node in `Player.tscn`.<br>2. In the Inspector, scroll to the top and click the **Attach Script** icon (looks like a scroll with a + sign).<br>3. In the Attach Node Script dialog, ensure the **Language** is set to **C#**, and the **Path** is set to `Scripts/PlayerController.cs`. Click **Create**.<br>4. In your IDE (VS Code/Rider), open the newly created `PlayerController.cs` file.<br>5. Replace the default code with this beginner-friendly movement template:<br><br>```csharp<br>using Godot;<br>using System;<br><br>public partial class PlayerController : CharacterBody2D<br>{<br>    // Movement speed in pixels per second<br>    private const float Speed = 200.0f;<br><br>    // Called every physics frame (60 times per second by default)<br>    public override void _PhysicsProcess(double delta)<br>    {<br>        // Get input direction using Godot's built-in UI actions<br>        // This returns a normalized Vector2 (-1 to 1 on each axis)<br>        Vector2 inputDirection = Input.GetVector(<br>            "ui_left",   // A key or Left Arrow<br>            "ui_right",  // D key or Right Arrow<br>            "ui_up",     // W key or Up Arrow<br>            "ui_down"    // S key or Down Arrow<br>        );<br><br>        // Set the velocity based on input and speed<br>        Velocity = inputDirection * Speed;<br><br>        // Apply the movement and handle collisions<br>        MoveAndSlide();<br>    }<br>}<br>```<br><br>6. Save the file. Build the project by pressing **Build** in Godot's top menu (or press F6).<br>7. Fix any compilation errors before proceeding. |
| **8h - 9h** | **Movement Testing and Collision**<br>1. Press **F5** to run the game (or click the Play button). You should see your player character.<br>2. Test movement using WASD or arrow keys. The character should move smoothly in all four directions.<br>3. **Diagonal Movement Check:** Press two keys at once (e.g., W+D). The character should move diagonally. If the movement feels too fast diagonally, add this code after the `inputDirection` line:<br><br>```csharp<br>// Normalize diagonal movement to prevent faster diagonal speed<br>inputDirection = inputDirection.Normalized();<br>```<br><br>4. **Setting Up TileMap Collisions** (if not done in Task 1):<br>    - Open `World.tscn` and select the **WorldMap** TileMap node.<br>    - Click the **TileSet** property in the Inspector to open the TileSet editor at the bottom.<br>    - Select your Ground tile. In the middle panel, click the **Physics** tab.<br>    - Click **Add Physics Layer** if no physics layer exists.<br>    - Use the polygon tool to draw a collision shape around the tile (usually a rectangle for ground tiles).<br>    - Save and test. The player should not be able to walk through tiles with collision enabled. |
| **9h - 10h**| **Camera and Y-Sort Setup**<br>1. In `Player.tscn`, right-click the **Player** root node and select **Add Child Node**.<br>2. Search for and add a **Camera2D** node.<br>3. With the Camera2D selected, check these properties in the Inspector:<br>    - **Enabled:** ON (checked)<br>    - **Position Smoothing > Enabled:** ON (for smooth camera tracking)<br>    - **Position Smoothing > Speed:** 5.0 (adjust for desired smoothness)<br>4. **Implementing Y-Sort for 2.5D Depth:**<br>    - In `PlayerController.cs`, add this line inside `_PhysicsProcess`, after `MoveAndSlide()`:<br><br>```csharp<br>// Update Z-index based on Y position for proper 2.5D layering<br>// Objects lower on screen (higher Y value) should draw on top<br>ZIndex = (int)Position.Y;<br>```<br><br>5. **Alternative Y-Sort Method** (for Godot 4.x with Y-Sort enabled on nodes):<br>    - Select the **WorldMap** TileMap node in `World.tscn`.<br>    - In the Inspector, under **Ordering**, set **Y Sort Enabled** to **ON**.<br>    - Do the same for the **Player** node.<br>    - This automatically handles Y-sorting without manual Z-index updates.<br>6. **Testing Y-Sort:**<br>    - Place a test sprite or tree in the world with a higher Y position than the player.<br>    - Walk the player behind it (move up). The player should be drawn behind the tree.<br>    - Walk the player in front of it (move down). The player should be drawn in front.<br>7. Your complete `PlayerController.cs` should now look like this:<br><br>```csharp<br>using Godot;<br>using System;<br><br>public partial class PlayerController : CharacterBody2D<br>{<br>    private const float Speed = 200.0f;<br><br>    public override void _PhysicsProcess(double delta)<br>    {<br>        // Get input direction from keyboard/gamepad<br>        Vector2 inputDirection = Input.GetVector(<br>            "ui_left", "ui_right", "ui_up", "ui_down"<br>        );<br><br>        // Set velocity based on input<br>        Velocity = inputDirection * Speed;<br><br>        // Move the character and handle collisions<br>        MoveAndSlide();<br><br>        // Update Z-index for proper 2.5D depth sorting<br>        ZIndex = (int)Position.Y;<br>    }<br>}<br>```

### Task 4: Resource Node Linking (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **Resource Node Scene Creation**<br>1. Click **Scene > New Scene** in Godot.<br>2. Select **Other Node** and search for **Area2D**. This will be our interactive resource node.<br>3. Rename the root node to **ResourceNode** and save the scene as `ResourceNode.tscn` in the `Scenes` folder.<br>4. Add a **Sprite2D** child node to display the resource visual.<br>5. Add a **CollisionShape2D** child node. Set its **Shape** to **New CircleShape2D** (for a circular interaction area).<br>6. Click the CircleShape2D to expand it, and set **Radius** to about 32 pixels.<br>7. **Create the C# Script:**<br>    - Select the **ResourceNode** root node.<br>    - Click the **Attach Script** icon.<br>    - Set the path to `Scripts/ResourceNode.cs` and click **Create**.<br>8. In your IDE, open `ResourceNode.cs` and replace the default code:<br><br>```csharp<br>using Godot;<br>using System;<br><br>public partial class ResourceNode : Area2D<br>{<br>    // Reference to the resource data (from Sprint 1)<br>    // This will hold all the properties of this specific resource<br>    public RawResource ResourceData { get; set; }<br><br>    // Visual representation of the resource<br>    private Sprite2D _sprite;<br><br>    public override void _Ready()<br>    {<br>        // Get reference to the Sprite2D child<br>        _sprite = GetNode<Sprite2D>("Sprite2D");<br><br>        // Add this node to the "Resources" group for easy finding<br>        AddToGroup("Resources");<br><br>        // Optional: Set sprite based on resource type<br>        if (ResourceData != null)<br>        {<br>            // You can add logic here to change sprite based on<br>            // ResourceData.Name (e.g., different ore types)<br>            GD.Print($"Resource node created: {ResourceData.Name}");<br>        }<br>    }<br><br>    // This method will be called when the player interacts with this node<br>    public void Harvest()<br>    {<br>        GD.Print($"Harvesting {ResourceData.Name}");<br>        // Play sound effect, spawn particles, etc.<br>        QueueFree(); // Remove this node from the scene<br>    }<br>}<br>```<br><br>9. Build the project (F6) to ensure it compiles. |
| **11h - 12h**| **Instantiate Resource Nodes During World Generation**<br>1. Open `WorldGenerator.cs` in your IDE.<br>2. At the top of the class, add a reference to the ResourceNode scene:<br><br>```csharp<br>// Scene to instantiate for resources<br>private PackedScene _resourceNodeScene;<br>```<br><br>3. In the `_Ready()` method, load the scene:<br><br>```csharp<br>public override void _Ready()<br>{<br>    _tileMap = GetNode<TileMap>(".");<br>    // Load the ResourceNode scene for instantiation<br>    _resourceNodeScene = GD.Load<PackedScene>("res://Scenes/ResourceNode.tscn");<br>    <br>    GenerateInitialWorld();<br>    RenderWorld();<br>}<br>```<br><br>4. Modify the `RenderWorld()` method to instantiate ResourceNode objects:<br><br>```csharp<br>private void RenderWorld()<br>{<br>    for (int x = 0; x < WorldSize; x++)<br>    {<br>        for (int y = 0; y < WorldSize; y++)<br>        {<br>            int tileId = _worldData[x, y];<br>            <br>            // Place the tile<br>            _tileMap.SetCell(0, new Vector2I(x, y), 0, <br>                new Vector2I(tileId, 0));<br><br>            // If this is an Ore (ID 1) or Wood (ID 2) tile,<br>            // spawn a ResourceNode<br>            if (tileId == 1 || tileId == 2)<br>            {<br>                // Instantiate the ResourceNode<br>                ResourceNode node = _resourceNodeScene.Instantiate<ResourceNode>();<br>                <br>                // Convert grid position to world position<br>                // Multiply by tile size (adjust if your tiles are different size)<br>                Vector2 worldPos = new Vector2(x * 64, y * 64);<br>                node.Position = worldPos;<br>                <br>                // Create and assign resource data<br>                RawResource resource = new RawResource();<br>                if (tileId == 1) // Ore<br>                {<br>                    resource.Name = "Base Ore";<br>                    resource.GenerateProperties(x + y); // Use position as seed<br>                }<br>                else // Wood<br>                {<br>                    resource.Name = "Soft Wood";<br>                    resource.GenerateProperties(x * y);<br>                }<br>                <br>                node.ResourceData = resource;<br>                <br>                // Add to scene tree<br>                AddChild(node);<br>            }<br>        }<br>    }<br>}<br>```<br><br>5. Save and build the project. Run it to see resource nodes spawning on Ore and Wood tiles. |
| **12h - 13h**| **Player Resource Detection and Interaction**<br>1. Open `PlayerController.cs` in your IDE.<br>2. Add a field to track the nearby resource:<br><br>```csharp<br>// Track the resource node the player is currently near<br>private ResourceNode _nearbyResource = null;<br>```<br><br>3. Add a method to check for nearby resources:<br><br>```csharp<br>// Check if player is near any resource nodes<br>private void CheckForResourceNode()<br>{<br>    // Clear previous nearby resource<br>    _nearbyResource = null;<br>    <br>    // Get all nodes in the "Resources" group<br>    var resources = GetTree().GetNodesInGroup("Resources");<br>    <br>    // Check each resource to see if player is close enough<br>    foreach (Node node in resources)<br>    {<br>        if (node is ResourceNode resourceNode)<br>        {<br>            // Calculate distance between player and resource<br>            float distance = Position.DistanceTo(resourceNode.Position);<br>            <br>            // If within interaction range (50 pixels)<br>            if (distance < 50.0f)<br>            {<br>                _nearbyResource = resourceNode;<br>                break; // Found a nearby resource, stop searching<br>            }<br>        }<br>    }<br>}<br>```<br><br>4. Add input handling for harvesting:<br><br>```csharp<br>// Handle harvesting input<br>private void HandleHarvestInput()<br>{<br>    // Check if player presses the interact key (E) or left mouse button<br>    if (Input.IsActionJustPressed("ui_accept") || <br>        Input.IsActionJustPressed("ui_select"))<br>    {<br>        if (_nearbyResource != null)<br>        {<br>            // Harvest the resource<br>            _nearbyResource.Harvest();<br>            GD.Print($"Harvested: {_nearbyResource.ResourceData.Name}");<br>        }<br>    }<br>}<br>```<br><br>5. Call these methods in `_PhysicsProcess`:<br><br>```csharp<br>public override void _PhysicsProcess(double delta)<br>{<br>    // Existing movement code...<br>    Vector2 inputDirection = Input.GetVector(<br>        "ui_left", "ui_right", "ui_up", "ui_down"<br>    );<br>    Velocity = inputDirection * Speed;<br>    MoveAndSlide();<br>    ZIndex = (int)Position.Y;<br>    <br>    // New resource interaction code<br>    CheckForResourceNode();<br>    HandleHarvestInput();<br>}<br>```<br><br>6. **Optional:** Add visual feedback by displaying a prompt. Add this at the end of `_PhysicsProcess`:<br><br>```csharp<br>// Display interaction prompt (you'll create the UI in Sprint 3)<br>if (_nearbyResource != null)<br>{<br>    // For now, just print to console<br>    GD.Print($"Near: {_nearbyResource.ResourceData.Name} - Press E to harvest");<br>}<br>```<br><br>7. Test the game: Walk near resource nodes and press E (or Enter) to harvest them. They should disappear and print messages to the console. |

### Task 5: Aesthetic & Review (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **13h - 14h**| **Art/Sound: Final Asset Integration**<br>1. Replace the placeholder colored squares for the Player, Ground, Ore, and Wood tiles with the final art assets developed in Sprint 1/2 concept tasks.<br>2. Add simple **footstep sound effects** to the PlayerController (e.g., play a sound when velocity is non-zero). |
| **14h - 15h**| **Debugging and Visual Check**<br>1. Run the game. Verify the 64x64 map loads.<br>2. Check Y-Sorting: If the player walks *behind* a tree (a static sprite placed on a higher Y-coordinate), the player must be visually occluded by it. If they walk in front of it (lower Y-coordinate), they must be visible.<br>3. Verify the debug message from Task 4 (e.g., "Over: Dull Grey Ore") correctly displays the *specific* resource linked to the instantiated node. |
| **15h - 16h**| **Review and Commit**<br>1. Review all scripts for clean C\# syntax and Godot C\# best practices.<br>2. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 2 Complete: Grid World, Player Movement, and Resource Voxel Linking." |

# ⛏️ Sprint 3: Resource Engine & ProcGen V1 (16 Hours)

## Summary

This sprint fully integrates the procedural data architecture (Sprint 1) with the physical world (Sprint 2). We will finalize the resource generation, implement the core harvesting mechanic, and build the foundational inventory manager and HUD necessary for the primary gameplay loop.

## 🎯 Goal

The player can successfully land, harvest one type of wood and two types of ore, see the resources added to a working inventory, and get the first hints of the deduction puzzle via the Hand Scanner.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Singleton management, Input handling, UI integration.
  * **C\# / .NET:** Random number generation for resource properties, Inventory Management class.

#### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Singleton and Resource Initialization | 180 | |
| 2: Inventory Manager Implementation | 180 | |
| 3: Basic HUD and Inventory UI | 240 | |
| 4: Hand Scanner and Vague Deduction | 180 | |
| 5: Save/Load System | 180 | |
| 6: Aesthetic & Review | 120 | |
| **Total** | **18 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Singleton and Resource Initialization (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **GameManager Singleton Setup (Autoload)**<br>1. Create a new C# script file called `GameManager.cs` in the `Scripts` folder.<br>2. Implement the singleton pattern:<br><br>```csharp<br>using Godot;<br>using System;<br><br>// Autoload singleton that manages game state<br>public partial class GameManager : Node<br>{<br>    // Singleton instance<br>    public static GameManager Instance { get; private set; }<br>    <br>    // Core game data<br>    public PlanetaryConstants Constants { get; private set; }<br>    public PortalRequirement Required { get; private set; }<br>    <br>    // Static accessors for easy access from anywhere<br>    public static PlanetaryConstants WorldConstants => Instance.Constants;<br>    public static PortalRequirement PortalGoal => Instance.Required;<br>    <br>    public override void _EnterTree()<br>    {<br>        // Set up singleton instance<br>        if (Instance == null)<br>        {<br>            Instance = this;<br>        }<br>        else<br>        {<br>            // Prevent duplicate singletons<br>            QueueFree();<br>        }<br>    }<br>    <br>    public override void _Ready()<br>    {<br>        // Initialize world data<br>        Constants = new PlanetaryConstants();<br>        Constants.GenerateWorld(1); // Complexity level 1 for VS<br>        <br>        Required = new PortalRequirement();<br>        Required.SetRequirements(Constants);<br>        <br>        GD.Print($"World Generated: Gravity={Constants.GravimetricShear}, " +<br>                 $"Corrosion={Constants.CorrosiveIndex}");<br>        GD.Print($"Portal Requires: Strength={Required.RequiredStats[ResourcePropertyType.Strength]}");<br>    }<br>}<br>```<br><br>3. **Register as Autoload:**<br>    - In Godot, go to **Project > Project Settings**.<br>    - Click the **Autoload** tab.<br>    - Click the folder icon next to **Path** and navigate to `Scripts/GameManager.cs`.<br>    - Set **Node Name** to "GameManager".<br>    - Check **Enable** and click **Add**.<br>    - Close Project Settings.<br>4. Build the project (F6) and run it. You should see the initialization messages in the Output console. |
| **1h - 2h** | **Enhanced Resource Property Generation**<br>1. Open `RawResource.cs` (from Sprint 1) in your IDE.<br>2. Refine the `GenerateProperties` method to use proper randomization:<br><br>```csharp<br>// Generate random properties for this resource using a seed<br>public void GenerateProperties(int seed)<br>{<br>    // Create a random number generator with the seed<br>    // This ensures the same seed always produces the same properties<br>    RandomNumberGenerator rng = new RandomNumberGenerator();<br>    rng.Seed = (ulong)seed;<br>    <br>    // Generate a value for each property type<br>    foreach (ResourcePropertyType propType in Enum.GetValues(typeof(ResourcePropertyType)))<br>    {<br>        // Generate a random value between 1.0 and 10.0<br>        float value = rng.RandfRange(1.0f, 10.0f);<br>        <br>        // Create a ResourceProperty and add it to the dictionary<br>        ResourceProperty property = new ResourceProperty(propType, value);<br>        Properties[propType] = property;<br>    }<br>    <br>    GD.Print($"Generated {Name}: Strength={Properties[ResourcePropertyType.Strength].Value:F1}");<br>}<br>```<br><br>3. **Add Harvest Method Tracking:**<br>    - According to MECHANICS_DETAILS.md, different resources require different acquisition methods.<br>    - Add a new field to `RawResource.cs`:<br><br>```csharp<br>// The method required to harvest this resource (from MECHANICS_DETAILS.md)<br>public string RequiredHarvestMethod { get; set; } = "Kinetic Mining";<br>```<br><br>4. Build and test. Run the game and verify that resources generate random but consistent properties. |
| **2h - 3h** | **Create Hardcoded VS Resource Presets**<br>1. Create a new file `VSResourcePresets.cs` in the `Scripts` folder:<br><br>```csharp<br>using Godot;<br>using System;<br><br>// Hardcoded resource definitions for the Vertical Slice<br>// These ensure consistent, predictable gameplay for testing<br>public static class VSResourcePresets<br>{<br>    // Base Ore: Low strength, not enough for portal<br>    public static RawResource CreateBaseOre()<br>    {<br>        RawResource ore = new RawResource<br>        {<br>            Name = "Base Ore",<br>            Description = "A dull grey metallic ore with low structural integrity.",<br>            BaseHarvestDifficulty = 1,<br>            RequiredHarvestMethod = "Kinetic Mining" // Standard mining<br>        };<br>        <br>        // Manually set properties for predictable VS gameplay<br>        ore.Properties[ResourcePropertyType.Strength] = <br>            new ResourceProperty(ResourcePropertyType.Strength, 2.0f);<br>        ore.Properties[ResourcePropertyType.Resistance] = <br>            new ResourceProperty(ResourcePropertyType.Resistance, 5.0f);<br>        ore.Properties[ResourcePropertyType.Toughness] = <br>            new ResourceProperty(ResourcePropertyType.Toughness, 3.0f);<br>        ore.Properties[ResourcePropertyType.Conductivity] = <br>            new ResourceProperty(ResourcePropertyType.Conductivity, 4.0f);<br>        ore.Properties[ResourcePropertyType.Reactivity] = <br>            new ResourceProperty(ResourcePropertyType.Reactivity, 6.0f);<br>        <br>        return ore;<br>    }<br>    <br>    // Catalyst Ore: Medium strength, adds modifier<br>    public static RawResource CreateCatalystOre()<br>    {<br>        RawResource ore = new RawResource<br>        {<br>            Name = "Catalyst Ore",<br>            Description = "A shimmering purple ore with reactive properties.",<br>            BaseHarvestDifficulty = 2,<br>            RequiredHarvestMethod = "Gas Siphoning" // Requires advanced tool<br>        };<br>        <br>        // Higher strength - will provide +4.0 boost when used in compositing<br>        ore.Properties[ResourcePropertyType.Strength] = <br>            new ResourceProperty(ResourcePropertyType.Strength, 4.0f);<br>        ore.Properties[ResourcePropertyType.Resistance] = <br>            new ResourceProperty(ResourcePropertyType.Resistance, 2.0f);<br>        ore.Properties[ResourcePropertyType.Toughness] = <br>            new ResourceProperty(ResourcePropertyType.Toughness, 2.5f);<br>        ore.Properties[ResourcePropertyType.Conductivity] = <br>            new ResourceProperty(ResourcePropertyType.Conductivity, 8.0f);<br>        ore.Properties[ResourcePropertyType.Reactivity] = <br>            new ResourceProperty(ResourcePropertyType.Reactivity, 9.0f);<br>        <br>        return ore;<br>    }<br>    <br>    // Soft Wood: Low difficulty, used as fuel<br>    public static RawResource CreateSoftWood()<br>    {<br>        RawResource wood = new RawResource<br>        {<br>            Name = "Soft Wood",<br>            Description = "Lightweight timber suitable for burning.",<br>            BaseHarvestDifficulty = 1,<br>            RequiredHarvestMethod = "Kinetic Mining" // Can be chopped normally<br>        };<br>        <br>        wood.Properties[ResourcePropertyType.Strength] = <br>            new ResourceProperty(ResourcePropertyType.Strength, 1.0f);<br>        wood.Properties[ResourcePropertyType.Resistance] = <br>            new ResourceProperty(ResourcePropertyType.Resistance, 1.5f);<br>        wood.Properties[ResourcePropertyType.Toughness] = <br>            new ResourceProperty(ResourcePropertyType.Toughness, 2.0f);<br>        wood.Properties[ResourcePropertyType.Conductivity] = <br>            new ResourceProperty(ResourcePropertyType.Conductivity, 0.5f);<br>        wood.Properties[ResourcePropertyType.Reactivity] = <br>            new ResourceProperty(ResourcePropertyType.Reactivity, 7.0f); // Burns well<br>        <br>        return wood;<br>    }<br>}<br>```<br><br>2. **Update WorldGenerator to Use Presets:**<br>    - Open `WorldGenerator.cs` and modify the `RenderWorld()` method.<br>    - Replace the resource creation code from Sprint 2 with:<br><br>```csharp<br>// In RenderWorld(), where resources are spawned:<br>if (tileId == 1) // Ore<br>{<br>    resource = VSResourcePresets.CreateBaseOre();<br>}<br>else if (tileId == 2) // Wood<br>{<br>    resource = VSResourcePresets.CreateSoftWood();<br>}<br>```<br><br>3. Build and test. Resources should now have consistent, predictable properties. Verify by scanning them with the debug console. |

### Task 2: Inventory Manager Implementation (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Create Inventory Manager Singleton**<br>1. Create a new file `InventoryManager.cs` in the `Scripts` folder.<br>2. Implement the singleton autoload class:<br><br>```csharp<br>using Godot;<br>using System;<br>using System.Collections.Generic;<br><br>// Manages the player's inventory of resources<br>public partial class InventoryManager : Node<br>{<br>    // Singleton instance<br>    public static InventoryManager Instance { get; private set; }<br>    <br>    // Storage: Key = resource name, Value = quantity<br>    public Dictionary<string, int> ResourceCounts { get; private set; }<br>    <br>    // Event fired whenever inventory changes (for UI updates)<br>    public event Action OnInventoryUpdated;<br>    <br>    public override void _EnterTree()<br>    {<br>        if (Instance == null)<br>        {<br>            Instance = this;<br>            ResourceCounts = new Dictionary<string, int>();<br>        }<br>        else<br>        {<br>            QueueFree();<br>        }<br>    }<br>    <br>    public override void _Ready()<br>    {<br>        GD.Print("InventoryManager initialized");<br>    }<br>}<br>```<br><br>3. **Register as Autoload:**<br>    - Go to **Project > Project Settings > Autoload**.<br>    - Add `Scripts/InventoryManager.cs` with name "InventoryManager".<br>    - Click **Add** and close.<br>4. Build the project (F6) to verify compilation. |
| **4h - 5h** | **Implement Add and Remove Methods**<br>1. Open `InventoryManager.cs` and add these methods after `_Ready()`:<br><br>```csharp<br>/// <summary><br>/// Adds an item to the inventory<br>/// </summary><br>/// <param name="resourceName">Name of the resource to add</param><br>/// <param name="amount">Quantity to add (default: 1)</param><br>public void AddItem(string resourceName, int amount = 1)<br>{<br>    // Input validation<br>    if (string.IsNullOrEmpty(resourceName))<br>    {<br>        GD.PrintErr("Cannot add item: resource name is null or empty");<br>        return;<br>    }<br>    <br>    if (amount <= 0)<br>    {<br>        GD.PrintErr($"Cannot add {amount} of {resourceName}: amount must be positive");<br>        return;<br>    }<br>    <br>    // Check if resource already exists in inventory<br>    if (ResourceCounts.ContainsKey(resourceName))<br>    {<br>        // Increment existing count<br>        ResourceCounts[resourceName] += amount;<br>    }<br>    else<br>    {<br>        // Add new entry<br>        ResourceCounts[resourceName] = amount;<br>    }<br>    <br>    // Log for debugging<br>    GD.Print($"Added {amount}x {resourceName}. Total: {ResourceCounts[resourceName]}");<br>    <br>    // Notify UI listeners that inventory has changed<br>    OnInventoryUpdated?.Invoke();<br>}<br><br>/// <summary><br>/// Attempts to remove an item from inventory<br>/// </summary><br>/// <param name="resourceName">Name of the resource to remove</param><br>/// <param name="amount">Quantity to remove</param><br>/// <returns>True if successful, false if insufficient quantity</returns><br>public bool TryRemoveItem(string resourceName, int amount = 1)<br>{<br>    // Check if resource exists<br>    if (!ResourceCounts.ContainsKey(resourceName))<br>    {<br>        GD.PrintErr($"Cannot remove {resourceName}: not in inventory");<br>        return false;<br>    }<br>    <br>    // Check if we have enough<br>    if (ResourceCounts[resourceName] < amount)<br>    {<br>        GD.PrintErr($"Cannot remove {amount}x {resourceName}: " +<br>                   $"only have {ResourceCounts[resourceName]}");<br>        return false;<br>    }<br>    <br>    // Deduct the amount<br>    ResourceCounts[resourceName] -= amount;<br>    <br>    // Remove entry if count reaches zero<br>    if (ResourceCounts[resourceName] <= 0)<br>    {<br>        ResourceCounts.Remove(resourceName);<br>    }<br>    <br>    GD.Print($"Removed {amount}x {resourceName}. " +<br>            $"Remaining: {ResourceCounts.GetValueOrDefault(resourceName, 0)}");<br>    <br>    // Notify UI<br>    OnInventoryUpdated?.Invoke();<br>    return true;<br>}<br><br>/// <summary><br>/// Gets the count of a specific resource<br>/// </summary><br>public int GetItemCount(string resourceName)<br>{<br>    return ResourceCounts.GetValueOrDefault(resourceName, 0);<br>}<br><br>/// <summary><br>/// Checks if the player has at least the specified amount<br>/// </summary><br>public bool HasItem(string resourceName, int amount = 1)<br>{<br>    return GetItemCount(resourceName) >= amount;<br>}<br>```<br><br>2. Build and test that the code compiles without errors. |
| **5h - 6h** | **Integrate Inventory with Resource Harvesting**<br>1. Open `PlayerController.cs` from Sprint 2.<br>2. Modify the `HandleHarvestInput()` method to add resources to inventory:<br><br>```csharp<br>// Handle harvesting input (updated version)<br>private void HandleHarvestInput()<br>{<br>    // Check if player presses the interact key<br>    if (Input.IsActionJustPressed("ui_accept") || <br>        Input.IsActionJustPressed("ui_select"))<br>    {<br>        if (_nearbyResource != null)<br>        {<br>            // Get resource data before harvesting<br>            string resourceName = _nearbyResource.ResourceData.Name;<br>            <br>            // Add to inventory<br>            InventoryManager.Instance.AddItem(resourceName, 1);<br>            <br>            // Harvest (destroys) the resource node<br>            _nearbyResource.Harvest();<br>            <br>            // Clear the reference since node is being destroyed<br>            _nearbyResource = null;<br>        }<br>    }<br>}<br>```<br><br>3. **Testing the Inventory:**<br>    - Run the game (F5).<br>    - Walk near resource nodes and press E to harvest them.<br>    - Watch the Output console for messages like "Added 1x Base Ore. Total: 1".<br>    - Harvest multiple of the same resource and verify the count increments.<br>4. **Optional Debug Command:**<br>    - Add a debug method to InventoryManager to print all items:<br><br>```csharp<br>// Debug method to print entire inventory<br>public void PrintInventory()<br>{<br>    GD.Print("=== INVENTORY ===");<br>    if (ResourceCounts.Count == 0)<br>    {<br>        GD.Print("  (empty)");<br>    }<br>    else<br>    {<br>        foreach (var kvp in ResourceCounts)<br>        {<br>            GD.Print($"  {kvp.Key}: {kvp.Value}");<br>        }<br>    }<br>    GD.Print("=================");<br>}<br>```<br><br>5. Call this from PlayerController by pressing a debug key (e.g., F1):<br><br>```csharp<br>// In _PhysicsProcess, add:<br>if (Input.IsActionJustPressed("ui_home")) // F1 key<br>{<br>    InventoryManager.Instance.PrintInventory();<br>}<br>```<br><br>6. Test: Harvest some resources, press F1, and verify the inventory list appears in the console. |

### Task 3: Basic HUD and Inventory UI (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **HUD Scene Setup**<br>1. Create a new `CanvasLayer` scene named `HUD.tscn`. Add a main `Control` node for layout.<br>2. Create a small panel in the bottom-right for the **Inventory Display**. Use 3 `Label` nodes for the VS key resources (Base Ore, Catalyst Ore, Soft Wood). |
| **7h - 8h** | **Inventory UI Scripting**<br>1. Create a C\# script `InventoryUI.cs` and attach it to the HUD's main panel.<br>2. In `_Ready()`, subscribe to the inventory update event: `InventoryManager.Instance.OnInventoryUpdated += UpdateResourceDisplay;`<br>3. Implement `UpdateResourceDisplay()` to read the counts from `InventoryManager.ResourceCounts` and update the 3 `Label.Text` properties. |
| **8h - 9h** | **Hardcoded Target Display**<br>1. Add a `Label` to the top-right of the HUD for the Portal Goal.<br>2. In `InventoryUI.cs`, grab the hardcoded target Strength (8.0f) from `GameManager.PortalGoal`.<br>3. Set the label text: **"Portal Foundation: Strength ???. (Req: $>8.0$)"**. *The actual required value is shown to the player, but they don't know the material property values yet.* |
| **9h - 10h**| **Art/Sound: UI Assets**<br>1. Create final icons for the 3 VS resources (Base Ore, Catalyst Ore, Soft Wood).<br>2. Create a clean, sci-fi font and theme for the HUD. |

### Task 4: Hand Scanner and Vague Deduction (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **Hand Scanner Input Logic**<br>1. In `PlayerController.cs`, implement the **Right-Click** input action (e.g., using `Input.IsActionJustPressed("RightClick")`).<br>2. When right-click is pressed, check for an overlapping `ResourceNode` (re-use the logic from Sprint 2). |
| **11h - 12h**| **Hand Scanner UI Feedback**<br>1. Create a small overlay UI panel (e.g., top-center) that will only show when the player is scanning.<br>2. When a node is scanned, call `resourceNode.ResourceData.Properties[Strength].VagueDescription;` (e.g., "High Integrity").<br>3. Display the **Vague Description** and **Name** of the resource on the overlay panel for 2 seconds, then hide it. |
| **12h - 13h**| **Vague Deduction Test**<br>1. Run the game and scan the Base Ore (Strength $\approx 2.0$, shows "Low Integrity").<br>2. Scan the Catalyst Ore (Strength $\approx 4.0$, shows "Medium Integrity").<br>3. This confirms the initial deduction loop: *Player observes vague properties and knows "Low" isn't enough for the required "\>8.0".* |

### Task 5: Aesthetic and Review (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **13h - 14h**| **Art/Sound: Harvesting FX**<br>1. Create a simple **"mining" sound effect** (e.g., a metallic impact) to play when the Left-Click/Harvest action is successful.<br>2. Create a simple **VFX** (e.g., small dust particles) to spawn and disappear where the resource node was harvested.<br>3. Create a short, subtle **"scan successful" sound** for the Hand Scanner action. |
| **14h - 15h**| **Code Review and Tuning**<br>1. Review the C\# singleton implementations to ensure no memory leaks or improper initialization order.<br>2. Tune the player's harvesting animation speed and the time it takes to destroy a resource node to feel satisfying (e.g., a 0.5-second hold before the resource is harvested). |
| **15h - 16h**| **Final Playtest and Commit**<br>1. Play the entire loop: Land $\rightarrow$ Scan (get vague data) $\rightarrow$ Harvest Wood $\rightarrow$ Harvest both Ores $\rightarrow$ Check Inventory and HUD.<br>2. Confirm the hardcoded portal target is visible.<br>3. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 3 Complete: Resource Engine, Inventory, and Hand Scanner Deduction." |

# 🖱️ Sprint 4: Interaction & Scanner UI (16 Hours)

## Summary

In this sprint, we enhance the player's primary interaction tool—the **Hand Scanner**—and implement the structure for displaying detailed environmental information. We move the scanning feedback from a temporary debug message (Sprint 3) to a dedicated, persistent UI element, laying the groundwork for the more advanced analysis machines in future sprints.

## 🎯 Goal

The player can effectively use the Hand Scanner (Right Click) to display the **Vague Descriptions** of a resource and the **Planetary Constants** (though still hardcoded to the VS values) in a clean, dedicated UI panel.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** UI design, AnimationPlayer for UI transitions, and input binding.
  * **C\# / .NET:** Data formatting for display, constant access.

#### Time Log

| Task | Estimate (mins) | Actual (mins) |
| --- | --- | --- |
| 1: Dedicated Scanner UI Panel | 180 | |
| 2: Scanner UI Manager Script | 180 | |
| 3: Player Controller Integration | 180 | |
| 4: UI Polish and Animations | 180 | |
| 5: Review and Testing | 240 | |
| **Total** | **16 hrs** | **-** |

-----

## Task Breakdown (16 Hours)

### Task 1: Dedicated Scanner UI Panel (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Scanner UI Scene Setup**<br>1. Create a new `Control` scene named `ScannerUI.tscn`. Add this as a child of the `HUD.tscn` (from Sprint 3).<br>2. Position the panel in a convenient, non-obtrusive location (e.g., top-center of the screen).<br>3. Add a title label: **"SCANNER ANALYSIS"**.<br>4. Add two main sections (sub-`Control` nodes): **Resource Data** and **Planetary Data**. |
| **1h - 2h** | **Resource Data Display**<br>1. In the **Resource Data** section, add five `Label` nodes to display the Vague Descriptions for the 5 core properties:<br>    - `Label_Strength`, `Label_Resistance`, `Label_Toughness`, `Label_Conductivity`, `Label_Reactivity`.<br>2. Give them prefixes: "Strength:", "Resistance:", etc. (The values will be set dynamically). |
| **2h - 3h** | **Planetary Data Display**<br>1. In the **Planetary Data** section, add three `Label` nodes for the VS Planetary Constants (from Sprint 1):<br>    - `Label_Gravity`, `Label_Corrosion`, `Label_Tectonics`.<br>2. Initialize these labels with the current hardcoded VS value (e.g., "Gravity: 3.2 g"). *This acts as a placeholder until the Gravimeter is built.* |

### Task 2: Scanner UI Manager Script (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **ScannerUIManager Script**<br>1. Create a C\# script `ScannerUIManager.cs` and attach it to the `ScannerUI` root node.<br>2. Define public methods: `ShowScanPanel()` and `HideScanPanel()`.<br>3. In `_Ready()`, hide the panel initially: `Visible = false;`. |
| **4h - 5h** | **Update Resource Data Method**<br>1. Implement a public method `UpdateResourceData(RawResource resource)`.<br>2. Inside this method, iterate through the `resource.Properties` dictionary.<br>3. For each property, find the corresponding `Label` (e.g., find `Label_Strength`) and update its text to display the `VagueDescription` (e.g., "Strength: Low"). |
| **5h - 6h** | **Update Planetary Data Method**<br>1. Implement `UpdatePlanetaryData(PlanetaryConstants constants)`.<br>2. Get the current constants from `GameManager.WorldConstants` (even if the `constants` argument isn't used yet).<br>3. Update the three Planetary Data labels using the hardcoded VS values and their units (e.g., `Label_Gravity.Text = $"Gravity: {constants.GravimetricShear:F1} g";`). |

### Task 3: Player Controller Integration (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Player Input Refinement**<br>1. In Godot's Input Map, ensure the **"RightClick"** action is defined.<br>2. In `PlayerController.cs`, modify the Right-Click logic (from Sprint 3) to be **hold-based** rather than **press-based**.<br>3. When `Input.IsActionPressed("RightClick")` is true, perform the scan and keep the panel visible. |
| **7h - 8h** | **Scanner Activation Logic**<br>1. Create a public variable `ScannerUIManager _scannerUI;` and initialize it in `_Ready()`.<br>2. In the scanning section of `_PhysicsProcess(double delta)`:<br>    - If the player is holding Right-Click AND is overlapping a `ResourceNode`:<br>        - Call `_scannerUI.UpdateResourceData(overlappingResource);`<br>        - Call `_scannerUI.UpdatePlanetaryData(...)`<br>        - Call `_scannerUI.ShowScanPanel();`<br>    - ELSE IF the player is NOT holding Right-Click, call `_scannerUI.HideScanPanel();`. |
| **8h - 9h** | **Scan Range Check**<br>1. Refine the logic to ensure the player must be within a short distance of the resource node (e.g., within 20 pixels) to scan it.<br>2. Provide clear visual feedback (e.g., a momentary green glow/highlight) on the resource node that is successfully being scanned. |

### Task 4: UI Polish and Animations (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **9h - 10h**| **Aesthetics: Sci-Fi Styling**<br>1. Apply the UI theme (from Sprint 3) to the `ScannerUI` panel.<br>2. Use subtle background colors, dividers, and fixed-width fonts (like a terminal font) for the data values to give it a scientific/engineering feel. |
| **10h - 11h**| **Panel Animation**<br>1. Add an `AnimationPlayer` node to `ScannerUI.tscn`.<br>2. Create two short (0.3 second) animation tracks: **"Show"** (fades opacity from 0 to 1, or slides the panel in) and **"Hide"** (does the reverse).<br>3. In `ScannerUIManager.cs`, replace `Visible = true/false` with `_animationPlayer.Play("Show")` or `_animationPlayer.Play("Hide")`. |
| **11h - 12h**| **Code Documentation**<br>1. Add XML documentation comments (`/// <summary>`) to all public methods and properties in `ScannerUIManager.cs` and `PlayerController.cs`.<br>2. Ensure the logic linking the player's input to the UI update is clean and easy to follow. |

### Task 5: Review and Testing (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **12h - 13h**| **Gameplay Flow Test**<br>1. Run the game. Right-click and hold near a **Base Ore** node. Confirm the UI panel appears and shows **"Strength: Low"** for the Resource Data.<br>2. Confirm the Planetary Data section shows the static values (e.g., **"Gravity: 3.2 g"**).<br>3. Move away from the resource node while holding Right-Click. The Resource Data should disappear/zero out, and the Planetary Data should remain (as it's global). Release Right-Click, and the entire panel should disappear smoothly. |
| **13h - 14h**| **Y-Sort and Z-Ordering Check**<br>1. Place a resource node near the player's collision shape.<br>2. Verify that the `ScannerUI` panel (a `CanvasLayer`) is drawn **above** the game world, player, and all sprites. The UI should never be clipped by the game world geometry. |
| **14h - 15h**| **Final Code Review and Cleanup**<br>1. Check for any unnecessary `GD.Print()` calls used during debugging.<br>2. Ensure the C\# code adheres to naming conventions (PascalCase for methods, camelCase for local variables). |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 4 Complete: Dedicated Scanner UI, Hold-to-Scan, and Planetary Data Display." |

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

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Field Lab Scene Setup**<br>1. Create a new scene `FieldLab.tscn` using an `Area2D` root node. Add a `Sprite2D` and a `CollisionShape2D`.<br>2. Add a `Marker2D` child node named `InputSlot` to visually mark where the player deposits the resource.<br>3. Add the `FieldLab.tscn` instance to the `World.tscn`. |
| **1h - 2h** | **FieldLabManager Script**<br>1. Create a C\# script `FieldLabManager.cs` and attach it to the `FieldLab` node.<br>2. Define a state variable: `public RawResource InputResource { get; private set; }`<br>3. Implement an interaction method `ReceiveResource(string resourceName)` that is triggered when the player interacts with the machine (using a Left-Click while standing near it). |
| **2h - 3h** | **Player Interaction Logic**<br>1. In `PlayerController.cs`, implement the interaction (Left-Click) logic for the Field Lab.<br>2. **VS Logic:** If the player is overlapping the Field Lab's `Area2D` and Left-Clicks:<br>    - Use `InventoryManager.TryRemoveItem("Base Ore", 1)`.<br>    - If successful, set `InputResource` in the `FieldLabManager.cs` to a new instance of the **Base Ore** data and start the analysis process. |

### Task 2: Analysis Logic and Knowledge Tracking (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Create Knowledge Manager Singleton**<br>1. Create a new file `KnowledgeManager.cs` in the `Scripts` folder.<br>2. Implement the knowledge tracking system:<br><br>```csharp<br>using Godot;<br>using System;<br>using System.Collections.Generic;<br><br>// Tracks what the player has learned about resources and the world<br>// This is the "brain" that remembers analyzed data<br>public partial class KnowledgeManager : Node<br>{<br>    public static KnowledgeManager Instance { get; private set; }<br>    <br>    // Track which resources have been analyzed<br>    // Key: Resource name, Value: true if data is unlocked<br>    public Dictionary<string, bool> DataUnlocked { get; private set; }<br>    <br>    // Track technology points earned through various activities<br>    // Key: Point type (Analysis, Smelting, Compositing)<br>    // Value: Current point total<br>    public Dictionary<string, int> TechPoints { get; private set; }<br>    <br>    // Track which planetary constants have been measured<br>    public Dictionary<string, bool> ConstantsMeasured { get; private set; }<br>    public float MeasuredGravity { get; set; } = 0.0f;<br>    <br>    // Track unlocked tech tree nodes<br>    public HashSet<string> UnlockedNodes { get; private set; }<br>    <br>    // Event fired when knowledge changes<br>    public event Action OnKnowledgeUpdated;<br>    <br>    public override void _EnterTree()<br>    {<br>        if (Instance == null)<br>        {<br>            Instance = this;<br>            InitializeData();<br>        }<br>        else<br>        {<br>            QueueFree();<br>        }<br>    }<br>    <br>    private void InitializeData()<br>    {<br>        DataUnlocked = new Dictionary<string, bool>();<br>        TechPoints = new Dictionary<string, int><br>        {<br>            { "Analysis", 0 },<br>            { "Smelting", 0 },<br>            { "Compositing", 0 }<br>        };<br>        ConstantsMeasured = new Dictionary<string, bool><br>        {<br>            { "Gravity", false },<br>            { "Corrosion", false },<br>            { "Tectonics", false }<br>        };<br>        UnlockedNodes = new HashSet<string>();<br>    }<br>    <br>    /// <summary><br>    /// Unlocks precise data for a specific resource<br>    /// </summary><br>    public void UnlockResourceData(string resourceName)<br>    {<br>        if (!DataUnlocked.ContainsKey(resourceName))<br>        {<br>            DataUnlocked[resourceName] = true;<br>            GD.Print($"[KNOWLEDGE] Unlocked data for: {resourceName}");<br>            OnKnowledgeUpdated?.Invoke();<br>        }<br>    }<br>    <br>    /// <summary><br>    /// Checks if resource data is unlocked<br>    /// </summary><br>    public bool IsResourceDataUnlocked(string resourceName)<br>    {<br>        return DataUnlocked.GetValueOrDefault(resourceName, false);<br>    }<br>    <br>    /// <summary><br>    /// Awards tech points of a specific type<br>    /// </summary><br>    public void AddTechPoints(string pointType, int amount)<br>    {<br>        if (TechPoints.ContainsKey(pointType))<br>        {<br>            TechPoints[pointType] += amount;<br>            GD.Print($"[KNOWLEDGE] Gained {amount} {pointType} points. " +<br>                    $"Total: {TechPoints[pointType]}");<br>            OnKnowledgeUpdated?.Invoke();<br>        }<br>    }<br>    <br>    /// <summary><br>    /// Attempts to spend tech points for research<br>    /// </summary><br>    public bool TrySpendPoints(Dictionary<string, int> costs)<br>    {<br>        // First check if we can afford all costs<br>        foreach (var cost in costs)<br>        {<br>            if (TechPoints.GetValueOrDefault(cost.Key, 0) < cost.Value)<br>            {<br>                GD.Print($"[KNOWLEDGE] Cannot afford: need {cost.Value} {cost.Key}, " +<br>                        $"have {TechPoints[cost.Key]}");<br>                return false;<br>            }<br>        }<br>        <br>        // Deduct all costs<br>        foreach (var cost in costs)<br>        {<br>            TechPoints[cost.Key] -= cost.Value;<br>        }<br>        <br>        GD.Print("[KNOWLEDGE] Research costs paid");<br>        OnKnowledgeUpdated?.Invoke();<br>        return true;<br>    }<br>    <br>    /// <summary><br>    /// Checks if a tech tree node is unlocked<br>    /// </summary><br>    public bool IsNodeUnlocked(string nodeId)<br>    {<br>        return UnlockedNodes.Contains(nodeId);<br>    }<br>}<br>```<br><br>3. **Register as Autoload:** Add to Project Settings > Autoload as "KnowledgeManager".<br>4. Build and verify compilation. |
| **4h - 5h** | **Implement Field Lab Analysis Process**<br>1. Open `FieldLabManager.cs` (from Task 1).<br>2. Add the analysis logic:<br><br>```csharp<br>/// <summary><br>/// Begins the analysis process on the input resource<br>/// </summary><br>public void StartAnalysis()<br>{<br>    if (InputResource == null)<br>    {<br>        GD.PrintErr("[Field Lab] Cannot analyze: no resource loaded");<br>        return;<br>    }<br>    <br>    GD.Print($"[Field Lab] Analyzing {InputResource.Name}...");<br>    <br>    // For the Vertical Slice, analysis is instant<br>    // In full game, this could be a timed process<br>    CompleteAnalysis();<br>}<br><br>/// <summary><br>/// Completes the analysis and grants knowledge<br>/// </summary><br>private void CompleteAnalysis()<br>{<br>    // Unlock precise data for this resource type<br>    KnowledgeManager.Instance.UnlockResourceData(InputResource.Name);<br>    <br>    // Grant Analysis tech points<br>    KnowledgeManager.Instance.AddTechPoints("Analysis", 10);<br>    <br>    // Create result summary for display<br>    string resultText = $"ANALYSIS COMPLETE\n{InputResource.Name.ToUpper()}\n";<br>    resultText += "---Properties---\n";<br>    <br>    foreach (var prop in InputResource.Properties)<br>    {<br>        resultText += $"{prop.Key}: {prop.Value.Value:F1}\n";<br>    }<br>    <br>    GD.Print($"[Field Lab] {resultText}");<br>    <br>    // Display result on screen (add to Task 3)<br>    ShowResult(resultText);<br>    <br>    // Clear input resource (it's been consumed)<br>    InputResource = null;<br>}<br><br>// Store reference to result label<br>private Label _resultLabel;<br><br>public override void _Ready()<br>{<br>    base._Ready();<br>    _resultLabel = GetNode<Label>("ResultLabel"); // Add this Label in scene<br>    _resultLabel.Visible = false;<br>}<br><br>/// <summary><br>/// Displays analysis results for a few seconds<br>/// </summary><br>private void ShowResult(string text)<br>{<br>    _resultLabel.Text = text;<br>    _resultLabel.Visible = true;<br>    <br>    // Create a timer to hide the result after 5 seconds<br>    Timer timer = GetTree().CreateTimer(5.0);<br>    timer.Timeout += () => _resultLabel.Visible = false;<br>}<br>```<br><br>3. Test by depositing a resource and calling `StartAnalysis()`. Verify that:<br>    - The resource data is unlocked in KnowledgeManager<br>    - Analysis points increase by 10<br>    - Messages print to console |
| **5h - 6h** | **Add Visual Result Display to Field Lab Scene**<br>1. Open `FieldLab.tscn` in Godot.<br>2. Add a **Label** node as a child of the FieldLab root:<br>    - Name it **ResultLabel**<br>    - Position: Above or next to the Field Lab sprite<br>    - **Horizontal Alignment:** Center<br>    - **Vertical Alignment:** Top<br>    - Set a good font size (e.g., 12-14)<br>3. Configure the label appearance:<br>    - Add a **LabelSettings** resource<br>    - Set **Font Color** to bright green (0, 255, 0) for sci-fi feel<br>    - Add a subtle **Outline** (black, size 1-2) for readability<br>4. Make sure **Visible** is unchecked by default.<br>5. **Testing:**<br>    - Run the game<br>    - Deposit a resource into the Field Lab<br>    - The analysis result should display for 5 seconds showing all properties<br>    - Verify the result text is readable and properly formatted |
| **6h - 7h** | **Add Audio Feedback and Polish**<br>1. **Resource Deposit Sound:**<br>    - Find or create a short "mechanical click" or "item placement" sound effect<br>    - Add an **AudioStreamPlayer2D** node to `FieldLab.tscn`, name it **DepositSound**<br>    - Load the sound file into its **Stream** property<br>2. **Analysis Complete Sound:**<br>    - Find or create a "computer beep" or "scan complete" chime<br>    - Add another **AudioStreamPlayer2D** named **AnalysisSound**<br>3. Update `FieldLabManager.cs` to play sounds:<br><br>```csharp<br>private AudioStreamPlayer2D _depositSound;<br>private AudioStreamPlayer2D _analysisSound;<br><br>public override void _Ready()<br>{<br>    base._Ready();<br>    _resultLabel = GetNode<Label>("ResultLabel");<br>    _depositSound = GetNode<AudioStreamPlayer2D>("DepositSound");<br>    _analysisSound = GetNode<AudioStreamPlayer2D>("AnalysisSound");<br>    _resultLabel.Visible = false;<br>}<br><br>// In ReceiveResource method (from Task 1):<br>public void ReceiveResource(RawResource resource)<br>{<br>    InputResource = resource;<br>    _depositSound.Play(); // Play deposit sound<br>    GD.Print($"[Field Lab] Received: {resource.Name}");<br>}<br><br>// In CompleteAnalysis:<br>private void CompleteAnalysis()<br>{<br>    _analysisSound.Play(); // Play completion sound<br>    // ... rest of analysis code<br>}<br>```<br><br>4. **Visual Polish:**<br>    - Consider adding a simple **AnimationPlayer** to pulse or glow the Field Lab sprite during analysis<br>    - Add particle effects (like sparkles or data streams) when analysis completes<br>5. **Final Test:**<br>    - Harvest resources<br>    - Approach Field Lab and deposit<br>    - Verify deposit sound plays<br>    - Wait for analysis<br>    - Verify completion sound and visual result display |

### Task 3: Displaying Precise Data (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **7h - 8h** | **Scanner UI Manager Update**<br>1. In `ScannerUIManager.cs`, modify the `UpdateResourceData(RawResource resource)` method.<br>2. **New Logic:** Check if the resource's data is unlocked:<br>    - `bool isUnlocked = KnowledgeManager.Instance.DataUnlocked.GetValueOrDefault(resource.Name);` |
| **8h - 9h** | **Precise vs. Vague Display**<br>1. Inside `UpdateResourceData`, implement the display logic:<br>    - IF `isUnlocked` is **true**:<br>        - Display the **exact float value** (e.g., "Strength: 2.1") for all properties using a format string (e.g., `:F1`).<br>    - ELSE IF `isUnlocked` is **false**:<br>        - Continue to display the **Vague Description** (e.g., "Strength: Low") as implemented in Sprint 4. |
| **9h - 10h**| **Visual Feedback (Color)**<br>1. When displaying the **precise value** (unlocked data), change the text color of the data to green or white (to show certainty).<br>2. When displaying the **vague value** (unlocked data), keep the text color grey or yellow (to show uncertainty). |
| **10h - 11h**| **Deduction Confirmation**<br>1. Run the game. Before analyzing, scan Base Ore (shows "Strength: Low").<br>2. Deposit Base Ore into the Field Lab and wait for analysis.<br>3. Scan Base Ore again (should now show "Strength: 2.1"). This proves the core deduction loop: **Investigate $\rightarrow$ Unlock Exact Data**. |

### Task 4: UI/UX Feedback and Polish (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **11h - 12h**| **Tech Point Visualization**<br>1. Add a small display section to the HUD for **Tech Points**.<br>2. Add a label: "Analysis Points: [Value]".<br>3. Update this label via the `KnowledgeManager` singleton whenever points are gained. |
| **12h - 13h**| **Player Interaction Prompt**<br>1. Implement a small `Label` over the Field Lab that says **"[E] Submit Base Ore"** when the player is within range.<br>2. Hide this prompt if the player does not have the required `Base Ore` in their inventory. |
| **13h - 14h**| **Art/Sound: Polish**<br>1. Add a simple **VFX** for the Field Lab's analysis (e.g., a looping energy effect during the analysis screen time).<br>2. Add an ambient hum sound effect that plays when the Field Lab is placed and idle. |
| **14h - 15h**| **Final Review and Testing**<br>1. Test the full cycle: Harvest $\rightarrow$ Interact $\rightarrow$ Inventory decreases $\rightarrow$ Points increase $\rightarrow$ Scanner UI updates.<br>2. Ensure that analyzing **Base Ore** does *not* unlock the data for **Catalyst Ore** (the system must be material-specific). |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 5 Complete: Field Lab Tier 1 Tech, Precise Data Analysis, and Knowledge Tracking." |

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

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Planetary Observatory Scene Setup**<br>1. Create a new scene `PlanetaryObservatory.tscn` (Area2D root, Sprite2D, CollisionShape2D). This machine should be physically larger than the Field Lab.<br>2. Add a visual element (a dish or telescope) that hints at its measurement function.<br>3. Instance the scene in `World.tscn`. |
| **1h - 2h** | **ObservatoryManager Script**<br>1. Create a C\# script `ObservatoryManager.cs` and attach it.<br>2. Add a `bool IsCalibrated = false;` to track the machine's state.<br>3. Implement a public method `StartCalibration()` that will initiate the measurement. |
| **2h - 3h** | **Player Placement & Tech Point Cost**<br>1. **Cost:** Define a constant in `ObservatoryManager.cs` for the calibration cost (e.g., `const int CalibrationCost = 25;` **Analysis Tech Points**).<br>2. In `PlayerController.cs`, implement a simplified placement/construction logic (Left-Click near the machine).<br>3. When the player "builds" the machine, check if `KnowledgeManager.TechPoints["Analysis"]` $\ge 25$. If true, deduct the points and call `StartCalibration()`. If false, display an error UI message. |

### Task 2: Implementing the Deduction Input (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Planetary Constant Measurement System**<br>1. The ConstantsMeasured dictionary was already added to `KnowledgeManager.cs` in Sprint 5.<br>2. Add a method to calculate portal requirements dynamically:<br><br>```csharp<br>/// <summary><br>/// Calculates the required portal foundation strength based on measured gravity<br>/// This implements the deduction formula from MECHANICS_DETAILS.md<br>/// Formula: Foundation Strength = Gravity × 2.5<br>/// </summary><br>public float GetCalculatedPortalStrengthRequirement()<br>{<br>    if (ConstantsMeasured.GetValueOrDefault("Gravity", false))<br>    {<br>        // Player has measured gravity, can calculate precise requirement<br>        float requiredStrength = MeasuredGravity * 2.5f;<br>        GD.Print($"[DEDUCTION] Portal requires Strength: {requiredStrength:F1} " +<br>                $"(based on Gravity: {MeasuredGravity:F1})");<br>        return requiredStrength;<br>    }<br>    else<br>    {<br>        // Gravity unknown - return placeholder or "unknown" value<br>        // For VS, we show the hardcoded target but player can't verify it<br>        GD.Print("[DEDUCTION] Cannot calculate requirement: Gravity not measured");<br>        return 8.0f; // Hardcoded VS target (player doesn't know where this comes from)<br>    }<br>}<br><br>/// <summary><br>/// Calculates required resistance based on corrosive index<br>/// Formula: Gate Resistance = CorrosiveIndex × 1.5<br>/// </summary><br>public float GetCalculatedPortalResistanceRequirement()<br>{<br>    if (ConstantsMeasured.GetValueOrDefault("Corrosion", false))<br>    {<br>        // In full game, player would measure this with another device<br>        // For VS, we use the hardcoded value from GameManager<br>        float corrosiveIndex = GameManager.WorldConstants.CorrosiveIndex;<br>        float requiredResistance = corrosiveIndex * 1.5f;<br>        return requiredResistance;<br>    }<br>    <br>    // Fallback to hardcoded VS target<br>    return 3.0f;<br>}<br>```<br><br>3. Build and verify compilation. |
| **4h - 5h** | **Implement Observatory Calibration Process**<br>1. Open `ObservatoryManager.cs` (from Task 1).<br>2. Implement the measurement logic:<br><br>```csharp<br>using Godot;<br>using System;<br><br>public partial class ObservatoryManager : Area2D<br>{<br>    // Calibration state<br>    public bool IsCalibrated { get; private set; } = false;<br>    <br>    // Cost to calibrate (Analysis tech points)<br>    private const int CalibrationCost = 25;<br>    <br>    // Visual and audio components<br>    private Label _statusLabel;<br>    private AudioStreamPlayer2D _calibrationSound;<br>    private AudioStreamPlayer2D _completeSound;<br>    <br>    public override void _Ready()<br>    {<br>        _statusLabel = GetNode<Label>("StatusLabel");<br>        _calibrationSound = GetNode<AudioStreamPlayer2D>("CalibrationSound");<br>        _completeSound = GetNode<AudioStreamPlayer2D>("CompleteSound");<br>        <br>        UpdateStatusDisplay();<br>    }<br>    <br>    /// <summary><br>    /// Attempts to start the calibration process<br>    /// Returns true if successful, false if insufficient points<br>    /// </summary><br>    public bool TryStartCalibration()<br>    {<br>        // Check if already calibrated<br>        if (IsCalibrated)<br>        {<br>            GD.Print("[Observatory] Already calibrated");<br>            return false;<br>        }<br>        <br>        // Check if player has enough tech points<br>        var costs = new System.Collections.Generic.Dictionary<string, int><br>        {<br>            { "Analysis", CalibrationCost }<br>        };<br>        <br>        if (!KnowledgeManager.Instance.TrySpendPoints(costs))<br>        {<br>            GD.Print($"[Observatory] Need {CalibrationCost} Analysis points to calibrate");<br>            return false;<br>        }<br>        <br>        // Start calibration<br>        StartCalibration();<br>        return true;<br>    }<br>    <br>    /// <summary><br>    /// Performs the gravity measurement<br>    /// This is where the "unknown" becomes "known"<br>    /// </summary><br>    private void StartCalibration()<br>    {<br>        GD.Print("[Observatory] Starting planetary gravity measurement...");<br>        _calibrationSound.Play();<br>        <br>        // In a full game, this could be a timed process<br>        // For VS, it's instant<br>        CompleteCalibration();<br>    }<br>    <br>    /// <summary><br>    /// Completes calibration and stores the measured gravity value<br>    /// </summary><br>    private void CompleteCalibration()<br>    {<br>        // Get the TRUE gravity value from the world constants<br>        // This is the "measurement" - revealing the hidden variable<br>        float trueGravity = GameManager.WorldConstants.GravimetricShear;<br>        <br>        // Store in knowledge manager<br>        KnowledgeManager.Instance.MeasuredGravity = trueGravity;<br>        KnowledgeManager.Instance.ConstantsMeasured["Gravity"] = true;<br>        <br>        // Update state<br>        IsCalibrated = true;<br>        <br>        // Play completion sound<br>        _completeSound.Play();<br>        <br>        GD.Print($"[Observatory] MEASUREMENT COMPLETE");<br>        GD.Print($"[Observatory] Gravimetric Shear: {trueGravity:F2} g");<br>        GD.Print($"[Observatory] Calculating portal requirements...");<br>        <br>        // Trigger knowledge update event<br>        KnowledgeManager.Instance.OnKnowledgeUpdated?.Invoke();<br>        <br>        // Update visual display<br>        UpdateStatusDisplay();<br>    }<br>    <br>    /// <summary><br>    /// Updates the status label based on calibration state<br>    /// </summary><br>    private void UpdateStatusDisplay()<br>    {<br>        if (IsCalibrated)<br>        {<br>            _statusLabel.Text = $"CALIBRATED\nGravity: {KnowledgeManager.Instance.MeasuredGravity:F2} g";<br>            _statusLabel.Modulate = Colors.Green;<br>        }<br>        else<br>        {<br>            _statusLabel.Text = $"UNCALIBRATED\nCost: {CalibrationCost} Analysis Points";<br>            _statusLabel.Modulate = Colors.Yellow;<br>        }<br>    }<br>}<br>```<br><br>3. Update `PlayerController.cs` to allow activating the Observatory:<br><br>```csharp<br>// In _PhysicsProcess, check for Observatory interaction<br>private void CheckForObservatory()<br>{<br>    // Get all observatories (usually just one)<br>    var observatories = GetTree().GetNodesInGroup("Observatory");<br>    <br>    foreach (Node node in observatories)<br>    {<br>        if (node is ObservatoryManager obs)<br>        {<br>            float distance = Position.DistanceTo(obs.Position);<br>            <br>            if (distance < 80.0f)<br>            {<br>                // Show prompt<br>                if (Input.IsActionJustPressed("ui_accept"))<br>                {<br>                    obs.TryStartCalibration();<br>                }<br>            }<br>        }<br>    }<br>}<br>```<br><br>4. Don't forget to add the Observatory to the "Observatory" group in its `_Ready()` method. |
| **5h - 6h** | **Portal Requirement Dynamic Display**<br>1. The portal goal display from Sprint 3 needs to update dynamically.<br>2. Open `InventoryUI.cs` (or create a dedicated `PortalGoalUI.cs`).<br>3. Add a method to update the portal requirement label:<br><br>```csharp<br>private Label _portalGoalLabel;<br><br>public override void _Ready()<br>{<br>    // Get reference to portal goal label<br>    _portalGoalLabel = GetNode<Label>("PortalGoalLabel");<br>    <br>    // Subscribe to knowledge updates<br>    KnowledgeManager.Instance.OnKnowledgeUpdated += UpdatePortalGoal;<br>    <br>    // Initial display<br>    UpdatePortalGoal();<br>}<br><br>/// <summary><br>/// Updates the portal requirement display<br>/// Shows calculated value if gravity is measured, otherwise shows generic target<br>/// </summary><br>private void UpdatePortalGoal()<br>{<br>    // Get calculated requirement (uses deduction formula)<br>    float requiredStrength = <br>        KnowledgeManager.Instance.GetCalculatedPortalStrengthRequirement();<br>    <br>    // Check if player knows WHERE this number comes from<br>    bool gravityMeasured = <br>        KnowledgeManager.Instance.ConstantsMeasured.GetValueOrDefault("Gravity", false);<br>    <br>    if (gravityMeasured)<br>    {<br>        // Player can deduce the formula: Gravity × 2.5<br>        float gravity = KnowledgeManager.Instance.MeasuredGravity;<br>        _portalGoalLabel.Text = $"Portal Foundation Requirement:\n" +<br>            $"Strength > {requiredStrength:F1}\n" +<br>            $"(Calculated: {gravity:F1}g × 2.5)";<br>        _portalGoalLabel.Modulate = Colors.White; // Known value<br>    }<br>    else<br>    {<br>        // Player sees the target but doesn't know why<br>        _portalGoalLabel.Text = $"Portal Foundation Requirement:\n" +<br>            $"Strength > ???\n" +<br>            $"(Measure planetary gravity to calculate)";<br>        _portalGoalLabel.Modulate = Colors.Yellow; // Unknown<br>    }<br>}<br>```<br><br>4. **Testing the Deduction Loop:**<br>    - Run the game<br>    - Check the Portal Goal UI - should show "???" initially<br>    - Gain 25 Analysis points (use Field Lab repeatedly)<br>    - Activate the Observatory<br>    - Watch console for "Gravity: 3.20 g" message<br>    - Check Portal Goal UI - should now show "Strength > 8.0 (3.2g × 2.5)"<br>    - This proves the player has successfully measured X and calculated the requirement |
| **6h - 7h** | **Audio and Visual Polish**<br>1. **Create Observatory Scene Assets:**<br>    - Open `PlanetaryObservatory.tscn`<br>    - Add visual sprites (dish antenna, measurement device)<br>    - Add a **Label** node named "StatusLabel" positioned above the device<br>2. **Add Audio Players:**<br>    - Add **AudioStreamPlayer2D** named "CalibrationSound"<br>    - Add **AudioStreamPlayer2D** named "CompleteSound"<br>    - Find or create appropriate sci-fi sound effects:<br>        - Calibration: Low hum increasing in pitch<br>        - Complete: Confident "beep beep" confirmation tone<br>3. **Add Visual Feedback:**<br>    - Consider adding an **AnimationPlayer** to show:<br>        - Idle state: Slow rotation or blinking light<br>        - Calibrating state: Fast spinning, bright glowing<br>        - Calibrated state: Steady bright glow<br>4. **Test the Full Experience:**<br>    - Approach uncalibrated Observatory - see yellow "UNCALIBRATED" label<br>    - Press E to activate (with enough points)<br>    - Hear calibration sound<br>    - See label turn green and show "CALIBRATED\nGravity: 3.20 g"<br>    - Check that Portal Goal UI updates automatically<br>    - Verify console prints the deduction formula explanation |

### Task 3: Displaying Precise Planetary Data (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **7h - 8h** | **Scanner UI Manager Update (Planetary)**<br>1. In `ScannerUIManager.cs`, modify the `UpdatePlanetaryData(PlanetaryConstants constants)` method (from Sprint 4).<br>2. **New Logic:** Check if Gravity has been measured:<br>    - `bool isMeasured = KnowledgeManager.Instance.ConstantsMeasured.GetValueOrDefault("Gravity");` |
| **8h - 9h** | **Measured vs. Placeholder Display**<br>1. Inside `UpdatePlanetaryData`, implement the display logic for `Label_Gravity`:<br>    - IF `isMeasured` is **true**:<br>        - Display the **measured value** from `KnowledgeManager.MeasuredGravity` (e.g., "Gravity: 3.20 g"). Color the text green/white.<br>    - ELSE IF `isMeasured` is **false**:<br>        - Display the original placeholder (e.g., "Gravity: ??? g"). Color the text yellow/grey. |
| **9h - 10h**| **Updating the Portal Goal UI**<br>1. Modify the `Label` displaying the Portal Goal (from Sprint 3's HUD).<br>2. The text should now call the new method: **"Foundation Strength Required: $>\{KnowledgeManager.GetCalculatedPortalStrengthRequirement():F1\}$"**.<br>3. If the gravity is not measured, the label should still show "Req: $>8.0$" or a similar deduced target (for VS simplicity). Once measured, it shows the exact required value based on the formula: **$3.2 \times 2.5 = 8.0$** (in our hardcoded VS puzzle). |
| **10h - 11h**| **Deduction Confirmation Test**<br>1. Run the game. Check the Portal Goal (e.g., "Req: $>8.0$"). Check the Scanner UI (Gravity: ???).<br>2. Gain enough **Analysis Tech Points** (using the Field Lab repeatedly).<br>3. Build and activate the **Planetary Observatory** (points decrease).<br>4. Check the Scanner UI (Gravity: 3.20 g). Check the Portal Goal (Still shows "Req: $>8.0$"). This confirms the logic and gives the player the required *X* value. |

### Task 4: UI/UX Feedback and Polish (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **11h - 12h**| **Observatory VFX/State**<br>1. Add a VFX to the Observatory. While uncalibrated, it should have a low-power, idle look.<br>2. When `IsCalibrated` is true, the VFX should switch to a high-power, active look (e.g., a dish spinning or lights turning blue). |
| **12h - 13h**| **Construction / Placement UI**<br>1. Implement a simplified **Building Ghost** system for the Observatory: When the player intends to place it, a translucent image of the machine appears under the cursor.<br>2. The ghost should turn **Green** if the player has the points and is in a valid spot, and **Red** if they are missing the points or are placing it on an invalid tile (e.g., water). |
| **13h - 14h**| **Refining Tech Point Display**<br>1. Update the HUD to show the cost for the next Tier 2 tech (Observatory).<br>2. E.g., "Next Tech (Observatory): 25/25 Analysis Points." The cost should glow green when the player can afford it. |
| **14h - 15h**| **Final Review and Testing**<br>1. Verify the full loop: Field Lab (Tier 1) grants points, points unlock Observatory (Tier 2), Observatory measures gravity, gravity updates the numerical requirements on the HUD.<br>2. Ensure the tech point deduction happens correctly and prevents building if the points are insufficient. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 6 Complete: Planetary Observatory Tier 2 Tech, Gravity Measurement, and Deduction Input Implemented." |

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

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Furnace Scene Setup**<br>1. Create a new scene `BasicFurnace.tscn` using an `Area2D` root node. Add a `Sprite2D` and `CollisionShape2D`.<br>2. Add two `Marker2D` nodes: `Input_Ore` and `Input_Fuel` to visualize input slots.<br>3. Instance the scene in `World.tscn` near the player's starting area. |
| **1h - 2h** | **FurnaceManager Script**<br>1. Create a C\# script `FurnaceManager.cs` and attach it to the `BasicFurnace` node.<br>2. Define state variables:<br>    - `public bool HasOre = false;`<br>    - `public int FuelCount = 0;`<br>    - `private bool IsSmelting = false;` |
| **2h - 3h** | **Player Interaction Logic**<br>1. In `PlayerController.cs`, implement interaction (Left-Click) for the Furnace.<br>2. **Input Logic:** If overlapping the Furnace, check inventory:<br>    - If player has `Base Ore`, `InventoryManager.TryRemoveItem("Base Ore", 1)` and set `HasOre = true`.<br>    - If player has `Soft Wood`, `InventoryManager.TryRemoveItem("Soft Wood", 1)` and increment `FuelCount`.<br>3. Display appropriate interaction prompts: `[E] Deposit Ore` or `[E] Add Fuel`. |

### Task 2: Smelting Logic and State Management (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Smelting Initiation**<br>1. In `FurnaceManager.cs`, implement `CheckForSmeltStart()` method.<br>2. **Smelt Condition:** If `HasOre` is true AND `FuelCount` $\ge 1$, set `IsSmelting = true` and start a timer (e.g., 3.0 seconds for the VS).<br>3. Display a visual indicator (e.g., red glow/fire sprite) when `IsSmelting` is true. |
| **4h - 5h** | **Output Resource Definition**<br>1. Define a new C\# class/preset for the output: **"Base Metal Bar"**. This should be a processed version of "Base Ore."<br>2. **Property Inheritance:** The Metal Bar's properties (Strength, Resistance) should be derived from the Base Ore's properties, often with a slight increase (e.g., `Metal Strength = Ore Strength * 1.2`). |
| **5h - 6h** | **Smelting Completion**<br>1. Implement a method `FinishSmelting()` that runs when the timer finishes.<br>2. Reset state: `HasOre = false`, `FuelCount -= 1`, `IsSmelting = false`.<br>3. **Output:** Call `InventoryManager.AddItem("Base Metal Bar", 1)` and visually spawn the Metal Bar near the furnace (using a temporary `Area2D` that the player can pick up). |
| **6h - 7h** | **Art/Sound: Furnace Assets**<br>1. Create the final 2.5D sprite asset for the **Basic Furnace** (must include a clear visual state for "off" and "smelting/on").<br>2. Create sound effects for **"Smelting Loop"** (low furnace roar) and **"Smelt Finished"** (a metal clang). |

### Task 3: Inventory UI and Raw vs. Processed (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **7h - 8h** | **Inventory Manager Refinement**<br>1. Ensure `InventoryManager` can handle the new **"Base Metal Bar"** item.<br>2. Add a new `public event Action OnSmeltingComplete;` to the `InventoryManager` for UI updates related to processing machines. |
| **8h - 9h** | **HUD Update for Processed Items**<br>1. In the `HUD.tscn` (Inventory Display from Sprint 3), add a label to track the count of **"Base Metal Bar"**.<br>2. In `InventoryUI.cs`, subscribe to `OnSmeltingComplete` (and `OnInventoryUpdated`). Update the Metal Bar label when a smelting action occurs. |
| **9h - 10h**| **Machine Status Display**<br>1. Add a small text label above the `BasicFurnace` in its scene.<br>2. The `FurnaceManager.cs` should dynamically update this label based on state:<br>    - Idle: "Needs Ore and Fuel."<br>    - Ore Only: "Needs 1 Fuel."<br>    - Smelting: "Smelting... (3s)"<br>    - Finished: "Metal Bar Ready\!" |
| **10h - 11h**| **Smelting Knowledge Gain**<br>1. In `FurnaceManager.cs`'s `FinishSmelting()` method, grant new knowledge:<br>    - `KnowledgeManager.Instance.TechPoints["Smelting"] += 5;` (Introduce the new **Smelting Tech Point** category). |

### Task 4: Player Progression and Testing (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **11h - 12h**| **Progression Barrier Check**<br>1. Ensure the player *must* first harvest **Soft Wood** and **Base Ore** before they can even attempt to smelt.<br>2. The only way to get the Metal Bar needed for future progression is via the Furnace. |
| **12h - 13h**| **Resource Pickup Refinement**<br>1. Create a `PickUpItem.tscn` (simple `Area2D` with a sprite for the Metal Bar).<br>2. When the player overlaps the `PickUpItem`, it should call `InventoryManager.AddItem("Base Metal Bar", 1)`, play a simple pickup sound, and queue itself for deletion. |
| **13h - 14h**| **Full Cycle Test: Raw to Processed**<br>1. Play the game from start: Gather Wood and Ore. Scan both.<br>2. Use the Wood and Ore in the Furnace.<br>3. Wait for the timer, collect the Metal Bar.<br>4. Check that the Inventory HUD updates correctly: Ore/Wood decreases, Metal Bar increases. Check that Smelting points increase. |
| **14h - 15h**| **Art/Sound: Polish**<br>1. Refine the sound timing for the furnace loop (must sound continuous).<br>2. Ensure the visual transition from `InputSlot` to `OutputBar` is clear. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 7 Complete: Basic Furnace, Smelting Logic, Property Inheritance, and Smelting Tech Points." |

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

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Catalyst Ore Harvest Logic**<br>1. In `ResourceNode.cs`, add a new property: `public string RequiredHarvestTool;` (default is "Kinetic Mining").<br>2. Set the `Catalyst Ore` resource to require a different method (e.g., `Thermal Lancing` or, for VS simplicity, **"Gas Siphoning"**).<br>3. In `PlayerController.cs`, modify the harvest check: If the node requires **Gas Siphoning**, display a prompt: "Requires Gas Siphon." |
| **1h - 2h** | **Gas Siphon Tool Stub**<br>1. Since we skip building the Siphon tool in the VS, create a temporary mechanism.<br>2. In `PlayerController.cs`, if the player Left-Clicks a `Catalyst Ore` node AND has sufficient "Analysis Tech Points" (e.g., $\ge 50$), allow the harvest, simulating a high-tech tool purchase/unlock.<br>3. Call `InventoryManager.AddItem("Catalyst Ore", 1)`. |
| **2h - 3h** | **Art/Sound: Catalyst Ore & Siphon**<br>1. Create the final 2.5D sprite asset for **"Catalyst Ore"** (it should look distinct from Base Ore—e.g., bright purple or glowing).<br>2. Create a unique, high-pitched **"Siphon/Thermal" sound effect** for harvesting this ore, distinct from the dull thud of the Kinetic Mine. |

### Task 2: Gas Injector Scene and Interaction (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Gas Injector Scene Setup**<br>1. Create a new scene `GasInjector.tscn` (Area2D root, Sprite2D, CollisionShape2D). The sprite should suggest infusion (e.g., pipes and a pressurized tank).<br>2. Add two `Marker2D` input slots: `Input_Metal` and `Input_Catalyst`.<br>3. Instance the scene in `World.tscn`. |
| **4h - 5h** | **InjectorManager Script**<br>1. Create a C\# script `InjectorManager.cs` and attach it.<br>2. Define state variables: `public bool HasMetal = false;`, `public bool HasCatalyst = false;`, and `private bool IsCompositing = false;`. |
| **5h - 6h** | **Player Deposit Logic**<br>1. In `PlayerController.cs`, implement interaction for the Injector.<br>2. **Input Logic:** Check inventory and deposit items:<br>    - If player has **Base Metal Bar**, use `TryRemoveItem()` and set `HasMetal = true`.<br>    - If player has **Catalyst Ore**, use `TryRemoveItem()` and set `HasCatalyst = true`.<br>3. Display interaction prompts: `[E] Deposit Metal` and `[E] Deposit Catalyst`. |

### Task 3: Compositing Logic (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Implement Compositing Initiation Logic**<br>1. Open `InjectorManager.cs` from Task 2.<br>2. Add a timer for the compositing process:<br><br>```csharp<br>// Compositing takes longer than smelting<br>private const float CompositingTime = 5.0f;<br>private float _compositingTimer = 0.0f;<br><br>/// <summary><br>/// Checks if compositing can start and initiates the process<br>/// </summary><br>private void CheckForCompositingStart()<br>{<br>    // Need both materials to start<br>    if (HasMetal && HasCatalyst && !IsCompositing)<br>    {<br>        GD.Print("[Gas Injector] Starting composition process...");<br>        IsCompositing = true;<br>        _compositingTimer = CompositingTime;<br>        <br>        // Play start sound (add AudioStreamPlayer2D in scene)<br>        GetNode<AudioStreamPlayer2D>("CompositingSound").Play();<br>    }<br>}<br><br>public override void _Process(double delta)<br>{<br>    // Always check if we can start<br>    CheckForCompositingStart();<br>    <br>    // Update compositing timer<br>    if (IsCompositing)<br>    {<br>        _compositingTimer -= (float)delta;<br>        <br>        // Update status display<br>        UpdateStatusLabel($"COMPOSITING... {_compositingTimer:F1}s");<br>        <br>        if (_compositingTimer <= 0)<br>        {<br>            FinishCompositing();<br>        }<br>    }<br>}<br>```<br><br>3. Add visual feedback while compositing:<br>    - Create a pulsing animation on the Gas Injector sprite<br>    - Add particle effects (colored gas swirls)<br>    - Make a status light change color (idle=yellow, active=blue) |
| **7h - 8h** | **Update CompositeMaterial Class with Calculation Logic**<br>1. Open `CompositeMaterial.cs` (from Sprint 1).<br>2. Update with the complete compositing mathematics:<br><br>```csharp<br>using Godot;<br>using System;<br>using System.Collections.Generic;<br><br>// Represents a material created by combining other materials<br>// This is the key to solving the portal engineering puzzle<br>public class CompositeMaterial : RawResource<br>{<br>    // Input materials used to create this composite<br>    public RawResource PrimaryIngredient { get; set; }<br>    public RawResource ModifierIngredient { get; set; }<br>    <br>    // Modifiers that affect final properties<br>    public float StrengthModifier { get; set; } = 1.0f;<br>    <br>    /// <summary><br>    /// Calculates the final properties of the composite material<br>    /// This is where the engineering magic happens<br>    /// </summary><br>    public void CalculateProperties()<br>    {<br>        if (PrimaryIngredient == null || ModifierIngredient == null)<br>        {<br>            GD.PrintErr("[Composite] Cannot calculate: missing ingredients");<br>            return;<br>        }<br>        <br>        GD.Print($"[Composite] Calculating properties for {Name}...");<br>        GD.Print($"  Primary: {PrimaryIngredient.Name}");<br>        GD.Print($"  Modifier: {ModifierIngredient.Name}");<br>        <br>        // For each property type, calculate the composite value<br>        foreach (ResourcePropertyType propType in <br>                 Enum.GetValues(typeof(ResourcePropertyType)))<br>        {<br>            float finalValue = CalculatePropertyValue(propType);<br>            Properties[propType] = new ResourceProperty(propType, finalValue);<br>        }<br>        <br>        // Log final strength (most important for portal)<br>        GD.Print($"[Composite] Final Strength: " +<br>                $"{Properties[ResourcePropertyType.Strength].Value:F1}");<br>    }<br>    <br>    /// <summary><br>    /// Calculates a single property value based on composition rules<br>    /// Different properties may combine differently<br>    /// </summary><br>    private float CalculatePropertyValue(ResourcePropertyType propType)<br>    {<br>        // Get base values from ingredients<br>        float primaryValue = PrimaryIngredient.Properties[propType].Value;<br>        float modifierValue = ModifierIngredient.Properties[propType].Value;<br>        <br>        // Strength uses special additive logic for the VS<br>        if (propType == ResourcePropertyType.Strength)<br>        {<br>            // Base Metal Bar has strength ~4.2 (from smelting Base Ore with 1.2x multiplier)<br>            // Catalyst Ore provides +4.0 boost<br>            // Result: 4.2 + 4.0 = 8.2 (meets requirement of >8.0)<br>            return primaryValue + modifierValue;<br>        }<br>        <br>        // Other properties use averaging<br>        // (In full game, different properties could use different formulas)<br>        return (primaryValue + modifierValue) / 2.0f;<br>    }<br>    <br>    /// <summary><br>    /// Creates a descriptive name for the composite<br>    /// </summary><br>    public void GenerateCompositeName()<br>    {<br>        // Create a name based on ingredients<br>        // E.g., "Base Metal-Catalyst Alloy"<br>        Name = $"{PrimaryIngredient.Name}-{ModifierIngredient.Name} Composite";<br>    }<br>}<br>```<br><br>3. Build and verify the class compiles. |
| **8h - 9h** | **Implement Composite Creation in Gas Injector**<br>1. Continue in `InjectorManager.cs`.<br>2. Add references to the input materials:<br><br>```csharp<br>// Store actual resource data, not just bools<br>private RawResource _inputMetal;<br>private RawResource _inputCatalyst;<br><br>// Updated deposit methods<br>public void DepositMetal(RawResource metalBar)<br>{<br>    _inputMetal = metalBar;<br>    HasMetal = true;<br>    GD.Print($"[Gas Injector] Metal deposited: {metalBar.Name}");<br>}<br><br>public void DepositCatalyst(RawResource catalystOre)<br>{<br>    _inputCatalyst = catalystOre;<br>    HasCatalyst = true;<br>    GD.Print($"[Gas Injector] Catalyst deposited: {catalystOre.Name}");<br>}<br>```<br><br>3. Implement the finish compositing method:<br><br>```csharp<br>/// <summary><br>/// Completes the compositing process and creates the output material<br>/// </summary><br>private void FinishCompositing()<br>{<br>    GD.Print("[Gas Injector] Compositing complete!");<br>    <br>    // Create the composite material<br>    CompositeMaterial composite = new CompositeMaterial<br>    {<br>        PrimaryIngredient = _inputMetal,<br>        ModifierIngredient = _inputCatalyst,<br>        Description = "A refined alloy with enhanced structural properties."<br>    };<br>    <br>    // Generate name and calculate properties<br>    composite.GenerateCompositeName();<br>    composite.CalculateProperties();<br>    <br>    // Verify it meets portal requirements (for testing)<br>    float strength = composite.Properties[ResourcePropertyType.Strength].Value;<br>    float required = KnowledgeManager.Instance.GetCalculatedPortalStrengthRequirement();<br>    <br>    GD.Print($"[Gas Injector] Created: {composite.Name}");<br>    GD.Print($"[Gas Injector] Strength: {strength:F1} (Required: >{required:F1})");<br>    <br>    if (strength >= required)<br>    {<br>        GD.Print("[Gas Injector] ✓ MEETS PORTAL REQUIREMENTS!");<br>    }<br>    else<br>    {<br>        GD.Print("[Gas Injector] ✗ Does not meet requirements");<br>    }<br>    <br>    // Add to inventory (simplified - stores just the name)<br>    // In full game, would store the actual composite object<br>    InventoryManager.Instance.AddItem(composite.Name, 1);<br>    <br>    // Grant compositing tech points<br>    KnowledgeManager.Instance.AddTechPoints("Compositing", 10);<br>    <br>    // Reset state<br>    IsCompositing = false;<br>    HasMetal = false;<br>    HasCatalyst = false;<br>    _inputMetal = null;<br>    _inputCatalyst = null;<br>    <br>    // Play completion sound<br>    GetNode<AudioStreamPlayer2D>("CompleteSound").Play();<br>    <br>    UpdateStatusLabel("COMPLETE! Collect composite.");<br>}<br>```<br><br>4. **Critical: Store composite data for later verification:**<br>    - Add a static dictionary to GameManager to store created composites:<br><br>```csharp<br>// In GameManager.cs:<br>public static Dictionary<string, CompositeMaterial> CreatedComposites { get; private set; }<br>    = new Dictionary<string, CompositeMaterial>();<br>```<br><br>5. Update the inventory add to also store in GameManager:<br><br>```csharp<br>// In FinishCompositing, before adding to inventory:<br>GameManager.CreatedComposites[composite.Name] = composite;<br>```<br><br>6. This allows the Portal to verify actual properties later. |
| **9h - 10h**| **Test the Complete Compositing Chain**<br>1. **Preparation Test:**<br>    - Start the game and harvest Base Ore<br>    - Smelt it in the Basic Furnace to get Base Metal Bar (Strength ~4.2)<br>    - Use Field Lab to analyze the metal bar (verify strength)<br>    - Harvest Catalyst Ore (requires tech points or simplified access for VS)<br>    - Analyze the Catalyst Ore (should show Strength ~4.0)<br>2. **Compositing Test:**<br>    - Approach Gas Injector<br>    - Deposit Base Metal Bar (press E with metal in inventory)<br>    - Deposit Catalyst Ore (press E again with catalyst)<br>    - Watch status: "COMPOSITING... 5.0s"<br>    - Wait for timer to reach 0<br>3. **Verification Test:**<br>    - Check console output for "Created: Base Metal-Catalyst Composite"<br>    - Verify strength calculation: "Strength: 8.2 (Required: >8.0)"<br>    - Should see "✓ MEETS PORTAL REQUIREMENTS!"<br>    - Check inventory has the composite<br>    - Check Compositing points increased by 10<br>4. **Analysis Verification:**<br>    - Take the composite to the Field Lab<br>    - Analyze it to unlock precise data<br>    - Use Hand Scanner on it<br>    - Verify Scanner UI shows "Strength: 8.2" (precise, green text)<br>5. **Math Verification:**<br>    - Base Ore (2.0) → Smelt → Base Metal Bar (2.0 × 1.2 = 2.4)<br>    - Wait, we need to adjust! Let's check:<br>    - Base Metal needs to start at 4.2 to work with the formula<br>    - Adjust `VSResourcePresets.CreateBaseOre()` or smelting multiplier<br>    - OR adjust Catalyst boost to compensate<br>    - The key: Final Strength must be > 8.0<br>6. **Update Resource Presets if needed:**<br><br>```csharp<br>// In VSResourcePresets.cs, adjust Base Ore:<br>ore.Properties[ResourcePropertyType.Strength] = <br>    new ResourceProperty(ResourcePropertyType.Strength, 3.5f);<br>// With 1.2x smelting multiplier: 3.5 × 1.2 = 4.2<br>// Then: 4.2 + 4.0 (catalyst) = 8.2 ✓<br>```

### Task 4: UI/UX Feedback and Testing (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **HUD Update for New Resources**<br>1. Update the `HUD.tscn` to track the counts of **"Catalyst Ore"** and **"Composite Alloy"**. |
| **11h - 12h**| **Machine Status Display**<br>1. Add a status label above the `GasInjector` in its scene.<br>2. Update the label dynamically:<br>    - Idle: "Needs Metal and Catalyst."<br>    - One Input: "Needs Catalyst/Metal."<br>    - Compositing: "Refining... (5s)"<br>    - Finished: "Composite Alloy Ready\!" |
| **12h - 13h**| **Full Cycle Test: Composite Creation**<br>1. Start with Base Metal Bar (from Sprint 7) and Catalyst Ore (siphoned).<br>2. Deposit both into the Injector. Wait for the process.<br>3. Collect the **Composite Alloy**. Check Inventory. |
| **13h - 14h**| **Deduction Verification**<br>1. **Crucial Test:** Use the **Hand Scanner** (vague) or the **Field Lab** (precise) on the **Composite Alloy**.<br>2. The Scanner UI must show the calculated value: **Strength $\approx 8.2$**. This visually confirms the player has engineered a material that $\mathbf{meets}$ the Portal's $\mathbf{>8.0}$ requirement. |
| **14h - 15h**| **Art/Sound: Injector FX**<br>1. Add sound effects for **"Injector Activation"** (a whoosh/hiss of gas) and **"Compositing Loop"** (pressurized bubbling/mixing sound).<br>2. Add a strong VFX (e.g., colored swirling gas) to the injector while compositing. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 8 Complete: Gas Injector, Compositing Logic, and Required Composite Alloy Created." |

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

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Research UI Scene**<br>1. Create a new `CanvasLayer` scene named `ResearchUI.tscn`. This will be a pop-up window.<br>2. Add a main `Panel` or `Control` node that covers the center of the screen when active.<br>3. Add a title label: **"Research & Development"**. |
| **1h - 2h** | **Tech Point Display**<br>1. Create a dedicated section (e.g., a side panel) to display the player's current Tech Point totals from `KnowledgeManager.cs`.<br>2. Add three labels: `Label_AnalysisPoints`, `Label_SmeltingPoints`, and `Label_CompositingPoints`.<br>3. Implement a C\# script `TechPointDisplay.cs` to dynamically pull and update these values from the `KnowledgeManager` whenever the Research UI is opened. |
| **2h - 3h** | **Research Grid Structure**<br>1. Create the main research area using a `VBoxContainer` or `GridContainer` to hold the individual Tech Nodes.<br>2. Create a prototype scene `TechNodeUI.tscn` (a `Panel` or `Control`) that will represent one research item. This prototype needs:<br>    - A `Label` for the Title/Description.<br>    - A `Label` for the Cost (e.g., "Cost: 10 Analysis Points").<br>    - A `Button` named `Button_Research`. |
| **3h - 4h** | **UI Activation and Input**<br>1. In Godot's Input Map, define a new action: **"ToggleResearch"** (bind to a key like **Tab** or **R**).<br>2. In the `GameManager.cs` or `PlayerController.cs`, implement logic to toggle the visibility of the `ResearchUI.tscn` when "ToggleResearch" is pressed. |

### Task 2: Implementing the Tech Tree Data Structure (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **4h - 5h** | **TechNode Data Class**<br>1. Create a C\# class `TechNode.cs` (or struct) to hold the definition of one unlockable item.<br>2. Fields needed:<br>    - `public string ID;` (e.g., "Unlock\_GasInjector")<br>    - `public string DisplayName;`<br>    - `public Dictionary<string, int> Costs;` (e.g., {"Analysis", 10}, {"Smelting", 5})<br>    - `public string PrerequisiteID;` (ID of the node that must be unlocked first) |
| **5h - 6h** | **VS Tech Tree Definition**<br>1. Create a static C\# class `TechTreeData.cs` to hold the entire VS structure (list of `TechNode` objects).<br>2. Define the four core VS nodes:<br>    - **Node 1: Base Analysis V2:** ID: "Unlock\_PreciseAnalysis" (Cost: 15 Analysis). **Effect:** Allows Field Lab to show precise data (currently unlocked by default in S5).<br>    - **Node 2: Planetary Survey:** ID: "Unlock\_Observatory" (Cost: 25 Analysis, Prereq: N/A).<br>    - **Node 3: Advanced Compositing:** ID: "Unlock\_GasInjector" (Cost: 10 Compositing, 10 Smelting).<br>    - **Node 4: Portal Construction:** ID: "Unlock\_PortalBuild" (Cost: 50 Total Points, Prereq: "Unlock\_GasInjector"). |
| **6h - 7h** | **Knowledge Manager Integration**<br>1. In `KnowledgeManager.cs`, add a new field: `public HashSet<string> UnlockedNodes = new();`<br>2. Add a method `public bool IsNodeUnlocked(string id)`. |

### Task 3: Research Logic and Prerequisites (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **7h - 8h** | **TechNodeUI Script Logic**<br>1. Create a C\# script `TechNodeUI.cs` and attach it to the `TechNodeUI.tscn` scene.<br>2. Add a public method `Initialize(TechNode data)` that sets the display labels.<br>3. Implement `CheckState()`: checks if the node is unlocked, if prerequisites are met, and if the costs can be afforded (using `KnowledgeManager`). |
| **8h - 9h** | **Affordability and Button State**<br>1. In `CheckState()`, disable `Button_Research` if the node is already unlocked OR if any prerequisites are missing.<br>2. If prerequisites are met, check costs. If all costs are affordable, enable the button and change its color to green (to signal readiness). If not, disable the button and show "Not Enough Points." |
| **9h - 10h**| **The Unlock Function**<br>1. Connect the `Button_Research.Pressed` signal to a method `OnResearchPressed()`.<br>2. In this method:<br>    - Call `KnowledgeManager.TrySpendPoints(Costs)`.<br>    - If successful, add the node ID to `KnowledgeManager.UnlockedNodes`.<br>    - Call `CheckState()` again to update the UI (now showing "Unlocked"). |
| **10h - 11h**| **Art/Sound: UI Polish**<br>1. Create placeholder icons for the four VS Tech Nodes (e.g., a microscope for Analysis, a furnace icon for Smelting, a blueprint for Portal).<br>2. Create sound effects for **"UI Open"** (soft futuristic tone), **"Unlock Success"** (chime/jingle), and **"Unlock Fail"** (a soft error buzz). |

### Task 4: Gating the Machines (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **11h - 12h**| **Gating the Planetary Observatory**<br>1. Go to `ObservatoryManager.cs` (from Sprint 6).<br>2. Modify the "construction/placement" logic to add a gate check:<br>    - `if (!KnowledgeManager.Instance.IsNodeUnlocked("Unlock_Observatory"))`<br>    - If locked, display: "Requires Planetary Survey research." The player must now spend 25 Analysis points in the new UI before building this. |
| **12h - 13h**| **Gating the Gas Injector**<br>1. Go to `InjectorManager.cs` (from Sprint 8).<br>2. Implement a similar gate check:<br>    - `if (!KnowledgeManager.Instance.IsNodeUnlocked("Unlock_GasInjector"))`<br>    - If locked, the machine should not be placeable or usable. Display: "Requires Advanced Compositing research." |
| **13h - 14h**| **System Integration Test**<br>1. Run the game. Try to place the **Observatory** and **Gas Injector** (should fail/block).<br>2. Open the Research UI (Tab key). Confirm the point totals are correct and the nodes are locked.<br>3. Farm enough points (using Field Lab, Furnace, Injector).<br>4. Unlock "Planetary Survey" (points decrease, button changes to "UNLOCKED").<br>5. Confirm the Observatory can now be built/activated. |
| **14h - 15h**| **Final Review and Cleanup**<br>1. Ensure all `TechNodeUI` instances correctly refresh their state when a node is unlocked (since unlocking one might meet the prereq for another).<br>2. Check for potential recursive calls or infinite loops in the prerequisite checks. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 9 Complete: Functional ProcGen Tech Tree UI, Cost/Prerequisite Gating, and Machine Unlocks." |

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

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Portal Foundation Scene Setup**<br>1. Create a new scene `PortalFoundation.tscn` (Area2D root, Sprite2D, CollisionShape2D). The sprite should represent a large, partially finished structure (like a metal ring or base).<br>2. Instance the scene in a specific, marked location in `World.tscn` (or implement a simplified placement system). For the VS, we will use a pre-set location. |
| **1h - 2h** | **Construction Placement UI (Modal)**<br>1. Create a new `CanvasLayer` scene `ConstructionUI.tscn`. This is a small, centered modal window that appears when the player activates construction mode.<br>2. Add a `VBoxContainer` to list available structures.<br>3. Add a placeholder `Button` for the Portal Foundation: `Button_Portal`. Label it "Portal Foundation." |
| **2h - 3h** | **Construction Mode Activation**<br>1. In Godot's Input Map, define **"ToggleBuildMode"** (e.g., bind to **B**).<br>2. In `PlayerController.cs`, implement logic to toggle `ConstructionUI.Visible` when "ToggleBuildMode" is pressed.<br>3. **Gating:** Check if `KnowledgeManager.IsNodeUnlocked("Unlock_PortalBuild")` is true. If not, the Construction UI should display: "Requires Portal Construction research." |
| **3h - 4h** | **Placement Ghosting**<br>1. When the player clicks `Button_Portal`, activate a **Placement Ghost** mode.<br>2. The player's cursor should follow a translucent sprite of the `PortalFoundation.tscn`.<br>3. Implement a simple check: the ghost turns **Green** if placed on a valid tile (e.g., solid ground, not too close to other structures) and **Red** otherwise. |

### Task 2: Material Submission Logic (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **4h - 5h** | **PortalManager Script**<br>1. Create a C\# script `PortalManager.cs` and attach it to the `PortalFoundation` node.<br>2. Define state: `public bool FoundationBuilt = false;`<br>3. Add a list to track required materials: `private Dictionary<string, int> RequiredMaterials = new() { { "Composite Alloy", 20 } };` (The VS requires 20 units). |
| **5h - 6h** | **Submission Panel UI**<br>1. Once the `PortalFoundation` is placed, pressing Left-Click while near it should open a second modal: **SubmissionUI.tscn**.<br>2. This UI lists the required item: "Composite Alloy: 0/20".<br>3. Add a `Button` labeled **"Submit Alloy"**. |
| **6h - 7h** | **Submission Logic**<br>1. Connect the `Submit Alloy` button to a method in `PortalManager.cs`.<br>2. This method should attempt to take all available **Composite Alloy** from the player's inventory (up to the required 20 units).<br>3. `InventoryManager.TryRemoveItem("Composite Alloy", amount)`. Update the Submission UI count. |
| **7h - 8h** | **Art/Sound: Construction Assets**<br>1. Create the final 2.5D sprite asset for the **Portal Foundation** (an industrial, heavily armored base).<br>2. Create sound effects for **"Construction Mode Toggle"** (a clean UI click) and **"Material Submission"** (a heavy, automated loading sound). |

### Task 3: Property Verification (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **8h - 9h** | **Verification Method**<br>1. In `PortalManager.cs`, implement a method `VerifyMaterialProperties()`. This runs immediately after the required 20 units of alloy have been submitted.<br>2. Retrieve the *actual* properties of the submitted **Composite Alloy** (e.g., Strength $\approx 8.2$). Since the alloy is singular, we check its stats. |
| **9h - 10h**| **Requirement Check**<br>1. Retrieve the *required* portal properties (from Sprint 6): `float requiredStrength = KnowledgeManager.GetCalculatedPortalStrengthRequirement();` (e.g., $8.0$ in the VS).<br>2. Implement the critical check:<br>    - `if (submittedAlloy.Properties[Strength].Value >= requiredStrength)`<br>    - If true, the material is adequate: `FoundationBuilt = true;`. Display success message. |
| **10h - 11h**| **Failure State Logic**<br>1. If the check is **false** (e.g., the player mistakenly submitted a low-strength alloy), display a large error message:<br>    - "FOUNDATION FAILURE: Material Strength [X] too low for Planetary Shear Requirement [Y]\! Resources Lost."<br>2. The player must then go back and craft the correct alloy to try again. (For the VS, this forces them to understand the deduction loop). |
| **11h - 12h**| **Visual Status Update**<br>1. If `FoundationBuilt` is true, change the sprite of `PortalFoundation.tscn` to a **"Construction Complete"** state (e.g., the ring is sealed, lights turn on).<br>2. Hide the Submission UI, as this step is finished. |

### Task 4: Player Progression and Testing (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **12h - 13h**| **End-to-End Test (Success)**<br>1. Start with the Composite Alloy in inventory. Build/Place the Foundation.<br>2. Submit the 20 Composite Alloys.<br>3. Verify the `VerifyMaterialProperties()` check passes (8.2 $\ge$ 8.0).<br>4. Confirm the Foundation sprite changes, and the `FoundationBuilt` state is true. |
| **13h - 14h**| **End-to-End Test (Failure Mock)**<br>1. **Debug Injection:** Temporarily inject a **low-strength material** (e.g., Base Metal Bar) into the inventory, labeled as the "Composite Alloy."<br>2. Submit the low-strength material and verify the `VerifyMaterialProperties()` check fails (e.g., 4.2 $< 8.0$).<br>3. Confirm the failure message appears, demonstrating the deduction risk. |
| **14h - 15h**| **UI/UX Polish**<br>1. Ensure the Construction UI is intuitive and doesn't conflict with the Scanner UI.<br>2. Add a simple **VFX** to the final built foundation (e.g., a subtle energy aura). |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 10 Complete: Portal Foundation Placement, Material Submission, and Property Verification Logic." |

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

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Simulation Core Scene Setup**<br>1. Create a new scene `SimulationCore.tscn` (Area2D root, Sprite2D, CollisionShape2D). The sprite should look high-tech, like a powerful computer or projector.<br>2. Instance the scene in `World.tscn` (ideally near the Portal Foundation).<br>3. **Gating:** Add a check in `PlayerController.cs` to ensure this machine can only be built/activated IF **"Unlock\_PortalBuild"** is unlocked (from Sprint 9). |
| **1h - 2h** | **SimulationManager Script**<br>1. Create a C\# script `SimulationManager.cs` and attach it.<br>2. Add state: `public bool IsSimulating = false;` and `public bool LastSimPassed = false;`<br>3. Implement a public method `StartSimulation()` triggered by player interaction. |
| **2h - 3h** | **Player Interaction Logic**<br>1. In `PlayerController.cs`, implement interaction (Left-Click) for the Simulation Core.<br>2. **Prerequisite:** The player can only start the simulation if the **Portal Foundation** has been successfully built (`PortalManager.FoundationBuilt == true`). Display an error if the foundation is missing.<br>3. If conditions met, call `StartSimulation()`. |

### Task 2: Implementing the Simulation Logic (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Simulation Formula Check**<br>1. In `SimulationManager.cs`, create a method `RunChecks()`.<br>2. **Required Strength:** Get the required strength: `float requiredS = KnowledgeManager.GetCalculatedPortalStrengthRequirement();`<br>3. **Actual Strength:** Get the strength of the submitted material (e.g., from a property stored in `PortalManager.cs` after submission).<br>4. **Primary Test:** `bool strengthPass = actualS >= requiredS - 0.01f;` (Use a small tolerance for floating point comparisons). |
| **4h - 5h** | **Secondary (Placeholder) Test**<br>1. For completeness, implement a second check based on another constant (e.g., Corrosive Index vs. Material Resistance).<br>2. `float requiredR = GameManager.WorldConstants.CorrosiveIndex * 1.5f;` (The deduction formula).<br>3. `bool resistancePass = actualR >= requiredR - 0.01f;`<br>4. **Overall Result:** `bool finalPass = strengthPass && resistancePass;` |
| **5h - 6h** | **Simulation Time and Result**<br>1. In `StartSimulation()`, set `IsSimulating = true` and start a timer (e.g., 4.0 seconds).<br>2. When the timer finishes, call `RunChecks()`. Set `LastSimPassed = finalPass`. Display the results via the UI (see Task 3). |
| **6h - 7h** | **Art/Sound: Core Assets**<br>1. Create the final 2.5D sprite asset for the **Simulation Core** (needs a clear visual state for "idle" and "running simulation").<br>2. Create sound effects for **"Simulation Start"** (a dramatic power-up tone) and a looping **"Simulation Process"** sound (fast, frantic computer noise). |

### Task 3: Simulation Report UI (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **7h - 8h** | **Report Modal Setup**<br>1. Create a new `CanvasLayer` scene `SimulationReportUI.tscn`. This is a large, immersive modal window.<br>2. Add a main title: **"PORTAL INTEGRITY SIMULATION REPORT"**.<br>3. Add a large, central label for the **FINAL STATUS** (e.g., "SIMULATION SUCCESS" or "CATASTROPHIC FAILURE"). |
| **8h - 9h** | **Detailed Check Display**<br>1. Create two `Label` pairs to show the deduction results:<br>    - **Check 1 (Strength):** "Shear Stress Margin: [Actual] / [Required]"<br>    - **Check 2 (Resistance):** "Corrosion Margin: [Actual] / [Required]"<br>2. Color the text for each check **Green** if it passed, and **Red** if it failed. |
| **9h - 10h**| **Report UI Scripting**<br>1. Create a C\# script `SimulationReportUI.cs` and attach it.<br>2. Implement a public method `ShowReport(bool success, float actualS, float requiredS, float actualR, float requiredR)` that populates the labels.<br>3. The success/fail title should be set dynamically, with distinct colors (Green for Success, Red for Failure). |
| **10h - 11h**| **Report Integration**<br>1. In `SimulationManager.cs`, after `RunChecks()` completes, instance and show the `SimulationReportUI.tscn`, passing in the calculated results (actual vs. required values). |

### Task 4: Win State Implementation (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **11h - 12h**| **Win State Check**<br>1. In `SimulationManager.cs`, if `LastSimPassed` is true, display a final prompt over the Portal Foundation: **"SIMULATION SUCCESSFUL. [F] Activate Portal."**<br>2. Create a new input action **"ActivatePortal"** (e.g., bind to **F**). |
| **12h - 13h**| **Vertical Slice Win Scene**<br>1. Create a minimal new scene `WinScene.tscn`. This can be a simple black screen with a large title: **"VERTICAL SLICE COMPLETE: WORLD BLAZED\!"**<br>2. Add a simple success message (e.g., "The Portal is stable. You can now travel home."). Add a `Button` to quit the application. |
| **13h - 14h**| **Activation and Transition**<br>1. In `PlayerController.cs`, if the player presses "ActivatePortal" while near the successful Foundation:<br>    - Play a final, powerful **Portal Activation Sound** (a deep whoosh/energy surge).<br>    - Use `GetTree().ChangeSceneToFile("res://WinScene.tscn")` to transition the player out of the game world. |
| **14h - 15h**| **Full Cycle Final Test**<br>1. Play the game from start: Deduced requirements $\rightarrow$ Gathered and refined $\rightarrow$ Built Foundation (Sprint 10 pass) $\rightarrow$ Start Simulation.<br>2. Verify the simulation passes, the Report UI is correct, and the "Activate Portal" prompt appears.<br>3. Press F and confirm the transition to the **Win Scene**. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 11 Complete: Simulation Core Tier 3 Tech, Full Deduction Verification, and VS Win State Implemented." |

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

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **World and Resource Art Finalization**<br>1. Replace all placeholder sprites (colored squares, simple sketches) with the final 2.5D isometric art assets for:<br>    - Ground/Base Tiles.<br>    - **Base Ore** and **Catalyst Ore** nodes.<br>    - **Soft Wood** node.<br>2. Ensure all tile collisions and visual offsets are correctly aligned for the isometric perspective. |
| **1h - 2h** | **Machine Art Finalization**<br>1. Replace the final sprites for the four main machines:<br>    - **Field Lab** (Tier 1)<br>    - **Planetary Observatory** (Tier 2)<br>    - **Basic Furnace** (Smelting)<br>    - **Gas Injector** (Compositing)<br>2. Verify that the visual states (e.g., Furnace "off" vs. "smelting") use the correct animated sprites. |
| **2h - 3h** | **Portal & Simulation Core Art**<br>1. Replace the final sprites for the ultimate structures:<br>    - **Portal Foundation** (Built/Unbuilt states)<br>    - **Simulation Core** (Idle/Running states)<br>2. Ensure the Portal's "Construction Complete" state looks visually impressive and final. |

### Task 2: Audio & Music Integration (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Background Music Loop**<br>1. Add an `AudioStreamPlayer` node to the `World.tscn` (or `GameManager`).<br>2. Load the final 30-second **Ambient Exploration Music** (from Sprint 3) and set it to loop.<br>3. Set the volume to a low, non-intrusive level. |
| **4h - 5h** | **Final Sound Effects Integration**<br>1. Verify and tune the volume/pitch for all major SFX developed in previous sprints:<br>    - **Footsteps** (PlayerController)<br>    - **Harvesting/Siphoning** (ResourceNode interaction)<br>    - **Smelting/Compositing** (Furnace/Injector completion sounds)<br>    - **UI Clicks** (for Research, Construction, and all buttons). |
| **5h - 6h** | **Machine Ambient Soundscapes**<br>1. Add subtle, looping ambient sounds to the machines when they are idle (but built):<br>    - **Field Lab:** A soft, periodic "beep/scan" sound.<br>    - **Planetary Observatory:** A quiet, rhythmic hum.<br>    - **Basic Furnace:** A low, idle crackle/glow sound. |

### Task 3: Visual Juice and Feedback (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Screen Shake for Impact**<br>1. In `PlayerController.cs` (or a dedicated `CameraShake.cs` script), implement a function `ShakeCamera(float duration, float magnitude)`. This uses `Camera2D.Offset` to apply a random, high-frequency movement.<br>2. Apply a small shake (e.g., 0.1s, mag 3) when **Smelting/Compositing** finishes.<br>3. Apply a medium shake (e.g., 0.3s, mag 5) on **Simulation SUCCESS/FAIL** to emphasize the result. |
| **7h - 8h** | **Particle Effects (VFX)**<br>1. Create and integrate final particle systems for key events:<br>    - **Resource Collection:** Small, bright particles that fly from the resource node to the player's position on harvest.<br>    - **Smelting:** Plumes of smoke/heat that puff out when the Furnace timer resets. |
| **8h - 9h** | **UI Feedback & Responsiveness**<br>1. Implement simple color transitions/flashes for successful actions:<br>    - When an item is added to the inventory, the corresponding inventory label should briefly flash **Green**.<br>    - When a research node is unlocked, the **Tech Points** label that paid for it should briefly flash **Red** (to show points spent). |
| **9h - 10h**| **Machine Interaction Prompts**<br>1. Refine all interaction prompts (`[E] Deposit Ore`, `[F] Activate Portal`) to use a clean font, have a small background panel, and fade in/out smoothly when the player enters/exits the machine's interaction range. |

### Task 4: Win State Polish and Final Review (6 Hours)

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **Portal Activation Sequence**<br>1. Enhance the `Portal Activation` transition (from Sprint 11):<br>    - When "Activate Portal" is pressed, the screen should immediately fade to white (using a `ColorRect` and `AnimationPlayer`).<br>    - Play the final, powerful sound effect (energy surge).<br>2. This provides a dramatic climax before the `WinScene`. |
| **11h - 12h**| **Simulation Report Readability**<br>1. Review the `SimulationReportUI.tscn` (from Sprint 11). Ensure the **Red/Green** color coding is highly visible and the font size for the **FINAL STATUS** is large and commanding. |
| **12h - 13h**| **VS Demo Walkthrough Tuning**<br>1. Conduct a full, timed walkthrough of the entire Vertical Slice (Start $\rightarrow$ Gather $\rightarrow$ Analyze $\rightarrow$ Engineer $\rightarrow$ Simulate $\rightarrow$ Win).<br>2. Adjust game pacing (e.g., harvest speeds, smelt timers, point costs) to ensure the demo takes a suitable duration (e.g., 10-15 minutes of continuous play). |
| **13h - 14h**| **Performance Check**<br>1. Run the profiler in Godot. Ensure no major performance bottlenecks exist, especially with particle systems or complex UI updates. Optimize any identified slow spots. |
| **14h - 15h**| **Final Code and Comment Review**<br>1. Ensure all temporary debug code, placeholder logic (like the simplified Gas Siphon), and `GD.Print` calls are removed or commented out.<br>2. Review all C\# scripts one last time for clarity and consistency. |
| **15h - 16h**| **Final Commit**<br>1. **Tag Repository:** Create a final Git tag (e.g., `VS_1.0_Final`).<br>2. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 12 Complete: Aesthetic Polish, Juiciness, Final Audio/Art Integration, and Vertical Slice Finalized." |
