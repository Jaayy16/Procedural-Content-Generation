using System;
using System.Collections.Generic;
using ProceduralDungeon.Combat;
using ProceduralDungeon.Enemy;
using ProceduralDungeon.Pooling;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace ProceduralDungeon.Spawning
{
    public class GridBasedSpawner : MonoBehaviour
    {
        [SerializeField] private EnemySpawnerSettings spawnerSettings;
        [SerializeField] private Tilemap floorTilemap;

        private System.Random spawnRng;
        private bool playerFound = false;
        
        private Queue<(Rect bounds, bool[,] tileData)> spawnQueue = new Queue<(Rect, bool[,])>();
        
        void Start()
        {
            spawnRng = new System.Random();
        }

        void Update()
        {

            if (!Application.isPlaying) return;
            

            if (!playerFound)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                {
                    playerFound = true;
                    Debug.Log($"[SPAWNER] Player found! Starting enemy spawn queue...s");
                }
            }
            
            if (playerFound && spawnQueue.Count > 0)
            {
                var (bounds, tileData) = spawnQueue.Dequeue();
                SpawnEnemyOnGrid(bounds, tileData);
                Debug.Log($"[SPAWNER] Spawned enemy from queue. Remaining: {spawnQueue.Count}");

            }
        }
        
        public void SpawnEnemiesGridBased(int roomIndex, Rect roomBounds, bool[,] floorTileData)
        {
            if (spawnerSettings == null)
            {
                return;
            }

            if (!ShouldSpawnInRoom(roomIndex))
            {
                return;
            }

            int enemyCount = Random.Range(spawnerSettings.minPerRoom, spawnerSettings.maxPerRoom + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                spawnQueue.Enqueue((roomBounds, floorTileData));
            }
        }

        public void spawnAllQueued()
        {
            Debug.Log($"[SPAWNER] SpawnAllQueued called - spawning {spawnQueue.Count} enemies");

            int spawnedCount = 0;
            while (spawnQueue.Count > 0)
            {
                var (bounds, tileData) = spawnQueue.Dequeue();
                SpawnEnemyOnGrid(bounds, tileData);
                spawnedCount++;
            }
            
            Debug.Log($"[SPAWNER] Spawned All Queued Enemies");
        }
        
        private bool ShouldSpawnInRoom(int roomIndex)
        {

            float spawnChance = spawnerSettings.spawnChancePerRoom;

            if (roomIndex == 0)
            {
                return false;
            }
            
            if (!spawnerSettings.spawnEverywhere && roomIndex % 2 == 0)
            {
                return false;
            }
            
            int roll = Random.Range(0, 100);
            bool shouldSpawn = roll <= spawnChance;
            
            return shouldSpawn;

        }

        private void SpawnEnemyOnGrid(Rect roomBounds, bool[,] floorTileData)
        {
            if (PoolManager.GetInstance() == null)
            {
                return;
            }
            
            Vector3Int gridPos = GetValidGridPos(roomBounds, floorTileData);

            if (gridPos == Vector3Int.zero)
            {
                return;
            }
            
            Vector3 worldPos = floorTilemap.CellToWorld(gridPos) + new Vector3(0.5f, 0.5f, 0f);

            bool isRanged = Random.Range(0, 100) < spawnerSettings.rangedEnemyRatio;

            if (isRanged)
            {
                PooledRangedEnemy enemy = PoolManager.GetRanged(worldPos);
                if (enemy != null)
                {
                    Debug.Log(
                        $"[SPAWNER] Spawned Ranged Enemy at grid ({gridPos.x}, {gridPos.y}) (worldPos: {worldPos})");
                }
            }
            else
            {
                PooledMeleeEnemy enemy = PoolManager.GetMeleeEnemy(worldPos);
                if (enemy != null)
                {
                    Debug.Log(
                        $"[SPAWNER] Spawned Melee Enemy at grid ({gridPos.x}, {gridPos.y}) (worldPos: {worldPos})");
                }
            }
        }

        private Vector3Int GetValidGridPos(Rect roomBounds, bool[,] floorTileData)
        {
            int xMin = Mathf.Max(0, Mathf.FloorToInt(roomBounds.xMin) + 1);
            int xMax = Mathf.Min(floorTileData.GetLength(0) - 1, Mathf.FloorToInt(roomBounds.xMax) - 1);
            
            int yMin = Mathf.Max(0, Mathf.FloorToInt(roomBounds.yMin) + 1);
            int yMax = Mathf.Min(floorTileData.GetLength(1) - 1, Mathf.FloorToInt(roomBounds.yMax) - 1);

            for (int atmpt = 0; atmpt < 10; atmpt++)
            {
                int xRand =  Random.Range(xMin, xMax);
                int yRand = Random.Range(yMin, yMax);

                if (floorTileData[xRand, yRand])
                {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(
                        floorTilemap.CellToWorld(new Vector3Int(xRand, yRand, 0)) + new Vector3(0.5f, 0.5f, 0f), 0.3f);

                    bool isBlocked = false;

                    foreach (Collider2D col in colliders)
                    {
                        if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
                        {
                            isBlocked = true;
                            break;
                        }
                    }

                    if (!isBlocked)
                    {
                        return new Vector3Int(xRand, yRand, 0);
                    }
                }
            }
            
            Debug.LogWarning("[SPAWNER] Failed to find valid grid position after 10 attempts");
            
            return Vector3Int.zero;
            
        }
        
    }
}