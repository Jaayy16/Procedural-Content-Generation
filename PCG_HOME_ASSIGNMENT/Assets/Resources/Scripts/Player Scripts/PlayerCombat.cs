using System;
using UnityEngine;

namespace ProceduralDungeon.Combat
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private DetectionScript detectionScript;
        [SerializeField] private float atkDmg = 10f;
        [SerializeField] private float atkCooldown = 0.5f;
        [SerializeField] private float atkRotationSpd = 10f;

        private float lastAtkTime = 0f;
        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (detectionScript == null)
            {
                detectionScript = GetComponent<DetectionScript>();
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
                PerformAtk(enemy);
                lastAtkTime = Time.time;
            }
        }

        private void PerformAtk(IAttackable target)
        {
            target.TakeDamage(atkDmg);
        }

        private void RotateToClosestEnemy()
        {
            IAttackable enemy  = detectionScript.GetClosestTarget();

            if (enemy == null) return;
            
            Vector2 dir = (enemy.GetTransform().position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward),
                Time.deltaTime * atkRotationSpd);
        }
    }
}