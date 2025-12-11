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
| **6h - 7h** | **Screen Shake for Impact (Subtle)**<br>1. In `PlayerController.cs` (or a dedicated `CameraShake.cs` script), implement a function `ShakeCamera(float duration, float magnitude)`. This uses `Camera2D.Offset` to apply a random, high-frequency movement.<br>2. Apply a **subtle** shake (e.g., 0.1s, mag 1-2) when **Smelting/Compositing** finishes. *(Note: Keep it subtle to avoid annoyance during batch production.)*<br>3. Apply a medium shake (e.g., 0.3s, mag 5) on **Simulation SUCCESS/FAIL** to emphasize the result.<br>4. **Optional:** Consider making shake intensity a setting if time permits. |
| **7h - 8h** | **Particle Effects (VFX)**<br>1. Create and integrate final particle systems for key events:<br>    - **Resource Collection:** Small, bright particles that fly from the resource node to the player's position on harvest.<br>    - **Smelting:** Plumes of smoke/heat that puff out when the Furnace timer resets. |
| **8h - 9h** | **UI Feedback & Responsiveness**<br>1. Implement simple color transitions/flashes for successful actions:<br>    - When an item is added to the inventory, the corresponding inventory label should briefly flash **Green**.<br>    - When a research node is unlocked, the **Tech Points** label that paid for it should briefly flash **Red** (to show points spent). |
| **9h - 10h**| **Machine Interaction Prompts**<br>1. Refine all interaction prompts (`[E] Deposit Ore`, `[F] Activate Portal`) to use a clean font, have a small background panel, and fade in/out smoothly when the player enters/exits the machine's interaction range. |

### Task 4: Tutorial, Bug Fixing & Final Review (6 Hours)

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **Basic Tutorial Tooltips**<br>1. Add contextual tooltips/hints for first-time interactions:<br>    - When player first approaches a resource node: "Hold Right-Click to scan, Left-Click to harvest"<br>    - When player first approaches Field Lab: "Deposit resources here to analyze their exact properties"<br>    - When player first opens Research UI: "Spend Tech Points to unlock new machines and tools"<br>2. Use simple pop-up labels that appear once and can be dismissed. |
| **11h - 12h**| **Portal Activation Sequence**<br>1. Enhance the `Portal Activation` transition (from Sprint 11):<br>    - When "Activate Portal" is pressed, the screen should immediately fade to white (using a `ColorRect` and `AnimationPlayer`).<br>    - Play the final, powerful sound effect (energy surge).<br>2. This provides a dramatic climax before the `WinScene`. |
| **12h - 13h**| **Bug Fixing Buffer**<br>1. Reserve time for fixing bugs discovered during final testing.<br>2. Common areas to check: UI state consistency, machine state machines (prevent double-activation), inventory edge cases (full inventory, zero resources).<br>3. Test edge cases: What if player tries to submit wrong materials? What if they spam interact buttons? |
| **13h - 14h**| **Optional: Pause Menu & Settings**<br>1. **If time permits:** Add a basic pause menu (ESC key) with Resume/Quit options.<br>2. **If time permits:** Add a simple settings menu with volume sliders for Music and SFX.<br>3. **Note:** These are nice-to-have for demo purposes but not critical for VS functionality. |
| **14h - 15h**| **Final Code and Comment Review**<br>1. Ensure all temporary debug code and `GD.Print` calls are removed or commented out.<br>2. Review all C\# scripts one last time for clarity and consistency.<br>3. Verify all hardcoded VS values are documented in `VSResourcePresets.cs` for reference. |
| **15h - 16h**| **Final Commit**<br>1. **Tag Repository:** Create a final Git tag (e.g., `VS_1.0_Final`).<br>2. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 12 Complete: Tutorial Tooltips, Bug Fixes, Polish, and Vertical Slice Finalized." |
