# Wayblazer: Godot Implementation Tasks

**Purpose:** Step-by-step tasks for implementing the remaining game features in Godot 4 (C#).

The core game logic (inventory, crafting, scanning, tech tree, portal verification, etc.) is implemented and tested in `src/Wayblazer.GameLogic/`. These tasks cover the Godot-side integration: scenes, UI, input handling, assets, and wiring the logic systems into the engine.

**Reference Documents:**
- `VERTICAL_SLICE_PLAN.md` — Sprint details with code examples
- `MECHANICS_DETAILS.md` — Full game design (planetary properties, harvesting, tech tree)
- `PROCGEN_TUTORIAL.md` — Procedural generation implementation guide
- `PROCGEN_PROTO.md` — WFC and biome tile placement specifics
- `VERTICAL_SLICE_ASSETS.md` — Complete art asset list

---

## Current State (Sprints 1–2 Complete)

**What exists in Godot:**
- `world.tscn` — Main world scene with TileMap
- `player.tscn` — Player character with movement
- `WorldGenerator.cs` — Noise-based height/biome map generation
- `PlayerController.cs` — Character movement (CharacterBody2D)
- `GameCamera.cs` — Camera following player
- `GameManager.cs` — Singleton managing game state
- `ResourceNode.cs` — Harvestable resource scene instances
- `NoiseService.cs` / `GlobalRandom.cs` — Seeded generation utilities
- 5 object scenes (trees, ore) in `Scenes/Objects/`
- TinySwords tileset and placeholder graphics

**What does NOT exist yet:**
- No UI/HUD system
- No inventory display
- No scanner, field lab, observatory, or crafting UI
- No interaction system (player can't harvest/interact)
- No tech tree UI
- No portal construction scene
- No save/load integration
- No objective/quest tracking UI

---

## Sprint 3: Resource Engine & ProcGen V1

### 3.1 — GameManager Integration with GameLogic Library

| # | Task | Type |
|---|------|------|
| 1 | Add project reference from `Wayblazer.csproj` to `Wayblazer.GameLogic.csproj` | Configuration |
| 2 | Update `GameManager.cs` to instantiate `InventorySystem`, `ScannerSystem`, `TechTreeSystem` from GameLogic library | Code |
| 3 | Create `VSResourcePresets.cs` defining the 3 VS resources (Base Ore, Catalyst Ore, Soft Wood) using `RawResource` from GameLogic | Code |
| 4 | Wire `ResourceNode.cs` to reference `RawResource` instances from the GameLogic library | Code |
| 5 | Register GameManager systems as accessible singletons (or via static accessor) | Code |

### 3.2 — Harvesting Interaction System

| # | Task | Type |
|---|------|------|
| 1 | Create `InteractionArea.cs` — an Area2D on the player that detects nearby ResourceNodes | Code |
| 2 | Add input action `"interact"` in Project Settings → Input Map (E key or gamepad button) | Configuration |
| 3 | Implement harvest logic: on interact, check `HarvestSystem.CanHarvest()`, consume node, add to `InventorySystem` | Code |
| 4 | Add harvest animation/particles: node shrinks and particles emit on successful harvest | Visual |
| 5 | Add harvest sound effect (pick/chop sound) | Audio |
| 6 | Implement harvest cooldown (0.5s between harvests) | Code |
| 7 | Show floating text "+1 Base Ore" on harvest using a Label with tween animation | Visual |

### 3.3 — Inventory HUD

| # | Task | Type |
|---|------|------|
| 1 | Create `inventory_hud.tscn` — CanvasLayer with a panel anchored to bottom-left | Scene |
| 2 | Create `InventoryHUD.cs` script that listens to `InventorySystem` changes and updates display | Code |
| 3 | Design inventory slot UI: icon + count label in an HBoxContainer | UI Design |
| 4 | Create placeholder resource icons (32×32 colored squares: grey=ore, green=wood, purple=catalyst) | Art |
| 5 | Wire HUD to open/close with Tab key (toggle visibility) | Code |
| 6 | Add inventory full feedback (flash red, play error sound) if capacity exceeded | Visual/Audio |

### 3.4 — Hand Scanner (Basic)

| # | Task | Type |
|---|------|------|
| 1 | Create `scanner_panel.tscn` — popup panel that displays when player scans a resource | Scene |
| 2 | Create `ScannerPanel.cs` that calls `ScannerSystem.ScanResource()` and displays vague descriptions | Code |
| 3 | Add input action `"scan"` (F key) — targets nearest resource in InteractionArea | Configuration |
| 4 | Display scan results: resource name + vague property hints ("High Integrity", "Low Conductivity") | UI Design |
| 5 | Add scan sound effect and brief scanning animation (progress bar fills over 0.5s) | Visual/Audio |
| 6 | Mark scanned resources with a subtle overlay/icon so player knows what they've scanned | Visual |

### 3.5 — Save/Load System

| # | Task | Type |
|---|------|------|
| 1 | Create `SaveManager.cs` autoload that serializes `GameState` using `GameStateSerializer` | Code |
| 2 | Map Godot-side state (player position, harvested nodes) into `GameState` model | Code |
| 3 | Implement auto-save on quit and manual save via pause menu | Code |
| 4 | Implement load on game start (check for existing save file in `user://`) | Code |
| 5 | Add save file location: `user://saves/save_001.json` | Configuration |

### 3.6 — Objective Tracker UI

| # | Task | Type |
|---|------|------|
| 1 | Create `objective_tracker.tscn` — small panel anchored top-right showing current objective | Scene |
| 2 | Create `ObjectiveTracker.cs` that reads from `ObjectiveSystem` and displays active objective text | Code |
| 3 | Add objective completion animation (checkmark, brief highlight, then advance to next) | Visual |
| 4 | Wire objective system to trigger on relevant events (first harvest, first scan, etc.) | Code |

### 3.7 — ProcGen Visual Improvements

| # | Task | Type |
|---|------|------|
| 1 | Implement decoration scattering per `PROCGEN_TUTORIAL.md` Phase 1 (noise-based placement) | Code |
| 2 | Add biome-specific decoration configs (trees in forest, rocks in mountain, etc.) | Configuration |
| 3 | Implement resource node placement tied to biome types (ore in rocky areas, wood in forests) | Code |
| 4 | Add depth sorting (Y-sort) for all placed objects so player walks behind tall objects | Configuration |
| 5 | Create 4 ground tile variants for visual variety (clean, pebbled, cracked, hue-shifted) | Art |

---

## Sprint 4: Interaction & Scanner UI

### 4.1 — Full Scanner Interface

| # | Task | Type |
|---|------|------|
| 1 | Create `scanner_full.tscn` — dedicated scanner screen (replaces basic popup from Sprint 3) | Scene |
| 2 | Design scanner layout: resource icon, name, category, vague property bars | UI Design |
| 3 | Implement `ScannerUI.cs` with property visualization (colored bars, "???" for unmeasured) | Code |
| 4 | Add "Known Resources" list panel showing all previously scanned resources | UI Design |
| 5 | Implement scan history (list of all scanned resources with their vague descriptions) | Code |
| 6 | Add scanner equip/unequip animation on player character | Visual |

### 4.2 — Interaction System Refinement

| # | Task | Type |
|---|------|------|
| 1 | Add interaction prompt ("Press E to harvest", "Press F to scan") when near interactable objects | UI |
| 2 | Create `InteractableHighlight.cs` — outline shader or glow on nearest interactable | Visual |
| 3 | Implement multiple interaction types per object (harvest OR scan via different keys) | Code |
| 4 | Add tool requirement feedback ("Requires Gas Siphoning" if player lacks tool) | UI |
| 5 | Create tool unlock notifications when new harvest methods become available | Visual |

---

## Sprint 5: Field Lab & Analysis Logic

### 5.1 — Field Lab Building

| # | Task | Type |
|---|------|------|
| 1 | Create `field_lab.tscn` — placeable building scene (static sprite + interaction area) | Scene |
| 2 | Create `FieldLab.cs` script that opens analysis UI when player interacts | Code |
| 3 | Design Field Lab sprite (workbench with instruments, 64×64 or 128×128) | Art |
| 4 | Implement building placement system (player selects location, confirms placement) | Code |
| 5 | Add construction animation (parts assemble over 2 seconds) | Visual |
| 6 | Wire tech tree: Field Lab unlocks after spending 15 Analysis Knowledge | Code |

### 5.2 — Property Measurement UI

| # | Task | Type |
|---|------|------|
| 1 | Create `field_lab_ui.tscn` — full-screen panel showing resource in analysis chamber | Scene |
| 2 | Design measurement interface: resource slot + "Analyze" button + results panel | UI Design |
| 3 | Implement drag-and-drop: player drags resource from inventory into analysis slot | Code |
| 4 | Show measurement animation (progress bar, sparks, beeping sounds) lasting 3 seconds | Visual/Audio |
| 5 | Display exact numerical values after measurement (replacing "???" with "5.2") | Code |
| 6 | Store measured state in `ScannerSystem.MarkAsAnalyzed()` | Code |
| 7 | Add "Analysis Complete" notification with revealed property values | Visual |

---

## Sprint 6: Planetary Analysis & Deduction Input

### 6.1 — Observatory Building

| # | Task | Type |
|---|------|------|
| 1 | Create `observatory.tscn` — larger building scene (requires more resources to build) | Scene |
| 2 | Create `Observatory.cs` script providing planetary measurement interface | Code |
| 3 | Design Observatory sprite (dome/telescope structure, 128×128) | Art |
| 4 | Wire tech tree: Observatory unlocks after spending 25 Analysis Knowledge | Code |
| 5 | Add ambient animation (dome rotates slowly, telescope tracks) | Visual |

### 6.2 — Planetary Measurement UI

| # | Task | Type |
|---|------|------|
| 1 | Create `observatory_ui.tscn` — data readout screen showing planetary constants | Scene |
| 2 | Design readout layout: list of constants with "Measure" button per constant | UI Design |
| 3 | Implement measurement mini-game or timer (each constant takes time to measure) | Code |
| 4 | Display measured constants with flavor units (e.g., "Gravity: 3.2 G-Force") | Code |
| 5 | Wire to `PlanetaryAnalysisSystem.MeasureConstant()` / `MarkAsMeasured()` | Code |

### 6.3 — Deduction Notepad

| # | Task | Type |
|---|------|------|
| 1 | Create `notepad.tscn` — player's deduction journal accessible via hotkey (N) | Scene |
| 2 | Design notepad layout: formulas section, measured values, calculated requirements | UI Design |
| 3 | Auto-populate with measured planetary values as player discovers them | Code |
| 4 | Show portal requirement formulas (revealed progressively as player measures constants) | Code |
| 5 | Add "Calculate" button that computes requirements from known constants | Code |
| 6 | Highlight which requirements are met vs. unmet based on current inventory | Visual |

---

## Sprint 7: Basic Crafting & Inventory

### 7.1 — Furnace Building

| # | Task | Type |
|---|------|------|
| 1 | Create `furnace.tscn` — smelting building scene | Scene |
| 2 | Create `Furnace.cs` script that opens smelting UI on interaction | Code |
| 3 | Design Furnace sprite (stone/metal structure with fire glow, animated) | Art |
| 4 | Wire tech tree: Furnace unlocks after spending 10 Analysis Knowledge | Code |
| 5 | Add fire particle effects when furnace is active | Visual |
| 6 | Add smelting sound effects (crackling fire, metal clanging) | Audio |

### 7.2 — Smelting/Crafting UI

| # | Task | Type |
|---|------|------|
| 1 | Create `crafting_ui.tscn` — crafting interface with input slots and output preview | Scene |
| 2 | Design crafting layout: 2 input slots + arrow + output slot + "Craft" button | UI Design |
| 3 | Implement drag-and-drop from inventory into input slots | Code |
| 4 | Show output preview (calculated properties) before confirming craft | Code |
| 5 | Wire to `CraftingSystem.Craft()` and update inventory on success | Code |
| 6 | Add crafting animation (items merge, sparks fly, output appears) | Visual |
| 7 | Show recipe discovery notification when player crafts something new | Visual |
| 8 | Implement recipe book UI showing all discovered recipes | UI Design |

### 7.3 — Enhanced Inventory

| # | Task | Type |
|---|------|------|
| 1 | Expand inventory UI to show crafted materials alongside raw resources | UI Design |
| 2 | Add item detail tooltip on hover (shows all measured properties) | Code |
| 3 | Implement inventory sorting (by type, by property value) | Code |
| 4 | Add visual distinction between raw resources and crafted composites | Visual |
| 5 | Implement stack splitting (shift-click to split stacks) | Code |

---

## Sprint 8: Advanced Refining (Composition)

### 8.1 — Gas Injector Building

| # | Task | Type |
|---|------|------|
| 1 | Create `gas_injector.tscn` — advanced crafting building | Scene |
| 2 | Create `GasInjector.cs` script for gas-infusion crafting | Code |
| 3 | Design Gas Injector sprite (pressurized chamber with pipes) | Art |
| 4 | Wire tech tree: requires Field Lab + 10 Synthesis + 10 Conductivity Knowledge | Code |
| 5 | Add gas flow particle effects and pressure gauge animation | Visual |
| 6 | Add hissing/pressurization sound effects | Audio |

### 8.2 — Multi-Step Crafting Chain

| # | Task | Type |
|---|------|------|
| 1 | Implement recipe chain visualization (shows Metal Bar → Composite Alloy path) | UI Design |
| 2 | Add "What can I craft?" button that calls `CraftingSystem.GetCraftableRecipes()` | Code |
| 3 | Show property combination preview with formula explanation tooltip | Code |
| 4 | Implement batch crafting (craft multiple at once if resources available) | Code |
| 5 | Add crafting log/history showing recent crafts and their results | UI Design |

---

## Sprint 9: ProcGen Tech Tree V2 & Unlocks

### 9.1 — Tech Tree UI

| # | Task | Type |
|---|------|------|
| 1 | Create `tech_tree.tscn` — full-screen tech tree visualization | Scene |
| 2 | Design node-graph layout showing tech nodes with connections/arrows | UI Design |
| 3 | Create `TechTreeUI.cs` that renders `TechTreeSystem` state as interactive nodes | Code |
| 4 | Implement node states visually: locked (grey), available (glowing), unlocked (colored) | Visual |
| 5 | Add unlock animation (node lights up, connections pulse) | Visual |
| 6 | Wire knowledge gain: scanning/analyzing resources adds to knowledge categories | Code |
| 7 | Add knowledge counter display showing current points per category | UI Design |
| 8 | Add hotkey (T) to open/close tech tree | Configuration |

### 9.2 — Simulation Core Building

| # | Task | Type |
|---|------|------|
| 1 | Create `sim_core.tscn` — the final analysis building before portal construction | Scene |
| 2 | Create `SimCore.cs` script that runs material simulations | Code |
| 3 | Design Sim Core sprite (high-tech holographic display machine) | Art |
| 4 | Wire tech tree: requires Gas Injector + 15A + 10S + 10C Knowledge | Code |
| 5 | Implement simulation UI: select material, see pass/fail per portal component | Code |
| 6 | Add holographic projection animation showing material stress tests | Visual |

---

## Sprint 10: Portal Construction & UI

### 10.1 — Portal Construction Site

| # | Task | Type |
|---|------|------|
| 1 | Create `portal_site.tscn` — large multi-part construction scene | Scene |
| 2 | Design portal sprite: 3-component structure (Foundation base, Gate arch, Energy Core center) | Art |
| 3 | Show construction progress visually (each component appears as it's built) | Visual |
| 4 | Add portal ambient effects (energy crackling, glow, floating particles) | Visual |
| 5 | Add portal construction sound effects (heavy placement, energy charging) | Audio |

### 10.2 — Portal Construction UI

| # | Task | Type |
|---|------|------|
| 1 | Create `portal_construction_ui.tscn` — interface showing 3 component slots | Scene |
| 2 | Design component slot layout: Foundation + Gate + Energy Core with requirements displayed | UI Design |
| 3 | Implement material assignment: drag crafted material into component slot | Code |
| 4 | Wire to `PortalConstructionSystem` — validate materials meet requirements | Code |
| 5 | Show pass/fail indicators per property per component (green checkmarks / red X) | Visual |
| 6 | Add "Lock In" confirmation button per component (can't change after locking) | Code |
| 7 | Wire to `PortalVerificationSystem.VerifyPortal()` for final validation | Code |

### 10.3 — Diagnostic Feedback

| # | Task | Type |
|---|------|------|
| 1 | Create diagnostic report panel showing exactly which properties pass/fail | UI Design |
| 2 | Implement "Simulate" button that shows how close each property is to requirement | Code |
| 3 | Color-code property bars: green (exceeds), yellow (close), red (fails) | Visual |
| 4 | Add helpful hints: "Foundation Strength is 7.2, needs 8.0 — try adding more Base Ore" | Code |

---

## Sprint 11: Simulation Core & Win State

### 11.1 — Final Simulation Sequence

| # | Task | Type |
|---|------|------|
| 1 | Create `simulation_sequence.tscn` — cinematic verification scene | Scene |
| 2 | Implement simulation playback: portal powers up, tests each component in sequence | Visual |
| 3 | Add pass sequence: green glow → portal stabilizes → "Portal Active!" | Visual |
| 4 | Add fail sequence: red sparks → component cracks → "Structural Failure" with feedback | Visual |
| 5 | Add dramatic sound design (building tension, success fanfare OR failure alarm) | Audio |

### 11.2 — Win State & Transition

| # | Task | Type |
|---|------|------|
| 1 | Create `win_screen.tscn` — victory screen with stats (time, resources used, deductions made) | Scene |
| 2 | Implement transition: player walks into portal → screen fade → win screen | Visual |
| 3 | Add "New Planet" button that generates a new seed at higher complexity | Code |
| 4 | Implement complexity progression: each successful portal increases difficulty for next world | Code |
| 5 | Add end-of-run statistics tracking (completion time, efficiency score) | Code |
| 6 | Create brief cutscene/animation of portal opening to new world | Visual |

### 11.3 — Fail State & Retry

| # | Task | Type |
|---|------|------|
| 1 | Implement graceful failure: player can deconstruct portal components and try again | Code |
| 2 | Add "Retry" button on simulation failure that returns player to portal construction | UI |
| 3 | Preserve all inventory/progress on failure (player just needs better materials) | Code |
| 4 | Show specific failure diagnostics: which component failed and by how much | UI |

---

## Sprint 12: Aesthetic Polish & Juiciness

### 12.1 — Visual Polish

| # | Task | Type |
|---|------|------|
| 1 | Replace all placeholder sprites with final art (see `VERTICAL_SLICE_ASSETS.md`) | Art |
| 2 | Add ambient world particles (floating dust, wind effects, biome-specific particles) | Visual |
| 3 | Implement day/night cycle with lighting changes (shader or modulate) | Visual |
| 4 | Add screen shake on significant events (harvesting, crafting, portal activation) | Visual |
| 5 | Add UI transitions (panels slide/fade in rather than appearing instantly) | Visual |
| 6 | Implement tooltip system across all UI elements | Code |
| 7 | Add idle animations to all buildings (smoke from furnace, spinning gears, etc.) | Visual |

### 12.2 — Audio Polish

| # | Task | Type |
|---|------|------|
| 1 | Create biome-specific ambient soundscapes (wind, water, wildlife) | Audio |
| 2 | Add UI sound effects (button clicks, panel open/close, notification pings) | Audio |
| 3 | Add contextual music transitions (exploration → discovery → crafting → portal) | Audio |
| 4 | Implement audio bus mixing (separate volume for music, SFX, ambient) | Configuration |
| 5 | Add footstep sounds that change based on terrain type | Audio |

### 12.3 — UX Polish

| # | Task | Type |
|---|------|------|
| 1 | Create pause menu with settings (volume, controls, save/load, quit) | Scene/UI |
| 2 | Add tutorial tooltips for first-time actions (first harvest, first scan, first craft) | Code |
| 3 | Implement control remapping UI | Code |
| 4 | Add accessibility options (text size, color blind mode, reduced motion) | Code |
| 5 | Create main menu scene with New Game, Continue, Settings, Quit | Scene |
| 6 | Add loading screen between world generation and gameplay | Visual |
| 7 | Performance profiling and optimization pass (draw calls, physics bodies) | Code |

---

## Cross-Cutting Tasks (Any Sprint)

### Art Production Pipeline

| # | Task | Description |
|---|------|-------------|
| 1 | Create resource sprites | Base Ore (grey rock), Catalyst Ore (purple crystal), Soft Wood (alien tree) — see `VERTICAL_SLICE_ASSETS.md` |
| 2 | Create building sprites | Field Lab, Observatory, Furnace, Gas Injector, Sim Core, Portal (6 buildings) |
| 3 | Create UI panel theme | Consistent sci-fi panel style for all menus (9-patch NinePatchRect backgrounds) |
| 4 | Create icon set | 32×32 icons for all resources, tools, knowledge categories, tech nodes |
| 5 | Create player animations | Idle (4-dir), Walk (4-dir), Harvest (4-dir), Interact (4-dir) |
| 6 | Create particle effects library | Harvest sparks, craft glow, portal energy, scanner beam, measurement pulses |

### Audio Production

| # | Task | Description |
|---|------|-------------|
| 1 | Background music tracks | Exploration theme, Discovery theme, Crafting theme, Portal theme (4 tracks minimum) |
| 2 | SFX library | Harvest (3 variants), Craft (3 variants), UI clicks, Success/Fail stingers, Scan beep |
| 3 | Ambient loops | Wind, Rain, Machinery hum, Energy crackle (4 biome-neutral loops) |

### Configuration & Project Setup

| # | Task | Description |
|---|------|-------------|
| 1 | Input map | Define all input actions: interact, scan, inventory, tech_tree, notepad, pause, craft |
| 2 | Autoload registration | Register all manager singletons: GameManager, InventoryManager, SaveManager, AudioManager |
| 3 | Project settings | Set window size (1920×1080), stretch mode, default clear color, physics layers |
| 4 | Export presets | Configure export for Windows, Linux, macOS (if targeting multi-platform) |
| 5 | .gitignore updates | Ensure `.godot/` cache, `.import/` generated files are properly excluded |

---

## Task Summary

| Sprint | Scene/UI Tasks | Code Tasks | Art Tasks | Audio Tasks | Total |
|--------|---------------|------------|-----------|-------------|-------|
| 3 | 6 | 18 | 5 | 3 | 32 |
| 4 | 2 | 7 | 1 | 1 | 11 |
| 5 | 3 | 8 | 2 | 1 | 14 |
| 6 | 4 | 8 | 2 | 0 | 14 |
| 7 | 3 | 10 | 2 | 2 | 17 |
| 8 | 1 | 6 | 1 | 1 | 9 |
| 9 | 3 | 8 | 2 | 0 | 13 |
| 10 | 4 | 9 | 2 | 2 | 17 |
| 11 | 3 | 8 | 0 | 2 | 13 |
| 12 | 3 | 8 | 4 | 5 | 20 |
| **Total** | **32** | **90** | **21** | **17** | **160** |

---

## Dependency Order

```
Sprint 3 (Foundation)
├── 3.1 GameManager Integration ──→ Everything else depends on this
├── 3.2 Harvesting ──→ Sprint 4 (Scanner), Sprint 5 (Field Lab)
├── 3.3 Inventory HUD ──→ Sprint 7 (Crafting)
├── 3.5 Save/Load ──→ Can be deferred to Sprint 11
└── 3.7 ProcGen ──→ Sprint 12 (Polish)

Sprint 4 (Scanner) ──→ Sprint 5 (Field Lab)
Sprint 5 (Field Lab) ──→ Sprint 6 (Observatory)
Sprint 6 (Observatory) ──→ Sprint 7 (Crafting) [player needs to know requirements]
Sprint 7 (Crafting) ──→ Sprint 8 (Advanced Refining)
Sprint 8 (Advanced Refining) ──→ Sprint 9 (Tech Tree gating)
Sprint 9 (Tech Tree) ──→ Sprint 10 (Portal unlocks)
Sprint 10 (Portal) ──→ Sprint 11 (Win State)
Sprint 12 (Polish) ── Independent, can run in parallel after Sprint 7
```

## Quick Start

To begin Sprint 3 implementation:

1. Open `Wayblazer.sln` in your IDE
2. Add `<ProjectReference Include="..\Wayblazer.GameLogic\Wayblazer.GameLogic.csproj" />` to the Godot project's `.csproj`
3. Open Godot, load `world.tscn`
4. Start with Task 3.1 — wire up `GameManager.cs` to use `InventorySystem` from the GameLogic library
5. Then build the interaction system (3.2) so the player can actually harvest resources
6. Then add the HUD (3.3) so the player can see their inventory

The core game logic is already tested (209 tests passing). Your job is to make it visible and interactive.
