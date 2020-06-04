using Godot;
using System;

[Tool]
public class MapDisplay : CSGMesh
{
    [Export]
    public int MapSeed
    {
        get
        {
            return _MapSeed;
        }
        set
        {
            _MapSeed = value;
            Init();
        }
    }

    [Export]
    public int MapWidth
    {
        get
        {
            return _MapWidth;
        }
        set
        {
            _MapWidth = value;
            Init();
        }
    }
    [Export]
    public int MapHeight
    {
        get
        {
            return _MapHeight;
        }
        set
        {
            _MapHeight = value;
            Init();
        }
    }
    [Export]
    public float MapScale
    {
        get
        {
            return _MapScale;
        }
        set
        {
            _MapScale = value;
            Init();
        }
    }

    [Export]
    public Vector2 MapOffset
    {
        get
        {
            return _MapOffset;
        }
        set
        {
            _MapOffset = value;
            Init();
        }
    }

    private int _MapSeed, _MapWidth, _MapHeight;
    private float _MapScale;
    private Vector2 _MapOffset;
    private float[,] noiseMap;

    public override void _Ready()
    {
        Init();
    }

    private void Init()
    {
        GenerateNoiseMap();
        GenerateTexture();
    }

    private void GenerateNoiseMap()
    {
        noiseMap = Noise.NoiseMap(MapSeed, MapWidth, MapHeight, MapScale, MapOffset);
    }

    private void GenerateTexture()
    {
        Image mapImage = new Image();
        mapImage.Create(MapWidth, MapHeight, true, Image.Format.Rgb8);
        mapImage.Lock();
        for (int y = 0; y < MapHeight; ++y)
        {
            for (int x = 0; x < MapWidth; ++x)
            {
                mapImage.SetPixel(x, y, Colors.White.LinearInterpolate(Colors.Black, noiseMap[x, y]));
            }
        }
        mapImage.Unlock();

        ImageTexture mapTexture = new ImageTexture();
        mapTexture.CreateFromImage(mapImage);

        SpatialMaterial material = Material as SpatialMaterial;
        material.AlbedoTexture = mapTexture;
    }
}
