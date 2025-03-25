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
        
        public ChunkPool(GameObject chunkPrefab, Transform parent, int initialSize = 10)
        {
            pool = new Stack<GameObject>(initialSize);
            this.chunkPrefab = chunkPrefab;
            this.parentContainer = parent;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject chunk = GameObject.Instantiate(this.chunkPrefab, parentContainer);
                chunk.SetActive(false);
                pool.Push(chunk);
            }
        }

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

        public void Return(GameObject chunk)
        {
            chunk.SetActive(false);
            pool.Push(chunk);
        }
    }
}