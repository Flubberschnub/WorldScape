using UnityEngine;
using UnityEngine.Serialization;

namespace Scripting.Terrain_Generation
{
    public class TerrainObjectScatterer : MonoBehaviour
    {
        public GameObject[] environmentObjects; // Should contain prefabs of objects to scatter. Can be set in the inspector for now.
        public float worldScatterDensity = 0.1f;
        private System.Random seededRandom;

        /// Scatters objects randomly onto a specified terrain chunk using predefined rules
        /// and weighted selection based on terrain height and a seeded random generator.
        /// <param name="chunk">
        /// The terrain chunk GameObject on which objects will be scattered. The chunk
        /// must have a MeshGenerator component to provide terrain height and vertex data.
        /// </param>
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

        /// Finds the closest vertex within a set of vertices to the given x and z coordinates,
        /// considering the position of the chunk in world space.
        /// <param name="vertices">
        /// An array of vertices representing points on the mesh in local coordinates.
        /// </param>
        /// <param name="x">
        /// The x coordinate in world space to find the closest vertex to.
        /// </param>
        /// <param name="z">
        /// The z coordinate in world space to find the closest vertex to.
        /// </param>
        /// <param name="chunkPosition">
        /// The position of the chunk in world space, used to translate vertices to world coordinates.
        /// </param>
        /// <returns>
        /// The closest vertex in world space to the specified x and z coordinates.
        /// </returns>
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

        /// Generates a random point on the specified chunk using the vertex data and chunk configuration.
        /// <param name="vertices">
        /// An array of vertices representing the points on the mesh in local coordinates.
        /// </param>
        /// <param name="meshGenerator">
        /// The MeshGenerator instance associated with the chunk, providing information such as size,
        /// resolution, and transformation data.
        /// </param>
        /// <returns>
        /// A Vector3 position in world space representing a randomly selected point on the chunk.
        /// </returns>
        private Vector3 GenerateRandomPointOnChunk(Vector3[] vertices, MeshGenerator meshGenerator)
        {
            // Use seeded random to generate consistent random values
            float randomX = (float)seededRandom.NextDouble() * meshGenerator.xSize * meshGenerator.xResolution;
            float randomZ = (float)seededRandom.NextDouble() * meshGenerator.zSize * meshGenerator.zResolution;

            return FindClosestVertex(vertices, randomX, randomZ, meshGenerator.transform.position);
        }
    }
}