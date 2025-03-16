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

        // size of the total terrain in chunks
        public int xSize = 20;
        public int zSize = 20;

        public float speed;

        public HeightMapper heightMapper;

        public bool realTimeUpdate;

        private GameObject[] chunks;

        public Gradient gradient;

        public float globalMinHeight;
        public float globalMaxHeight;

        public GameObject emptyChunkPrefab;

        public void GenerateChunks()
        {
            globalMinHeight = float.MaxValue;
            globalMaxHeight = float.MinValue;
            chunks = new GameObject[xSize * zSize];

            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    // Instantiate as a child of the current object
                    GameObject chunk = Instantiate(emptyChunkPrefab,
                        new Vector3(x * chunkSizeX * chunkResolutionX, 0, z * chunkSizeZ * chunkResolutionZ),
                        Quaternion.identity, transform);
                    chunk.GetComponent<MeshGenerator>().xSize = chunkSizeX;
                    chunk.GetComponent<MeshGenerator>().zSize = chunkSizeZ;
                    chunk.GetComponent<MeshGenerator>().xResolution = chunkResolutionX;
                    chunk.GetComponent<MeshGenerator>().zResolution = chunkResolutionZ;
                    chunk.GetComponent<MeshGenerator>().heightMapper = heightMapper;
                    chunk.GetComponent<MeshGenerator>().realTimeUpdate = realTimeUpdate;
                    chunk.GetComponent<MeshGenerator>().gradient = gradient;

                    chunks[z * xSize + x] = chunk;

                    chunk.GetComponent<MeshGenerator>().Create();
                    globalMinHeight = Mathf.Min(globalMinHeight, chunk.GetComponent<MeshGenerator>().minTerrainHeight);
                    globalMaxHeight = Mathf.Max(globalMaxHeight, chunk.GetComponent<MeshGenerator>().maxTerrainHeight);
                }
            }
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

        public void ClearChunks()
        {
            foreach (GameObject chunk in chunks)
            {
                DestroyImmediate(chunk);
            }
        }

        void Start()
        {
            emptyChunkPrefab = GenerateEmptyChunkPrefab();
            GenerateChunks();
        }

        void Update()
        {
            if (realTimeUpdate)
            {
                if (chunks != null)
                {
                    foreach (GameObject chunk in chunks)
                    {
                        chunk.GetComponent<MeshGenerator>().speed = speed;
                        chunk.GetComponent<MeshGenerator>().UpdateVertices();
                        globalMinHeight = Mathf.Min(globalMinHeight,
                            chunk.GetComponent<MeshGenerator>().minTerrainHeight);
                        globalMaxHeight = Mathf.Max(globalMaxHeight,
                            chunk.GetComponent<MeshGenerator>().maxTerrainHeight);
                    }

                    foreach (GameObject chunk in chunks)
                    {
                        chunk.GetComponent<MeshGenerator>().UpdateColors(globalMinHeight, globalMaxHeight);
                    }
                }
            }
        }

    }
}