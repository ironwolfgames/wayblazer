namespace Wayblazer.Core.Utility;

public static class RandomUtility
{
	public static int Next() => _random.Next();
	public static int Next(int max) => _random.Next(max);
	public static int Next(int min, int max) => _random.Next(min, max);
	public static bool NextBool() => _random.Next(2) == 0;

	public static void SetSeed(int seed)
	{
		_random = new Random(seed);
	}

	private static Random _random = new Random();
}
