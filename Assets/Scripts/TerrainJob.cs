using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct TerrainJob : IJobParallelFor
{
    public int gridSize;
    public float cellSize;
    public float noiseScale;
    public float heightMultiplier;
    public float2 offset;

    public NativeArray<float3> vertices;
    public NativeArray<float3> normals;

    public void Execute(int index)
    {
        int x = index % (gridSize + 1);
        int z = index / (gridSize + 1);

        float worldX = x + offset.x;
        float worldZ = z + offset.y;

        float height = noise.cnoise(new float2(worldX * noiseScale, worldZ * noiseScale));
        height *= heightMultiplier;

        vertices[index] = new float3(x * cellSize, height, z * cellSize);

        // Gradient approximation
        float hL = noise.cnoise(new float2((worldX - 1) * noiseScale, worldZ * noiseScale)) * heightMultiplier;
        float hR = noise.cnoise(new float2((worldX + 1) * noiseScale, worldZ * noiseScale)) * heightMultiplier;
        float hD = noise.cnoise(new float2(worldX * noiseScale, (worldZ - 1) * noiseScale)) * heightMultiplier;
        float hU = noise.cnoise(new float2(worldX * noiseScale, (worldZ + 1) * noiseScale)) * heightMultiplier;

        float3 normal = math.normalize(new float3(
            hL - hR,
            2f,
            hD - hU
        ));

        normals[index] = normal;
    }
}