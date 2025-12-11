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

-----

## Task Breakdown (16 Hours)

### Task 1: Basic Smelting Furnace Scene and Interaction (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Furnace Scene Setup**<br>1. Create a new scene `BasicFurnace.tscn` using an `Area2D` root node. Add a `Sprite2D` and `CollisionShape2D`.<br>2. Add two `Marker2D` nodes: `Input_Ore` and `Input_Fuel` to visualize input slots.<br>3. **VS Simplification:** Instance the scene in `World.tscn` near the player's starting area as a **pre-placed machine**. For the VS, machines are already built in the world; no construction/placement mechanic is needed. *(Note: A full placement system would add 2-3 hours to this or a previous sprint.)* |
| **1h - 2h** | **FurnaceManager Script**<br>1. Create a C\# script `FurnaceManager.cs` and attach it to the `BasicFurnace` node.<br>2. Define state variables:<br>    - `public bool HasOre = false;`<br>    - `public int FuelCount = 0;`<br>    - `private bool IsSmelting = false;` |
| **2h - 3h** | **Player Interaction Logic**<br>1. In `PlayerController.cs`, implement interaction (Left-Click) for the Furnace.<br>2. **Input Logic:** If overlapping the Furnace, check inventory:<br>    - If player has `Base Ore`, `InventoryManager.TryRemoveItem("Base Ore", 1)` and set `HasOre = true`.<br>    - If player has `Soft Wood`, `InventoryManager.TryRemoveItem("Soft Wood", 1)` and increment `FuelCount`.<br>3. Display appropriate interaction prompts: `[E] Deposit Ore` or `[E] Add Fuel`. |

### Task 2: Smelting Logic and State Management (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Smelting Initiation**<br>1. In `FurnaceManager.cs`, implement `CheckForSmeltStart()` method.<br>2. **Smelt Condition:** If `HasOre` is true AND `FuelCount` $\ge 1$, set `IsSmelting = true` and start a timer (e.g., **5-8 seconds** for more satisfying pacing).<br>3. Display a visual indicator (e.g., red glow/fire sprite) when `IsSmelting` is true.<br>4. **Note:** 3 seconds feels too fast and unsatisfying. 5-8 seconds with clear visual progress is better. |
| **4h - 5h** | **Output Resource Definition**<br>1. Define a new C\# class/preset for the output: **"Base Metal Bar"**. This should be a processed version of "Base Ore."<br>2. **Property Inheritance:** The Metal Bar's properties (Strength, Resistance) should be derived from the Base Ore's properties, often with a slight increase (e.g., `Metal Strength = Ore Strength * 1.2`). |
| **5h - 6h** | **Smelting Completion**<br>1. Implement a method `FinishSmelting()` that runs when the timer finishes.<br>2. Reset state: `HasOre = false`, `FuelCount -= 1`, `IsSmelting = false`.<br>3. **Output:** Call `InventoryManager.AddItem("Base Metal Bar", 1)` and visually spawn the Metal Bar near the furnace (using a temporary `Area2D` that the player can pick up). |
| **6h - 7h** | **Art/Sound: Furnace Assets**<br>1. Create the final 2.5D sprite asset for the **Basic Furnace** (must include a clear visual state for "off" and "smelting/on").<br>2. Create sound effects for **"Smelting Loop"** (low furnace roar) and **"Smelt Finished"** (a metal clang). |

### Task 3: Inventory UI and Raw vs. Processed (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **7h - 8h** | **Inventory Manager Refinement**<br>1. Ensure `InventoryManager` can handle the new **"Base Metal Bar"** item.<br>2. Add a new `public event Action OnSmeltingComplete;` to the `InventoryManager` for UI updates related to processing machines. |
| **8h - 9h** | **HUD Update for Processed Items**<br>1. In the `HUD.tscn` (Inventory Display from Sprint 3), add a label to track the count of **"Base Metal Bar"**.<br>2. In `InventoryUI.cs`, subscribe to `OnSmeltingComplete` (and `OnInventoryUpdated`). Update the Metal Bar label when a smelting action occurs. |
| **9h - 10h**| **Machine Status Display**<br>1. Add a small text label above the `BasicFurnace` in its scene.<br>2. The `FurnaceManager.cs` should dynamically update this label based on state:<br>    - Idle: "Needs Ore and Fuel."<br>    - Ore Only: "Needs 1 Fuel."<br>    - Smelting: "Smelting... (5-8s)"<br>    - Finished: "Metal Bar Ready\!" |
| **10h - 11h**| **Smelting Knowledge Gain**<br>1. In `FurnaceManager.cs`'s `FinishSmelting()` method, grant new knowledge:<br>    - `KnowledgeManager.Instance.TechPoints["Smelting"] += 5;` (Introduce the new **Smelting Tech Point** category).<br>2. **Note on Tech Gating:** For VS simplicity, the Furnace (Tier 1 tech) is freely available from the start. Higher-tier machines like the Gas Injector (Sprint 8) will require tech points/research unlocks. |

### Task 4: Player Progression and Testing (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **11h - 12h**| **Progression Barrier Check**<br>1. Ensure the player *must* first harvest **Soft Wood** and **Base Ore** before they can even attempt to smelt.<br>2. The only way to get the Metal Bar needed for future progression is via the Furnace. |
| **12h - 13h**| **Resource Pickup Refinement**<br>1. Create a `PickUpItem.tscn` (simple `Area2D` with a sprite for the Metal Bar).<br>2. When the player overlaps the `PickUpItem`, it should call `InventoryManager.AddItem("Base Metal Bar", 1)`, play a simple pickup sound, and queue itself for deletion. |
| **13h - 14h**| **Full Cycle Test: Raw to Processed**<br>1. Play the game from start: Gather Wood and Ore. Scan both.<br>2. Use the Wood and Ore in the Furnace.<br>3. Wait for the timer, collect the Metal Bar.<br>4. Check that the Inventory HUD updates correctly: Ore/Wood decreases, Metal Bar increases. Check that Smelting points increase. |
| **14h - 15h**| **Art/Sound: Polish**<br>1. Refine the sound timing for the furnace loop (must sound continuous).<br>2. Ensure the visual transition from `InputSlot` to `OutputBar` is clear. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 7 Complete: Basic Furnace, Smelting Logic, Property Inheritance, and Smelting Tech Points." |
