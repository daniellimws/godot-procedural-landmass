using Godot;
using System;

public static class TextureGenerator
{
    public static Texture GenerateNoiseTexture(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Image mapImage = new Image();
        mapImage.Create(width, height, true, Image.Format.Rgb8);
        mapImage.Lock();
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                mapImage.SetPixel(x, y, Colors.Black.LinearInterpolate(Colors.White, noiseMap[x, y]));
            }
        }
        mapImage.Unlock();

        ImageTexture mapTexture = new ImageTexture();
        mapTexture.CreateFromImage(mapImage);
        mapTexture.Flags = (uint)ImageTexture.FlagsEnum.Mipmaps;

        return mapTexture;
    }

    public static Texture GenerateColorTexture(float[,] noiseMap, float[] regionThresholds, Color[] regionColors)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Image mapImage = new Image();
        mapImage.Create(width, height, true, Image.Format.Rgb8);
        mapImage.Lock();
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                for (int i = 0; i < regionThresholds.Length; ++i)
                {
                    if (noiseMap[x, y] <= regionThresholds[i])
                    {
                        mapImage.SetPixel(x, y, regionColors[i]);
                        break;
                    }
                }
            }
        }
        mapImage.Unlock();

        ImageTexture mapTexture = new ImageTexture();
        mapTexture.CreateFromImage(mapImage);
        mapTexture.Flags = (uint)ImageTexture.FlagsEnum.Mipmaps;

        return mapTexture;
    }
}
