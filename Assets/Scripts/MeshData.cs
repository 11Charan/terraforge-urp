using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;
    public Vector3[] normals;

    public MeshData(int vertexCount, int triangleCount)
    {
        vertices = new Vector3[vertexCount];
        triangles = new int[triangleCount];
        uv = new Vector2[vertexCount];
        normals = new Vector3[vertexCount];
    }
}