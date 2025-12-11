# ⛏️ Sprint 3: Resource Engine & ProcGen V1 (18 Hours)

## Summary

This sprint fully integrates the procedural data architecture (Sprint 1) with the physical world (Sprint 2). We will finalize the resource generation, implement the core harvesting mechanic, and build the foundational inventory manager and HUD necessary for the primary gameplay loop.

## 🎯 Goal

The player can successfully land, harvest one type of wood and two types of ore, see the resources added to a working inventory, and get the first hints of the deduction puzzle via the Hand Scanner.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Singleton management, Input handling, UI integration.
  * **C\# / .NET:** Random number generation for resource properties, Inventory Management class.

-----

## Task Breakdown (18 Hours)

### Task 1: Singleton and Resource Initialization (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **GameManager Singleton Refinement**<br>1. In `GameManager.cs`, ensure the class is set up as a true autoload singleton in Godot Project Settings.<br>2. Add public static properties to easily access the critical data:<br>    - `public static PlanetaryConstants WorldConstants => Instance.Constants;`<br>    - `public static PortalRequirement PortalGoal => Instance.Required;`<br>3. This simplifies access from `PlayerController` and other nodes. |
| **1h - 2h** | **RawResource Generation Logic**<br>1. Refine the `RawResource.GenerateProperties(int seed)` method (from Sprint 1).<br>2. Implement a loop to randomly assign a numerical value (e.g., between 1.0f and 10.0f) to *all* `ResourcePropertyType` enums (Strength, Resistance, Toughness, Conductivity, Reactivity).<br>3. Use the `seed` parameter and a `RandomNumberGenerator` to ensure a resource type is **consistent** across the run, but **random** between runs. |
| **2h - 3h** | **Hardcoded VS Resources**<br>1. In a new C\# file `VSResourcePresets.cs`, define the data for the three key VS resources:<br>    - **"Base Ore":** Strength $\approx$ 2.0 (Too low for portal).<br>    - **"Catalyst Ore":** Strength $\approx$ 4.0 (Better, but still not enough).<br>    - **"Soft Wood":** Low Burn Time (easy to harvest).<br>2. Modify `WorldGenerator.cs` to use these specific presets when instantiating `RawResource` objects on the map (IDs 1 and 2). |

### Task 2: Inventory Manager Implementation (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **InventoryManager Class**<br>1. Create a new C\# class `InventoryManager.cs`. Make it a singleton (like `GameManager`).<br>2. Define the storage dictionary: `public Dictionary<string, int> ResourceCounts = new();` (key is resource name, value is count).<br>3. Define inventory capacity: `public const int MaxInventorySlots = 999;` (effectively unlimited for VS, but structure is in place for future limits).<br>4. Implement `public event Action OnInventoryUpdated;` for UI updates. |
| **4h - 5h** | **AddItem and RemoveItem Methods**<br>1. Implement `public void AddItem(string resourceName, int amount = 1)`:<br>    - Check total item count. If adding would exceed capacity, display a "Inventory Full" message and return early.<br>    - Check if `resourceName` exists; if not, add it with the amount.<br>    - If it exists, increment the count.<br>    - Call `OnInventoryUpdated?.Invoke();` at the end.<br>2. Implement `public bool TryRemoveItem(string resourceName, int amount)`: Checks count, decrements, and returns `false` if not enough. |
| **5h - 6h** | **Player Harvest Integration**<br>1. In `PlayerController.cs`, modify the Left-Click/Harvest logic (from Sprint 2).<br>2. When a `ResourceNode` is clicked and destroyed, call: `InventoryManager.Instance.AddItem(resourceNode.ResourceData.Name, 1);`<br>3. Verify in the Godot debug console that the dictionary in `InventoryManager` correctly updates. |

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

### Task 5: Save/Load System (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **13h - 14h**| **Save System Design**<br>1. Create a new C\# class `SaveManager.cs` as a singleton.<br>2. Define a save data structure: `public class SaveData` with fields for:<br>    - Player position (`Vector2`)<br>    - Inventory contents (`Dictionary<string, int>`)<br>    - Tech points and unlocked nodes (`Dictionary<string, int>`, `HashSet<string>`)<br>    - World seed (for regenerating the same world) |
| **14h - 15h**| **Save and Load Methods**<br>1. Implement `public void SaveGame(string filePath)`:<br>    - Collect data from GameManager, InventoryManager, and PlayerController.<br>    - Serialize to JSON using `System.Text.Json` or Godot's built-in serialization.<br>    - Write to file in user data directory.<br>2. Implement `public void LoadGame(string filePath)`:<br>    - Read file, deserialize, and restore game state. |
| **15h - 16h**| **Integration and Testing**<br>1. Add save/load hooks: Save on quit, load on start if save file exists.<br>2. Add a simple keybind (e.g., F5 to quick save, F9 to quick load) for testing.<br>3. Test: Harvest resources, save, restart game, load, verify inventory persists.<br>4. **Note:** This basic system prevents having to replay from start during development and testing. |

### Task 6: Aesthetic and Review (2 Hours)

| Duration | Steps |
| :--- | :--- |
| **16h - 17h**| **Art/Sound: Harvesting FX**<br>1. Create a simple **"mining" sound effect** (e.g., a metallic impact) to play when the Left-Click/Harvest action is successful.<br>2. Create a simple **VFX** (e.g., small dust particles) to spawn and disappear where the resource node was harvested.<br>3. Create a short, subtle **"scan successful" sound** for the Hand Scanner action (plays when right-click is held on a valid target). |
| **17h - 18h**| **Final Playtest and Commit**<br>1. Play the entire loop: Land → Scan (get vague data) → Harvest Wood → Harvest both Ores → Check Inventory and HUD.<br>2. Test save/load functionality with a partial playthrough.<br>3. Confirm the hardcoded portal target is visible.<br>4. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 3 Complete: Resource Engine, Inventory, Hand Scanner, and Save System." |
