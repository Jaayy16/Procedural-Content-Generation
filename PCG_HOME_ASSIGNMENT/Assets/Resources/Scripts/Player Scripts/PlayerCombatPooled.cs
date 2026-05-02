using ProceduralDungeon.Combat;
using ProceduralDungeon.Pooling;
using UnityEngine;

namespace ProceduralDungeon.Player
{
    public class PlayerCombatPooled : MonoBehaviour
    {
        [SerializeField] private DetectionScript detectionScript;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private GameObject projectilePrefab;

        [Header("Attack Settings")]
        [SerializeField] private float projectileDmg = 10f;
        [SerializeField] private float projectileSpd = 15f;
        [SerializeField] private float atkCooldown = 0.5f;
        [SerializeField] private float rotationSpd = 10f;
        
        private float lastAtkTime = 0f;

        void Start()
        {
            if (detectionScript == null)
            {
                detectionScript = GetComponent<DetectionScript>();
                if (detectionScript == null)
                {
                    detectionScript = gameObject.AddComponent<DetectionScript>();
                }
            }

            if (shootPoint == null)
            {
                shootPoint = transform;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryAtk();
            }

            RotateToClosestEnemy();
        }

        private void TryAtk()
        {
            if (Time.time - lastAtkTime < atkCooldown) return;

            IAttackable enemy = detectionScript.GetClosestTarget();

            if (enemy != null)
            {
                FireProjectile(enemy.GetTransform().position);
                lastAtkTime = Time.time;
            }
        }

        private void FireProjectile(Vector3 enemyPos)
        {
            Vector2 shootDir = (enemyPos - shootPoint.position).normalized;

            PooledProjectile projectile = PoolManager.GetProjectile(shootPoint.position);
            
            if (projectile != null)
            {
                projectile.Initialize(shootDir, projectileSpd, projectileDmg, "Enemy");
            }
            else
            {
                Debug.LogError("Projectile Inst. missing Projectile Component!");
            }
        }

        private void RotateToClosestEnemy()
        {
            IAttackable enemy  = detectionScript.GetClosestTarget();

            if (enemy == null) return;
            
            Vector2 dir = (enemy.GetTransform().position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward),
                Time.deltaTime * rotationSpd);
        }
    }
}