using System.Collections.Generic;
using UnityEngine;

namespace Scripting.Terrain_Generation
{
    public class ChunkGenerator : MonoBehaviour
    {
        private static readonly int LowTex = Shader.PropertyToID("_LowTex");
        private static readonly int MidTex = Shader.PropertyToID("_MidTex");
        private static readonly int HighTex = Shader.PropertyToID("_HighTex");

        // resolution of vertices (how many units per vertex)
        public float chunkResolutionX = 1f;
        public float chunkResolutionZ = 1f;
        public float offsetX;
        public float offsetZ;

        public Gradient gradient;

        public float globalMinHeight = 0f;
        public float globalMaxHeight = 80f;

        public GameObject emptyChunkPrefab;
        public ChunkPool chunkPool;

        /// Generates a single chunk at the specified world position and configures its properties for terrain generation.
        /// The method calculates world coordinates for the chunk based on input parameters, retrieves a pooled chunk object,
        /// and sets its position, rotation, and mesh generation parameters. It finalizes by creating the mesh for the chunk.
        /// <param name="chunkX">
        /// The X-coordinate of the chunk in the chunk grid.
        /// </param>
        /// <param name="chunkZ">
        /// The Z-coordinate of the chunk in the chunk grid.
        /// </param>
        /// <param name="chunkSizeX">
        /// The size of the chunk along the X-axis in units.
        /// </param>
        /// <param name="chunkSizeZ">
        /// The size of the chunk along the Z-axis in units.
        /// </param>
        /// <param name="LOD">
        /// The level of detail (LOD) for the chunk. There are three levels - 0 (Full detail), 1 (Half detail), 2 (Quarter detail)
        /// </param>
        /// <returns>
        /// Returns a GameObject representing the newly generated and configured chunk.
        /// </returns>
        public GameObject GenerateSingleChunk(int chunkX, int chunkZ, int chunkSizeX, int chunkSizeZ, int LOD)
        {
            // Calculate world position
            Vector3 position = new Vector3(
                chunkX * chunkSizeX * chunkResolutionX,
                0f,
                chunkZ * chunkSizeZ * chunkResolutionZ
            );

            // Retrieve a chunk from the pool
            GameObject pooledChunk = chunkPool.Get();
            pooledChunk.transform.position = position;
            pooledChunk.transform.rotation = Quaternion.identity;

            // Configure mesh generator (Set values based on LOD)
            MeshGenerator mg = pooledChunk.GetComponent<MeshGenerator>();
            SetMeshGeneratorValues(mg, LOD, chunkSizeX, chunkResolutionX, chunkSizeZ, chunkResolutionZ);



            // Create mesh
            mg.Create();
            return pooledChunk;
        }

        /// Sets the values of the MeshGenerator based on the LOD
        public void SetMeshGeneratorValues(MeshGenerator mg, int LOD, int baseSizeX, float baseResolutionX, int baseSizeZ, float baseResolutionZ)
        {
            int reductionFactor = (int)Mathf.Pow(2, LOD); // The factor by which the amount of vertices along the x and z axis are reduced
            mg.xSize = baseSizeX / reductionFactor;
            mg.zSize = baseSizeZ / reductionFactor;
            mg.xResolution = baseResolutionX * reductionFactor;
            mg.zResolution = baseResolutionZ * reductionFactor;
            mg.offsetX = offsetX;
            mg.offsetZ = offsetZ;
            mg.gradient = gradient;
            mg.minTerrainHeight = globalMinHeight;
            mg.maxTerrainHeight = globalMaxHeight;
        }

        /// Generates a GameObject representing an empty chunk prefab with default components necessary for terrain generation.
        /// The method creates a new GameObject, adds essential components such as MeshFilter, MeshRenderer, and MeshGenerator,
        /// and assigns a built-in plane mesh and a custom shader material for rendering.
        /// <returns>
        /// Returns a GameObject configured as an empty chunk prefab with default properties.
        /// </returns>
        public GameObject GenerateEmptyChunkPrefab()
        {
            GameObject emptyChunk = new GameObject("ChunkPrefab");
            emptyChunk.AddComponent<MeshFilter>();
            emptyChunk.AddComponent<MeshRenderer>();
            emptyChunk.AddComponent<MeshGenerator>();
            emptyChunk.AddComponent<MeshCollider>();

            emptyChunk.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("New-Plane.fbx");

            Material texShader = new Material(Shader.Find("Custom/HeightmapTextureShader"));
            if (texShader != null)
            {
                texShader.SetTexture(LowTex, Resources.Load<Texture>("Textures/grass"));
                texShader.SetTexture(MidTex, Resources.Load<Texture>("Textures/stone"));
                texShader.SetTexture(HighTex, Resources.Load<Texture>("Textures/snow"));
            }

            emptyChunk.GetComponent<MeshRenderer>().material = texShader;

            return emptyChunk;
        }

        void Start()
        {
            emptyChunkPrefab = GenerateEmptyChunkPrefab();
            chunkPool = new ChunkPool(emptyChunkPrefab, transform, 25);
        }
    }
}