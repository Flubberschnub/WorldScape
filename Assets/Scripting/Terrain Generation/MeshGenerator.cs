using System;
using UnityEngine;

[ExecuteInEditMode]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public bool realTimeUpdate = false;

    // resolution of vertices (how many units per vertex)
    public float xResolution = 1f;
    public float zResolution = 1f;

    // size of the mesh in vertices
    public int xSize = 20;
    public int zSize = 20;
    
    public float speed;
    
    public Gradient gradient;
    
    public float minTerrainHeight;
    public float maxTerrainHeight;

    public HeightMapper heightMapper;
    public void Create()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        CreateBase();
        UpdateVertices();
        UpdateColors(minTerrainHeight, maxTerrainHeight);

    }

    void CreateBase()
    {

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x * xResolution, 0, z * zResolution);
                i++;
            }
        }


        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        
        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
        
    }
    
    public void UpdateVertices()
    {
        mesh.Clear();
        
        mesh.vertices = heightMapper.ApplyHeightMap(transform.position + new Vector3(Time.time*speed, 0, Time.time*speed), vertices, xSize, zSize);
        mesh.triangles = triangles;
        
        minTerrainHeight = Single.MaxValue;
        maxTerrainHeight = Single.MinValue;
        foreach (var vertex in mesh.vertices)
        {
            if (vertex.y < minTerrainHeight)
                minTerrainHeight = vertex.y;
            if (vertex.y > maxTerrainHeight)
                maxTerrainHeight = vertex.y;
        }
    }
    
    public void UpdateColors(float min, float max)
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            float height = mesh.vertices[i].y;
            float t = Mathf.InverseLerp(min, max, height);
            colors[i] = gradient.Evaluate(t);
        }
        mesh.colors = colors;

        mesh.RecalculateNormals();

    }

    void OnDrawGizmos()
    {
        // if (vertices == null)
        // {
        //     return;
        // }
        // for (int i = 0; i < vertices.Length; i++)
        // {
        //     Gizmos.DrawSphere(vertices[i] + transform.position, .1f);
        // }
    }
}
