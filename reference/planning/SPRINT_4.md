This breakdown is formatted as a GitHub Issue, providing a junior developer with clear, tutorial-style instructions to complete **Sprint 4: Interaction & Scanner UI (16 Hours)**.

-----

# 🖱️ Sprint 4: Interaction & Scanner UI (16 Hours)

## Summary

In this sprint, we enhance the player's primary interaction tool—the **Hand Scanner**—and implement the structure for displaying detailed environmental information. We move the scanning feedback from a temporary debug message (Sprint 3) to a dedicated, persistent UI element, laying the groundwork for the more advanced analysis machines in future sprints.

## 🎯 Goal

The player can effectively use the Hand Scanner (Right Click) to display the **Vague Descriptions** of a resource and the **Planetary Constants** (though still hardcoded to the VS values) in a clean, dedicated UI panel.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** UI design, AnimationPlayer for UI transitions, and input binding.
  * **C\# / .NET:** Data formatting for display, constant access.

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

-----

**Status:** **Sprint 4 Complete.**
*Ready to begin Sprint 5: Field Lab & Analysis Logic.*
