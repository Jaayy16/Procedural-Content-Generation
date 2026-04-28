using UnityEngine;

namespace ProceduralDungeon.Spawning
{
    public class EnemyRoomSpawner : MonoBehaviour
    {
        [SerializeField] private EnemySpawnerSettings spawnerSettings;
        [SerializeField] private LayerMask roomLayer;

        private int roomIndex = 0;

        public void SpawnEnemiesForRoom(int currentRoomIndex, Rect roomBounds)
        {
            roomIndex = currentRoomIndex;

            bool shouldSpawn = DetermineSpawn();

            if (!shouldSpawn) return;

            int enemyCount = Random.Range(spawnerSettings.minPerRoom, spawnerSettings.maxPerRoom + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnRandEnemy(roomBounds);
            }
        }

        private bool DetermineSpawn()
        {
            if (spawnerSettings.spawnEverywhere)
            {
                return Random.Range(0, 100) < spawnerSettings.spawnChance;
            }
            else
            {
                return roomIndex % 2 == 1 && Random.Range(0, 100) < spawnerSettings.spawnChance;
            }
        }

        private void SpawnRandEnemy(Rect roomBounds)
        {
            GameObject prefabToSpawn = DetermineEnemyTyping();

            if (prefabToSpawn == null) return;
            
            Vector2 spawnPos = GetRandomSpawnPosInRoom(roomBounds);
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        }

        private GameObject DetermineEnemyTyping()
        {
            float roll = Random.Range(0, 100);

            if (roll < spawnerSettings.rangedEnemyRatio)
            {
                return spawnerSettings.rangedEnemyPrefab;
            }
            else
            {
                return spawnerSettings.meleeEnemyPrefab;
            }
        }

        private Vector2 GetRandomSpawnPosInRoom(Rect roomBounds)
        {
            float xRand = Random.Range(roomBounds.xMin + 1f, roomBounds.xMax - 1f);
            float yRand = Random.Range(roomBounds.yMin + 1f, roomBounds.yMax - 1f);
            
            return new Vector2(xRand, yRand);
        }

    }
}