using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainChunk : MonoBehaviour
{
    public int gridSize = 100;
    public float cellSize = 1f;
    public float noiseScale = 0.05f;
    public float heightMultiplier = 5f;
    public Material material;

    public void Initialize(Vector2 offset)
    {
        GenerateMesh(offset);
    }

    void GenerateMesh(Vector2 offset)
    {
        int vertexCount = (gridSize + 1) * (gridSize + 1);
        int triangleCount = gridSize * gridSize * 6;

        NativeArray<float3> vertices = new NativeArray<float3>(vertexCount, Allocator.TempJob);
        NativeArray<float3> normals = new NativeArray<float3>(vertexCount, Allocator.TempJob);

        TerrainJob job = new TerrainJob
        {
            gridSize = gridSize,
            cellSize = cellSize,
            noiseScale = noiseScale,
            heightMultiplier = heightMultiplier,
            offset = new float2(offset.x, offset.y),
            vertices = vertices,
            normals = normals
        };

        JobHandle handle = job.Schedule(vertexCount, 64);
        handle.Complete();

        // Convert to Unity arrays
        Vector3[] finalVertices = new Vector3[vertexCount];
        Vector3[] finalNormals = new Vector3[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            finalVertices[i] = vertices[i];
            finalNormals[i] = normals[i];
        }

        vertices.Dispose();
        normals.Dispose();

        int[] triangles = GenerateTriangles();

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = finalVertices;
        mesh.triangles = triangles;
        mesh.normals = finalNormals;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }

    int[] GenerateTriangles()
    {
        int[] triangles = new int[gridSize * gridSize * 6];

        int triangleIndex = 0;
        int vert = 0;

        for (int z = 0; z < gridSize; z++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                triangles[triangleIndex + 0] = vert;
                triangles[triangleIndex + 1] = vert + gridSize + 1;
                triangles[triangleIndex + 2] = vert + 1;

                triangles[triangleIndex + 3] = vert + 1;
                triangles[triangleIndex + 4] = vert + gridSize + 1;
                triangles[triangleIndex + 5] = vert + gridSize + 2;

                vert++;
                triangleIndex += 6;
            }
            vert++;
        }

        return triangles;
    }
}