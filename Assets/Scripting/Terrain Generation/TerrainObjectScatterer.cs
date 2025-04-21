using System.Collections.Generic;
using UnityEngine;

namespace Scripting.Terrain_Generation
{
    public class TerrainObjectScatterer : MonoBehaviour
    {
        public GameObject[] environmentObjects;
        public float objectCount = 1;

        public void ScatterObjects(GameObject chunk)
        {
            if (objectCount < 1)
                if (Random.Range(0f, 1f) > objectCount)
                    return;
                
            MeshGenerator meshGenerator = chunk.GetComponent<MeshGenerator>();
            Mesh mesh = meshGenerator.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            
            int count = 0;
            foreach (Transform child in chunk.transform)
            {
                // Reposition existing children to scatter locations
                if (count < objectCount)
                {
                    Vector3 randomPoint = GenerateRandomPointOnChunk(vertices, meshGenerator);
                    child.position = randomPoint;
                    child.gameObject.SetActive(true);
                    count++;
                }
                else
                {
                    // Deactivate extra pooled objects
                    child.gameObject.SetActive(false);
                }
            }

            for (; count < objectCount; count++)
            {
                // Instantiate or retrieve new pooled objects
                Vector3 randomPoint = GenerateRandomPointOnChunk(vertices, meshGenerator);
                GameObject selectedObject = environmentObjects[Random.Range(0, environmentObjects.Length)];
                GameObject obj = Instantiate(selectedObject, randomPoint, Quaternion.identity, chunk.transform);
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
        
        private Vector3 GenerateRandomPointOnChunk(Vector3[] vertices, MeshGenerator meshGenerator)
        {
            float randomX = Random.Range(0, meshGenerator.xSize * meshGenerator.xResolution);
            float randomZ = Random.Range(0, meshGenerator.zSize * meshGenerator.zResolution);
            return FindClosestVertex(vertices, randomX, randomZ, meshGenerator.transform.position);
        }

    }
}