# Wayblazer: Comprehensive Game Design Document (GDD)

## 1. Overview and Vision

### 1.1 High-Level Pitch

**Wayblazer** is a single-player, 2.5D isometric exploration and engineering game where the player must solve a unique, procedurally generated deduction puzzle on every new planet to build a portal and advance civilization. The core loop is about transforming vague environmental data into precise engineering solutions.

### 1.2 Player Fantasy

The player is a lone pioneer, a space engineer, and a scientist. They are tasked with blazing the way forward for a civilization by mastering alien environments, deducing unknown physics, and synthesizing materials that meet the harsh, procedurally generated requirements of each new world.

### 1.3 Core Characteristics

* **Viewpoint:** 2.5D Isometric (Tile-based environments).
* **Core Mechanic:** Deduction and Engineering Synthesis (not simple resource grinding).
* **Procedural Generation:** World characteristics, resource properties, technology path, and portal requirements are unique to each run.
* **Control Scheme:** WSAD movement, Left-click (Interaction/Harvest), Right-click (Scanning/Analysis).

---

## 2. Core Gameplay Loop: The Deduction Cycle

The central experience of Wayblazer follows a repeatable, five-step deduction cycle on every planet.

### 2.1 The Wayblazer Cycle (L.D.E.S.A.)

| Step | Action | Tools/Systems Used | Output |
| :--- | :--- | :--- | :--- |
| **1. Land & Assess** | Establish the initial base and gain vague data. | Hand Scanner (Right-Click) | Vague Resource Descriptions ("Low Strength," "High Resistance"). |
| **2. Deduce & Research** | Use Tier 1 and 2 tech to turn vague data into precise formulas. | Field Lab, Planetary Observatory, Research UI | Exact material stats (e.g., Strength: 4.2), Exact planetary constants (e.g., Gravimetric Shear: 3.2), and the **Final Portal Requirement** (e.g., Foundation Strength $\ge 8.0$). |
| **3. Engineer & Synthesize** | Use the discovered formulas to craft the required material. | Basic Furnace, Gas Injector, Inventory | A single, high-stat **Composite Alloy** that meets the required threshold (e.g., Final Strength: 8.2). |
| **4. Simulate & Verify** | Construct the portal and run a virtual test on the engineered material. | Portal Foundation, Simulation Core | **PASS/FAIL** report confirming the deduction and engineering success. |
| **5. Advance & Repeat** | Activate the portal and travel to the next, more complex world. | Portal Gate | Progression to the next complexity level. |

---

## 3. Procedural Core Systems

The deduction puzzle is built entirely on the interaction between randomly generated World Constants and Resource Characteristics.

### 3.1 Planetary Constants (The Puzzle Frame)

Each planet is defined by a unique set of global constants. These values are the variables ($X, Y, Z$) used in the deduction formulas to determine the Portal's requirements.

| Constant (VS Focus) | Description | VS Value (Tier 1) | Deduction Role |
| :--- | :--- | :--- | :--- |
| **Gravimetric Shear** | The intensity of the world's gravity and resulting structural stress. | $3.2 \text{ g}$ | Dictates the **Material Strength** requirement. |
| **Corrosive Index** | The atmospheric acidity and material decay rate. | $2.0 \text{ pH}$ | Dictates the **Material Resistance** requirement. |
| **Tectonic Volatility** | The frequency of ground movement and material stress endurance. | $1.0 \text{ tremor}$ | Dictates the **Material Toughness** requirement. |

### 3.2 Resource Characteristics (The Puzzle Pieces)

Every single raw and processed material in the game is defined by these five core numerical properties.

| Property | Effect | Vague Reading (Scanner) | Precise Reading (Field Lab) |
| :--- | :--- | :--- | :--- |
| **Strength** | Resistance to sheer force and crushing. | Low / Medium / High Integrity | Float Value (e.g., 2.1, 4.2) |
| **Resistance** | Resistance to atmospheric, thermal, or corrosive damage. | Low / Medium / High Purity | Float Value (e.g., 5.0, 7.3) |
| **Toughness** | Resistance to impact and shattering (ductility). | Low / Medium / High Elasticity | Float Value |
| **Conductivity** | Ability to transfer heat or electrical energy. | Low / Medium / High Flow | Float Value |
| **Reactivity** | How violently it interacts with other materials. | Low / Medium / High Volatility | Float Value |

### 3.3 The Deduction Formula (VS Example)

The player must discover the Planetary Constant and use the formula (unlocked via research) to determine the exact required material value.

| Portal Component | Required Material Property | VS Deduction Formula | VS Calculated Requirement |
| :--- | :--- | :--- | :--- |
| **Foundation** | Material Strength | $Strength_{REQ} = Gravimetric Shear \times 2.5$ | $3.2 \times 2.5 = 8.0$ |
| **Gate** | Material Resistance | $Resistance_{REQ} = Corrosive Index \times 1.5$ | $2.0 \times 1.5 = 3.0$ |

---

## 4. Mechanics and Tiered Technology

The player uses a tiered set of tools, each gated by the Tech Tree, to progress through the deduction loop.

### 4.1 Tier 0: Basic Exploration

| Tool | Function | VS Usage |
| :--- | :--- | :--- |
| **WSAD/Click** | Movement and interaction. | Basic harvesting of readily available resources (Soft Wood, Base Ore). |
| **Hand Scanner** | Vague analysis of nearby materials. | Displays "Low," "Medium," or "High" for resource properties and the general Planetary Constants (e.g., "Gravity is Strong"). |

### 4.2 Tier 1: Precise Analysis

| Machine | Unlock | Cost/Prereq | Function |
| :--- | :--- | :--- | :--- |
| **Field Lab** | Base Analysis V2 Research | 15 Analysis Points | **Turns Vague $\rightarrow$ Precise.** Consumes raw resource to unlock its exact numerical properties for all future scans. |
| **Basic Furnace** | Smelting Research | Basic Ground Resources | **Smelting.** Consumes Ore + Wood (Fuel) to produce a slightly stronger **Metal Bar**. |

### 4.3 Tier 2: Deduction Input

| Machine | Unlock | Cost/Prereq | Function |
| :--- | :--- | :--- | :--- |
| **Planetary Observatory** | Planetary Survey Research | 25 Analysis Points | **Deduction Input.** Measures the planetary constant (e.g., Gravimetric Shear) to reveal the true required portal threshold (e.g., $8.0$ Strength). |
| **Gas Injector** | Advanced Compositing Research | 10 Smelting + 10 Compositing Points | **Compositing.** Consumes a Metal Bar + a Modifying Element (e.g., Catalyst Ore) to create the high-stat **Composite Alloy** (e.g., Strength $\approx 8.2$). |

### 4.4 Tier 3: Final Verification

| Machine | Unlock | Cost/Prereq | Function |
| :--- | :--- | :--- | :--- |
| **Simulation Core** | Portal Construction Research | 50 Total Points | **Verification.** Runs a virtual test on the constructed Portal Foundation against the planetary constants. Provides a detailed PASS/FAIL report. |

---

## 5. Progression and Tech Tree

Progression is driven by the accumulation and spending of specialized **Tech Points** (Knowledge). This system ensures players must engage in all aspects of the game (science, mining, crafting) to advance.

### 5.1 Knowledge/Tech Point Categories

| Category | Gain Points By... | Drives Unlocks For... |
| :--- | :--- | :--- |
| **Analysis** | Scanning, using the Field Lab, measuring planetary data. | Advanced analysis tools, Planetary Observatory. |
| **Smelting** | Using the Basic Furnace to process ore. | Higher-level smelting techniques, refining efficiency. |
| **Compositing** | Using the Gas Injector to create composite materials. | Advanced alloying, complex resource synthesis. |

### 5.2 Tech Tree Gating (Vertical Slice)

The player must follow a sequence of research unlocks, gated by the specialized knowledge points.

| Tech Node ID | Cost | Prerequisite | Effect |
| :--- | :--- | :--- | :--- |
| **Unlock\_PreciseAnalysis** | 15 Analysis | None | Unlocks Field Lab to show exact resource stats. |
| **Unlock\_Observatory** | 25 Analysis | None | Allows placement of the Planetary Observatory. |
| **Unlock\_GasInjector** | 10 Smelting, 10 Compositing | Unlock\_PreciseAnalysis | Allows construction of the Gas Injector. |
| **Unlock\_PortalBuild** | 50 Total Points | Unlock\_GasInjector | Allows construction of the Portal Foundation. |

---

## 6. Vertical Slice (VS) Scope Summary

The 12-Sprint Vertical Slice demonstrates a full, successful run of the deduction cycle (L.D.E.S.A.) on a Complexity 1 world.

### 6.1 VS Feature Checklist

| System | Status (Sprint) | Goal Achieved |
| :--- | :--- | :--- |
| **Data Architecture** | Complete (S1) | C# classes for Resources, Properties, Constants, and Deduction Formulas are linked. |
| **World/Player** | Complete (S2) | Character movement, tile-based world, and basic resource nodes are implemented. |
| **Resource Economy** | Complete (S3, S7, S8) | Raw harvest $\rightarrow$ Base Metal Bar $\rightarrow$ Final **Composite Alloy** synthesis is functional. |
| **Deduction Tools** | Complete (S4, S5, S6) | Hand Scanner (Vague), Field Lab (Precise Resource Data), and Planetary Observatory (Deduction Input) are implemented. |
| **Tech Tree** | Complete (S9) | Research UI, Knowledge Point tracking, and machine gating are functional. |
| **Win Condition** | Complete (S10, S11) | Portal Foundation placement, **Material Verification** against the calculated requirement, **Simulation Core** check, and Win Scene transition are implemented. |
| **Aesthetics** | Complete (S12) | Final 2.5D art, UI polish, sound effects, music, and screen shake are integrated. |
