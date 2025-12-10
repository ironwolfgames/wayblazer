## Revised Vertical Slice Plan: Proving the Procedural Deduction Hook (12 Weeks)

The goal is no longer just to build *a* portal, but to prove that the player can **deduce a unique, procedurally generated solution** for a **Level 1 Portal** using a **procedural tech tree** and **procedurally defined resources**.

### Phase 1: Core Architecture & Resource Engine (Weeks 1-3)

| Focus | Tasks | Goal |
| :--- | :--- | :--- |
| **Data Backbone** | Implement the base classes for all resources (Ore, Gas, Metal, Composite). | Every resource instance has the 5 core properties (Strength, Resistance, Toughness, Conductivity, Reactivity). |
| **ProcGen V1: The Puzzle** | Hardcode **one set of Planetary Constants** (e.g., Gravity 3.2, Corrosive Index 5.0). Hardcode **one Portal Requirement** based on this (e.g., Foundation Strength $\ge$ 8.0, Gate Resistance $\ge$ 5.0). | The "puzzle frame" is ready, even if not fully randomized yet. |
| **Player Input** | WSAD movement, Mouse Look, and implement the basic **Hand Scanner** tool. Left-click/Right-click interaction logic. | Player can move and *vaguely* inspect resources. |

### Phase 2: Mechanics of Deduction (Weeks 4-6)

| Focus | Tasks | Goal |
| :--- | :--- | :--- |
| **Analysis Tools** | Build the **Field Lab** (Tier 1 Tech). This machine takes a raw resource sample and outputs its **exact numerical stats** (e.g., Strength: 4.2). | Player can move from "vague" to "exact" knowledge, proving the first step of the deduction loop. |
| **Planetary Analysis** | Build the **Gravimeter** (Tier 2 Tech). This machine must be built and placed to output the exact value of the **Gravimetric Shear** (e.g., 3.2g). | Player gains the $X$ variable for the Portal Equation (e.g., $3.2 \times 2.5 = 8.0$). |
| **Harvesting V1** | Implement **Kinetic Mining** (default) and **one complex method** (e.g., Thermal Lancing) to show how resources require different tools (Tier 1 Tech requirement). | Player is forced to research a tool to access a key resource. |

### Phase 3: The Engineered Solution (Weeks 7-9)

| Focus | Tasks | Goal |
| :--- | :--- | :--- |
| **Refining Loop** | Implement the **Basic Furnace** (Smelting) and the **Gas Injector** (Composition). | Player can take two raw resources and combine them to create a high-stat composite (e.g., an Alloy with Strength 9.0). |
| **ProcGen V2: The Path** | Implement the **procedural prerequisite logic** for the limited VS tech tree. The progression must be linear: *Basic Scanner $\rightarrow$ Field Lab $\rightarrow$ Gas Siphoning $\rightarrow$ Gas Injector*. | Player must follow a dynamically generated path dictated by the resources needed for the final Portal Solution. |
| **Inventory & Crafting UI** | Functional UI for combining materials based on discovered recipes (e.g., `Metal(4.0)` + `Gas(x1.5)` = `Composite(6.0)`). | Player can execute the synthesis step of the engineering solution. |

### Phase 4: Final Polish & Simulation (Weeks 10-12)

| Focus | Tasks | Goal |
| :--- | :--- | :--- |
| **Portal Construction** | Implement the placement and resource submission for the Level 1 Portal Foundation. | The structure is physically built using the high-stat composite. |
| **The Simulation Core** | Build the **Simulation Core** (Tier 3 Tech). This machine executes the deduction logic: `If Material.Strength < Required.Strength -> FAIL`. | This is the payoff. The player verifies their deduction before risking the run. Feedback must be clear: "Structural Integrity: 112% - PASS" or "Shear Stress Exceeded - FAIL." |
| **Aesthetics & Sound** | Apply the 2.5D Isometric art style to all VS assets (Player, 5 Machines, 3 Resources). Add key sound effects for the Furnace, Gravimeter, and the Portal Simulation success/fail state. | The game must *feel* like a finished product for the duration of the VS demonstration. |

---
**Next Step:** This revised plan ensures we are building the most critical and unique parts of *Wayblazer* first.

**Would you like me to detail the specific data structure (e.g., in a programming language format like C#) for how the Planetary Properties and Resource Characteristics interact to simplify the programming phase?**
