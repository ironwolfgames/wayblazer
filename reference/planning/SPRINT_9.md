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
| **5h - 6h** | **VS Tech Tree Definition**<br>1. Create a static C\# class `TechTreeData.cs` to hold the entire VS structure (list of `TechNode` objects).<br>2. Define the core VS nodes (noting Tier 1 vs Tier 2+):<br>    - **Node 1: Gas Siphon Tool:** ID: "Unlock\_GasSiphon" (Cost: 50 Analysis). **Effect:** Allows harvesting Catalyst Ore (implemented in Sprint 8).<br>    - **Node 2: Planetary Survey:** ID: "Unlock\_Observatory" (Cost: 25 Analysis, Prereq: N/A).<br>    - **Node 3: Advanced Compositing:** ID: "Unlock\_GasInjector" (Cost: 10 Compositing, 10 Smelting).<br>    - **Node 4: Portal Construction:** ID: "Unlock\_PortalBuild" (Cost: 50 Total Points, Prereq: "Unlock\_GasInjector").<br>3. **Note on Tier 1 Tech:** The Field Lab and Basic Furnace are freely available (Tier 1 tech requires no research unlock). Only Tier 2+ machines require research as defined above. |
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
