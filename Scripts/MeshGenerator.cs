using Godot;
using System;

public static class MeshGenerator
{
    public static ArrayMesh GenerateMesh(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];
        int[] indices = new int[(width - 1) * (height - 1) * 6];
        int indices_idx = 0;

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / -2f;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                int index = x + y * width;
                vertices[index] = new Vector3(topLeftX + x, noiseMap[x, y] * 10f, topLeftZ + y);
                uvs[index] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    indices[indices_idx * 6 + 0] = index;
                    indices[indices_idx * 6 + 1] = index + width;
                    indices[indices_idx * 6 + 2] = index + width + 1;
                    indices[indices_idx * 6 + 3] = index;
                    indices[indices_idx * 6 + 4] = index + 1;
                    indices[indices_idx * 6 + 5] = index + width + 1;
                    ++indices_idx;
                }
            }
        }

        var meshArrays = new Godot.Collections.Array();
        meshArrays.Resize((int)ArrayMesh.ArrayType.Max);
        meshArrays[(int)ArrayMesh.ArrayType.Vertex] = vertices;
        meshArrays[(int)ArrayMesh.ArrayType.TexUv] = uvs;
        meshArrays[(int)ArrayMesh.ArrayType.Index] = indices;

        ArrayMesh mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArrays);
        return mesh;
    }
}
