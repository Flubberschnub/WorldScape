using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Scripting.Jobs
{
    [BurstCompile]
    public struct PerlinNoiseHeightMapJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;
        
        [ReadOnly] public Vector3 positionOffset;
        [ReadOnly] public float xScale;
        [ReadOnly] public float zScale;
        [ReadOnly] public float amplitude;

        public void Execute(int index)
        {
            Vector3 vertex = vertices[index];
            float y = Mathf.PerlinNoise((positionOffset.x + vertex.x) * xScale, (positionOffset.z + vertex.z) * zScale) *
                      amplitude;
            vertices[index] = new Vector3(vertex.x, y, vertex.z);
        }
    }
}