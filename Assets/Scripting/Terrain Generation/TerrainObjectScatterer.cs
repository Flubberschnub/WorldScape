using UnityEngine;
using UnityEngine.Serialization;

namespace Scripting.Terrain_Generation
{
    public class TerrainObjectScatterer : MonoBehaviour
    {
        public GameObject[] environmentObjects;
        public float worldScatterDensity = 0.1f;
        private System.Random seededRandom;
        
        public void ScatterObjects(GameObject chunk)
        {
            // Seed the random generator based on chunk position
            Vector3 chunkPosition = chunk.transform.position;
            int seed = chunkPosition.GetHashCode();
            seededRandom = new System.Random(seed);
            
            if (seededRandom.NextDouble() > worldScatterDensity)
            {
                return; // Skip scattering objects for this chunk
            }
            
            MeshGenerator meshGenerator = chunk.GetComponent<MeshGenerator>();
            Mesh mesh = meshGenerator.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            
            // Generate a random point on the chunk
            Vector3 randomPoint = GenerateRandomPointOnChunk(vertices, meshGenerator);
            
            float normalizedHeight = Mathf.InverseLerp(
                meshGenerator.minTerrainHeight,
                meshGenerator.maxTerrainHeight,
                randomPoint.y
            );

            int objectIndex;
            if (normalizedHeight < 0.35f)
                objectIndex = Mathf.FloorToInt((float)seededRandom.NextDouble() * ((float)environmentObjects.Length / 3));
            else if (normalizedHeight < 0.65f)
                objectIndex = Mathf.FloorToInt((float)seededRandom.NextDouble() * ((float)environmentObjects.Length / 3)) + environmentObjects.Length / 3;
            else
                objectIndex = Mathf.FloorToInt((float)seededRandom.NextDouble() * ((float)environmentObjects.Length / 3)) + 2 * (environmentObjects.Length / 3);

            // Instantiate the object
            GameObject selectedObject = environmentObjects[Mathf.Abs(objectIndex) % environmentObjects.Length];
            float randomYRotation = (float)seededRandom.NextDouble() * 360f;
            float randomZRotation = (float)seededRandom.NextDouble() * 360f;
            Quaternion randomRotation = Quaternion.Euler(-90, randomYRotation, randomZRotation);
            Instantiate(selectedObject, randomPoint, randomRotation, chunk.transform);
        }

        Vector3 FindClosestVertex(Vector3[] vertices, float x, float z, Vector3 chunkPosition)
        {
            Vector3 closest = Vector3.zero;
            float minDistance = float.MaxValue;

            foreach (var vertex in vertices)
            {
                Vector3 worldVertex = vertex + chunkPosition;
                float distance = Vector2.Distance(new Vector2(worldVertex.x, worldVertex.z), new Vector2(x, z));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = worldVertex;
                }
            }

            return closest;
        }
        
        private Vector3 GenerateRandomPointOnChunk(Vector3[] vertices, MeshGenerator meshGenerator)
        {
            // Use seeded random to generate consistent random values
            float randomX = (float)seededRandom.NextDouble() * meshGenerator.xSize * meshGenerator.xResolution;
            float randomZ = (float)seededRandom.NextDouble() * meshGenerator.zSize * meshGenerator.zResolution;

            return FindClosestVertex(vertices, randomX, randomZ, meshGenerator.transform.position);
        }

    }
}