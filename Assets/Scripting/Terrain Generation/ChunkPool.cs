using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripting.Terrain_Generation
{
    public class ChunkPool
    {
        private Stack<GameObject> pool;

        private GameObject chunkPrefab;
        private Transform parentContainer;

        /// Manages a pool of reusable chunk GameObjects to optimize performance and reduce memory allocation
        /// overhead during runtime. The pool instantiates and reuses instances of a provided chunk prefab,
        /// minimizing the need for frequent object destruction and creation.
        public ChunkPool(GameObject chunkPrefab, Transform parent, int initialSize = 10)
        {
            // Initialize the pool with the specified initial size
            pool = new Stack<GameObject>(initialSize);
            this.chunkPrefab = chunkPrefab;
            parentContainer = parent;

            // Instantiate the initial pool of chunks on game start
            for (int i = 0; i < initialSize; i++)
            {
                GameObject chunk = GameObject.Instantiate(this.chunkPrefab, parentContainer);
                chunk.SetActive(false);
                pool.Push(chunk);
            }
        }

        /// Retrieves a chunk object from the pool. If the pool is empty, a new chunk is instantiated.
        /// The retrieved chunk will be activated and ready for use.
        /// <returns>Returns a GameObject representing the retrieved chunk.</returns>
        public GameObject Get()
        {
            if (pool.Count > 0)
            {
                GameObject chunk = pool.Pop();
                chunk.SetActive(true);
                return chunk;
            }
            else
            {
                return GameObject.Instantiate(chunkPrefab, parentContainer);
            }
        }

        /// Returns a chunk object back to the pool. The chunk will be deactivated and reused later.
        /// <param name="chunk">The GameObject representing the chunk to be returned to the pool.</param>
        public void Return(GameObject chunk)
        {
            // Deactivate the chunk and its children
            chunk.SetActive(false);
            pool.Push(chunk);
        }

    }
}