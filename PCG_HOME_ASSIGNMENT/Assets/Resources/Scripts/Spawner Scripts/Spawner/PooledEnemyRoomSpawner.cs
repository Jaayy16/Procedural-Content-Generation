using ProceduralDungeon.Combat;
using ProceduralDungeon.Enemy;
using ProceduralDungeon.Pooling;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralDungeon.Spawning
{
    public class PooledEnemyRoomSpawner : MonoBehaviour
    {
        [SerializeField] private EnemySpawnerSettings spawnerSettings;

        private System.Random spawnRng;

        void Start()
        {
            spawnRng = new System.Random();
        }
        
        public void SpawnEnemiesForRoom(int roomIndex, Rect roomBounds)
        {
            if (spawnerSettings == null)
            {
                Debug.LogError("EnemySpawnerSettings not assigned");
                return;
            }

            if (!ShouldSpawnInRoom(roomIndex))
            {
                return;
            }

            int enemyCount = Random.Range(spawnerSettings.minPerRoom, spawnerSettings.maxPerRoom + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnRandEnemy(roomBounds);
            }
        }

        private bool ShouldSpawnInRoom(int roomIndex)
        {
            float spawnChance = spawnerSettings.spawnChancePerRoom;
            
            if (!spawnerSettings.spawnEverywhere && roomIndex % 2 == 0)
            {
                return false;
            }
            
            return Random.Range(0, 100) < spawnChance;
        }

        private void SpawnRandEnemy(Rect roomBounds)
        {
            Vector2 spawnPos = GetValidSpawnPos(roomBounds);
            bool isRanged = Random.Range(0, 100) < spawnerSettings.rangedEnemyRatio;

            if (isRanged)
            {
                PooledRangedEnemy enemy = PoolManager.GetRanged(spawnPos);

                if (enemy != null)
                {
                    Debug.Log($"[SPAWNER] Spawn pooled ranged enemy at {spawnPos}");
                }
            }
            else
            {
                PooledMeleeEnemy enemy = PoolManager.GetMeleeEnemy(spawnPos);

                if (enemy != null)
                {
                    Debug.Log($"[SPAWNER] Spawn pooled melee enemy at {spawnPos}");
                }
            }
        }

        private Vector2 GetValidSpawnPos(Rect roomBounds)
        {
            Vector2 spawnPos;
            int atmpts = 0;
            int maxAtmpts = 10;

            do
            {
                float xRand = Random.Range(roomBounds.xMin + 1f, roomBounds.xMax - 1f);
                float yRand = Random.Range(roomBounds.yMin + 1f, roomBounds.yMax - 1f);
                
                spawnPos = new Vector2(xRand, yRand);
                
                atmpts++;
            }
            while (IsPosBlocked(spawnPos) && atmpts < maxAtmpts);

            return spawnPos;
        }

        private bool IsPosBlocked(Vector2 Pos)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(Pos, 0.3f);

            foreach (Collider2D col in colliders)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}