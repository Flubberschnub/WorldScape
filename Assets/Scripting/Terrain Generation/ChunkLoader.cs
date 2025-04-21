using UnityEngine;
using System.Collections.Generic;

namespace Scripting.Terrain_Generation
{
    public class ChunkLoader : MonoBehaviour
    {
        public Transform cameraTransform;
        private ChunkGenerator chunkGenerator;
        private TerrainObjectScatterer terrainObjectScatterer;
        public int chunkWorldSizeX = 20;
        public int chunkWorldSizeZ = 20;
        public int viewDistance = 1; // how many chunks around the player to keep loaded
        
        private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
        private Vector2Int currentChunkCoord;

        private void Start()
        {
            chunkGenerator = GetComponent<ChunkGenerator>();
            terrainObjectScatterer = GetComponent<TerrainObjectScatterer>();
            
            // Set initial random offsets to perlin noise to ensure unique terrain generation
            chunkGenerator.offsetX = Random.Range(0f, 9999999f);
            chunkGenerator.offsetZ = Random.Range(0f, 9999999f);
        }

        private void Update()
        {
            Vector2Int newChunkCoord = GetChunkCoord(cameraTransform.position);
            if (newChunkCoord != currentChunkCoord)
            {
                currentChunkCoord = newChunkCoord;
                LoadNearbyChunks();
                UnloadDistantChunks();
            }
        }

        /// <summary>
        /// Calculates the chunk coordinate (x, z) based on the provided world position.
        /// The coordinate is determined by dividing the world position by the corresponding
        /// chunk world size along the x and z axes and flooring the values to integers.
        /// </summary>
        /// <param name="pos">The 3D position in the world space, representing the current position to evaluate.</param>
        /// <returns>A Vector2Int containing the chunk coordinates (x, z) corresponding to the given position.</returns>
        Vector2Int GetChunkCoord(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x / chunkWorldSizeX);
            int z = Mathf.FloorToInt(pos.z / chunkWorldSizeZ);
            return new Vector2Int(x, z);
        }

        /// <summary>
        /// Loads all chunks surrounding the player's current chunk position, within the specified view distance.
        /// Chunks that are not yet loaded are instantiated using the chunk generator, and any necessary terrain
        /// objects are scattered onto them. Loaded chunks are added to the dictionary for management.
        /// </summary>
        void LoadNearbyChunks()
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                for (int x = -viewDistance; x <= viewDistance; x++)
                {
                    Vector2Int coord = new Vector2Int(currentChunkCoord.x + x, currentChunkCoord.y + z);
                    if (!loadedChunks.ContainsKey(coord))
                    {
                        GameObject newChunk = chunkGenerator.GenerateSingleChunk(coord.x, coord.y, chunkWorldSizeX, chunkWorldSizeZ);
                        terrainObjectScatterer.ScatterObjects(newChunk);
                        loadedChunks.Add(coord, newChunk);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads chunks that are outside the player's view distance to free up resources.
        /// This is determined by calculating the distance between the current chunk coordinates
        /// and the coordinates of each loaded chunk. If a chunk lies beyond the specified view distance,
        /// it is removed from the dictionary of loaded chunks, deactivated, and returned to the chunk pool.
        /// </summary>
        void UnloadDistantChunks()
        {
            List<Vector2Int> toRemove = new List<Vector2Int>();
            foreach (var kvp in loadedChunks)
            {
                int distX = Mathf.Abs(kvp.Key.x - currentChunkCoord.x);
                int distZ = Mathf.Abs(kvp.Key.y - currentChunkCoord.y);
                if (distX > viewDistance || distZ > viewDistance)
                {
                    GameObject chunk = kvp.Value;
                    foreach (Transform child in chunk.transform)
                    {
                        child.gameObject.SetActive(false); // Deactivate scattered objects
                    }
                    chunkGenerator.chunkPool.Return(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var coord in toRemove)
            {
                loadedChunks.Remove(coord);
            }
        }
    }
}