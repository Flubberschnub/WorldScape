namespace Scripting.Jobs
{
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;

    [BurstCompile]
    public struct FractalPerlinNoiseHeightMapJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;

        [ReadOnly] public Vector3 positionOffset;
        [ReadOnly] public float offsetX;
        [ReadOnly] public float offsetZ;
        [ReadOnly] public float xScale;
        [ReadOnly] public float zScale;
        [ReadOnly] public float amplitude;
        [ReadOnly] public float persistence;
        [ReadOnly] public float lacunarity;
        [ReadOnly] public int octaves;

        public void Execute(int index)
        {
            Vector3 vertex = vertices[index];
            float x = vertex.x + positionOffset.x;
            float z = vertex.z + positionOffset.z;
            float sampleX = (x + offsetX) * xScale;
            float sampleZ = (x + offsetZ) * zScale;
            float y = FractalPerlinNoise(sampleX, sampleZ) * amplitude;

            vertices[index] = new Vector3(vertex.x, y, vertex.z);
        }

        // Your original fractal perlin noise function reimplemented here for burst compatibility
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