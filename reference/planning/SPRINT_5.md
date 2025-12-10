This breakdown is formatted as a GitHub Issue, providing a junior developer with clear, tutorial-style instructions to complete **Sprint 5: Field Lab & Analysis Logic (16 Hours)**. This sprint introduces the first crucial **Tier 1 Technology** required for the game's core deduction loop: transitioning from vague data to precise, numerical analysis.

-----

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
| **3h - 4h** | **Knowledge Tracking Manager**<br>1. Create a new C\# class `KnowledgeManager.cs`. Make it a singleton.<br>2. Define a dictionary: `public Dictionary<string, bool> DataUnlocked = new();` (Key: ResourceName, Value: True/False).<br>3. Define a dictionary for Tech Points: `public Dictionary<string, int> TechPoints = new();` (Key: "Analysis", "Harvesting", etc., Value: Points). |
| **4h - 5h** | **The Analysis Process**<br>1. In `FieldLabManager.cs`, create a method `StartAnalysis()`.<br>2. **VS Logic:** This is a simple, instant process for the VS:<br>    - **Unlock Data:** `KnowledgeManager.Instance.DataUnlocked[InputResource.Name] = true;`<br>    - **Gain Knowledge:** `KnowledgeManager.Instance.TechPoints["Analysis"] += 10;`<br>3. Print a debug message: "Analysis Complete: Data for [ResourceName] Unlocked\!" |
| **5h - 6h** | **Resource Disposal**<br>1. After analysis, the resource is spent. The Field Lab needs a visual output (a screen or light) to show the analysis result.<br>2. Create a small label on the Field Lab scene to show the analysis result for 5 seconds (e.g., "BASE ORE: STRENGTH 2.1"). |
| **6h - 7h** | **Art/Sound: Field Lab Assets**<br>1. Create the final 2.5D sprite asset for the **Field Lab** (a table or device with an input port and a screen).<br>2. Create sound effects for **"Resource Deposit"** (a click/thud) and **"Analysis Complete"** (a computer chime). |

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

-----

**Status:** **Sprint 5 Complete.**
*Ready to begin Sprint 6: Planetary Analysis & Deduction Input.*
