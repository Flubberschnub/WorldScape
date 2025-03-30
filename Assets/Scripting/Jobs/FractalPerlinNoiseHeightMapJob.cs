using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Scripting.Jobs
{
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

        /// Executes the job to calculate and update the y-coordinate of vertices based on fractal Perlin noise for a height map.
        /// It processes a single vertex at a time to enable parallelization using the Unity Job System.
        /// <param name="index">
        /// The index of the vertex in the vertices array being processed.
        /// </param>
        public void Execute(int index)
        {
            Vector3 vertex = vertices[index];
            float x = vertex.x + positionOffset.x;
            float z = vertex.z + positionOffset.z;
            float sampleX = (x + offsetX) * xScale;
            float sampleZ = (z + offsetZ) * zScale;
            float y = FractalPerlinNoise(sampleX, sampleZ) * amplitude;

            vertices[index] = new Vector3(vertex.x, y, vertex.z);
        }


        /// Calculates fractal Perlin noise based on the specified coordinates, using defined fractal parameters.
        /// <param name="x">
        /// The x-coordinate for the Perlin noise calculation.
        /// </param>
        /// <param name="z">
        /// The z-coordinate for the Perlin noise calculation.
        /// </param>
        /// <return>
        /// The calculated fractal Perlin noise value at the given coordinates.
        /// </return>
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