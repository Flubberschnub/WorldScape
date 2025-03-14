using UnityEngine;

[CreateAssetMenu(fileName = "FractalPerlinNoiseHeightMapper", menuName = "Scriptable Objects/FractalPerlinNoiseHeightMapper")]
public class FractalPerlinNoiseHeightMapper : HeightMapper
{

    public float xScale = 1f;
    public float zScale = 1f;
    public float amplitude = 1f;

    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public int octaves = 4;

    public override Vector3[] ApplyHeightMap(Vector3 positionOffset, Vector3[] vertices, int xSize, int zSize)
    {
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = FractalPerlinNoise((positionOffset.x + x) * xScale, (positionOffset.z + z) * zScale);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        return vertices;
    }

    private float FractalPerlinNoise(float x, float z){

        float total = 0;
        float frequency = 1;
        float amplitude = this.amplitude;
        float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
        
    }
    
}
