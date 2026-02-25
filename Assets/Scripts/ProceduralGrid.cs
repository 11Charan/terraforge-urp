using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProceduralGrid : MonoBehaviour
{
    public int gridSize = 10;
    public float cellSize = 1f;
    
    [Header("Noise Settings")]
    public float noiseScale = 0.05f;
    public float heightMultiplier = 5f;
    private Mesh mesh;

    void Start()
    {
        GenerateMesh();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        int sizeDelta = 0;
        if (keyboard.rKey.wasPressedThisFrame) sizeDelta++;
        if (keyboard.tKey.wasPressedThisFrame) sizeDelta--;

        if (sizeDelta == 0)
            return;

        int nextSize = Mathf.Clamp(gridSize + sizeDelta, 1, 200);
        if (nextSize == gridSize)
            return;

        gridSize = nextSize;
        GenerateMesh();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            GenerateMesh();
        }
    }

    void GenerateMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
                name = "Procedural Grid"
            };
            GetComponent<MeshFilter>().mesh = mesh;
        }

        mesh.Clear();

        int vertexCount = (gridSize + 1) * (gridSize + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[gridSize * gridSize * 6];

        int vertexIndex = 0;
        for (int z = 0; z <= gridSize; z++)
        {
            for (int x = 0; x <= gridSize; x++)
            {
                float height = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightMultiplier;
                vertices[vertexIndex] = new Vector3(x * cellSize, height, z * cellSize);
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

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        
        mesh.RecalculateNormals();
    }
}
