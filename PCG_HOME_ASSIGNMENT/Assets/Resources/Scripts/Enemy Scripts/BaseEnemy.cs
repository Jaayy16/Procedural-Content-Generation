using ProceduralDungeon.Combat;
using UnityEngine;

namespace ProceduralDungeon.Combat
{
    public abstract class BaseEnemy : MonoBehaviour, IAttackable
    {
        [SerializeField] protected float hp = 30f;
        [SerializeField] protected float detectionRad = 3.5f;
        [SerializeField] protected LayerMask detectionLayer;
        [SerializeField] protected float moveSpd = 3f;
        
        protected Transform playerTransform;
        protected bool isPlayerDetected = false;
        protected Rigidbody2D rb;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            FindPlayer();
        }

        protected virtual void Update()
        {
            DetectPlayer();
        }

        protected virtual void FixedUpdate()
        {
            if (isPlayerDetected)
            {
                OnPlayerDetected();
            }
            else
            {
                OnPlayerNotDetected();
            }
        }

        protected virtual void DetectPlayer()
        {
            if (playerTransform == null) return;
            
            float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            isPlayerDetected = distToPlayer <= detectionRad;
        }

        protected virtual void OnPlayerDetected()
        {
            
        }

        protected virtual void OnPlayerNotDetected()
        {
            rb.linearVelocity = Vector2.zero;    
        }

        public virtual void TakeDamage(float damage)
        {
            hp -= damage;

            if (hp <= 0)
            {
                Die();
            }
        }

        public virtual float GetHealth()
        {
            return hp;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }

        protected void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        protected void RotateTowards(Vector2 dir)
        {
            if (dir.magnitude < 0.01f) return;
            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
