using UnityEngine;

[CreateAssetMenu(fileName = "PerlinNoiseHeightMapper", menuName = "Scriptable Objects/PerlinNoiseHeightMapper")]
public class PerlinNoiseHeightMapper : HeightMapper
{

    public float xScale = 1f;
    public float zScale = 1f;
    public float amplitude = 1f;

    public override Vector3[] ApplyHeightMap(Vector3 positionOffset, Vector3[] vertices, int xSize, int zSize)
    {
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise((positionOffset.x + x) * xScale, (positionOffset.z + z) * zScale) * amplitude;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        return vertices;
    }
    
}
