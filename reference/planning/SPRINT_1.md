This detailed breakdown is formatted as a GitHub Issue, providing a junior developer with clear, tutorial-style instructions to complete **Sprint 1: Project Setup & Data Core (16 Hours)**.

-----

# ⚙️ Sprint 1: Project Setup & Data Core (16 Hours)

## Summary

This sprint establishes the C\# data architecture for the game's core deduction system. We will create the foundational classes for resources, planetary properties, and portal requirements. This data layer is the "Brain" of the game and must be logically sound before visual elements are built.

## 🎯 Goal

A set of fully defined, functional C\# classes (not necessarily Godot nodes yet) that can generate a unique world puzzle and the corresponding material solution.

## 💻 Tech Stack Focus

  * **Godot Engine:** Project creation and configuration.
  * **C\# / .NET:** All core logic and data structures.

-----

## Task Breakdown (16 Hours)

### Task 1: Project Setup and Godot Configuration (2 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Create and Configure Godot Project**<br>1. Create a new Godot 4 project using the **C\#** template.<br>2. Set the renderer to **2D** or **Compatibility** (since we are using 2D for the VS).<br>3. Create a top-level folder named `Scripts` to hold all C\# files.<br>4. Create a main scene (`World.tscn`) and save it. |
| **1h - 2h** | **Editor Workflow Check**<br>1. Open the project in your IDE (e.g., Visual Studio Code or Rider).<br>2. Verify that the Godot C\# solution file (`.sln`) is loaded correctly and that C\# scripts can be added and compiled without error. *Troubleshoot any .NET SDK path issues now.* |

### Task 2: Data Structure - Resource Property (2 Hours)

This class defines a single measurable quality (e.g., "Strength") and its value.

| Duration | Steps |
| :--- | :--- |
| **2h - 3h** | **Create Enumerator and Base Class**<br>1. In the `Scripts` folder, create `Enums.cs`. Define `enum ResourcePropertyType { Strength, Resistance, Toughness, Conductivity, Reactivity }`.<br>2. Create `ResourceProperty.cs`. Define a C\# `struct` named `ResourceProperty` (use `struct` for performance and value semantics). |
| **3h - 4h** | **Define Fields and Helper Methods**<br>1. Add the following fields to `ResourceProperty`:<br>    - `public ResourcePropertyType Type;`<br>    - `public float Value;`<br>    - `public string VagueDescription;` (e.g., "High Integrity")<br>2. Implement a constructor `public ResourceProperty(ResourcePropertyType type, float value)` that sets `Type` and `Value`.<br>3. Implement a private method `SetVagueDescription()` inside the constructor: `if (Value > 7.0f) { VagueDescription = "High"; } else if (Value < 3.0f) { VagueDescription = "Low"; } else { VagueDescription = "Medium"; }` |

### Task 3: Data Structure - Raw Resource (2 Hours)

This is the data object for all gatherable materials in the game.

| Duration | Steps |
| :--- | :--- |
| **4h - 5h** | **Create RawResource Class**<br>1. Create `RawResource.cs`. Define a C\# class `RawResource`.<br>2. Add core descriptive fields:<br>    - `public string Name;` (e.g., "Dull Grey Ore")<br>    - `public string Description;`<br>    - `public int BaseHarvestDifficulty;` (1 for VS). |
| **5h - 6h** | **Implement Property Storage**<br>1. Add the crucial storage field: `public Dictionary<ResourcePropertyType, ResourceProperty> Properties = new();`<br>2. Implement a simple stub method `public void GenerateProperties(int seed)`:<br>    - For the VS, hardcode the properties of one "Low Strength Ore" and one "High Strength Ore" using a simple random or fixed seed logic. (e.g., Low Strength Ore: Strength=2.0, Resistance=5.0. High Strength Ore: Strength=8.0, Resistance=2.0). |

### Task 4: Data Structure - Composite Material (2 Hours)

This handles the combining of materials, the core of the engineering puzzle.

| Duration | Steps |
| :--- | :--- |
| **6h - 7h** | **Composite Class Definition**<br>1. Create `CompositeMaterial.cs`. This class should inherit from `RawResource`.<br>2. Add tracking fields for its inputs:<br>    - `public RawResource PrimaryIngredient;`<br>    - `public RawResource ModifierIngredient;`<br>    - `public float StrengthModifier;` (to track the effect of gas/additive). |
| **7h - 8h** | **Implement Calculation Logic**<br>1. Implement a method `public void CalculateProperties()`.<br>2. **VS Logic:** The Composite's final Strength will be the sum of its ingredients' strength, plus a multiplier from a gas.<br>   - `float baseStrength = PrimaryIngredient.Properties[Strength].Value + ModifierIngredient.Properties[Strength].Value;`<br>   - `float finalStrength = baseStrength * StrengthModifier;`<br>3. Set the Composite's Strength property using this calculated value. |

### Task 5: Data Structure - Planetary Constants (2 Hours)

This defines the unique puzzle for the current run (the "Puzzle Frame").

| Duration | Steps |
| :--- | :--- |
| **8h - 9h** | **Planetary Constants Class**<br>1. Create `PlanetaryConstants.cs`. Define a C\# class `PlanetaryConstants`.<br>2. Add float fields for the 3 key VS properties (from the GDD):<br>    - `public float GravimetricShear;` (Range 0.5 - 5.0)<br>    - `public float CorrosiveIndex;` (Range 0.0 - 14.0)<br>    - `public float TectonicVolatility;` (Range 0.0 - 9.0) |
| **9h - 10h** | **World Generation Logic**<br>1. Implement `public void GenerateWorld(int complexityLevel)`.<br>2. For the VS, hardcode the Level 1 values for the first playthrough:<br>    - `GravimetricShear = 3.2f;` (High but manageable)<br>    - `CorrosiveIndex = 2.0f;` (Low)<br>    - `TectonicVolatility = 1.0f;` (Low)<br>3. This ensures every test run uses the same baseline puzzle. |

### Task 6: Art/Sound - Initial Concept (2 Hours)

Begin the visual design process to guide future art sprints.

| Duration | Steps |
| :--- | :--- |
| **10h - 11h**| **Player & Resource Concepts**<br>1. Create 3 simple visual drafts (sketches) for the **Wayblazer Suit** (2.5D Isometric style). Focus on utility/function.<br>2. Create 3 visual drafts for the primary **Low-Strength Ore** and the **Gas Vent** visual styles. |
| **11h - 12h**| **Machine & UI Concepts**<br>1. Sketch a concept for the **Basic Furnace** (must look like it melts things).<br>2. Sketch a concept for the **Hand Scanner** (what does it look like when the player holds it?).<br>3. Design a minimalist UI color palette (e.g., muted blues/grays with high-contrast text). |

### Task 7: Data Structure - Portal Requirement (2 Hours)

This defines the ultimate goal and the mathematical deduction.

| Duration | Steps |
| :--- | :--- |
| **12h - 13h**| **Portal Requirement Class**<br>1. Create `PortalRequirement.cs`. Define a C\# class `PortalRequirement`.<br>2. Add a dictionary to store the requirements: `public Dictionary<ResourcePropertyType, float> RequiredStats = new();`<br>3. Add fields to store the World Constants (for reference): `public PlanetaryConstants WorldContext;` |
| **13h - 14h**| **Deduction Formula Implementation**<br>1. Implement `public void SetRequirements(PlanetaryConstants constants)`:<br>2. **VS Logic:** Implement the primary deduction formula from the GDD:<br>    - **Foundation Strength:** `RequiredStats[Strength] = constants.GravimetricShear * 2.5f;` (Target: 8.0)<br>    - **Gate Resistance:** `RequiredStats[Resistance] = constants.CorrosiveIndex * 1.5f;` (Target: 3.0)<br>3. This ensures the requirements are generated *from* the world properties. |

### Task 8: Simple Unit Test and Review (2 Hours)

Verify the core deduction math works outside of the Godot environment.

| Duration | Steps |
| :--- | :--- |
| **14h - 15h**| **Unit Test Project (C\#/.NET)**<br>1. Create a separate C\# Console Application project in the solution (e.g., `Wayblazer.Tests`).<br>2. Reference the main Godot C\# assembly (`Wayblazer`).<br>3. Write a single function `TestPortalDeductionMath()`: |
| | a. Instantiate `PlanetaryConstants` and call `GenerateWorld()`.<br>b. Instantiate `PortalRequirement` and call `SetRequirements()`.<br>c. Instantiate a `CompositeMaterial` and manually set its properties to a known "passing" value (e.g., Strength 9.0) and a "failing" value (e.g., Strength 7.0).<br>d. Use a simple `if` check (`if (composite.Properties[Strength].Value > required.RequiredStats[Strength])`) to confirm the pass/fail logic is mathematically correct. |
| **15h - 16h**| **Code Review and Cleanup**<br>1. Review all C\# code for consistent naming conventions and commenting.<br>2. Delete the temporary Console Test project if it's no longer needed, or commit it as a verification tool.<br>3. **Commit Code:** Commit all changes to the Version Control System (VCS) with the message: "Sprint 1 Complete: Initial Data Core Architecture." |

-----

**Status:** **Sprint 1 Complete.**
*Ready to begin Sprint 2: Grid, Player Movement & Voxel Data.*
