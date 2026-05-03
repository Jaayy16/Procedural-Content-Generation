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
            if (!ValidAssignment()) return;
            
            int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
        
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 spawnPos = GetRandomFloorTileInRoom(roomBounds);
        
                if (spawnPos == Vector3.zero) continue;
        
                bool isRanged = Random.value < rangedEnemyRatio;
                GameObject prefab = isRanged ? rangedEnemyPrefab : meleeEnemyPrefab;
                
                GameObject spawnedEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        
                BaseEnemy enemyComponent = spawnedEnemy.GetComponent<BaseEnemy>();
        
                if (enemyComponent != null)
                {
                    string enemyType = isRanged ? "Ranged" : "Melee";
                    spawnedEnemy.name = $"{enemyType}Enemy_{totalEnemiesSpawned}";
                    
                    totalEnemiesSpawned++;
                }
                else
                {
                    Destroy(spawnedEnemy);
                }
            }
        }

        private Vector3 GetRandomFloorTileInRoom(Rect roomBounds)
        {
            Debug.Log($"[SPAWNER] GET RANDOM SPAWN POS CALLED with bounds: {roomBounds}");

            //delete the above
            
            for (int attempts = 0; attempts < 10; attempts++)
            {
                float xRand = Random.Range(roomBounds.xMin + 1f, roomBounds.xMax - 1f);
                float yRand = Random.Range(roomBounds.yMin + 1f, roomBounds.yMax - 1f);
                Vector3 randomPos = new Vector3(xRand, yRand, 0f);
                
                Vector3Int cellPos = floorTilemap.WorldToCell(randomPos);
                
                cellPos = new Vector3Int(cellPos.x, cellPos.y, 0);
                
                TileBase tile = floorTilemap.GetTile(cellPos);

                if (tile != null) 
                {
                    Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);
                    return worldPos;
                }
            }
            return Vector3Int.zero;
        }

        private bool ValidAssignment()
        {
            if(meleeEnemyPrefab == null) return false;
            
            if(rangedEnemyPrefab == null) return false;
            
            if(floorTilemap == null) return false;
            
            return true;
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