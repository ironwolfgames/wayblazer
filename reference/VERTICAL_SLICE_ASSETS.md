# Wayblazer Vertical Slice: Asset List

## 1\. 2D Art Assets (Pixel Art)

### A. Tilesets (Isometric)

*Assume 64x64 or 32x32 grid size. Tiles need to tile seamlessly.*

| Asset Name | Description | Variations |
| :--- | :--- | :--- |
| **Tile\_Ground\_Basic** | The primary surface (Alien Soil/Regolith). Muted color to let resources pop. | 4 variants (clean, pebbled, cracked, slight hue shift) to break repetition. |
| **Tile\_Ground\_Rock** | Harder terrain patches. Darker/rougher texture. | 2 variants. |
| **Tile\_Liquid\_Base** | Water or alien fluid pools. | **Animated** (4-frame loop: shimmering/waves). |
| **Tile\_Coastline** | Transitions between Ground and Liquid. | N/A (or simple overlay). |
| **Tile\_Foundation** | Metal plating/floor that appears under constructed machines (optional, nice for polish). | 1 variant. |

### B. Resources (Overlay Objects)

*Placed on top of tiles. Must be distinct from decoration.*

| Asset Name | Description | State/Animation |
| :--- | :--- | :--- |
| **Res\_BaseOre** | Dull grey metallic rock cluster. Looks heavy/dense. | Static. |
| **Res\_CatalystOre** | Shimmering purple/bright crystal formation. Distinct silhouette. | **Animated** (Subtle glow pulse). |
| **Res\_SoftWood** | Alien tree-like flora. bulbous or twisting trunk (not just an Earth pine tree). | **Animated** (Wind sway). |
| **Res\_Debris** | Breakable rocks/scrap (optional source for basic stone/metal). | Static. |

### C. Environmental Decoration (The "Juice")

*Non-interactive objects that make the world feel alive. "Walk-behind" depth sorting is critical here.*

| Asset Name | Description | Animation |
| :--- | :--- | :--- |
| **Deco\_GrassTufts** | Small patches of alien grass (different color than ground). | **Animated** (Wind sway). |
| **Deco\_AlienBush** | Small shrubbery. Player can walk through. | **Animated** (Reacts/rustles when player walks through). |
| **Deco\_TallFlower** | Large alien flower (e.g., bioluminescent bulb). Acts as a visual landmark. | **Animated** (Idle sway). |
| **Deco\_Boulder** | Large background rock. Player collides with it (blocks path). | Static. |
| **Deco\_Vines** | Flat sprites placed on the ground or creeping up Boulders. | Static. |

### D. Characters & Creatures

| Asset Name | Description | Animations Required |
| :--- | :--- | :--- |
| **Player\_Wayblazer** | The engineer in a sci-fi EVA suit. Gender-neutral, bulky but agile. Backpack visible. | **Idle** (4 directions: breathing).<br>**Run** (4 directions).<br>**Harvest** (4 directions: swinging tool or arm motion).<br>**Interact** (Typing on wrist pad/holding scanner). |
| **Critter\_Passive** | Small alien herbivore (e.g., a "space rabbit" or slug). Adds life. | **Idle** (Eating/looking).<br>**Move** (Hopping/crawling). |
| **Critter\_Bird** | Flying atmospheric creature (for shadows/overhead). | **Fly** (Loop). |

### E. Buildings & Machines (2.5D Isometric)

*These are the stars of the show. They need clear "On/Off" states.*

| Asset Name | Description | Visual States |
| :--- | :--- | :--- |
| **Build\_FieldLab** | Compact workbench with a scanner arm and monitor screen. | **Idle:** Screen static, lights dim.<br>**Active:** Screen bright/scrolling data, arm moving or lights blinking. |
| **Build\_Observatory** | Large dish or telescope mounted on a tripod base. | **Uncalibrated:** Dish pointed down/neutral, red light.<br>**Calibrated:** Dish pointed up/spinning, green light. |
| **Build\_Furnace** | Industrial smelter. Looks heavy, reinforced. | **Idle:** Cold, dark vent.<br>**Smelting:** Fire/Glow visible in chamber, chimney smoke. |
| **Build\_GasInjector** | Pressurized tank system with pipes and a mixing chamber. | **Idle:** Gauge at zero, liquid static.<br>**Compositing:** Bubbling liquid, vibration, gas venting. |
| **Build\_PortalBase** | Large metallic ring base. The "Foundation" only. | **Construction:** Holographic wireframe (Ghost).<br>**Built:** Solid metal, sealed panels, heavy look. |
| **Build\_SimCore** | High-tech server rack/projector. Sits near Portal. | **Idle:** Low power mode.<br>**Running:** Projecting a hologram grid over the Portal. |

### F. UI Elements (User Interface)

| Asset Name | Description |
| :--- | :--- |
| **UI\_Icon\_Items** | Individual 16x16 or 32x32 icons for: Base Ore, Catalyst Ore, Soft Wood, Metal Bar, Composite Alloy. |
| **UI\_Icon\_Tech** | Icons for Tech Points: Analysis (Microscope/Eye), Smelting (Flame), Compositing (Beaker/Atom). |
| **UI\_Panel\_9Slice** | Modular background for windows (Inventory, Research). Sci-fi style (borders, corners). |
| **UI\_Progress** | Progress bar styling (for Smelting/Scanning timers). |
| **UI\_Cursor** | Custom mouse cursor (Precision reticle). |
| **UI\_ScannerOverlay** | The frame for the handheld scanner popup. |

### G. VFX (Visual Effects)

| Asset Name | Description |
| :--- | :--- |
| **VFX\_MiningHit** | Dust/Sparks triggered when hitting a resource. |
| **VFX\_SmokePlume** | Looping smoke puff for the Furnace chimney. |
| **VFX\_GasLeak** | Swirling colored gas for the Injector. |
| **VFX\_Hologram** | Grid lines or static noise for the Construction Ghost and Simulation Core. |
| **VFX\_PortalActive** | **Major Asset:** The final swirling energy event when the game is won. |
| **VFX\_FloatingText** | (Code-generated, but needs font) Numbers floating up when harvesting. |

-----

## 2\. Audio Assets

### A. Sound Effects (SFX)

| Category | Asset Name | Description |
| :--- | :--- | :--- |
| **Player** | `sfx_footstep_dirt` | Crunching sound (random pitch variance). |
| | `sfx_footstep_metal` | Clanking sound (for walking on foundations). |
| | `sfx_tool_impact` | Pickaxe hitting rock/wood. |
| | `sfx_scan_loop` | High-pitched electronic warble (while holding Right-Click). |
| | `sfx_scan_success` | Satisfying "Ping" or "Chime". |
| **Machines** | `sfx_machine_place` | Heavy metallic thud (construction). |
| | `sfx_furnace_loop` | Low roaring fire / fan noise. |
| | `sfx_injector_hiss` | Pneumatic gas release (pshhht). |
| | `sfx_lab_process` | Computer thinking/hard drive read noise. |
| | `sfx_sim_pass` | Harmonious major-chord confirmation. |
| | `sfx_sim_fail` | Discordant buzzer/error tone. |
| | `sfx_portal_open` | **Major Asset:** Massive bass drop + energy surge. |
| **UI** | `sfx_ui_hover` | Subtle click or tick. |
| | `sfx_ui_select` | Clean electronic blip. |
| | `sfx_ui_error` | Soft buzz (trying to buy without points). |
| | `sfx_tech_unlock` | Uplifting jingle. |

### B. Music Tracks

| Asset Name | Mood/Style | Context |
| :--- | :--- | :--- |
| **Mus\_Title** | Mysterious, hopeful, slow build. | Main Menu. |
| **Mus\_Exploration** | "Lo-fi Sci-Fi." Atmospheric pads, sparse melody, light rhythm. Not distracting. | Main Gameplay Loop. |
| **Mus\_Victory** | Triumphant, orchestral swell. | Win Scene. |
