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

            float perlin = FractalPerlinNoise(sampleX, sampleZ);
            float voronoi = VoronoiNoise(sampleX, sampleZ, 10f);

            float cliffs = Mathf.Pow(1f - voronoi, 8f);

            // Blend Perlin and Voronoi
            float y = (perlin * 0.3f + cliffs * 0.7f) * amplitude;

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

        /// Computes the Voronoi noise value at the specified coordinates, factoring in a defined cell size.
        /// Voronoi noise determines the closest distance from the point to the nearest randomly offset cell center, yielding a value normalized between 0 and 1.
        /// <param name="x">
        /// The x-coordinate for the noise calculation.
        /// </param>
        /// <param name="z">
        /// The z-coordinate for the noise calculation.
        /// </param>
        /// <param name="cellSize">
        /// The size of each grid cell for calculating the Voronoi noise.
        /// </param>
        /// <return>
        /// The normalized Voronoi noise value at the given coordinates.
        /// </return>
        private float VoronoiNoise(float x, float z, float cellSize)
        {
            float minDist = float.MaxValue;
            int cellX = Mathf.FloorToInt(x / cellSize);
            int cellZ = Mathf.FloorToInt(z / cellSize);

            for (int dz = -1; dz <= 1; dz++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int neighborX = cellX + dx;
                    int neighborZ = cellZ + dz;
                    // Random offset for cell center
                    float cx = neighborX * cellSize + Mathf.PerlinNoise(neighborX, neighborZ) * cellSize;
                    float cz = neighborZ * cellSize + Mathf.PerlinNoise(neighborZ, neighborX) * cellSize;
                    float dist = Vector2.Distance(new Vector2(x, z), new Vector2(cx, cz));
                    if (dist < minDist) minDist = dist;
                }
            }
            return minDist / cellSize; // Normalized [0,1]
        }

    }
}