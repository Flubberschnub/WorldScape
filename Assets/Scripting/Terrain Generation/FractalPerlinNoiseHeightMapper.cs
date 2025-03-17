using System;

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

        // Offsets to avoid symmetrical patterns
        public float offsetX;
        public float offsetZ;

        public float persistence = 0.5f;
        public float lacunarity = 2f;
        public int octaves = 4;

        private void OnEnable()
        {
            offsetX = Random.Range(0f, 9999999f);
            offsetZ = Random.Range(0f, 9999999f);
        }

        public override Vector3[] ApplyHeightMap(Vector3 positionOffset, Vector3[] vertices, int xSize, int zSize)
        {
            var newVertices = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                float x = vertices[i].x + positionOffset.x;
                float z = vertices[i].z + positionOffset.z;
                float y = FractalPerlinNoise((x + offsetX) * xScale, (z + offsetZ) * zScale) * amplitude;
                newVertices[i] = new Vector3(vertices[i].x, y, vertices[i].z);
            }
            return newVertices;
        }

        private float FractalPerlinNoise(float x, float z)
        {
            float total = 0f;
            float frequency = 1f;
            float curAmplitude = 1f;
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
