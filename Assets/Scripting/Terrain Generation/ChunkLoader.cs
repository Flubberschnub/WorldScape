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

        void Update()
        {
            var newChunkCoord = GetChunkCoord(cameraTransform.position);
            if (newChunkCoord != currentChunkCoord)
            {
                currentChunkCoord = newChunkCoord;
                LoadNearbyChunks();
                UnloadDistantChunks();
                terrainObjectScatterer.ScatterObjects(GetLoadedChunks());
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
        /// Loads the nearby chunks centered around the current chunk coordinate.
        /// Loops within the defined view distance along both x and z axes to ensure that
        /// all chunks within the view distance are loaded. If a chunk at a specific coordinate
        /// is not already loaded, it requests the chunk from the ChunkGenerator and records it
        /// in the loadedChunks dictionary.
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
                        loadedChunks.Add(coord, newChunk);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads chunks that are beyond the defined view distance from the current chunk coordinate.
        /// Iterates through the loadedChunks dictionary to identify chunks that exceed the view distance
        /// along either the x or z axis. Removes these distant chunks by returning them to the ChunkPool
        /// and removing their references from the loadedChunks dictionary.
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
                    chunkGenerator.chunkPool.Return(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var coord in toRemove)
            {
                loadedChunks.Remove(coord);
            }
        }
        
        public List<GameObject> GetLoadedChunks()
        {
            List<GameObject> chunks = new List<GameObject>();
            foreach (var kvp in loadedChunks)
            {
                chunks.Add(kvp.Value);
            }

            return chunks;
        }
    }
}