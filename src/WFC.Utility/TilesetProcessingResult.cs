using System.Collections.Generic;
using WFC.Core;

namespace WFC.Utility;

/// <summary>
/// Contains the results of processing a tileset image.
/// </summary>
public class TilesetProcessingResult
{
	/// <summary>
	/// List of ProtoTiles ready for wave function collapse.
	/// Each ProtoTile contains neighbor indices for all four cardinal directions.
	/// </summary>
	public List<ProtoTile> ProtoTiles { get; set; } = new();

	/// <summary>
	/// Maps each tile index to the list of biomes present in that tile.
	/// The key is the tile index (corresponding to the index in ProtoTiles list).
	/// The value is a list of biome IDs found in the tile.
	/// </summary>
	public Dictionary<int, List<int>> TileToBiomes { get; set; } = new();
}
