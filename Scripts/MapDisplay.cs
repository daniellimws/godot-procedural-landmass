using Godot;
using System;

[Tool]
public class MapDisplay : Spatial
{

	public enum DrawMode { NOISE_MAP, COLOUR_MAP, MESH };

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

	[Export]
	public float MeshHeightMultiplier
	{
		get
		{
			return _MeshHeightMultiplier;
		}
		set
		{
			_MeshHeightMultiplier = value;
			DrawMap();
		}
	}

	[Export]
	public Curve MeshHeightCurve
	{
		get {
			return _MeshHeightCurve;
		}
		set {
			_MeshHeightCurve = value;
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

	private float _MeshHeightMultiplier;
	private Curve _MeshHeightCurve;

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
		else if (drawMode == DrawMode.MESH)
			GenerateMesh();
		else
			GenerateNoiseTexture();
	}

	private void GenerateNoiseMap()
	{
		noiseMap = Noise.NoiseMap(MapSeed, MapWidth, MapHeight, MapScale, MapOffset);
	}

	private void GenerateMesh()
	{
		ArrayMesh mesh = MeshGenerator.GenerateMesh(noiseMap, MeshHeightMultiplier, MeshHeightCurve);
		MeshInstance meshInstance = GetNode<MeshInstance>("MeshInstance");
		meshInstance.Mesh = mesh;

		SpatialMaterial material = new SpatialMaterial();
		material.AlbedoTexture = TextureGenerator.GenerateColorTexture(noiseMap, RegionThresholds, RegionColors);
		material.ParamsCullMode = SpatialMaterial.CullMode.Disabled;
		// material.FlagsUnshaded = true;
		mesh.SurfaceSetMaterial(0, material);
	}

	private void GenerateNoiseTexture()
	{
		SpatialMaterial material = GetNode<CSGMesh>("Plane").Material as SpatialMaterial;
		material.AlbedoTexture = TextureGenerator.GenerateNoiseTexture(noiseMap);
	}

	private void GenerateColorTexture()
	{
		SpatialMaterial material = GetNode<CSGMesh>("Plane").Material as SpatialMaterial;
		material.AlbedoTexture = TextureGenerator.GenerateColorTexture(noiseMap, RegionThresholds, RegionColors);
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
