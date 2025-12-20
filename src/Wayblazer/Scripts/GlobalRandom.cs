using System;

namespace Wayblazer;

public static class GlobalRandom
{
	private static Random _random = new Random();

	public static void Seed(int seed)
	{
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
}
