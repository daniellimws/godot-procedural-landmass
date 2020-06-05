using Godot;
using System;

public static class MeshGenerator
{
    public static ArrayMesh GenerateMesh(float[,] noiseMap, float heightMultiplier, Curve heightCurve)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * height];
        Vector3[] normals = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];
        int[] indices = new int[(width - 1) * (height - 1) * 6];

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / -2f;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                int index = x + y * width;
                vertices[index] = new Vector3(topLeftX + x, heightCurve.Interpolate(noiseMap[x, y]) * heightMultiplier, topLeftZ + y);
                uvs[index] = new Vector2(x / (float)width, y / (float)height);
            }
        }

        int indices_idx = 0;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                int index = x + y * width;
                Vector3 normal;
                if (x < width - 1 && y < height - 1)
                {
                    indices[indices_idx * 6 + 0] = index;
                    indices[indices_idx * 6 + 1] = index + width + 1;
                    indices[indices_idx * 6 + 2] = index + width;

                    normal = CalculateNormal(vertices[index], vertices[index + width + 1], vertices[index + width]);
                    normals[index] += normal;
                    normals[index] = normals[index].Normalized();
                    normals[index + width + 1] += normal;
                    normals[index + width + 1] = normals[index + width + 1].Normalized();
                    normals[index + width] += normal;
                    normals[index + width] = normals[index + width].Normalized();

                    indices[indices_idx * 6 + 3] = index;
                    indices[indices_idx * 6 + 4] = index + 1;
                    indices[indices_idx * 6 + 5] = index + width + 1;

                    normal = CalculateNormal(vertices[index], vertices[index + 1], vertices[index + width + 1]);
                    normals[index] += normal;
                    normals[index] = normals[index].Normalized();
                    normals[index + 1] += normal;
                    normals[index + 1] = normals[index + 1].Normalized();
                    normals[index + width + 1] += normal;
                    normals[index + width + 1] = normals[index + width + 1].Normalized();

                    ++indices_idx;
                }
            }
        }

        var meshArrays = new Godot.Collections.Array();
        meshArrays.Resize((int)ArrayMesh.ArrayType.Max);
        meshArrays[(int)ArrayMesh.ArrayType.Vertex] = vertices;
        meshArrays[(int)ArrayMesh.ArrayType.TexUv] = uvs;
        meshArrays[(int)ArrayMesh.ArrayType.Index] = indices;
        meshArrays[(int)ArrayMesh.ArrayType.Normal] = normals;

        ArrayMesh mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArrays);
        return mesh;
    }

    private static Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v1 = b - a;
        Vector3 v2 = b - c;
        return v1.Cross(v2).Normalized();
    }
}
