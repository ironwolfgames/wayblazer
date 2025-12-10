## The Critical Interconnected Systems

These four elements must work together seamlessly to create the compelling gameplay experience defined in the updated design document:

### 1. Procedural World Generation (Environment & Constraints)
This system generates the **Planetary Constants** (Gravity, Atmosphere, Tectonics) that define the engineering problem the player must solve.
* **Why it's Critical:** It establishes the **difficulty** and the **required solution** for the run. For example, high Gravity forces the **Portal Foundation** to require high Strength/Compressive values. The world is the *puzzle frame*.

---

### 2. Procedural Resource Generation (The Tools)
This system generates the randomized **characteristics** of Ore, Wood, Gas, and Ground materials (Strength, Resistance, Toughness, etc.).
* **Why it's Critical:** It determines the **available ingredients** and forces the player to discard pre-set knowledge. If "Iron" is weak in this run, the player must discover which locally generated element will satisfy the strength requirement. The resources are the *puzzle pieces*.

---

### 3. Procedural Tech Tree Generation (The Path)
This system dictates which tools and machines (Smelters, Condensers, Alloy Forges) the player can research and in what order, based on the resources they analyze.
* **Why it's Critical:** It manages the **progression rate** and forces *adaptation*. If the primary metal requires extremely high heat to smelt, the tech tree should prioritize unlocking a "High-Temp Smelting" node before a basic furnace. The tech tree is the *puzzle manual*.

---

### 4. Procedural Portal Requirements (The Goal)
This system sets the final required material properties (e.g., Net Strength > 8.0) for the **Portal Foundation**, **Energy Core**, and **Gate** based on the generated World Complexity/Planetary Constants.
* **Why it's Critical:** It defines the **winning condition** and the final **engineering deduction**. This is the metric the player must optimize their **Composite Resources** to meet, ensuring the core challenge changes with every run. The requirements are the *puzzle key*.

**In short:** The goal of *Wayblazer* is to use the **Procedural Tech Tree (The Path)** to refine the **Procedural Resources (The Tools)** into composites that meet the specific **Procedural Portal Requirements (The Goal)** imposed by the **Procedural World (The Constraints)**. Breaking any of these links breaks the core roguelite engineering deduction loop.

---

### 1. The Puzzle Frame: Planetary Properties
These are the hidden variables generated at the start of a run. They drive the difficulty and dictate the Portal Requirements.
* **Scale:** Most are normalized to a **0.0 - 10.0** Gameplay Scale for calculation, but displayed with flavor units.

| Property Name | Flavor Unit | Gameplay Range (0-10) | Description | Impact on Portal |
| :--- | :--- | :--- | :--- | :--- |
| **Gravimetric Shear** | G-Force ($g$) | 0.5 - 5.0 | The downward force and tidal stress of the planet. | Determines **Foundation Compressive Strength**. High gravity requires denser alloys. |
| **Atmospheric Pressure** | Atmospheres (atm) | 0.0 - 100.0 | The weight/density of the air. | Determines **Gate Seal Integrity**. Vacuum requires airtight seals; High pressure requires crush-resistant casing. |
| **Thermal Variance** | Kelvin ($K$) | 50 - 1200 | The ambient temperature range (and fluctuation). | Determines **Material Thermal Resistance**. Extreme heat melts weak frames; extreme cold shatters brittle ones. |
| **Corrosive Index** | pH / acidity | 0.0 - 14.0 | The chemical hostility of the air/rain. | Determines **Material Chemical Resistance**. High index requires non-reactive coatings (gold/glass/plastic). |
| **Geomagnetic Flux** | Tesla ($T$) | 0.1 - 20.0 | The strength of the planet's magnetic field. | Determines **Core Shielding**. High flux interferes with electronics, requiring lead/dense shielding. |
| **Bio-Density** | Biomass/m² | 0 - 10 | How active/aggressive the local microscopic life is. | Determines **Bio-foul Protection**. High density eats organic-based components over time. |
| **Tectonic Volatility** | Richter Scale | 0.0 - 9.0 | Frequency of ground tremors. | Determines **Foundation Flexibility**. Rigid structures crack here; the portal needs shock absorbers (Toughness). |
| **Solar Radiation** | Sieverts/hr | 0 - 500 | Intensity of stellar radiation hitting the surface. | Determines **Core Cooling**. Radiation heats up the core; needs higher cooling efficiency. |
| **Wind Shear** | km/h | 0 - 300 | Average wind speed and gust strength. | Determines **Structure Aerodynamics**. High wind requires streamlined or anchored designs. |
| **Entropy Proximity** | % | 0 - 100% | How close the "Threat" is to this sector. | **Hard Time Limit Modifier.** Accelerates weather events and resource decay. |

---

### 2. Resource Acquisition Methods (The "Harvesting" Matrix)
Instead of just "right-click to mine," the method depends on the **State of Matter** and **Environmental Context** of the resource. The game checks the resource's tags and assigns one of these interaction methods.

1.  **Kinetic Mining (Standard):** User holds a drill/pick. Effective on **Brittle/Solid** nodes.
    * *Context:* Standard stone, iron deposits.
2.  **Thermal Lancing:** User directs a high-heat beam. Required for **High-Hardness/Metallic** nodes that break drills.
    * *Context:* Titanium veins, Obsidians.
3.  **Resonance Shattering:** User tunes a sonic emitter to match the frequency of the object. Required for **Crystalline** structures that shatter if hit physically.
    * *Context:* Energy crystals, fragile glass forests.
4.  **Gas Siphoning:** User places a pump on a vent. Required for **Gaseous** resources.
    * *Context:* Steam vents, Toxic gas pockets.
5.  **Atmospheric Condensation:** User builds a passive collector (net/tower) to pull moisture/particulates from the air.
    * *Context:* Water on hot planets (humidity), floating spore clouds.
6.  **Fluid Dredging:** User places a pump in a liquid body.
    * *Context:* Acid lakes, Oil pools, Water.
7.  **Thermal Sublimation:** User must **heat** a solid resource to turn it into gas, then capture the gas.
    * *Context:* "Frozen Oxygen" rocks, Dry Ice deposits.
8.  **Chemical Leaching:** User sprays a solvent on the ground, then vacuums up the slurry. Required for **Dispersed/Soil-based** minerals.
    * *Context:* Rare earth metals mixed into sand/dirt.
9.  **Bio-Tapping:** User attaches a spigot to flora without destroying it.
    * *Context:* Sap from giant trees, blood from fungal stalks.
10. **Plasma Cutting:** High-energy requirement. Used to slice through **Ancient Scrap/Wreckage** found on the planet.
    * *Context:* Harvesting pre-existing structures or crashed probes.

---

### 3. The Procedural Tech Tree & Prerequisites
The Tech Tree is generated backward: **Problem → Solution**.
* *Logic:* If the world contains `Ore_HighMelt` (Melting Point > 2000K), the tree *must* generate a `Blast Furnace` node.

**Prerequisite Rule:**
* To unlock Tier X technology, you must possess **Knowledge** gained from using Tier X-1 technology OR analyzing Tier X resources.

#### Technology Categories & Ranges

**A. Smelting & Refining (State Change)**
* **Stone Hearth:** (Tier 0) Smelts Low Melt (< 500K) ores.
* **Basic Furnace:** (Tier 1) Smelts Medium Melt (< 1200K).
* **Blast Furnace:** (Tier 2) Smelts High Melt (< 2500K). *Prereq: Refractory Bricks.*
* **Arc Furnace:** (Tier 3) Smelts Ultra-High Melt (> 2500K). *Prereq: High Energy Power.*
* **Cryo-Stabilizer:** (Variant) required if an ore explodes when heated; processes "Cold-Forged" metals.

**B. Composition & Alloying (Material Improvement)**
* **Basic Mixer:** Combines 2 solids (e.g., Copper + Tin).
* **Gas Injector:** Infuses a metal with a gas (e.g., Iron + Carbon Gas). *Prereq: Gas Siphoning.*
* **Molecular Weaver:** (Tier 3) Aligns crystalline structures. Increases **Strength** without increasing **Weight**.
* **Chemical Bath:** Dips materials to add **Corrosion Resistance**. *Prereq: Chemical Leaching.*

**C. Power & Utility (Infrastructure)**
* **Biomass Burner:** (Tier 0) Low efficiency.
* **Geothermal Tap:** (Tier 1) Only available if `Geothermal` tag exists on map.
* **Solar Array:** (Tier 2) Efficiency scales with `Solar Radiation` property.
* **RTG (Isotope Gen):** (Tier 3) Efficiency scales with collected Radioactive Ores.

**D. Analysis & Science (The "Brain")**
* **Hand Scanner:** (Tier 0) Gives "Low/Med/High" vague readings.
* **Field Lab:** (Tier 1) Gives exact stats of **Resources**.
* **Planetary Observatory:** (Tier 2) Gives exact stats of **Planetary Properties** (Gravity, Flux, etc.).
* **Simulation Core:** (Tier 3) Allows running "Virtual Portal Tests" without spending resources.

---

### 4. Portal Requirements & The Deduction Gameplay
You asked if this is "Guess and Check" or "Logical Deduction."
**Design Decision:** It is **Logical Deduction** with a "Verify" step. Guessing is too expensive (wasting rare resources on a failed portal is a "run killer").

#### The Deduction Mechanics
The player deduces requirements by using **Measurement Tools** to find the unknown variables ($X$) in the Portal Equation.

**The Universal Equations (Known to Player):**
1.  *Foundation Strength Needed* = $Planetary Gravity \times 2.5$
2.  *Shielding Needed* = $Geomagnetic Flux \times 1.5$ + $Solar Radiation / 100$

**The Gameplay Loop:**
1.  **Initial State:** The player knows the equations, but the variables (Gravity, Flux) are unknown ($?$).
2.  **Investigation (Finding X):**
    * *Method A (Observation):* Player observes falling rocks. They fall fast. "Gravity seems high." (Vague).
    * *Method B (Tooling):* Player builds a **Gravimeter**. It outputs: "Gravity: 3.2g".
3.  **Calculation:** Player math: $3.2 \times 2.5 = 8.0$.
    * **Result:** "I need a Foundation with Strength 8.0."
4.  **Engineering (Finding Y):**
    * Player scans "Red Ore" -> Strength 4.0. (Too low).
    * Player scans "Blue Gas" -> Strength Modifier x1.5.
    * Player scans "Dense Rock" -> Strength Modifier +2.0.
    * *Synthesis:* Smelt Red Ore (4.0) + Dense Rock (+2.0) = Alloy (6.0). Still too low.
    * *Synthesis 2:* Alloy (6.0) infused with Blue Gas (x1.5) = **Hyper-Alloy (9.0)**.
    * **Result:** 9.0 > 8.0. This will work.
5.  **Simulation (The Safety Net):**
    * Before building the massive structure, the player runs a **Simulation**.
    * System checks: `If (Material.Strength < World.Gravity * 2.5) -> FAIL`.
    * Feedback: "Simulation Successful: Structural Integrity at 112%."

**Possible Portal Requirements (The Final Exam):**

* **Foundation:** Needs **Compressive Strength** (vs Gravity) and **Anchoring** (vs Tectonics).
* **Frame/Gate:** Needs **Tensile Strength** (vs Portal Vacuum) and **Corrosion Resistance** (vs Atmosphere).
* **Energy Core:** Needs **Thermal Resistance** (vs Self-Heat) and **Shielding** (vs Magnetic Flux).
* **Cooling System:** Needs **Heat Dissipation** (vs Solar Radiation + Core Heat).
