using ProceduralDungeon.Pooling;
using UnityEngine;

namespace ProceduralDungeon.Combat
{
    public class PooledProjectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifetime = 5f;

        private float dmg;
        private float spd;
        private string targetTag;
        private Rigidbody2D rb;
        private float timeAlive;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        void Update()
        {
            timeAlive += Time.deltaTime;

            if (timeAlive > lifetime)
            {
                ReturnToPool();
            }
        }

        public void Initialize(Vector2 shootDir, float shootSpd, float projectileDmg, string targetTagName)
        {
            Vector2 dir = shootDir.normalized;
            spd = shootSpd;
            dmg = projectileDmg;
            targetTag = targetTagName;
            timeAlive = 0f;

            if (rb != null)
            {
                rb.linearVelocity = dir * spd;
            }
            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(targetTag))
            {
                IAttackable atkable = other.GetComponent<IAttackable>();

                if (atkable != null)
                {
                    atkable.TakeDamage(dmg);
                    Debug.Log($"[POOL] Projectile hit {other.gameObject.name} for {dmg} damage!");
                    ReturnToPool();
                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                Debug.Log("[POOL] Projectile hit a Wall!");
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            PoolManager.ReturnProjectile(this);
        }

        public void OnPoolCreated()
        {
            //Is called once when the pool is created
        }

        public void OnPoolGet()
        {
            timeAlive = 0f;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        public void OnPoolReturn()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}