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
| **4h - 5h** | **Secondary Check (Simplified for VS)**<br>1. For a complete deduction puzzle, implement a second check based on another constant (e.g., Corrosive Index vs. Material Resistance).<br>2. `float requiredR = GameManager.WorldConstants.CorrosiveIndex * 1.5f;` (The deduction formula).<br>3. `bool resistancePass = actualR >= requiredR - 0.01f;`<br>4. **Overall Result:** `bool finalPass = strengthPass && resistancePass;`<br>5. **Note on Third Property:** The GDD mentions Toughness/Tectonics as a potential third check. For VS simplicity, we use only two properties (Strength and Resistance). A third check could be added if time permits, but is not critical for demonstrating the deduction mechanics. |
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
