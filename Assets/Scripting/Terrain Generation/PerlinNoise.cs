using UnityEngine;

public class PerlinNoise : HeightMapper
{

    public float xScale = 1f;
    public float zScale = 1f;
    public float amplitude = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override Vector3[] ApplyHeightMap(Vector3[] vertices, int xSize, int zSize)
    {
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * xScale, z * zScale) * amplitude;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        return vertices;
    }
}
