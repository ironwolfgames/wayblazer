# Quick Start: World Generator Editor Plugin

## Enable Plugin
1. **Project → Project Settings → Plugins**
2. Enable "World Generator Tools"

## Use Plugin
1. **Select WorldGenerator node** in scene tree
2. **Edit parameters** in Inspector
3. **Click "Generate World"** in bottom panel

## Key Parameters

### Noise Config
- **Frequency**: Lower = larger features (try 0.005 - 0.02)
- **Octaves**: More = more detail (try 2-5)

### Biome Ranges
```
BiomeType: What biome to create
MinHeight/MaxHeight: 0.0 (low) to 1.0 (high)
MinEquatorValue/MaxEquatorValue: 0.0 (equator) to 1.0 (pole)
```

### Decoration Configs
```
DecorationType: Tree, Rock, Ore, etc.
MinValue/MaxValue: Placement threshold
ValidBiomes: Where it can spawn
```

## Quick World Types

**Islands**: Ocean max height = 0.4
**Continents**: Ocean max height = 0.15, Frequency = 0.005
**Mountains**: Mountain min height = 0.6, Octaves = 5

## Tips
- Start with small worlds (100x100) while testing
- Save configurations as .tres files for reuse
- Check Output panel for errors
