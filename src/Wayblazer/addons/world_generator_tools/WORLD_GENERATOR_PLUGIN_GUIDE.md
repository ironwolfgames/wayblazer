# World Generator Editor Plugin Guide

## Overview

This editor plugin allows you to edit world generation parameters directly in the Godot editor and see the results immediately. You can modify:

- **Biome Noise Configuration**: Controls how biomes are distributed
- **Biome Ranges**: Defines which height/equator values produce which biomes
- **Environmental Decoration Configs**: Controls tree, rock, ore placement, etc.

## Installation & Setup

### 1. Enable the Plugin

1. Open your project in Godot
2. Go to **Project → Project Settings → Plugins**
3. Find "World Generator Tools" in the list
4. Check the box to enable it

### 2. Open Your World Scene

1. Open the scene containing your WorldGenerator node (e.g., `world.tscn`)
2. Select the WorldGenerator node in the scene tree

## Using the Plugin

### Basic Workflow

1. **Select the WorldGenerator node** in your scene
2. **Edit parameters in the Inspector**:
   - Adjust Width/Height
   - Modify Biome Noise Config (Frequency, Octaves, Lacunarity, Persistence)
   - Add/remove/edit Biome Ranges
   - Configure Decoration Placement
3. **Click "Generate World"** in the bottom panel
4. **See the results** immediately in the editor viewport

### Understanding the Parameters

#### Biome Noise Config
```
- Frequency (0.01): How "zoomed in" the noise is (lower = larger features)
- Octaves (3): Number of noise layers (more = more detail)
- Lacunarity (2.0): How quickly frequency increases per octave
- Persistence (0.5): How quickly amplitude decreases per octave
```

#### Biome Ranges
Each BiomeRange defines when a biome appears:
```
- Biome: The type of biome (Ocean, Plains, Forest, etc.)
- Minimum/Maximum Height: Height range from noise (0.0 to 1.0)
- Minimum/Maximum Equator Value: Distance from equator (0.0 = equator, 1.0 = pole)
```

Example: `BiomeRange(BiomeType.Ocean, 0.0, 0.2, 0.0, 1.0)`
- Appears at heights 0.0-0.2 (low areas)
- Can appear anywhere from equator to poles

#### Decoration Configs
Controls environmental object placement:
```
- Decoration Type: Tree, Rock, OreDeposit, etc.
- Noise Config: Separate noise for this decoration
- Minimum/Maximum Value: Noise threshold for placement
- Valid Biomes: Which biomes can have this decoration
```

## Advanced Tips

### Creating Custom Biome Configurations

You can save biome configurations as resources:

1. In the Inspector, click the dropdown next to "Biome Ranges"
2. Click "Save" to save as a `.tres` file
3. Reuse this configuration in different worlds

### Tweaking for Different World Styles

**Archipelago World** (many islands):
```
- Increase Ocean biome range: MinHeight = 0.0, MaxHeight = 0.4
- Reduce Beach range: MinHeight = 0.4, MaxHeight = 0.45
```

**Continental World** (large landmasses):
```
- Reduce Ocean range: MinHeight = 0.0, MaxHeight = 0.15
- Increase Plains/Forest ranges
- Use lower Frequency (0.005) for larger features
```

**Mountainous World**:
```
- Increase Mountain range: MinHeight = 0.6, MaxHeight = 1.0
- Increase Octaves (4-5) for more jagged terrain
```

### Performance Considerations

- Large worlds (Width/Height > 500) may take time to generate
- More decorations = slower generation
- Use smaller test worlds while tweaking parameters

## Troubleshooting

### Plugin Not Showing Up
- Make sure the plugin is enabled in Project Settings → Plugins
- Check the Output panel for any error messages
- Verify the plugin.cfg file is in the correct location

### Generate Button Does Nothing
- Ensure the WorldGenerator node is selected
- Check that the node is in the scene tree (not just in the FileSystem)
- Look for errors in the Output panel

### World Looks Wrong
- Verify your BiomeRanges don't overlap incorrectly
- Check that DecorationConfigs have valid biomes
- Ensure Width/Height are set to reasonable values (> 0)

### Changes Not Appearing
- Make sure to click "Generate World" after changing parameters
- If using [Tool] attribute, the scene must be saved and reopened sometimes

## Code Structure

The plugin consists of:

1. **WorldGenerator.cs**: The main generator with [Tool] attribute
   - Now has exported properties for all configurations
   - `GenerateWorldFromZero()` method used by the editor for regeneration

2. **WorldGeneratorEditorPlugin.cs**: The editor plugin
   - Adds custom UI to the Godot editor
   - Handles the "Generate World" button

3. **Resource Classes**: BiomeRange, NoiseLayerConfig, etc.
   - All marked with [GlobalClass] for editor visibility
   - Can be saved as `.tres` files

## Next Steps

### Extend the Plugin

You could add:
- **Seed Control**: Input field to set specific seeds
- **Preview Mode**: Show just biomes without decorations
- **Save/Load Presets**: Quick buttons for different world types
- **Real-time Updates**: Regenerate as you type (might be slow)

### Example: Add a Seed Control

In WorldGeneratorEditorPlugin.cs, add:
```csharp
private LineEdit _seedInput;

private void ShowGenerateButton()
{
    // ... existing code ...

    _seedInput = new LineEdit();
    _seedInput.PlaceholderText = "Seed (leave empty for random)";
    _buttonContainer.AddChild(_seedInput);

    // ... rest of code ...
}

private void OnGenerateButtonPressed()
{
    if (_currentWorldGenerator != null && _currentWorldGenerator.IsInsideTree())
    {
        // Use custom seed if provided
        if (!string.IsNullOrEmpty(_seedInput.Text) && int.TryParse(_seedInput.Text, out int customSeed))
        {
            GlobalRandom.InitializeWithSeed(customSeed);
        }

        _currentWorldGenerator.GenerateWorldFromZero();
    }
}
```

## Resources

- [Godot EditorPlugin Documentation](https://docs.godotengine.org/en/stable/classes/class_editorplugin.html)
- [Godot C# Documentation](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/index.html)
- [Perlin Noise Explained](https://en.wikipedia.org/wiki/Perlin_noise)
