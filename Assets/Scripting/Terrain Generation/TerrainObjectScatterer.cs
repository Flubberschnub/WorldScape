using System.Collections.Generic;
using UnityEngine;

namespace Scripting.Terrain_Generation
{
    public class TerrainObjectScatterer : MonoBehaviour
    {
        public GameObject[] environmentObjects;
        public int objectCount = 100;

        public void ScatterObjects(List<GameObject> loadedChunks)
        {
            foreach (var chunk in loadedChunks)
            {
                MeshGenerator meshGenerator = chunk.GetComponent<MeshGenerator>();
                Mesh mesh = meshGenerator.GetComponent<MeshFilter>().mesh;
                Vector3[] vertices = mesh.vertices;

                for (int i = 0; i < objectCount; i++)
                {
                    // Generate random position within the chunk bounds
                    float randomX = Random.Range(0, meshGenerator.xSize * meshGenerator.xResolution);
                    float randomZ = Random.Range(0, meshGenerator.zSize * meshGenerator.zResolution);

                    // Find the closest vertex to determine the height
                    Vector3 closestVertex = FindClosestVertex(vertices, randomX, randomZ, meshGenerator.transform.position);

                    // Instantiate the object at the calculated position
                    GameObject selectedObject = environmentObjects[Random.Range(0, environmentObjects.Length)];
                    Instantiate(selectedObject, closestVertex, Quaternion.identity, chunk.transform);
                }
            }
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
    }
}