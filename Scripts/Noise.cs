using Godot;
using System;

public static class Noise
{
    public static float[,] NoiseMap(int seed, int height, int width, float scale, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];

        OpenSimplexNoise simplexNoise = new OpenSimplexNoise();
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();

        simplexNoise.Seed = seed;
        simplexNoise.Period = 8;
        simplexNoise.Octaves = 3;
        simplexNoise.Persistence = 0.8f;

        float maxNoise = float.MinValue;
        float minNoise = float.MaxValue;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                float sampleX = (x + offset.x - width / 2) / scale;
                float sampleY = (y + offset.y - height / 2) / scale;
                noiseMap[x, y] = simplexNoise.GetNoise2d(sampleX, sampleY);
                maxNoise = Mathf.Max(maxNoise, noiseMap[x, y]);
                minNoise = Mathf.Min(minNoise, noiseMap[x, y]);
            }
        }

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
