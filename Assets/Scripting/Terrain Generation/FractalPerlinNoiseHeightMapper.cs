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
                float y = FractalPerlinNoise((positionOffset.x + x) * xScale, (positionOffset.z + z) * zScale) * amplitude;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        return vertices;
    }

    private float FractalPerlinNoise(float x, float z){

        float total = 0;
        float frequency = 1;
        float curAmplitude = 1;

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * curAmplitude;

            curAmplitude *= persistence;
            frequency *= lacunarity;
        }

        return total;
        
    }
    
}
