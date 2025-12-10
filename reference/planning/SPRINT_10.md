This breakdown is formatted as a GitHub Issue, providing a junior developer with clear, tutorial-style instructions to complete **Sprint 10: Portal Construction & UI (16 Hours)**. This sprint focuses on the final engineering steps: using the expertly crafted materials to construct the ultimate goal—the **Portal Foundation**.

-----

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

-----

**Status:** **Sprint 10 Complete.**
*Ready to begin Sprint 11: Simulation Core & Win State.*
