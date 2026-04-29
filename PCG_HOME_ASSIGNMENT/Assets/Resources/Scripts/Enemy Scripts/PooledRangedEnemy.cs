using ProceduralDungeon.Enemy;
using UnityEngine;
using UnityEngine.Serialization;

namespace ProceduralDungeon.Combat
{
    public class PooledRangedEnemy : PooledBaseEnemy
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private float shootCooldown = 1f;
        [SerializeField] private float projectileSpd = 8f;
        [SerializeField] private float projectileDmg = 8f;
        
        private float lastShotTime = 0f;

        protected override void Start()
        {
            base.Start();

            if (shootPoint == null)
            {
                shootPoint = transform;
            }
        }
        
        protected override void OnPlayerDetected()
        {
            if (playerTransform == null) return;
            
            Vector2 dirToPlayer = (playerTransform.position - base.transform.position).normalized;
            RotateTowards(dirToPlayer);
            
            if (Time.time - lastShotTime > shootCooldown)
            {
                ShootProjectile();
                lastShotTime = Time.time;
            }
        }

        private void ShootProjectile()
        {
            if (projectilePrefab == null || playerTransform == null) return;
            
            Vector2 shootDir = (playerTransform.position - shootPoint.position).normalized;
            
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            PooledProjectile projectileScript = projectilePrefab.GetComponent<PooledProjectile>();
            
            if (projectileScript != null)
            {
                projectileScript.Initialize(shootDir, projectileSpd, projectileDmg, "Player");
            }
        }

        public override void OnPoolGet()
        {
            base.OnPoolGet();
            lastShotTime = 0f;
        }
    }
}