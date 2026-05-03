using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralDungeon.Enemy
{

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Enemy Prefabs")] 
        [SerializeField] private GameObject meleeEnemyPrefab;
        [SerializeField] private GameObject rangedEnemyPrefab;

        [Header("Spawn Settings")] 
        [SerializeField] private int minEnemiesPerRoom = 1;
        [SerializeField] private int maxEnemiesPerRoom = 5;
        [SerializeField] private float rangedEnemyRatio = 0.4f;
        [SerializeField] private Tilemap floorTilemap;

        private int totalEnemiesSpawned;
        public void SpawnEnemiesInRoom(Rect roomBounds)
        {
            if (meleeEnemyPrefab == null || rangedEnemyPrefab == null)
            {
                Debug.LogError("No enemy prefabs in spawner");
                return;
            }
            
            int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
            
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosInRoom(roomBounds);

                if (spawnPos == Vector3.zero)
                {
                    continue;
                }

                bool isRanged = Random.value < rangedEnemyRatio;

                GameObject enemyPrefab = isRanged ? rangedEnemyPrefab : meleeEnemyPrefab;
                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                BaseEnemy enemyComponent = spawnedEnemy.GetComponent<BaseEnemy>();

                if (enemyComponent != null)
                {
                    string enemyType = isRanged ? "Ranged" : "Melee";
                    spawnedEnemy.name = $"{enemyType}Enemy_{totalEnemiesSpawned}";
                    
                    Debug.Log($"[SPAWNER] ✓ Spawned {enemyType} enemy: {spawnedEnemy.name} at {spawnPos}");
                    totalEnemiesSpawned++;
                }
                else
                {
                    Destroy(spawnedEnemy);
                }

            }
        }

        private Vector3 GetRandomSpawnPosInRoom(Rect roomBounds)
        {
            for (int attempts = 0; attempts < 10; attempts++)
            {
                float xRand = Random.Range(roomBounds.xMin + 0.5f, roomBounds.xMax - 0.5f);
                float yRand = Random.Range(roomBounds.yMin + 0.5f, roomBounds.yMax- 0.5f);

                Vector3 randomPos = new Vector3(xRand, yRand, 0f);

                Vector3Int cellPos = floorTilemap.WorldToCell(randomPos);
                TileBase tile = floorTilemap.GetTile(cellPos);

                if (tile != null)
                {
                    return floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);
                }
            }
            
            return Vector3.zero;
        }

        public void ApplyDifficultyScaling(float difficultyMultiplier)
        {
            BaseEnemy[] allEnemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.InstanceID);

            foreach (BaseEnemy enemy in allEnemies)
            {
                Debug.Log($"[SPAWNER] Applied Difficulty {difficultyMultiplier}x");
            }
            
        }

        public int GetTotalEnemiesSpawned()
        {
            return totalEnemiesSpawned;
        }

        public void ResetCount()
        {
            totalEnemiesSpawned = 0;
        }
    }
}