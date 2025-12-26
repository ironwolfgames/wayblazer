using System;

namespace Wayblazer;

public static class GlobalRandom
{
	public static int Seed { get; private set; }

	public static void InitializeWithSeed(int seed)
	{
		Seed = seed;
		_random = new Random(seed);
	}

	public static float NextFloat(float min, float max)
	{
		return (float)(_random.NextDouble() * (max - min) + min);
	}

	public static int Next(int min, int max)
	{
		return _random.Next(min, max);
	}

	private static Random _random = new Random();
}
