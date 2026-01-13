using System;

namespace WFC.Utility;

/// <summary>
/// Represents a deterministic hash for a tile edge based on the biomes present.
/// The hash is calculated from two biome samples taken along the edge.
/// </summary>
public class BiomeEdgeHash
{
	public int Biome1 { get; }
	public int Biome2 { get; }

	public BiomeEdgeHash(int biome1, int biome2)
	{
		Biome1 = biome1;
		Biome2 = biome2;
	}

	/// <summary>
	/// Calculates a deterministic hash value for this edge.
	/// The hash is order-dependent: (Ocean, Beach) != (Beach, Ocean)
	/// </summary>
	public override int GetHashCode()
	{
		return HashCode.Combine(Biome1, Biome2);
	}

	public override bool Equals(object? obj)
	{
		if (obj is BiomeEdgeHash other)
		{
			return Biome1 == other.Biome1 && Biome2 == other.Biome2;
		}
		return false;
	}

	/// <summary>
	/// Gets the complementary edge hash (for matching opposite sides).
	/// The order is reversed to match the opposite orientation.
	/// </summary>
	public BiomeEdgeHash GetComplementary()
	{
		return new BiomeEdgeHash(Biome2, Biome1);
	}

	public override string ToString()
	{
		return $"({Biome1}, {Biome2})";
	}
}
