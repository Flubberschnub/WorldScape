namespace Scripting.Terrain_Generation
{
    using UnityEngine;

    public class ChunkGenerator : MonoBehaviour
    {

        // size of the chunks in vertices
        public int chunkSizeX = 20;
        public int chunkSizeZ = 20;

        // resolution of vertices (how many units per vertex)
        public float chunkResolutionX = 1f;
        public float chunkResolutionZ = 1f;

        public float speed;

        public HeightMapper heightMapper;

        public bool realTimeUpdate;

        public Gradient gradient;

        public float globalMinHeight = 0f;
        public float globalMaxHeight = 80f;

        public GameObject emptyChunkPrefab;

        public GameObject GenerateSingleChunk(int chunkX, int chunkZ)
        {
            // Calculate world position
            Vector3 position = new Vector3(
                chunkX * chunkSizeX * chunkResolutionX,
                0f,
                chunkZ * chunkSizeZ * chunkResolutionZ
            );

            // Instantiate chunk
            GameObject newChunk = Instantiate(emptyChunkPrefab, position, Quaternion.identity, transform);

            // Configure mesh generator
            MeshGenerator mg = newChunk.GetComponent<MeshGenerator>();
            mg.xSize = chunkSizeX;
            mg.zSize = chunkSizeZ;
            mg.xResolution = chunkResolutionX;
            mg.zResolution = chunkResolutionZ;
            mg.heightMapper = heightMapper;
            mg.realTimeUpdate = realTimeUpdate;
            mg.gradient = gradient;
            mg.speed = speed;
            mg.minTerrainHeight = globalMinHeight;
            mg.maxTerrainHeight = globalMaxHeight;

            // Create mesh
            mg.Create();
            return newChunk;
        }

        public GameObject GenerateEmptyChunkPrefab()
        {
            GameObject emptyChunk = new GameObject("ChunkPrefab");
            emptyChunk.AddComponent<MeshFilter>();
            emptyChunk.AddComponent<MeshRenderer>();
            emptyChunk.AddComponent<MeshGenerator>();

            emptyChunk.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("New-Plane.fbx");

            emptyChunk.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/HeightmapColorShader"));

            return emptyChunk;
        }

        void Start()
        {
            emptyChunkPrefab = GenerateEmptyChunkPrefab();
            // GenerateChunks();
        }
    }
}