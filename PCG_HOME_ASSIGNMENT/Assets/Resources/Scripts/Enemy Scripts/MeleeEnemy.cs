using ProceduralDungeon.Combat;
using UnityEngine;

namespace ProceduralDungeon.Combat
{
    public class MeleeEnemy : BaseEnemy
    {
        [SerializeField] private float atkDmg = 15f;
        [SerializeField] private float atkCooldown = 1f;
        [SerializeField] private float atkRange = 0.5f;

        private float lastAtkTime = 0f;

        protected override void OnPlayerDetected()
        {
            Vector2 dirToPlayer = (playerTransform.position - base.transform.position).normalized;
            RotateTowards(dirToPlayer);
            
            rb.linearVelocity = dirToPlayer * moveSpd;
            
            float distToPlayer = Vector2.Distance(base.transform.position, playerTransform.position);

            if (distToPlayer <= atkRange && Time.time - lastAtkTime > atkCooldown)
            {
                AtkPlayer();
                lastAtkTime = Time.time;
            }
        }

        private void AtkPlayer()
        {
            IAttackable playerAtkable = playerTransform.GetComponent<IAttackable>();

            if (playerAtkable != null)
            {
                playerAtkable.TakeDamage(atkDmg);
            }
        }
    }
}
