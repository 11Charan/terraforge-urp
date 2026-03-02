using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Streaming Target")]
    public Transform target;

    [Header("Chunk Settings")]
    public int chunkSize = 100;
    public int viewDistance = 1;
    public int loadDistance = 2;
    public int unloadDistance = 3;

    [Header("Noise Settings")]
    public float noiseScale = 0.05f;
    public float heightMultiplier = 5f;
    public Material terrainMaterial;
    private Dictionary<Vector2Int, TerrainChunk> activeChunks = new Dictionary<Vector2Int, TerrainChunk>();
    private Vector2Int currentChunkCoord;

    void Start()
    {
        UpdateChunks();
    }

    void Update()
    {
        if (target == null) return;

        Vector2Int newChunkCoord = GetChunkCoordFromPosition(target.position);

        if (newChunkCoord != currentChunkCoord)
        {
            currentChunkCoord = newChunkCoord;
            UpdateChunks();
        }
    }

    Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int z = Mathf.FloorToInt(position.z / chunkSize);
        return new Vector2Int(x, z);
    }

    void UpdateChunks()
    {
        HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();

        for (int z = -loadDistance; z <= loadDistance; z++)
        {
            for (int x = -loadDistance; x <= loadDistance; x++)
            {
                Vector2Int coord = new Vector2Int(
                    currentChunkCoord.x + x,
                    currentChunkCoord.y + z
                );

                chunksToKeep.Add(coord);

                if (!activeChunks.ContainsKey(coord))
                {
                    CreateChunk(coord);
                }
            }
        }

        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (var coord in activeChunks.Keys)
        {
            int dx = Mathf.Abs(coord.x - currentChunkCoord.x);
            int dz = Mathf.Abs(coord.y - currentChunkCoord.y);

            if (dx > unloadDistance || dz > unloadDistance)
            {
                chunksToRemove.Add(coord);
            }
        }

        foreach (var coord in chunksToRemove)
        {
            Destroy(activeChunks[coord].gameObject);
            activeChunks.Remove(coord);
        }
    }

    void CreateChunk(Vector2Int coord)
    {
        GameObject chunkObj = new GameObject($"Chunk_{coord.x}_{coord.y}");

        chunkObj.transform.parent = transform;
        chunkObj.transform.position = new Vector3(
            coord.x * chunkSize,
            0,
            coord.y * chunkSize
        );

        TerrainChunk chunk = chunkObj.AddComponent<TerrainChunk>();
        chunk.gridSize = chunkSize;
        chunk.noiseScale = noiseScale;
        chunk.heightMultiplier = heightMultiplier;
        chunk.material = terrainMaterial;

        chunk.Initialize(new Vector2(
            coord.x * chunkSize,
            coord.y * chunkSize
        ));

        activeChunks.Add(coord, chunk);
    }
}