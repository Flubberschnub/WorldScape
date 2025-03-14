using UnityEngine;

[ExecuteInEditMode]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public bool realTimeUpdate = false;

    public int xSize = 20;
    public int zSize = 20;

    public float xScale = 1f;
    public float zScale = 1f;


    public HeightMapper heightMapper;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateBase();
        UpdateMesh();

    }

    void CreateBase()
    {

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x * xScale, 0, z * zScale);
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

    }

    void UpdateMesh()
    {

        mesh.Clear();

        mesh.vertices = heightMapper.ApplyHeightMap(transform.position, vertices, xSize, zSize);
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

    }

    // Update is called once per frame
    void Update()
    {

        if (realTimeUpdate)
        {
            //CreateBase();
            UpdateMesh();
        }

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
