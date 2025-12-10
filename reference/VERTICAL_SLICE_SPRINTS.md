## Wayblazer Vertical Slice: 16-Hour Sprint Plan (12 Sprints)

| Phase | Sprint # | Focus (16 Hours) | Core Technical Goal |
| :--- | :--- | :--- | :--- |
| **P1: Core Architecture** | **Sprint 1** | Project Setup & Data Core | C# classes for Resources, World, and Properties. |
| | **Sprint 2** | Grid, Player Movement & Voxel Data | Godot C# tile-based world, basic player controller. |
| | **Sprint 3** | Resource Engine & ProcGen V1 | Load the first hardcoded puzzle and place randomized resources. |
| **P2: Deduction Mechanics** | **Sprint 4** | Interaction & Scanner UI | Click-to-interact logic and the Hand Scanner (vague info). |
| | **Sprint 5** | Field Lab & Analysis Logic | Tier 1 Tech: Build the **Field Lab** and reveal exact resource stats. |
| | **Sprint 6** | Planetary Analysis & Deduction Input | Tier 2 Tech: Build the **Gravimeter** and determine the final Portal $X$ value. |
| **P3: Solution Engineering** | **Sprint 7** | Basic Crafting & Inventory | Implement the Inventory and the **Basic Furnace** (Smelting). |
| | **Sprint 8** | Advanced Refining (Composition) | Implement **Gas Siphoning** and the **Gas Injector** (Alloying/Compositing). |
| | **Sprint 9** | ProcGen Tech Tree V2 & Unlocks | Implement the Knowledge currency, research UI, and prerequisite logic. |
| **P4: Final Polish & Sell** | **Sprint 10** | Portal Construction & UI | Implement the final Portal Structure placement and submission UI. |
| | **Sprint 11** | Simulation Core & Win State | Tier 3 Tech: Build the **Simulation Core** and run the verification math. |
| | **Sprint 12** | Aesthetic Polish & Juiciness | Art, sound, music integration, and final performance polish. |

---

## Detailed Sprint Breakdown (Sprints 1-3)

### 🚀 Sprint 1: Project Setup & Data Core (16 Hours)

**Goal:** Create the foundational C# data structures for the World and its Resources. This is the **Brain** of the deduction system.

| Duration | Task | Description / Tutorial Steps (C#/.NET) |
| :--- | :--- | :--- |
| **0h - 2h** | **Project Setup** | 1. Create a new Godot project. 2. Enable C# support and ensure the .NET SDK is working. 3. Set the initial project settings (Window size, main scene). 4. Create a top-level `Scripts` folder. |
| **2h - 4h** | **Data Structure: Resource Property** | 1. Create a C# class `ResourceProperty.cs` (or struct). 2. Define `Name`, `Value` (float), and `HiddenValue` (string for "Low/Med/High" text). 3. Create an `enum` for property types (`Strength`, `Resistance`, etc.). |
| **4h - 6h** | **Data Structure: Raw Resource** | 1. Create a C# class `RawResource.cs`. 2. Give it a `Name`, `Description`, and a `Dictionary<PropertyType, ResourceProperty>` to hold its stats. 3. Add a public method `GenerateProperties(int level)`. |
| **6h - 8h** | **Data Structure: Composite Material** | 1. Create a C# class `CompositeMaterial.cs` inheriting from `RawResource`. 2. Add properties: `PrimaryIngredient` and `ModifierIngredient`. 3. Add a method `CalculateProperties()` to apply the combining logic (e.g., `New Strength = Base Strength * Modifier`). |
| **8h - 10h** | **Data Structure: Planetary Constants** | 1. Create a C# class `PlanetaryConstants.cs`. 2. Add float fields for the 5-10 properties (e.g., `GravimetricShear`, `CorrosiveIndex`). 3. Add a `GenerateWorld(int complexity)` method to randomize these values within their range. |
| **10h - 12h**| **Art/Sound: Initial Concept** | 1. Sketch out 3 concept drafts for the player character (Wayblazer suit). 2. Sketch 3 concepts for the look of the **Basic Ore Node** (e.g., Crystalline, Vein, or Sphere). |
| **12h - 14h**| **Data Structure: Portal Requirement** | 1. Create a C# class `PortalRequirement.cs`. 2. Add a dictionary linking a property type to a required threshold (e.g., `Strength` $\rightarrow$ 8.0). 3. Add a method `SetRequirements(PlanetaryConstants constants)` using the deduction formulas (e.g., `Strength = constants.GravimetricShear * 2.5`). |
| **14h - 16h**| **Simple Unit Test (C#)** | 1. Create a new C# console project (separate from Godot) to test your logic. 2. Write a test that generates a `PlanetaryConstants` object, calculates the `PortalRequirement`, and then creates a `CompositeMaterial` to see if the material meets the requirement. *Verify the math works.* |

### 🚀 Sprint 2: Grid, Player Movement & Voxel Data (16 Hours)

**Goal:** Get a playable character moving on a tile-based map that is loaded from the World data.

| Duration | Task | Description / Tutorial Steps (Godot C#) |
| :--- | :--- | :--- |
| **0h - 2h** | **Grid Map Setup** | 1. Create a new `TileMap` node in the main scene. 2. Design a simple 2D tileset (use basic colored squares for now: Green for grass, Gray for rock). 3. Define the size of the world map (e.g., 64x64). |
| **2h - 4h** | **World Generation Script** | 1. Create a C# script `WorldGenerator.cs` (attach to the main scene). 2. In `_Ready()`, create a 2D array of ints representing tile IDs. 3. Populate the array with a simple Perlin Noise or random fill for basic terrain variation. |
| **4h - 6h** | **Tilemap Rendering** | 1. Write the logic in `WorldGenerator.cs` to iterate over the array and use `TileMap.SetCell(x, y, tileID)` to draw the map. 2. Verify the generated map loads correctly in Godot. |
| **6h - 8h** | **Player Scene & Controller** | 1. Create a new `CharacterBody2D` scene for the player. Add a `Sprite2D` and `CollisionShape2D`. 2. Create `PlayerController.cs`. 3. Implement basic WSAD movement using `Velocity` and `MoveAndSlide()`. *Use isometric/top-down movement logic.* |
| **8h - 10h**| **Camera & Collision** | 1. Add a `Camera2D` to the player scene. 2. Enable simple TileMap collision. 3. Test player movement and confirm the camera tracks correctly. |
| **10h - 12h**| **Art/Sound: Player Assets** | 1. Create the final 2D sprite asset for the player character (idle and walking animations, 4-way direction). 2. Create the final assets for the basic **Ground** and **Ore** tiles. |
| **12h - 14h**| **Voxel/Resource Node Logic** | 1. Create a `ResourceNode.cs` script (inherits from `Area2D`). 2. Add a `RawResource` object as a member variable. 3. Place a few instances on the map. |
| **14h - 16h**| **Refining Player-to-World Logic** | 1. Implement a method in `PlayerController` to get the tile/node the player is currently over. 2. Display a debug message with the name of the resource node (e.g., "Over: Dull Grey Ore"). |

### 🚀 Sprint 3: Resource Engine & ProcGen V1 (16 Hours)

**Goal:** Fully link the C# data core to the Godot world, creating a single, playable "puzzle world."

| Duration | Task | Description / Tutorial Steps (Godot C#) |
| :--- | :--- | :--- |
| **0h - 2h** | **ProcGen V1: Resource Instantiation** | 1. Modify `WorldGenerator.cs` to instantiate the **RawResource** C# objects, not just the tiles. 2. For the VS, hardcode 3 types of raw ore, each with a different set of **Strength** values (e.g., Low, Medium, High). |
| **2h - 4h** | **World Context Initialization** | 1. In the main scene's script, create a static or singleton `GameManager.cs`. 2. Call `PlanetaryConstants.GenerateWorld(1)` to hardcode the Level 1 constants (Gravity=3.2, etc.). 3. Call `PortalRequirement.SetRequirements()` to finalize the target strength (8.0). |
| **4h - 6h** | **Inventory Manager** | 1. Create a C# class `InventoryManager.cs` (singleton). 2. Use a `Dictionary<string, int>` to track resource counts (e.g., "RedOre" $\rightarrow$ 5). 3. Implement methods for `AddItem(string name, int count)` and `HasItem(string name, int count)`. |
| **6h - 8h** | **The Interaction Mechanic** | 1. In `PlayerController.cs`, implement a method for Left-Click/Harvest. 2. When the player clicks a resource node, destroy the node and call `InventoryManager.AddItem()` based on its type. |
| **8h - 10h**| **Basic HUD UI** | 1. Create a basic UI scene (CanvasLayer). 2. Add labels to display the current counts of 3 key resources from the `InventoryManager`. 3. Add a label to display the hidden target requirement (e.g., "Target Foundation Strength: ???"). |
| **10h - 12h**| **Art/Sound: UI and Music** | 1. Design a simple inventory icon for a Metal Bar, a Raw Ore, and a Gas Canister. 2. Create a 30-second loop of quiet, ambient **exploration music** (lo-fi sci-fi). |
| **12h - 14h**| **Initial Deduction/Vagueness** | 1. Update `RawResource` to include a method `GetVagueDescription()`. 2. If Strength is 0-3, return "Low Integrity." 3. If Strength is 7-10, return "High Integrity." 4. Display this vague description in the debug overlay when inspecting the resource. |
| **14h - 16h**| **Playtest & Tuning** | 1. Play the game from start (landing) to the point of gathering resources. 2. Ensure movement feels responsive. 3. Adjust resource drop rates so the player can collect 20 units of two types of ore within 5 minutes. |

---
**Next Step:** With these 48 hours completed, we have a functional world, a moving character, and the entire C# data system for the game's central puzzle logic. The next phase will focus entirely on building the *tools* the player uses to deduce the solution.
