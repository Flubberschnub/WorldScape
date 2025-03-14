using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{

    public int chunkSizeX = 20;
    public int chunkSizeZ = 20;

    public float chunkScaleX = 1f;
    public float chunkScaleZ = 1f;

    public int xSize = 20;
    public int zSize = 20;

    public HeightMapper heightMapper;

    public bool realTimeUpdate = false;

    private GameObject[] chunks;

    public GameObject emptyChunkPrefab;

    public void GenerateChunks()
    {
        chunks = new GameObject[chunkSizeX * chunkSizeZ];

        for (int z = 0; z < chunkSizeZ; z++)
        {
            for (int x = 0; x < chunkSizeX; x++)
            {
                // Instantiate as a child of the current object
                GameObject chunk = Instantiate(emptyChunkPrefab, new Vector3(x * xSize * chunkScaleX, 0, z * zSize * chunkScaleZ), Quaternion.identity, transform);
                chunk.GetComponent<MeshGenerator>().xSize = xSize;
                chunk.GetComponent<MeshGenerator>().zSize = zSize;
                chunk.GetComponent<MeshGenerator>().xScale = chunkScaleX;
                chunk.GetComponent<MeshGenerator>().zScale = chunkScaleZ;
                chunk.GetComponent<MeshGenerator>().heightMapper = heightMapper;
                chunk.GetComponent<MeshGenerator>().realTimeUpdate = realTimeUpdate;
                chunks[z * chunkSizeX + x] = chunk;
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

        if (realTimeUpdate)
        {
            foreach (GameObject chunk in chunks)
            {
                Destroy(chunk);
            }
            GenerateChunks();
        }

    }
}
