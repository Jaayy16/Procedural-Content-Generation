using System.Collections.Generic;
using UnityEngine;

namespace ProceduralDungeon.Pooling
{
    public class ObjectPool<T> where T : MonoBehaviour, IPoolable
    {
        private Queue<T> availableObjects = new Queue<T>();
        private List<T> allObjects = new List<T>();
        private T prefab;
        private int initialSize;
        private bool expandable;
        private Transform parent;
        private string poolName;

        public int TotalCount => allObjects.Count;
        public int AvailableCount => availableObjects.Count;
        public int ActiveCount => allObjects.Count - availableObjects.Count;

        public ObjectPool(T prefab, int initialSize, bool expandable, Transform parent, string poolName)
        {
            this.prefab = prefab;
            this.initialSize = initialSize;
            this.expandable = expandable;
            this.parent = parent;
            this.poolName = poolName;

            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
            
            Debug.Log($"[Pool] Created {{poolName}} pool with {initialSize} objects");
        }

        private void CreateNewObject()
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.name = $"{poolName}_{allObjects.Count}";
            obj.gameObject.SetActive(false);
            
            obj.OnPoolCreate();
            
            allObjects.Add(obj);
            availableObjects.Enqueue(obj);
        }

        public T GetObject()
        {
            if (availableObjects.Count == 0)
            {
                if (expandable)
                {
                    Debug.LogWarning($"[Pool] {poolName} pool exhausted! Expanding...(Curren: {allObjects.Count})");
                    CreateNewObject();
                }
                else
                {
                    Debug.LogWarning($"[Pool] {poolName} pool empty and not expandable!");
                    return null;
                }
            }
            
            T obj = availableObjects.Dequeue();
            obj.gameObject.SetActive(true);
            obj.OnPoolGet();
            
            return obj;
        }

        public void ReturnObject(T obj)
        {
            if (obj == null) return;
            
            obj.OnPoolReturn();
            obj.gameObject.SetActive(false);
            availableObjects.Enqueue(obj);
        }

        public void PrintStats()
        {
           Debug.Log($"[Pool] {poolName}: {AvailableCount}/{TotalCount} available (Active: {ActiveCount})");
        }
    }
}