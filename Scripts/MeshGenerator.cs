using Godot;
using System;

public static class MeshGenerator
{
    public static ArrayMesh GenerateMesh(float[,] noiseMap, float heightMultiplier, Curve heightCurve, int lod)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * height];
        Vector3[] normals = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];
        int[] indices = new int[(width - 1) * (height - 1) * 6];

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / -2f;

        int simplificationFactor = lod == 0 ? 1 : lod * 2;
        int verticesPerLine = (width - 1) / simplificationFactor + 1;

        int vertices_idx = 0;
        for (int y = 0; y < height; y += simplificationFactor)
        {
            for (int x = 0; x < width; x += simplificationFactor)
            {
                vertices[vertices_idx] = new Vector3(topLeftX + x, heightCurve.Interpolate(noiseMap[x, y]) * heightMultiplier, topLeftZ + y);
                uvs[vertices_idx] = new Vector2(x / (float)width, y / (float)height);
                ++vertices_idx;
            }
        }

        int indices_idx = 0;
        vertices_idx = 0;
        for (int y = 0; y < height; y += simplificationFactor)
        {
            for (int x = 0; x < width; x += simplificationFactor)
            {
                if (x < width - 1 && y < height - 1)
                {
                    AddTriangle(vertices, indices, normals, vertices_idx, vertices_idx + verticesPerLine + 1, vertices_idx + verticesPerLine, indices_idx);
                    AddTriangle(vertices, indices, normals, vertices_idx, vertices_idx + 1, vertices_idx + verticesPerLine + 1, indices_idx + 3);
                    indices_idx += 6;
                }
                ++vertices_idx;
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

    private static void AddTriangle(Vector3[] vertices, int[] indices, Vector3[] normals, int a, int b, int c, int index)
    {
        indices[index] = a;
        indices[index + 1] = b;
        indices[index + 2] = c;

        Vector3 normal = CalculateNormal(vertices[a], vertices[b], vertices[c]);
        normals[a] += normal;
        normals[a] = normals[a].Normalized();
        normals[b] += normal;
        normals[b] = normals[b].Normalized();
        normals[c] += normal;
        normals[c] = normals[c].Normalized();
    }
}
