using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Scripting.Terrain_Generation
{
    public class ChunkLoader : MonoBehaviour
    {
        public GameObject loadingPanel;
        public Slider loadingProgress;

        public Transform cameraTransform;
        public Transform characterTransform;
        private ChunkGenerator chunkGenerator;
        private TerrainObjectScatterer terrainObjectScatterer;
        public int chunkWorldSizeX = 20;
        public int chunkWorldSizeZ = 20;
        public float terrainAmplitude = 300f;
        public int viewDistance = 20; // how many chunks around the player to keep loaded
        public bool randomOffset;
        public int lodUpdatesPerFrame = 4;
        public int chunkUpdatesPerFrame = 4;
        public float LOD0Dist = 0.2f; // The ratio of the distance from the player where the chunks are LOD 0 (compared to view distance)
        public float LOD1Dist = 0.5f; // The ratio of the distance from the player where the chunks are LOD 0 (compared to view distance)

        private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
        private Vector2Int currentChunkCoord;
        private bool initialized = false;

        private void Start()
        {
            FirstPersonMovement movement = FindFirstObjectByType<FirstPersonMovement>();
            if (movement != null)
            {
                characterTransform = movement.transform;
                currentChunkCoord = GetChunkCoord(characterTransform.position);
            }
            else
            {
                return;
            }
            chunkGenerator = GetComponent<ChunkGenerator>();
            terrainObjectScatterer = GetComponent<TerrainObjectScatterer>();

            // Set initial random offsets to perlin noise to ensure unique terrain generation
            if (randomOffset)
            {
                chunkGenerator.offsetX = Random.Range(0f, 9999999f);
                chunkGenerator.offsetZ = Random.Range(0f, 9999999f);
            }

            StartCoroutine(InitializeChunks()); // Initialize chunks around the player
            StartCoroutine(UpdateChunkLODs());
        }

        private void Update()
        {
            if (!initialized)
                return;

            Vector2Int newChunkCoord = GetChunkCoord(characterTransform.position);
            if (newChunkCoord != currentChunkCoord)
            {
                currentChunkCoord = newChunkCoord;
                StartCoroutine(LoadNearbyChunks(chunkUpdatesPerFrame));
                UnloadDistantChunks();
                UpdateChunkColliders();
                StartCoroutine(UpdateChunkLODs());
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
        /// Initializes the surrounding chunks within the defined view distance based on the player's current position.
        /// It ensures that chunks are created and loaded into the environment if they are not already present.
        /// The initialization process calculates the level of detail (LOD) for each chunk based on its distance
        /// from the player's current chunk, then generates the terrain and scatters objects within the chunk.
        /// </summary>
        private IEnumerator InitializeChunks()
        {
            loadingPanel.SetActive(true);
            loadingProgress.value = 0f;
            yield return null;

            int totalChunks = (2 * viewDistance + 1) * (2 * viewDistance + 1);
            int loadedChunksCount = 0;
            int yieldInterval = totalChunks / 5;

            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                for (int x = -viewDistance; x <= viewDistance; x++)
                {
                    Vector2Int coord = new Vector2Int(currentChunkCoord.x + x, currentChunkCoord.y + z);
                    if (!loadedChunks.ContainsKey(coord))
                    {
                        int distance = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
                        int LOD = GetLODFromDistance(distance, viewDistance);
                        GameObject newChunk = chunkGenerator.GenerateSingleChunk(coord.x, coord.y, chunkWorldSizeX, chunkWorldSizeZ, terrainAmplitude, LOD);
                        terrainObjectScatterer.ScatterObjects(newChunk);
                        loadedChunks.Add(coord, newChunk);
                    }

                        loadedChunksCount++;
                    loadingProgress.value = (float)loadedChunksCount / totalChunks;
                    if (loadedChunksCount % yieldInterval == 0)
                        yield return null; // UI updates every 10 chunks
                    
                }
            }

            loadingPanel.SetActive(false);
            initialized = true;
            StartCoroutine(UpdateChunkLODs());
        }

        /// <summary>
        /// Loads nearby chunks around the player's current position within the specified view distance.
        /// Chunks are generated dynamically if they are not already loaded, with a specified limit
        /// on the number of chunks that can be processed per frame to maintain performance.
        /// </summary>
        /// <param name="maxPerFrame">The maximum number of chunks that can be loaded in a single frame.</param>
        /// <returns>An IEnumerator used to handle the asynchronous loading of chunks over multiple frames.</returns>
        private IEnumerator LoadNearbyChunks(int maxPerFrame)
        {
            int loadedThisFrame = 0;
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                for (int x = -viewDistance; x <= viewDistance; x++)
                {
                    Vector2Int coord = new Vector2Int(currentChunkCoord.x + x, currentChunkCoord.y + z);
                    if (!loadedChunks.ContainsKey(coord))
                    {
                        int distance = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
                        int LOD = GetLODFromDistance(distance, viewDistance); // Calculate LOD based on distance
                        // Debug.Log($"Generating chunk at ({x}, {z}) with LOD {LOD}, baseSizeX={chunkWorldSizeX}, baseSizeZ={chunkWorldSizeZ}");
                        GameObject newChunk = chunkGenerator.GenerateSingleChunk(coord.x, coord.y, chunkWorldSizeX, chunkWorldSizeZ, terrainAmplitude, LOD);
                        terrainObjectScatterer.ScatterObjects(newChunk);
                        loadedChunks.Add(coord, newChunk);

                        loadedThisFrame++;
                        if (loadedThisFrame >= maxPerFrame)
                        {
                            loadedThisFrame = 0;
                            yield return null; // Wait for next frame
                        }
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
        private void UnloadDistantChunks()
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

        /// Updates the LOD for all loaded chunks based on each chunk's distance from the player.
        private IEnumerator UpdateChunkLODs()
        {
            var chunkKeys = new List<Vector2Int>(loadedChunks.Keys);
            int updatesThisFrame = 0;

            foreach (var coord in chunkKeys)
            {
                if (!loadedChunks.TryGetValue(coord, out GameObject chunk))
                    continue;

                int dx = Mathf.Abs(coord.x - currentChunkCoord.x); // x distance from player
                int dz = Mathf.Abs(coord.y - currentChunkCoord.y); // z distance from player
                int distance = Mathf.Max(dx, dz); // LOD calculated based on max distance 
                int requiredLOD = GetLODFromDistance(distance, viewDistance); // Calculating required LOD based on chunk distance from player
                MeshGenerator mg = chunk.GetComponent<MeshGenerator>();
                int currentLOD = GetLODFromSize(mg.xSize, chunkWorldSizeX); // Getting current LOD based on current chunk's size
                if (currentLOD != requiredLOD) // If the current LOD of the chunk is different from the calculated LOD based on distance, update it
                {
                    chunkGenerator.SetMeshGeneratorValues(mg, requiredLOD, chunkWorldSizeX, chunkGenerator.chunkResolutionX, chunkWorldSizeZ, chunkGenerator.chunkResolutionZ, terrainAmplitude);
                    mg.Create();
                    updatesThisFrame++;
                }

                if (updatesThisFrame >= lodUpdatesPerFrame)
                {
                    updatesThisFrame = 0;
                    yield return null;
                }
            }
        }

        // This enables chunk colliders for chunks located around the player (3x3 grid)
        void UpdateChunkColliders()
        {
            foreach (var kvp in loadedChunks)
            {
                Vector2Int coord = kvp.Key;
                GameObject chunk = kvp.Value;
                int dx = Mathf.Abs(coord.x - currentChunkCoord.x);
                int dz = Mathf.Abs(coord.y - currentChunkCoord.y);
                int distance = Mathf.Max(dx, dz);
                MeshCollider collider = chunk.GetComponent<MeshCollider>();
                if (distance <= 1 && collider.enabled == false)
                {
                    collider.enabled = true; // Enable collider for current chunk and the 8 chunks surrounding it
                    // Debug.Log($"Collider enabled at ({coord.x}, {coord.y})");
                }
                else if (distance > 1 && collider.enabled == true)
                {
                    collider.enabled = false; // Disable colliders for chunks that aren't surrounding the player when the player moves
                    // Debug.Log($"Collider disabled at ({coord.x}, {coord.y})");
                }
            }
        }

        /// Determines the LOD level based on the chunk distance from the player
        private int GetLODFromDistance(int distance, int viewDistance)
        {

            float distancePercentage = (float)distance / viewDistance;

            if (distancePercentage <= LOD0Dist) // If the chunk distance is under LOD0DistRange from player between player and max distance, LOD is 0 (Full)
            {
                return 0;
            }
            else if (distancePercentage <= LOD1Dist) // If the chunk distance is between LOD0Dist and LOD1Dist from player between player and max distance, LOD is 1 (Half)
            {
                return 1;
            }
            else // If the chunk distance is over LOD1Dist from player between player and max distance, LOD is 2 (Quarter)
            {
                return 2;
            }
        }

        // Determines LOD based on the size of the chunk (Since chunk size is square it can be xSize or zSize)
        private int GetLODFromSize(int currentSize, int baseSize)
        {
            if (currentSize == baseSize) // LOD 0 since full detail (Same as chunk size)
            {
                return 0;
            }
            else if (currentSize == baseSize / 2) // LOD 1 since half detail (Chunk size / 2)
            {
                return 1;
            }
            else // LOD 2 since quarter detail (Chunk size / 4)
            {
                return 2;
            }
        }
    }
}