This breakdown is formatted as a GitHub Issue, providing a junior developer with clear, tutorial-style instructions to complete **Sprint 8: Advanced Refining (Composition) (16 Hours)**. This sprint introduces the critical **Compositing** mechanic needed to create the high-stat material that meets the calculated Portal Requirement.

-----

# 🧪 Sprint 8: Advanced Refining (Composition) (16 Hours)

## Summary

This sprint implements the **Gas Injector** machine. This machine is crucial for taking a basic refined material (like the **Base Metal Bar** from Sprint 7) and enhancing its properties by infusing it with a modifying element (**Catalyst Ore**), finally creating the high-strength **Composite Alloy** required to build the Portal Foundation.

## 🎯 Goal

  * Implement the **Gas Siphoning** mechanism to acquire the modifying element (**Catalyst Ore** in its raw form).
  * Design, implement, and integrate the **Gas Injector** machine (our stand-in for an alloying or compositing device).
  * Implement the core **Compositing Logic**: Base Metal + Catalyst Ore $\rightarrow$ Composite Alloy with boosted Strength.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** New resource acquisition method, advanced machine input validation.
  * **C\# / .NET:** Property calculation for composite materials, Tech Point consumption/gain.

-----

## Task Breakdown (16 Hours)

### Task 1: Acquiring the Modifying Element (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Catalyst Ore Harvest Logic**<br>1. In `ResourceNode.cs`, add a new property: `public string RequiredHarvestTool;` (default is "Kinetic Mining").<br>2. Set the `Catalyst Ore` resource to require a different method (e.g., `Thermal Lancing` or, for VS simplicity, **"Gas Siphoning"**).<br>3. In `PlayerController.cs`, modify the harvest check: If the node requires **Gas Siphoning**, display a prompt: "Requires Gas Siphon." |
| **1h - 2h** | **Gas Siphon Tool Stub**<br>1. Since we skip building the Siphon tool in the VS, create a temporary mechanism.<br>2. In `PlayerController.cs`, if the player Left-Clicks a `Catalyst Ore` node AND has sufficient "Analysis Tech Points" (e.g., $\ge 50$), allow the harvest, simulating a high-tech tool purchase/unlock.<br>3. Call `InventoryManager.AddItem("Catalyst Ore", 1)`. |
| **2h - 3h** | **Art/Sound: Catalyst Ore & Siphon**<br>1. Create the final 2.5D sprite asset for **"Catalyst Ore"** (it should look distinct from Base Ore—e.g., bright purple or glowing).<br>2. Create a unique, high-pitched **"Siphon/Thermal" sound effect** for harvesting this ore, distinct from the dull thud of the Kinetic Mine. |

### Task 2: Gas Injector Scene and Interaction (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Gas Injector Scene Setup**<br>1. Create a new scene `GasInjector.tscn` (Area2D root, Sprite2D, CollisionShape2D). The sprite should suggest infusion (e.g., pipes and a pressurized tank).<br>2. Add two `Marker2D` input slots: `Input_Metal` and `Input_Catalyst`.<br>3. Instance the scene in `World.tscn`. |
| **4h - 5h** | **InjectorManager Script**<br>1. Create a C\# script `InjectorManager.cs` and attach it.<br>2. Define state variables: `public bool HasMetal = false;`, `public bool HasCatalyst = false;`, and `private bool IsCompositing = false;`. |
| **5h - 6h** | **Player Deposit Logic**<br>1. In `PlayerController.cs`, implement interaction for the Injector.<br>2. **Input Logic:** Check inventory and deposit items:<br>    - If player has **Base Metal Bar**, use `TryRemoveItem()` and set `HasMetal = true`.<br>    - If player has **Catalyst Ore**, use `TryRemoveItem()` and set `HasCatalyst = true`.<br>3. Display interaction prompts: `[E] Deposit Metal` and `[E] Deposit Catalyst`. |

### Task 3: Compositing Logic (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Compositing Initiation**<br>1. In `InjectorManager.cs`, implement `CheckForCompStart()`.<br>2. **Comp Condition:** If `HasMetal` and `HasCatalyst` are true, set `IsCompositing = true` and start a timer (e.g., 5.0 seconds—longer than smelting).<br>3. Display a complex visual indicator (e.g., colored lights, pressurized gauge) when `IsCompositing` is true. |
| **7h - 8h** | **Composite Alloy Definition**<br>1. Define the final output: **"Composite Alloy"**. This material MUST meet the required Strength of $>8.0$.<br>2. **The Math:** In the `CompositeMaterial` class (from Sprint 1), implement the final VS logic:<br>    - `Base Strength = Base Metal Bar.Strength (e.g., 4.2f)`<br>    - `Catalyst Strength Modifier = Catalyst Ore.Strength (e.g., 2.0f)`<br>    - `Final Strength = Base Strength + Catalyst Strength Modifier = 6.2f` (Wait, this is not enough\! See next step.) |
| **8h - 9h** | **The Final Strength Boost**<br>1. **Crucial Correction:** To meet the 8.0 requirement (with Base Metal at $\approx 4.2$), the **Catalyst Ore** must apply a **multiplicative** boost or a very high additive boost.<br>2. **Revised Logic:** Assume the Catalyst Ore, when processed, provides a **+4.0** additive boost. *Check the exact value needed: $8.0 - 4.2 = 3.8$. So, define the Catalyst to grant $\mathbf{+4.0}$ Strength.*<br>3. `Final Strength = Base Metal Strength + 4.0f = 8.2f`. This meets the goal. Store this final composite property set. |
| **9h - 10h**| **Compositing Completion and Knowledge**<br>1. Implement `FinishCompositing()` when the timer ends. Reset state.<br>2. **Output:** Call `InventoryManager.AddItem("Composite Alloy", 1)`.<br>3. **Knowledge Gain:** `KnowledgeManager.Instance.TechPoints["Compositing"] += 10;` (New **Compositing Tech Point** category). |

### Task 4: UI/UX Feedback and Testing (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **HUD Update for New Resources**<br>1. Update the `HUD.tscn` to track the counts of **"Catalyst Ore"** and **"Composite Alloy"**. |
| **11h - 12h**| **Machine Status Display**<br>1. Add a status label above the `GasInjector` in its scene.<br>2. Update the label dynamically:<br>    - Idle: "Needs Metal and Catalyst."<br>    - One Input: "Needs Catalyst/Metal."<br>    - Compositing: "Refining... (5s)"<br>    - Finished: "Composite Alloy Ready\!" |
| **12h - 13h**| **Full Cycle Test: Composite Creation**<br>1. Start with Base Metal Bar (from Sprint 7) and Catalyst Ore (siphoned).<br>2. Deposit both into the Injector. Wait for the process.<br>3. Collect the **Composite Alloy**. Check Inventory. |
| **13h - 14h**| **Deduction Verification**<br>1. **Crucial Test:** Use the **Hand Scanner** (vague) or the **Field Lab** (precise) on the **Composite Alloy**.<br>2. The Scanner UI must show the calculated value: **Strength $\approx 8.2$**. This visually confirms the player has engineered a material that $\mathbf{meets}$ the Portal's $\mathbf{>8.0}$ requirement. |
| **14h - 15h**| **Art/Sound: Injector FX**<br>1. Add sound effects for **"Injector Activation"** (a whoosh/hiss of gas) and **"Compositing Loop"** (pressurized bubbling/mixing sound).<br>2. Add a strong VFX (e.g., colored swirling gas) to the injector while compositing. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 8 Complete: Gas Injector, Compositing Logic, and Required Composite Alloy Created." |

-----

**Status:** **Sprint 8 Complete.**
*Ready to begin Sprint 9: ProcGen Tech Tree V2 & Unlocks.*
