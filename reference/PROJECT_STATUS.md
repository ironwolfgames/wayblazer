# Wayblazer: Project Status & Implementation Plan

**Last Updated:** 2026-05-28

---

## 1. Project Overview

**Wayblazer** is a single-player 2.5D isometric exploration and engineering game built in **Godot 4 (C#)**. The player lands on procedurally generated planets, deduces unknown physics through scientific tools, engineers composite materials to meet portal requirements, and advances civilization by building portals to new worlds.

The core gameplay loop (L.D.E.S.A.) is: **Land & Assess → Deduce & Research → Engineer & Synthesize → Simulate & Verify → Advance & Repeat**.

---

## 2. Architecture Summary

The solution (`Wayblazer.sln`) consists of five projects:

| Project | Framework | Purpose |
|---------|-----------|---------|
| **Wayblazer** (Godot) | .NET 8.0 | Main game — scenes, scripts, assets |
| **Wayblazer.Core** | .NET 9.0 | Shared models, generators, utilities (no Godot dependency) |
| **Wayblazer.World** | .NET 9.0 | CLI tool to generate and preview world configs |
| **Wayblazer.Configurator** | .NET 9.0 | CLI tool to generate world config JSON files |
| **Wayblazer.TilesetProcessor** | .NET 8.0 | CLI tool to convert PNG tilesets to WFC-compatible JSON |

Additionally:
- **lib/wave-function-collapse**: Git submodule for WFC tile solving
- **addons/world_generator_tools**: Godot editor plugin for in-editor world generation

---

## 3. Current State (What's Done)

### Sprint 1: Project Setup & Data Core ✅ COMPLETE

All foundational data classes are implemented and functional:

| Component | File | Status |
|-----------|------|--------|
| Resource Property Type enum | `Scripts/ResourcePropertyType.cs` | ✅ Complete |
| Resource Property | `Scripts/ResourceProperty.cs` | ✅ Complete (Godot Resource, vague descriptions) |
| Raw Resource | `Scripts/RawResource.cs` | ✅ Complete (Godot Resource, name + kind + properties) |
| Resource Kind enum | `Scripts/ResourceKind.cs` | ✅ Complete (Ore, Wood, Gas, Composite) |
| Composite Resource | `Scripts/CompositeResource.cs` | ✅ Complete (merges inputs, type-specific combination rules) |
| Planetary Constants | `Scripts/PlanetaryConstants.cs` | ✅ Complete (gravity, pressure, corrosion, tectonics, temperature) |
| Portal Requirement | `Scripts/PortalRequirement.cs` | ✅ Complete (factory method from planetary constants) |

### Sprint 2: Grid, Player Movement & World ✅ COMPLETE

| Component | File/Scene | Status |
|-----------|------------|--------|
| World Generator | `Scripts/WorldGenerator.cs` | ✅ Functional (noise-based heightmap, biome assignment, decoration placement) |
| Noise Service | `Scripts/NoiseService.cs` | ✅ Complete (FastNoiseLite wrapper) |
| Noise Layer Config | `Scripts/NoiseLayerConfig.cs` | ✅ Complete (exported Godot Resource) |
| Biome System | `Scripts/BiomeType.cs`, `BiomeRange.cs` | ✅ Complete (10 biome types, height/equator mapping) |
| Decoration System | `Scripts/EnvironmentalDecorationPlacementConfig.cs` | ✅ Complete (noise-threshold based placement) |
| Player Controller | `Scripts/PlayerController.cs` | ✅ Complete (CharacterBody2D, WASD movement, sprint, harvest input, animations) |
| Game Camera | `Scripts/GameCamera.cs` | ✅ Complete (smooth follow, world bounds limiting) |
| Resource Nodes | `Scripts/ResourceNode.cs` + scenes | ✅ Complete (harvestable nodes with animated sprites) |
| Global Random | `Scripts/GlobalRandom.cs` | ✅ Complete (seeded PRNG) |
| Game Manager | `Scripts/GameManager.cs` | ⚠️ Minimal (initializes constants and empty resource arrays, no deeper logic) |
| Music Manager | `Scripts/MusicManager.cs` | ✅ Functional (background track looping) |
| Constants/Paths | `Scripts/Constants.cs` | ✅ Complete (asset path registry) |
| World Scene | `Scenes/world.tscn` | ✅ Configured (tileset, camera, noise configs, player, music) |
| Player Scene | `Scenes/player.tscn` | ✅ Complete (animated sprite atlases for idle/run/interact) |
| Object Scenes | `Scenes/Objects/` | ✅ 5 scenes (pine_tree, birch_tree, fir_tree, oak_tree, gold_ore) |
| Editor Plugin | `addons/world_generator_tools/` | ✅ Complete (Generate World button in editor) |

### Wayblazer.Core Library ✅ COMPLETE

The backend procedural generation system is fully implemented:

- **Models**: SystemObject, Resource, ResourceInfo, Energy, EnergyInfo, Building, BuildingAction, Action, Benefit, Upgrade, EnvironmentalObject
- **Generators**: WorldGenerator (generates full world config with energy, resources, buildings, upgrades), WorldGeneratorConfigGenerator (creates config from complexity/seed)
- **Config**: WorldConfig, WorldGeneratorConfig, ResourceNameConfig
- **Utilities**: RandomUtility, DictionaryUtility, StringUtility

### CLI Tools ✅ COMPLETE

- **Wayblazer.Configurator**: Generates `world-config-N.json` files from name configs
- **Wayblazer.World**: Loads configs and generates/prints full world data
- **Wayblazer.TilesetProcessor**: Converts PNG tilesets to WFC JSON format

---

## 4. What's NOT Done (Remaining Work)

### Sprint 3: Resource Engine & ProcGen V1 🟨 IN PROGRESS

| Feature | Status | Notes |
|---------|--------|-------|
| GameManager singleton (full Autoload) | ❌ Not done | Current implementation is minimal; needs proper singleton pattern, resource/preset initialization |
| Inventory Manager | ❌ Not done | No inventory system exists |
| Inventory HUD/UI | ❌ Not done | No UI scenes or scripts |
| Hand Scanner (vague deduction) | ❌ Not done | Right-click scanning not implemented |
| VS Resource Presets | ❌ Not done | Hardcoded Base Ore, Catalyst Ore, Soft Wood presets |
| Save/Load System | ❌ Not done | No persistence |
| Objective System | ❌ Not done | No guided objectives |

### Sprint 4: Interaction & Scanner UI ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Dedicated Scanner UI panel | ❌ |
| Planetary Constants display (vague) | ❌ |
| Scanner animation/sound feedback | ❌ |
| Interaction prompt system | ❌ |

### Sprint 5: Field Lab & Analysis Logic ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Field Lab machine (building/scene) | ❌ |
| Precise resource analysis (vague → exact values) | ❌ |
| Analysis Points generation | ❌ |
| Resource knowledge database (discovered stats) | ❌ |

### Sprint 6: Planetary Analysis & Deduction Input ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Planetary Observatory machine | ❌ |
| Measuring planetary constants precisely | ❌ |
| Deduction formula reveal to player | ❌ |
| Portal requirement calculation display | ❌ |

### Sprint 7: Basic Crafting & Inventory ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Basic Furnace machine | ❌ |
| Smelting mechanic (Ore + Wood → Metal Bar) | ❌ |
| Crafting UI | ❌ |
| Smelting Points generation | ❌ |

### Sprint 8: Advanced Refining (Composition) ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Gas Injector machine | ❌ |
| Composite material synthesis | ❌ |
| Advanced crafting UI | ❌ |
| Compositing Points generation | ❌ |

### Sprint 9: ProcGen Tech Tree V2 & Unlocks ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Research UI | ❌ |
| Knowledge Point tracking (Analysis, Smelting, Compositing) | ❌ |
| Tech node unlocking | ❌ |
| Machine gating by research | ❌ |

### Sprint 10: Portal Construction & UI ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Portal Foundation placement | ❌ |
| Construction ghost/hologram | ❌ |
| Material assignment UI | ❌ |
| Portal build sequence | ❌ |

### Sprint 11: Simulation Core & Win State ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Simulation Core machine | ❌ |
| Virtual portal test (PASS/FAIL) | ❌ |
| Win scene transition | ❌ |
| Detailed results report | ❌ |

### Sprint 12: Aesthetic Polish & Juiciness ❌ NOT STARTED

| Feature | Status |
|---------|--------|
| Final 2.5D art integration | ❌ |
| UI polish and theming | ❌ |
| Sound effects (mining, scanning, machines) | ❌ |
| VFX (particles, hologram, portal activation) | ❌ |
| Screen shake and juice | ❌ |

---

## 5. Known Issues & Technical Debt

| Issue | Location | Description |
|-------|----------|-------------|
| WFC disabled | `WorldGenerator.cs` | WFC tile painting path exists but `useWFC = false`; falls back to direct biome-tile mapping |
| CompositeResource incomplete | `CompositeResource.cs` | Throws `NotImplementedException` for unknown property types in `CombineProperties` |
| GameManager too thin | `GameManager.cs` | Only initializes default planetary constants and empty resource arrays; needs full game state management |
| No namespace consistency | `PlayerController.cs`, `GameManager.cs` | Some scripts use global namespace while others use `Wayblazer` namespace |
| No tests | `Tests/` folder | Directory exists but contains no test files |
| Framework mismatch | `.csproj` files | Godot project targets .NET 8.0, Core library targets .NET 9.0 |
| Core library disconnected | N/A | `Wayblazer.Core` world generation is not integrated into the Godot game; they exist as parallel systems |

---

## 6. Implementation Plan

### Phase A: Complete the Resource Economy (Sprints 3–4) — ~32 hours

**Goal:** Player can harvest resources, see them in inventory, and scan them for vague/precise data.

#### A1: Game State & Inventory (Sprint 3 partial)
1. Refactor `GameManager.cs` into a proper Autoload singleton with:
   - `PlanetaryConstants` generation from seed
   - `PortalRequirement` calculation
   - VS Resource preset initialization (Base Ore, Catalyst Ore, Soft Wood)
2. Create `InventoryManager.cs` (Autoload singleton):
   - Dictionary-based storage (resource name → count)
   - Add/Remove/Has/Count methods
   - `OnInventoryUpdated` event for UI binding
3. Create `HUD.tscn` (CanvasLayer):
   - Resource count labels (3 VS resources)
   - Portal requirement target display
   - Objective text label
4. Wire harvesting to inventory (update `PlayerController.HandleHarvestInput()`)
5. Create `ObjectiveManager.cs` (Autoload) for guided progression

#### A2: Scanner & Deduction UI (Sprint 4)
1. Implement right-click scanning in `PlayerController.cs`
2. Create `ScannerUI.tscn` — overlay panel showing:
   - Resource name + vague property descriptions
   - Planetary constants (vague: "Gravity feels strong")
3. Add scan sound effect and visual feedback
4. Implement interaction prompt ("Press E to harvest", "Right-click to scan")

### Phase B: Machines & Crafting (Sprints 5–8) — ~64 hours

**Goal:** Player can build machines, analyze resources precisely, smelt metals, and synthesize composite alloys.

#### B1: Field Lab (Sprint 5)
1. Create `Machine.cs` base class (placeable, interactive, state machine)
2. Create `FieldLab.tscn` scene + `FieldLabMachine.cs` script
3. Implement precise analysis: consume resource → reveal exact numerical stats
4. Create `KnowledgeDatabase.cs` — tracks which resources have been analyzed
5. Generate Analysis Points on use

#### B2: Planetary Observatory (Sprint 6)
1. Create `Observatory.tscn` + `ObservatoryMachine.cs`
2. Reveal precise planetary constants (Gravimetric Shear = 3.2g)
3. Unlock deduction formula display (Strength_REQ = Gravity × 2.5)
4. Show calculated portal requirement in HUD

#### B3: Basic Furnace (Sprint 7)
1. Create `Furnace.tscn` + `FurnaceMachine.cs`
2. Implement smelting recipe: Ore + Wood (fuel) → Metal Bar
3. Create `CraftingUI.tscn` — input/output slots, progress bar
4. Generate Smelting Points on use
5. Metal Bar inherits boosted properties from base ore

#### B4: Gas Injector (Sprint 8)
1. Create `GasInjector.tscn` + `GasInjectorMachine.cs`
2. Implement compositing: Metal Bar + Catalyst Ore → Composite Alloy
3. Use `CompositeResource.CalculateProperties()` for stat combination
4. Generate Compositing Points on use

### Phase C: Tech Tree & Gating (Sprint 9) — ~16 hours

**Goal:** Machines are gated behind research, requiring knowledge points to unlock.

1. Create `KnowledgeManager.cs` (Autoload):
   - Track Analysis, Smelting, Compositing points
   - Tech node definitions with costs and prerequisites
2. Create `ResearchUI.tscn`:
   - Visual tech tree (nodes with connections)
   - Point spending interface
   - Unlock notifications
3. Gate machine placement behind tech unlocks:
   - Field Lab: 15 Analysis Points
   - Observatory: 25 Analysis Points
   - Gas Injector: 10 Smelting + 10 Compositing
   - Portal Build: 50 Total Points

### Phase D: Win Condition (Sprints 10–11) — ~32 hours

**Goal:** Player can build the portal, verify their engineered material, and win the game.

#### D1: Portal Construction (Sprint 10)
1. Create `PortalFoundation.tscn` + `PortalMachine.cs`
2. Implement placement system (holographic ghost → built)
3. Create material assignment UI (drag composite alloy onto portal slot)
4. Portal requires: Foundation (Strength) + Gate (Resistance)

#### D2: Simulation & Win (Sprint 11)
1. Create `SimulationCore.tscn` + `SimCoreMachine.cs`
2. Implement virtual test: compare material stats vs. portal requirements
3. Generate detailed PASS/FAIL report with percentages
4. On PASS: trigger win scene (portal activation VFX → victory screen)
5. On FAIL: provide diagnostic feedback (which stat is too low)

### Phase E: Polish (Sprint 12) — ~16 hours

**Goal:** The game feels complete and satisfying to play.

1. Integrate final art assets (per VERTICAL_SLICE_ASSETS.md)
2. Implement VFX: mining particles, furnace smoke, hologram, portal energy
3. Add sound effects for all interactions (harvest, scan, craft, portal)
4. Add music tracks (exploration, victory)
5. Screen shake on harvest and portal activation
6. UI theming (sci-fi panels, custom cursor, progress bars)
7. Enable WFC tile rendering for polished terrain transitions

---

## 7. Integration Tasks (Cross-Cutting)

These tasks span multiple sprints and should be addressed as the phases progress:

| Task | Description | Priority |
|------|-------------|----------|
| **Bridge Core ↔ Godot** | Integrate `Wayblazer.Core` WorldGenerator output into the Godot GameManager for building/action/upgrade data | Medium |
| **Save/Load** | Implement JSON-based persistence (inventory, knowledge, discovered stats, world seed) | Medium |
| **Namespace cleanup** | Standardize all scripts to `Wayblazer` namespace | Low |
| **WFC activation** | Enable WFC tile painting once tileset metadata is configured | Low |
| **Unit tests** | ✅ `Wayblazer.Core.Tests` — 209 deterministic tests | Done |
| **Input mapping** | Add proper input actions for scan (right-click), place machine, open research UI | High |

---

## 8. Deterministic Test Suite

A pure C# test project (`Wayblazer.Core` + `tests/Wayblazer.Core.Tests`) provides **209 deterministic tests** that define acceptance criteria for each remaining sprint. These tests run without the Godot runtime.

**Run tests:** `dotnet test tests/Wayblazer.Core.Tests`

| Test Class | Sprint | Tests | Validates |
|------------|--------|-------|-----------|
| `Sprint03_InventoryTests` | 3 | 13 | Add/remove items, quantity tracking, events, edge cases |
| `Sprint04_ScannerTests` | 4 | 6 | Vague descriptions, planetary scan, no-prerequisite scanning |
| `Sprint05_FieldLabTests` | 5 | 7 | Analysis unlocks precise data, earns points, tracks state |
| `Sprint06_ObservatoryTests` | 6 | 10 | Measure constants, deduction formulas, requirement calculation |
| `Sprint07_SmeltingTests` | 7 | 9 | Furnace crafting, ingredient consumption, property combination |
| `Sprint08_CompositingTests` | 8 | 7 | Gas Injector, advanced synthesis, machine gating |
| `Sprint09_TechTreeTests` | 9 | 12 | Knowledge points, prerequisites, unlock chain, machine gating |
| `Sprint10_11_PortalVerificationTests` | 10–11 | 10 | Pass/fail verification, percentages, simulation report |
| `EndToEnd_FullCycleTests` | All | 3 | Complete L.D.E.S.A. cycle, failure case, skip prevention |

The `EndToEnd_FullCycleTests.FullVerticalSlice_CompletePlaythrough` test simulates an entire winning playthrough from landing to portal activation, validating the complete deduction puzzle chain works correctly.

---

## 9. Dependency Graph

```
Sprint 3 (Inventory/Harvest)
    ├── Sprint 4 (Scanner UI)
    │       └── Sprint 5 (Field Lab) ──┐
    │               └── Sprint 6 (Observatory) ──┐
    └── Sprint 7 (Furnace) ──────────────────────┤
            └── Sprint 8 (Gas Injector) ─────────┤
                                                  ├── Sprint 9 (Tech Tree)
                                                  │       └── Sprint 10 (Portal)
                                                  │               └── Sprint 11 (Simulation/Win)
                                                  │                       └── Sprint 12 (Polish)
```

---

## 10. Estimated Timeline

| Phase | Sprints | Estimated Hours | Cumulative |
|-------|---------|-----------------|------------|
| A: Resource Economy | 3–4 | 32 hrs | 32 hrs |
| B: Machines & Crafting | 5–8 | 64 hrs | 96 hrs |
| C: Tech Tree | 9 | 16 hrs | 112 hrs |
| D: Win Condition | 10–11 | 32 hrs | 144 hrs |
| E: Polish | 12 | 16 hrs | 160 hrs |

**Total remaining:** ~160 hours of development work.

**Already completed:** Sprints 1–2 (~8 hours actual) + Core library + CLI tools + World generation + Editor plugin.

---

## 11. Recommended Next Steps (Immediate)

1. **Refactor GameManager** to become the central game state authority with proper Autoload registration
2. **Create InventoryManager** with events for UI binding
3. **Build the HUD scene** with resource counts and objective display
4. **Wire harvesting → inventory** so the player can see progress
5. **Implement the Hand Scanner** (right-click → vague property popup)

These five tasks complete the core "harvest and observe" loop that all subsequent systems build upon.
