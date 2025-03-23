using Scripting.Jobs;
using Unity.Collections;
using Unity.Jobs;

namespace Scripting.Terrain_Generation
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class MeshGenerator : MonoBehaviour
    {

        Mesh mesh;

        Vector3[] vertices;
        int[] triangles;
        Color[] colors;

        // resolution of vertices (how many units per vertex)
        public float xResolution = 1f;
        public float zResolution = 1f;

        // size of the mesh in vertices
        public int xSize = 20;
        public int zSize = 20;
        
        public float offsetX;
        public float offsetZ;

        public Gradient gradient;

        public float minTerrainHeight;
        public float maxTerrainHeight;

        public void Create()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            CreateBase();
            
            // Allocate Native Arrays (must dispose later)
            NativeArray<Vector3> verticesNative = new NativeArray<Vector3>(vertices, Allocator.TempJob);

            // Initialize Job
            FractalPerlinNoiseHeightMapJob job = new FractalPerlinNoiseHeightMapJob
            {
                vertices = verticesNative,
                positionOffset = transform.position,
                offsetX = offsetX,
                offsetZ = offsetZ,
                xScale = 0.007f,
                zScale = 0.007f,
                amplitude = 60f,
                persistence = 0.5f,
                lacunarity = 2f,
                octaves = 5
            };

            // Schedule and run the Job
            JobHandle handle = job.Schedule(vertices.Length, 64);
            handle.Complete();
            
            // Apply computed vertices on main thread
            mesh.vertices = verticesNative.ToArray();
            mesh.triangles = triangles;
            
            UpdateColors(minTerrainHeight, maxTerrainHeight);
            
            // Dispose Native Arrays
            verticesNative.Dispose();
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
}