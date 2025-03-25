using UnityEngine;
using System.Collections.Generic;

namespace Scripting.Terrain_Generation
{
    public class ChunkLoader : MonoBehaviour
    {
        public Transform cameraTransform;
        private ChunkGenerator chunkGenerator;
        public int chunkWorldSizeX = 20;
        public int chunkWorldSizeZ = 20;
        public int viewDistance = 1; // how many chunks around the player to keep loaded
        
        private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
        private Vector2Int currentChunkCoord;

        private void Start()
        {
            chunkGenerator = GetComponent<ChunkGenerator>();
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
            }
        }

        Vector2Int GetChunkCoord(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x / chunkWorldSizeX);
            int z = Mathf.FloorToInt(pos.z / chunkWorldSizeZ);
            return new Vector2Int(x, z);
        }

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
    }
}