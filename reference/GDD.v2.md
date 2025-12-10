# Wayblazer: Game Design Document (v2.0)

**Genre:** Roguelite Survival / Engineering Simulation
**View:** 2.5D Isometric (Tile-based)
**Target Platform:** PC (Steam)
**Core Loop:** Land → Analyze → Adapt → Build → Escape

## 1. High-Level Concept

**Wayblazer** is a roguelite engineering game about blazing a trail for a nomadic civilization fleeing a cosmic threat. You are a **Wayblazer**—a scout sent ahead to procedural worlds to construct stable warp portals for the colony fleet.

Unlike traditional survival games where you memorize recipes (e.g., "Copper + Tin = Bronze"), Wayblazer relies on **procedural chemistry**. Every run features new elements with unknown properties. You must **deduce** the physical laws of the planet, discover resources, and engineer a portal capable of withstanding that specific world’s harsh environment before the "Entropy" (the pursuing threat) consumes the system.

### The "Hook"
* **Procedural Physics & Chemistry:** "Iron" on World A might be weak and brittle; "Iron" on World B might be a superconductor. You cannot rely on wiki knowledge; you must rely on your in-game scanner and deduction.
* **The Engineering Deduction:** Building a portal is not just depositing items. You must deduce the stress requirements. If the world has 3x Gravity, your foundation needs high *Compressive Strength*. If you guess wrong, the portal collapses during the activation sequence.

---

## 2. Gameplay Structure

### The Roguelite Run
A "Run" consists of traveling through a sector of 5-8 procedurally generated worlds of increasing complexity.
1.  **Arrival:** The Wayblazer pod lands. The player has limited time (The Entropy Timer) to work.
2.  **Scouting:** Scan local flora/minerals to identify randomized properties.
3.  **Tech-Up:** Use local resources to generate research data, unlocking a procedural tech tree adapted to the environment.
4.  **Construction:** Build infrastructure to refine materials into high-grade composites.
5.  **Portal Commission:** Construct the 3 Portal Stages (Foundation, Core, Gate).
6.  **Departure:** Activate the portal. If stable, the Fleet warps in, refuels, and you warp to the next world.
7.  **Failure:** If the timer runs out or the player dies, the current "Wayblazer" is lost. The Fleet loses fuel/population (Meta-Health) and must jump to a desperate, harder backup sector.

### The Threat: "The Entropy"
A cosmic instability chasing the fleet.
* **Visual:** The edges of the map slowly disintegrate into digital/cosmic noise.
* **Mechanic:** Acts as a soft time limit. As Entropy gets closer, planetary storms increase, and local fauna becomes aggressive.

---

## 3. World Generation & Environment

Worlds are generated with unique **Planetary Constants** that dictate the puzzle solution.

| Factor | Description | Gameplay Impact |
| :--- | :--- | :--- |
| **Gravity** | 0.5g to 5.0g | Affects movement speed and **Portal Foundation** strength requirements. |
| **Atmosphere** | Corrosive, Inert, Volatile | Affects machine durability and **Portal Resistance** requirements. |
| **Tectonics** | Stable vs. Volatile | Affects ground stability and **Portal Toughness** requirements. |
| **Biome** | Crystal, Fungal, Metallic, Aquatic | Determines the visual palette and base resource distribution. |

**Discovery:** The player does not know these constants immediately. They must build a **Planetary Analyzer** to reveal the exact Gravity or Atmospheric density, which gives clues on how to build the portal.

---

## 4. Procedural Resource System

This is the core complexity engine. In every run, resources are randomized.

### 4.1. Unidentified Resources
When a player lands, resources are named with procedural gibberish or generic descriptors (e.g., "Dull Grey Ore," "Unknown Resin A").
* **The Scanner Tool:** Hovering reveals a range (e.g., "Strength: ??? (Low)").
* **The Analyzer Machine:** Processing a sample reveals exact stats (e.g., "Strength: 3, Heat Resistance: 8").

### 4.2. Characteristics
Every material possesses a stat block generated at the start of the world.

* **Strength (0-10):** Structural integrity (Tensile/Compressive). Vital for construction.
* **Resistance (0-10):** Ability to withstand environment (Corrosion/Heat/Cold).
* **Toughness (0-10):** Durability against impact/wear.
* **Conductivity (0-10):** (New) Efficiency in transmitting power.
* **Reactivity (0-10):** (New) Volatility for fuels or explosives.

### 4.3. Resource Types
1.  **Ores:** Smelted into Rigid Materials (Metals/Ceramics).
    * *ProcGen:* An ore might be "High Strength/Low Conductive" in one run, and "Low Strength/High Explosive" in another.
2.  **Flora/Wood:** Burned for Energy or processed for Biopolymers.
    * *ProcGen:* Woods vary in Burn Duration and Temperature.
3.  **Gases:** Condensed for Chemical additives.
    * *Modifier System:* Gases act as multipliers in crafting. Adding "Noble Gas A" to a metal alloy might grant "+2 Resistance".

---

## 5. The Tech Tree (Procedural)

Instead of a static tree, the Tech Tree adapts to the resources found.

* **The "Eureka" Moment:** The player does not unlock "Iron Smelting." They unlock "High-Temp Smelting" because they analyzed a resource that requires high heat.
* **Knowledge Currency:** Gained by scanning, analyzing, and successfully crafting new composites.
* **Nodes:**
    * *Tier 1:* Basic Survival (Simple Furnace, Hand Tools).
    * *Tier 2:* Processing (Crushers, Mixers, Gas Condensers).
    * *Tier 3:* Compositing (Alloy Forges, Nanofabricators).
    * *Tier 4:* Portal Tech (Grav-Stabilizers, warp Coils).

---

## 6. The Deduction Mechanic (Portal Construction)

This is the "Boss Fight" of the engineering loop. The player must **deduce** the correct material composition for the portal.

### 6.1. The Requirements
The game does *not* tell the player: "Bring 50 Iron Bars."
The game tells the player: **"Portal Foundation requires Net Strength > 8.0 to resist Planetary Gravity."**

### 6.2. The Design Process
1.  **Analyze World:** Player sees Gravity is "High (Level 4)."
2.  **Check Blueprint:** Level 4 Gravity requires Material Strength 8.
3.  **Check Resources:**
    * "Red Ore" yields Metal with Strength 3. (Too weak).
    * "Blue Ore" yields Metal with Strength 5. (Too weak).
4.  **Hypothesis/Compositing:** The player must research an **Alloy Foundry** to mix Red and Blue metals.
    * *Drafting:* Player mixes Red + Blue + "Gas X" (which has a +1 Strength modifier).
    * *Result:* "Composite Alloy" with Strength 9.
5.  **Build:** Player constructs the Foundation using the new Alloy.

### 6.3. The Stress Test
Before the portal can be fully activated, the player runs a "Simulation."
* **Visual:** The Portal frame vibrates and hums.
* **Feedback:** If materials are too weak, hairline fractures appear (visual cue) and a warning flashes: "CRITICAL FAILURE: SHEAR STRESS EXCEEDED."
* **Fix:** Player must reinforce the structure or invent a stronger material.

---

## 7. Controls & User Interface

**Input:** Mouse & Keyboard (Rebindable). Gamepad support (Twin Stick style).

**HUD:**
* **Top Left:** "Entropy" Timer / Threat Level.
* **Bottom Left:** Status (Health, Suit Energy).
* **Bottom Center:** Hotbar (Tools: Pickaxe, Scanner, Builder Tool).
* **Right Side:** Quest Log (Current Portal Stage Requirements).
* **Tab Key:** Inventory & Tech Tree.

**Building Interface:**
* Grid-based placement (Factorio/Rimworld style).
* "Ghost" placement allowed before resources are committed.
* Rotate machines with 'R'.

---

## 8. Meta-Progression (Between Runs)

The "Fleet" is the persistent element.

1.  **The Archive:** While physical items are lost between runs, **Resource Data** is saved. If you encounter "Red Ore" again in a future run, you remember its stats (unless the "Universal Remix" modifier is active).
2.  **Suit Upgrades:** Spend "Fleet Credits" earned from successful worlds to upgrade the Wayblazer suit:
    * Faster movement speed.
    * Larger inventory size.
    * Better initial scanner (shows more detailed stats instantly).
3.  **Starting Loadout:** Unlock different "Pods" (e.g., The Mining Pod starts with a drill; The Science Pod starts with a better analyzer).

---

## 9. Vertical Slice Scope (for Prototype)

To prove this design works, the Vertical Slice will contain:

* **1 Biome:** "The Shattered Plains" (Standard gravity, breathable air).
* **3 Random Ores:** Low, Medium, High strength variants.
* **2 Random Trees:** Fast burn (low energy), Slow burn (high energy).
* **1 Portal Goal:** Build a portal requiring **Strength 4** materials.
* **Loop:**
    1.  Land.
    2.  Scan ore (find it has Strength 2).
    3.  Build Furnace.
    4.  Realize Strength 2 is insufficient.
    5.  Find "Dense Gas" vent (+2 Strength modifier).
    6.  Build Condenser to bottle gas.
    7.  Build Mixer to combine Molten Ore + Gas = Strength 4 Alloy.
    8.  Build Portal. Win.
