using UnityEngine;
using ProceduralDungeon.Combat;
using ProceduralDungeon.Enemy;
using ProceduralDungeon.Spawning;

namespace ProceduralDungeon.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager _instance;
        
        [Header("Pool Settings")] 
        [SerializeField] private int projectilePoolSize = 50;
        [SerializeField] private int meleeEnemyPoolSize = 15;
        [SerializeField] private int rangedEnemyPoolSize = 15;
        [SerializeField] private bool expandablePools = true;
        
        [Header("Prefabs")]
        [SerializeField] private PooledProjectile projectilePrefab;
        [SerializeField] private PooledMeleeEnemy meleeEnemyPrefab;
        [SerializeField] private PooledRangedEnemy rangedEnemyPrefab;
        
        private ObjectPool<PooledProjectile> projectilePool;
        private ObjectPool<PooledMeleeEnemy> meleeEnemyPool;
        private ObjectPool<PooledRangedEnemy> rangedEnemyPool;
        
        private Transform poolParent;

        public static PoolManager GetInstance()
        {
            if (_instance == null)
            {
               _instance = FindObjectOfType<PoolManager>();
            }
            
            return _instance;
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePools();
        }

        private void InitializePools()
        {
            poolParent = transform.Find("PooledObjects");

            if (poolParent == null)
            {
                GameObject poolParentGo = new GameObject("PooledObjects");
                poolParentGo.transform.SetParent(transform);
                poolParent = poolParentGo.transform;
            }

            if (projectilePrefab == null)
            {
                Debug.LogError("[POOL] projectilePrefab not assigned to PoolManager!");
                return;
            }

            if (rangedEnemyPrefab == null)
            {
                Debug.LogError("[POOL] Ranged Enemy Prefab not assigned to PoolManager!");
                return;
            }

            projectilePool = new ObjectPool<PooledProjectile>(projectilePrefab, projectilePoolSize, expandablePools,
                poolParent, "Projectile");

            meleeEnemyPool = new ObjectPool<PooledMeleeEnemy>(meleeEnemyPrefab, meleeEnemyPoolSize, expandablePools,
                poolParent, "MeleeEnemy");

            rangedEnemyPool = new ObjectPool<PooledRangedEnemy>(rangedEnemyPrefab, rangedEnemyPoolSize, expandablePools,
                poolParent, "RangedEnemy");
            
            Debug.Log("[Pool] PoolManager initialized successfully!");
        }

        #region Projectile Pool Methods

        public static PooledProjectile GetProjectile(Vector3 pos)
        {
            PoolManager manager = GetInstance();
            
            if (manager == null) return null;

            PooledProjectile projectile = manager.projectilePool.GetObject();

            if (projectile != null)
            {
                projectile.transform.position = pos;
            }
            return projectile;
        }

        public static void ReturnProjectile(PooledProjectile projectile)
        {
            PoolManager manager = GetInstance();

            if (manager != null)
            {
                manager.projectilePool.ReturnObject(projectile);
            }
        }

        #endregion
        
        #region Melee Enemy Pool Methods

        public static PooledMeleeEnemy GetMeleeEnemy(Vector3 pos)
        {
            PoolManager manager = GetInstance();
            
            if (manager == null) return null;

            PooledMeleeEnemy enemy = manager.meleeEnemyPool.GetObject();
            if (enemy != null)
            {
                enemy.transform.position = pos;
                enemy.ResetEnemy();
            }

            return enemy;
        }

        public static void ReturnMeleeEnemy(PooledMeleeEnemy enemy)
        {
            PoolManager manager = GetInstance();

            if (manager != null)
            {
                manager.meleeEnemyPool.ReturnObject(enemy);
            }
        }
        
        #endregion
        
        #region Ranged Enemy Pool Methods

        public static PooledRangedEnemy GetRanged(Vector3 pos)
        {
            PoolManager manager = GetInstance();
            
            if(manager == null) return null;
            
            PooledRangedEnemy enemy = manager.rangedEnemyPool.GetObject();

            if (enemy != null)
            {
                enemy.transform.position = pos;
                enemy.ResetEnemy();
            }

            return enemy;
        }

        public static void ReturnRangedEnemy(PooledRangedEnemy enemy)
        {
            PoolManager manager = GetInstance();

            if (manager != null)
            {
                manager.rangedEnemyPool.ReturnObject(enemy);
            }
        }
        
        #endregion

        public void PrintPoolStats()
        {
            Debug.Log("--- Pool Stats ---");
            projectilePool?.PrintStats();
            meleeEnemyPool?.PrintStats();
            rangedEnemyPool?.PrintStats();
            Debug.Log("------------------");
        }
        
    }
}