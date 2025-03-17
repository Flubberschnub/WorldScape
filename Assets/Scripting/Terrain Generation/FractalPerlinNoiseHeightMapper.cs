namespace Scripting.Terrain_Generation
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "FractalPerlinNoiseHeightMapper",
        menuName = "Scriptable Objects/FractalPerlinNoiseHeightMapper")]
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
            Vector3[] newVertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                float x = vertices[i].x + positionOffset.x;
                float z = vertices[i].z + positionOffset.z;

                float y = FractalPerlinNoise(x * xScale, z * zScale) * amplitude;

                newVertices[i] = new Vector3(vertices[i].x, y, vertices[i].z);
            }

            return newVertices;
        }

        private float FractalPerlinNoise(float x, float z)
        {

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
}
