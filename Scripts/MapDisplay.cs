using Godot;
using System;

[Tool]
public class MapDisplay : CSGMesh
{

	public enum DrawMode { NOISE_MAP, COLOUR_MAP };

	[Export]
	public DrawMode drawMode
	{
		get
		{
			return _drawMode;
		}
		set
		{
			_drawMode = value;
			DrawMap();
		}
	}

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
			DrawMap();
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
			DrawMap();
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
			DrawMap();
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
			DrawMap();
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
			DrawMap();
		}
	}

	[Export]
	public string[] RegionNames
	{
		get
		{
			return _RegionNames;
		}
		set
		{
			_RegionNames = value;
			DrawMap();
		}
	}

	[Export]
	public float[] RegionThresholds
	{
		get
		{
			return _RegionThresholds;
		}
		set
		{
			_RegionThresholds = value;
			DrawMap();
		}
	}

	[Export]
	public Color[] RegionColors
	{
		get
		{
			return _RegionColors;
		}
		set
		{
			_RegionColors = value;
			DrawMap();
		}
	}

	private DrawMode _drawMode;

	private int _MapSeed, _MapWidth, _MapHeight;
	private float _MapScale;
	private Vector2 _MapOffset;
	private float[,] noiseMap;

	private string[] _RegionNames;
	private float[] _RegionThresholds;
	private Color[] _RegionColors;

	private bool ready = false;

	public override void _Ready()
	{
		ready = true;
		DrawMap();
	}

	private void DrawMap()
	{
		if (!ready) return;
		GenerateNoiseMap();
		if (drawMode == DrawMode.COLOUR_MAP)
			GenerateColorTexture();
		else
			GenerateNoiseTexture();
	}

	private void GenerateNoiseMap()
	{
		noiseMap = Noise.NoiseMap(MapSeed, MapWidth, MapHeight, MapScale, MapOffset);
	}

	private void GenerateNoiseTexture()
	{
		Image mapImage = new Image();
		mapImage.Create(MapWidth, MapHeight, true, Image.Format.Rgb8);
		mapImage.Lock();
		for (int y = 0; y < MapHeight; ++y)
		{
			for (int x = 0; x < MapWidth; ++x)
			{
				mapImage.SetPixel(x, y, Colors.Black.LinearInterpolate(Colors.White, noiseMap[x, y]));
			}
		}
		mapImage.Unlock();

		ImageTexture mapTexture = new ImageTexture();
		mapTexture.CreateFromImage(mapImage);
		mapTexture.Flags = (uint)ImageTexture.FlagsEnum.Mipmaps;

		SpatialMaterial material = Material as SpatialMaterial;
		material.AlbedoTexture = mapTexture;
	}

	private void GenerateColorTexture()
	{
		Image mapImage = new Image();
		mapImage.Create(MapWidth, MapHeight, true, Image.Format.Rgb8);
		mapImage.Lock();
		for (int y = 0; y < MapHeight; ++y)
		{
			for (int x = 0; x < MapWidth; ++x)
			{
				for (int i = 0; i < RegionThresholds.Length; ++i)
				{
					if (noiseMap[x, y] <= RegionThresholds[i])
					{
						mapImage.SetPixel(x, y, RegionColors[i]);
						break;
					}
				}
			}
		}
		mapImage.Unlock();

		ImageTexture mapTexture = new ImageTexture();
		mapTexture.CreateFromImage(mapImage);
		mapTexture.Flags = (uint)ImageTexture.FlagsEnum.Mipmaps;

		SpatialMaterial material = Material as SpatialMaterial;
		material.AlbedoTexture = mapTexture;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		float moveSpeed = 10;
		if (@event is InputEventKey eventKey && eventKey.Pressed)
		{
			GD.Print("Detect keypress");
			if (eventKey.Scancode == (int)KeyList.W)
				_MapOffset.y -= moveSpeed;
			if (eventKey.Scancode == (int)KeyList.A)
				_MapOffset.x -= moveSpeed;
			if (eventKey.Scancode == (int)KeyList.S)
				_MapOffset.y += moveSpeed;
			if (eventKey.Scancode == (int)KeyList.D)
				_MapOffset.x += moveSpeed;
			DrawMap();
		}
	}
}
