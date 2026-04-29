using ProceduralDungeon.Combat;
using UnityEngine;

namespace ProceduralDungeon.Enemy
{
    public class PooledMeleeEnemy : PooledBaseEnemy
    {
        [SerializeField] private float atkDmg = 15f;
        [SerializeField] private float atkCooldown = 1f;
        [SerializeField] private float atkRange = 0.8f;

        private float lastAtkTime = 0f;
        protected override void OnPlayerDetected()
        {
            if (playerTransform == null) return;
            
            Vector2 dirToPlayer = (playerTransform.position - base.transform.position).normalized;
            RotateTowards(dirToPlayer);

            rb.linearVelocity = dirToPlayer * moveSpd;
            
            float distToPlayer = Vector2.Distance(base.transform.position, playerTransform.position);
            
            if (distToPlayer < atkRange && Time.time - lastAtkTime > atkCooldown)
            {
                AttackPlayer();
                lastAtkTime = Time.time;
            }
        }

        private void AttackPlayer()
        {
            IAttackable playerAttackable = playerTransform.GetComponent<IAttackable>();

            if (playerAttackable != null)
            {
                playerAttackable.TakeDamage(atkDmg);
                Debug.Log($"{gameObject.name} attacked player for {atkDmg} damage!");
            }
        }

        public override void OnPoolGet()
        {
            base.OnPoolGet();
            lastAtkTime = 0f;
        }
    }
}