using Godot;
using System;

public static class Noise
{
    public static float[,] NoiseMap(int height, int width, float scale)
    {
        float[,] noiseMap = new float[width, height];

        OpenSimplexNoise simplexNoise = new OpenSimplexNoise();
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();

        simplexNoise.Seed = (int)rng.Randi();
        simplexNoise.Period = 8;
        simplexNoise.Octaves = 3;
        simplexNoise.Persistence = 0.8f;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                noiseMap[x, y] = simplexNoise.GetNoise2d(x / scale, y / scale);
            }
        }

        return noiseMap;
    }
}
