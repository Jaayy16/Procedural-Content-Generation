using UnityEngine;
using UnityEngine.Serialization;

namespace ProceduralDungeon.Combat
{
    public class RangedEnemy : BaseEnemy
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpd = 8f;
        [SerializeField] private float shootCooldown = 2f;
        [SerializeField] private Transform shootPoint;
        
        private float lastShotTime = 0f;

        protected override void OnPlayerDetected()
        {
            Vector2 dirToPlayer = (playerTransform.position - transform.position).normalized;
            RotateTowards(dirToPlayer);

            if (Time.time - lastShotTime > shootCooldown)
            {
                ShootProjectile();
                lastShotTime = Time.time;
            }
        }

        private void ShootProjectile()
        {
            if (projectilePrefab == null)
            {
                return;
            }
            
            Vector2 shootDir = (playerTransform.position - transform.position).normalized;
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

            if (projectileRb != null)
            {
                projectileRb.linearVelocity = shootDir *  projectileSpd;
            }
            
        }
    }
}