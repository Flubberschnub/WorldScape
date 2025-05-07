using Scripting.Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


namespace Scripting.Terrain_Generation
{
    [ExecuteInEditMode]
    public class MeshGenerator : MonoBehaviour
    {

        Mesh mesh;

        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;
        Color[] colors;

        // resolution of vertices (how many units per vertex)
        public float xResolution = 1f;
        public float zResolution = 1f;

        // size of the mesh in vertices
        public int xSize = 20;
        public int zSize = 20;

        public float offsetX;
        public float offsetZ;

        public float amplitude;

        public Gradient gradient;

        public float minTerrainHeight;
        public float maxTerrainHeight;

        /// <summary>
        /// Generates a mesh with procedural terrain based on fractal Perlin noise, updating the mesh data
        /// (vertices, triangles) and visual properties (colors) for the assigned MeshFilter.
        /// </summary>
        /// <remarks>
        /// This method initializes the mesh, generates a base grid of vertices, and assigns it to
        /// a Mesh object. It employs a parallelized job system to calculate the heightmap using
        /// fractal Perlin noise, enhancing performance for large-scale terrain generation. Once
        /// the computations are complete, the updated vertices and triangles are applied to
        /// the mesh, and the terrain colors are updated to reflect height-based gradients.
        /// </remarks>
        public void Create()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            MeshCollider meshCollider = GetComponent<MeshCollider>();

            CreateBase();

            // Uses NativeArray for thread safety and optimal performance during parallelization.
            NativeArray<Vector3> verticesNative = new NativeArray<Vector3>(vertices, Allocator.TempJob);
            FractalPerlinNoiseHeightMapJob job = new FractalPerlinNoiseHeightMapJob
            {
                vertices = verticesNative,
                positionOffset = transform.position,
                offsetX = offsetX,
                offsetZ = offsetZ,
                xScale = 0.001f,
                zScale = 0.001f,
                amplitude = amplitude,
                persistence = 0.5f,
                lacunarity = 2f,
                octaves = 5
            };
            JobHandle handle = job.Schedule(vertices.Length, 64);
            handle.Complete();

            mesh.vertices = verticesNative.ToArray();
            mesh.uv = uvs;
            mesh.triangles = triangles;

            UpdateColors(minTerrainHeight, maxTerrainHeight);

            verticesNative.Dispose();

            if (meshCollider != null) // If the mesh is changed, update the new mesh collider
            {
                meshCollider.sharedMesh = mesh;
            }

        }

 
        /// <summary>
        /// Initializes the base structure of the mesh, including vertices, triangles, and colors,
        /// to represent a grid of defined resolution and size.
        /// </summary>
        /// <remarks>
        /// This method calculates the positions of vertices based on the specified x and z dimensions
        /// and resolution, forming the initial grid. It creates the indices for mesh triangles to
        /// define the connectivity of the geometry. Additionally, it applies a gradient color to each
        /// vertex based on its height relative to the given minimum and maximum terrain height values.
        /// </remarks>
        void CreateBase()
        {

            int vertexCount = (xSize + 1) * (zSize + 1);
            Debug.Log($"Creating base with xSize={xSize}, zSize={zSize}, vertices count={vertexCount}");
            vertices = new Vector3[vertexCount];
            uvs = new Vector2[vertices.Length];

            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    vertices[i] = new Vector3(x * xResolution, 0, z * zResolution);
                    uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
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

            // Assign colors based on height using an inverse lerp to map the height to a gradient color.
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

        /// <summary>
        /// Updates the mesh vertex colors based on their height relative to the
        /// specified minimum and maximum terrain height values.
        /// </summary>
        /// <param name="min">The minimum height value used for color evaluation.</param>
        /// <param name="max">The maximum height value used for color evaluation.</param>
        /// <remarks>
        /// This method utilizes a gradient to map the height of each vertex to a color
        /// and assigns the resulting colors to the mesh. It also recalculates the normals
        /// for the mesh to ensure accurate shading after updating the colors.
        /// </remarks>
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
    }
}
