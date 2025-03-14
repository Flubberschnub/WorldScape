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

    public HeightMapper heightMapper;

    public bool realTimeUpdate = false;

    private GameObject[] chunks;

    public GameObject emptyChunkPrefab;

    public void GenerateChunks()
    {
        chunks = new GameObject[xSize * zSize];

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // Instantiate as a child of the current object
                GameObject chunk = Instantiate(emptyChunkPrefab, new Vector3(x * chunkSizeX * chunkResolutionX, 0, z * chunkSizeZ * chunkResolutionZ), Quaternion.identity, transform);
                chunk.GetComponent<MeshGenerator>().xSize = chunkSizeX;
                chunk.GetComponent<MeshGenerator>().zSize = chunkSizeZ;
                chunk.GetComponent<MeshGenerator>().xResolution = chunkResolutionX;
                chunk.GetComponent<MeshGenerator>().zResolution = chunkResolutionZ;
                chunk.GetComponent<MeshGenerator>().heightMapper = heightMapper;
                chunk.GetComponent<MeshGenerator>().realTimeUpdate = realTimeUpdate;
                chunks[z * xSize + x] = chunk;

                chunk.GetComponent<MeshGenerator>().Create();
            }
        }
    }

    public void ClearChunks()
    {
        foreach (GameObject chunk in chunks)
        {
            DestroyImmediate(chunk);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (realTimeUpdate)
        // {
        //     if (chunks != null)
        //     {
        //         foreach (GameObject chunk in chunks)
        //         {
        //             Destroy(chunk);
        //         }
        //         GenerateChunks();
        //     }
        // }

    }
}
