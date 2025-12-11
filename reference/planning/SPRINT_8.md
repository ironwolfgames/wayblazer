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
| **0h - 1h** | **Catalyst Ore Harvest Logic**<br>1. In `ResourceNode.cs`, add a new property: `public string RequiredHarvestTool;` (default is "Kinetic Mining").<br>2. Set the `Catalyst Ore` resource to require **"Gas Siphon"** as its harvest tool.<br>3. In `PlayerController.cs`, modify the harvest check: If the node requires Gas Siphon AND the player doesn't have it unlocked (`!KnowledgeManager.Instance.UnlockedTools.Contains("GasSiphon")`), display a prompt: "Requires Gas Siphon (50 Analysis Points)".<br>4. If the tool is unlocked, allow the harvest and call `InventoryManager.AddItem("Catalyst Ore", 1)`. |
| **1h - 2h** | **Gas Siphon Tool Implementation**<br>1. Instead of a temporary mechanism, create a proper **Gas Siphon Tool** unlock system.<br>2. Create a C\# class `Tool.cs` to represent player tools. Add a tool tracking system to `KnowledgeManager`: `public HashSet<string> UnlockedTools = new();`<br>3. Implement tool unlock: When player has $\ge 50$ **Analysis Tech Points**, they can "research" or "craft" the Gas Siphon from the Research UI or a simple crafting menu.<br>4. Deduct 50 points and set: `KnowledgeManager.Instance.UnlockedTools.Add("GasSiphon");`<br>5. **Better Immersion:** This creates a proper progression gate rather than a hacky workaround. |
| **2h - 3h** | **Art/Sound: Catalyst Ore & Siphon**<br>1. Create the final 2.5D sprite asset for **"Catalyst Ore"** (it should look distinct from Base Ore—e.g., bright purple or glowing).<br>2. Create a unique, high-pitched **"Siphon/Thermal" sound effect** for harvesting this ore, distinct from the dull thud of the Kinetic Mine. |

### Task 2: Gas Injector Scene and Interaction (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Gas Injector Scene Setup**<br>1. Create a new scene `GasInjector.tscn` (Area2D root, Sprite2D, CollisionShape2D). The sprite should suggest infusion (e.g., pipes and a pressurized tank).<br>2. Add two `Marker2D` input slots: `Input_Metal` and `Input_Catalyst`.<br>3. Instance the scene in `World.tscn`. |
| **4h - 5h** | **InjectorManager Script**<br>1. Create a C\# script `InjectorManager.cs` and attach it.<br>2. Define state variables: `public bool HasMetal = false;`, `public bool HasCatalyst = false;`, and `private bool IsCompositing = false;`. |
| **5h - 6h** | **Player Deposit Logic**<br>1. In `PlayerController.cs`, implement interaction for the Injector.<br>2. **Input Logic:** Check inventory and deposit items:<br>    - If player has **Base Metal Bar**, use `TryRemoveItem()` and set `HasMetal = true`.<br>    - If player has **Catalyst Ore**, use `TryRemoveItem()` and set `HasCatalyst = true`.<br>3. Display interaction prompts: `[E] Deposit Metal` and `[E] Deposit Catalyst`. |

### Task 3: Compositing Logic (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Compositing Initiation**<br>1. In `InjectorManager.cs`, implement `CheckForCompStart()`.<br>2. **Comp Condition:** If `HasMetal` and `HasCatalyst` are true, set `IsCompositing = true` and start a timer (e.g., 5.0 seconds—longer than smelting).<br>3. Display a complex visual indicator (e.g., colored lights, pressurized gauge) when `IsCompositing` is true. |
| **7h - 8h** | **Composite Alloy Definition with Pre-Calculated Values**<br>1. Define the final output: **"Composite Alloy"**. This material MUST meet the required Strength of $>8.0$.<br>2. **Use Pre-Calculated Values from Sprint 1:** The exact values for Base Ore (Strength ~2.1), Base Metal Bar (Strength ~4.2 after 1.2x smelting boost), and Catalyst Ore (Strength ~4.0) should already be documented in `VSResourcePresets.cs`.<br>3. **The Math (Already Planned):** In the `CompositeMaterial` class (from Sprint 1):<br>    - `Base Strength = Base Metal Bar.Strength` (4.2f)<br>    - `Catalyst Boost = Catalyst Ore.Strength` (4.0f)<br>    - `Final Strength = Base Strength + Catalyst Boost = 8.2f` ✓ Meets requirement |
| **8h - 9h** | **Compositing Implementation**<br>1. Implement the compositing calculation in `CompositeMaterial.CalculateProperties()` (defined in Sprint 1).<br>2. Apply the formula: `FinalStrength = PrimaryIngredient.Properties[Strength].Value + ModifierIngredient.Properties[Strength].Value`<br>3. Store this final composite property set in the Composite Alloy instance.<br>4. **Verification:** With Base Metal at 4.2 and Catalyst at 4.0, Final Strength = 8.2, which exceeds the 8.0 requirement. ✓ |
| **9h - 10h**| **Compositing Completion and Knowledge**<br>1. Implement `FinishCompositing()` when the timer ends. Reset state.<br>2. **Output:** Call `InventoryManager.AddItem("Composite Alloy", 1)`.<br>3. **Knowledge Gain:** `KnowledgeManager.Instance.TechPoints["Compositing"] += 10;` (New **Compositing Tech Point** category). |

### Task 4: UI/UX Feedback and Testing (6 Hours)

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **HUD Update for New Resources**<br>1. Update the `HUD.tscn` to track the counts of **"Catalyst Ore"** and **"Composite Alloy"**. |
| **11h - 12h**| **Machine Status Display**<br>1. Add a status label above the `GasInjector` in its scene.<br>2. Update the label dynamically:<br>    - Idle: "Needs Metal and Catalyst."<br>    - One Input: "Needs Catalyst/Metal."<br>    - Compositing: "Refining... (5s)"<br>    - Finished: "Composite Alloy Ready\!" |
| **12h - 13h**| **Full Cycle Test: Composite Creation**<br>1. Start with Base Metal Bar (from Sprint 7) and Catalyst Ore (siphoned).<br>2. Deposit both into the Injector. Wait for the process.<br>3. Collect the **Composite Alloy**. Check Inventory. |
| **13h - 14h**| **Deduction Verification**<br>1. **Crucial Test:** Use the **Hand Scanner** (vague) or the **Field Lab** (precise) on the **Composite Alloy**.<br>2. The Scanner UI must show the calculated value: **Strength $\approx 8.2$**. This visually confirms the player has engineered a material that $\mathbf{meets}$ the Portal's $\mathbf{>8.0}$ requirement. |
| **14h - 15h**| **Art/Sound: Injector FX**<br>1. Add sound effects for **"Injector Activation"** (a whoosh/hiss of gas) and **"Compositing Loop"** (pressurized bubbling/mixing sound).<br>2. Add a strong VFX (e.g., colored swirling gas) to the injector while compositing. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 8 Complete: Gas Injector, Compositing Logic, and Required Composite Alloy Created." |
