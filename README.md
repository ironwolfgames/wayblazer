# Wayblazer

A single player game about blazing the way forward for a civilization in pursuit of knowledge and a new home.

## Overview

### High Level Characteristics

- 2d graphics and third person view, either isometric or top-down/side-on
- Tile-based environments and objects
- Almost everything in the game is procedurally generated, from the world resources to the available buildings and technologies/upgrades to the environment itself
- Primary player fantasy is one of exploration and advancement
- Possibly have an element of deduction in determining how to build the portal on each world
- WSAD, left-click/right-click OR left control stick and A/B buttons for main interactions

### Setting

You are sent to a new world to discover its natural resources and report back on its suitability for living, developing technology, etc. You begin with a basic set of tools that you will use to deduce which materials are available, what can be constructed using those materials and eventually you will build a portal to travel either back to your home world or on to another world to catalog.

## Mechanics

- The portal has particular requirements for being built. The requirements may change based on the particular world it is being built on. For example, if the world has harsher atmosphere or stronger gravity, the materials may need to be stronger or more resistant and the amount of energy needed to power the portal may increase or decrease based on various factors.
- You start with tools for scanning for resources, for basic construction of tools/machines (mines, condensers, lumber mills, etc.), and for analyzing resources.
- The core game loop:
  - Scan for resources
  - Gather resources
  - Analyze resources
  - Use resources to research and then build stronger, better tools/machines and upgrades for your main tools.
  - Repeat until you have the ability to meet the requirements for building a portal

### Characteristics of Resources

- Energy (heat, electrical, magic, etc.)
  - Smelting Level: 0-5 (rating for how well it can smelt various ores into metals)
  - Compositing Level: 0-10 (rating for how well it can be used to create composite resources)
  - Power Level: 0-10 (rating for how much power it can provide to a tool or machine)
- Ore
  - All ore can be smelted into a metal resource
  - Required smelting level to smelt
- Wood
  - All wood can be burnt to produce energy, but different woods will produce energy with different energy characteristic levels
  - In general, energy from burning wood cannot be used to reach high smelting, fuel, or power levels or for any compositing
- Metal, Wood, and Ground
  - Strength: 0-10 (tensile, compressive, shear, torsion)
  - Resistance: 0-10 (to chemicals, corrosion, thermal shock, and wear)
  - Toughness: 0-10 (how much it can resist scratches, impacts, cracks)
- Gas
  - Can have a +1-5 level modifier for any energy characteristic (e.g. Smelting +1, Compositing +2, Fuel +1, Power +3)
  - Can be used as an ingredient in making composite resources

### Portal Requirements

The portal requirements vary based on the size/complexity of the world:

**Portal Foundation Material Requirements**:

| World Complexity/Gravity | Material Strength | Material Resistance | Material Toughness |
| - | - | - | - |
| 1 | 2 | 3 | 1 |
| 2 | 4 | 3 | 2 |
| 3 | 6 | 4 | 3 |
| 4 | 8 | 4 | 4 |
| 5 | 10 | 5 | 5 |

 **Portal Energy Core Requirements**:

| World Complexity/Gravity | Energy Power Level | Material Strength | Material Resistance | Material Toughness |
| - | - | - | - | - |
| 1 | 2 | 2 | 4 | 4 |
| 2 | 4 | 2 | 5 | 4 |
| 3 | 6 | 3 | 6 | 5 |
| 4 | 8 | 3 | 7 | 5 |
| 5 | 10 | 4 | 8 | 6 |

 **Portal Gate Requirements**:

| World Complexity/Gravity | Material Strength | Material Resistance | Material Toughness |
| - | - | - | - |
| 1 | 2 | 2 | 3 |
| 2 | 3 | 4 | 5 |
| 3 | 4 | 6 | 7 |
| 4 | 6 | 8 | 9 |
| 5 | 8 | 10 | 10 |

### Basic Abilities

The following abilities are available to the player from the get-go on arrival at a new planet.

- Scan for all level 1 resources
- Harvest all wood with hardness level 1
- Harvest all ore resources with a required smelting level of 1
- Harvest all ground resources where all characteristics are at or below level 1
- Build the Basic Smelting Furnace from some level 1 ground resources
- Smelt level 1 metals from level 1 ores in the Basic Smelting Furnace

### Progression

- As you scan, you gain knowledge to unlock higher levels of scanning
- As you harvest, you gain knowledge to unlock higher levels of harvesting
- As you smelt, you gain knowledge to unlock higher levels of smelting
- Once you reach a certain amount of knowledge across all disciplines you can research compositing and gas condensation
- As you condense gas and compose resources, each of those areas gain you knowledge in those areas to unlock higher levels
- Eventually you will be able to construct a portal foundation, a portal energy core, and finally the portal gate, each from composite resources and then travel through it.

## Possible Expansion Ideas

- Co-op?
- Procgen'd vehicles within the tech tree
- Procgen'd animals
- Procgen'd plants
