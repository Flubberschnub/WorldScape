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

        public GameObject GenerateSingleChunk(int chunkX, int chunkZ, int chunkSizeX, int chunkSizeZ)
        {
            // Calculate world position
            Vector3 position = new Vector3(
                chunkX * chunkSizeX * chunkResolutionX,
                0f,
                chunkZ * chunkSizeZ * chunkResolutionZ
            );

            // Instantiate chunk
            GameObject pooledChunk = chunkPool.Get();
            pooledChunk.transform.position = position;
            pooledChunk.transform.rotation = Quaternion.identity;

            // Configure mesh generator
            MeshGenerator mg = pooledChunk.GetComponent<MeshGenerator>();
            mg.xSize = chunkSizeX;
            mg.zSize = chunkSizeZ;
            mg.offsetX = offsetX;
            mg.offsetZ = offsetZ;
            mg.xResolution = chunkResolutionX;
            mg.zResolution = chunkResolutionZ;
            mg.gradient = gradient;
            mg.minTerrainHeight = globalMinHeight;
            mg.maxTerrainHeight = globalMaxHeight;

            // Create mesh
            mg.Create();
            return pooledChunk;
        }

        public GameObject GenerateEmptyChunkPrefab()
        {
            GameObject emptyChunk = new GameObject("ChunkPrefab");
            emptyChunk.AddComponent<MeshFilter>();
            emptyChunk.AddComponent<MeshRenderer>();
            emptyChunk.AddComponent<MeshGenerator>();

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

            // GenerateChunks();
        }
    }
}