using Godot;

namespace Wayblazer;

public static class NoiseService
{
	public static float[,] GenerateNoiseMap(int width, int height, int seed, NoiseLayerConfig config)
	{
		float[,] map = new float[width, height];
		FastNoiseLite noise = new FastNoiseLite();

		noise.Seed = seed;
		noise.Frequency = config.Frequency;
		noise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
		noise.FractalOctaves = config.Octaves;
		noise.FractalLacunarity = config.Lacunarity;
		noise.FractalGain = config.Persistence;

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				// GetNoise returns -1 to 1, we normalize it to 0 to 1
				float val = noise.GetNoise2D(x, y);
				map[x, y] = (val + 1) / 2;
			}
		}
		return map;
	}
}
