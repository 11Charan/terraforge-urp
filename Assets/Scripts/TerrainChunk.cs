using UnityEngine;
using System.Threading;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainChunk : MonoBehaviour
{
    public int gridSize = 100;
    public float cellSize = 1f;
    public float noiseScale = 0.05f;
    public float heightMultiplier = 5f;
    public Material material;
    private Mesh data;
    private bool meshReady = false;
    private MeshData meshData;

    public void Initialize(Vector2 offset)
    {
        RequestMeshData(offset);
    }

    MeshData GenerateMeshData(Vector2 offset)
    {
        int vertexCount = (gridSize + 1) * (gridSize + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        int[] triangles = new int[gridSize * gridSize * 6];

        MeshData data = new(vertexCount, gridSize * gridSize * 6);

        int vertexIndex = 0;

        for (int z = 0; z <= gridSize; z++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
                float worldX = x + offset.x;
                float worldZ = z + offset.y;

                float height = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale) * heightMultiplier;

                vertices[vertexIndex] = new Vector3(x * cellSize, height, z * cellSize);

                float heightL = Mathf.PerlinNoise((worldX - 1) * noiseScale, worldZ * noiseScale) * heightMultiplier;
                float heightR = Mathf.PerlinNoise((worldX + 1) * noiseScale, worldZ * noiseScale) * heightMultiplier;
                float heightD = Mathf.PerlinNoise(worldX * noiseScale, (worldZ - 1) * noiseScale) * heightMultiplier;
                float heightU = Mathf.PerlinNoise(worldX * noiseScale, (worldZ + 1) * noiseScale) * heightMultiplier;

                Vector3 normal = new Vector3(
                    heightL - heightR,
                    2f,
                    heightD - heightU
                ).normalized;

                normals[vertexIndex] = normal;

                uv[vertexIndex] = new Vector2((float)x / gridSize, (float)z / gridSize);
                vertexIndex++;
            }
        }

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

        data.vertices = vertices;
        data.triangles = triangles;
        data.uv = uv;
        data.normals = normals;

        return data;
    }

    void RequestMeshData(Vector2 offset)
    {
        Thread thread = new Thread(() =>
        {
            meshData = GenerateMeshData(offset);
            meshReady = true;
        });

        thread.Start();
    }

    void Update()
    {
        if (meshReady)
        {
            ApplyMeshData();
            meshReady = false;
        }
    }

    void ApplyMeshData()
    {
        Mesh mesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,

            vertices = meshData.vertices,
            triangles = meshData.triangles,
            uv = meshData.uv,
            normals = meshData.normals
        };

        GetComponent<MeshFilter>().mesh = mesh;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = material;
    }
}