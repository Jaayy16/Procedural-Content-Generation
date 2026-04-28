using UnityEngine;

namespace ProceduralDungeon.Spawning
{
    [System.Serializable]
    public class EnemySpawnerSettings
    {
        [Header("Enemy Prefabs")]
        public GameObject rangedEnemyPrefab;
        public GameObject meleeEnemyPrefab;
        
        [Header("Spawn Settings")]
        [Tooltip("Makes Enemies spawn in every room")] 
        public bool spawnEverywhere = false; 
        [Range(1, 10)] public int minPerRoom = 1;
        [Range(1, 10)] public int maxPerRoom = 10;
        [Range(0,100)] public float spawnChance = 50f;
        
        [Header("Enemy Variance Settings")]
        [Range(0, 100)] public float rangedEnemyRatio = 40f;
        
    }
}