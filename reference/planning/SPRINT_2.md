# 🗺️ Sprint 2: Grid, Player Movement & Voxel Data (16 Hours)

## Summary

This sprint focuses on establishing the physical presence of the game. We will set up the Godot `TileMap` for our 2.5D world, implement the player character, and link the procedural data created in Sprint 1 to the visual representation on the map.

## 🎯 Goal

A playable scene where the character can move around a basic, generated world composed of different tile types (Ground, Ore, Wood) and visually identify the resources they are standing on.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** `TileMap`, `CharacterBody2D`, and basic scene setup.
  * **C\# / .NET:** World generation logic and data linking.

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
| **3h - 4h** | **ProcGen V1: Simple World Array**<br>1. In `WorldGenerator.cs`, define a private 2D array: `private int[,] _worldData = new int[WorldSize, WorldSize];`<br>2. Implement a stub method `GenerateInitialWorld()`:<br>    - Fill the entire array with `ID 0` (Ground).<br>    - Use `GD.RandRange(0, 100)` to randomly place `ID 1` (Ore) and `ID 2` (Wood) nodes at a density of about **2-3% each**. *(Note: 5% density on 64x64 = ~200 nodes each, which may cause performance issues. Start lower and tune.)* |
| **4h - 5h** | **Render World to TileMap**<br>1. Implement a method `RenderWorld()`:<br>    - Loop through `_worldData` from `x = 0` to `WorldSize` and `y = 0` to `WorldSize`.<br>    - Use `_tileMap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(_worldData[x, y], 0));` for ground tiles only.<br>    - **Note:** Resource nodes (Ore, Wood) will be separate `Area2D` instances spawned in Task 4, NOT TileMap cells. This allows for easier interaction logic.<br>2. Call `GenerateInitialWorld()` and then `RenderWorld()` in `_Ready()`. |
| **5h - 6h** | **World Context Link**<br>1. Create a `GameManager.cs` C\# class (make it a singleton via `[GlobalClass]` for easy access).<br>2. In `GameManager.cs`, create and initialize the C\# data objects from Sprint 1: `public PlanetaryConstants Constants = new();` and `public PortalRequirement Required = new();`<br>3. In `_Ready()` of `GameManager.cs`, call `Constants.GenerateWorld(1)` and `Required.SetRequirements(Constants)` to load the VS puzzle. |

### Task 3: Player Controller and Scene Setup (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Player Scene Setup**<br>1. Create a new scene called `Player.tscn` using the **CharacterBody2D** root node.<br>2. Add a `Sprite2D` child node and a `CollisionShape2D` (e.g., a simple `RectangleShape2D`).<br>3. Add the `Player.tscn` instance to the `World.tscn` scene. |
| **7h - 8h** | **Basic Movement Script**<br>1. Create a C\# script `PlayerController.cs` and attach it to the `CharacterBody2D` node.<br>2. Define a constant `const float Speed = 200.0f;`<br>3. In `_PhysicsProcess(double delta)`, implement basic 4-way input reading using `Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down")`. |
| **8h - 9h** | **Movement and Collision**<br>1. Use the input vector to set the `Velocity` property: `Velocity = direction * Speed;`<br>2. Call `MoveAndSlide()` to execute the movement. Test the player movement and confirm collisions with the TileMap prevent movement through walls (if you added collision shapes to the TileSet). |
| **9h - 10h**| **Camera and Y-Sort**<br>1. Add a `Camera2D` node as a child of the player. Enable `Limit Smoothing` for smooth tracking.<br>2. **Camera Bounds:** Set camera limits to prevent viewing outside the world:<br>    - `camera.LimitLeft = 0`, `camera.LimitTop = 0`<br>    - `camera.LimitRight = WorldSize * CellSize`, `camera.LimitBottom = WorldSize * CellSize`<br>3. **Y-Sort Setup (Allow extra time for debugging):** In `_PhysicsProcess`, ensure the player's Y-position correctly updates the global Z-order for 2.5D:<br>    - `ZIndex = (int)Position.Y;` (or use `Y_Sort` property, depending on Godot version/project settings).<br>    - **Note:** Y-sorting in isometric 2.5D can be tricky and may require 2-3 hours to perfect. Budget additional time if issues arise. |

### Task 4: Resource Node Linking (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **Resource Node Scene**<br>1. Create a new scene `ResourceNode.tscn` using the **Area2D** root node. Add a `Sprite2D` and a `CollisionShape2D`.<br>2. Create a C\# script `ResourceNode.cs` and attach it.<br>3. In `ResourceNode.cs`, add a property: `public RawResource ResourceData { get; set; }` (from Sprint 1). |
| **11h - 12h**| **Instantiate Data Nodes**<br>1. Modify `WorldGenerator.cs`'s `RenderWorld()` method (or create a new `SpawnResourceNodes()` method).<br>2. When an Ore or Wood location is determined in `_worldData` (ID 1 or 2), **instantiate the `ResourceNode.tscn` as a separate Area2D scene** at that tile's world position (convert grid coordinates to world coordinates).<br>3. **Important:** When instantiating, create a `RawResource` object (from Sprint 1's hardcoded definitions) and assign it to the `ResourceData` property of the new node.<br>4. **Clarification:** Resources are NOT TileMap cells—they are independent Area2D nodes that can be interacted with separately from the ground tiles. |
| **12h - 13h**| **Player Tile Inspection**<br>1. In `PlayerController.cs`, implement a method `CheckForResourceNode()` that runs in `_PhysicsProcess`.<br>2. Use a simple raycast or `GetTree().GetNodesInGroup("Resources")` to find a `ResourceNode` overlapping the player's collision or the tile the player is on. |

### Task 5: Aesthetic & Review (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **13h - 14h**| **Art/Sound: Final Asset Integration**<br>1. Replace the placeholder colored squares for the Player, Ground, Ore, and Wood tiles with the final art assets developed in Sprint 1/2 concept tasks.<br>2. Add simple **footstep sound effects** to the PlayerController (e.g., play a sound when velocity is non-zero). |
| **14h - 15h**| **Debugging and Visual Check**<br>1. Run the game. Verify the 64x64 map loads.<br>2. Check Y-Sorting: If the player walks *behind* a tree (a static sprite placed on a higher Y-coordinate), the player must be visually occluded by it. If they walk in front of it (lower Y-coordinate), they must be visible.<br>3. Verify the debug message from Task 4 (e.g., "Over: Dull Grey Ore") correctly displays the *specific* resource linked to the instantiated node. |
| **15h - 16h**| **Review and Commit**<br>1. Review all scripts for clean C\# syntax and Godot C\# best practices.<br>2. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 2 Complete: Grid World, Player Movement, and Resource Voxel Linking." |
