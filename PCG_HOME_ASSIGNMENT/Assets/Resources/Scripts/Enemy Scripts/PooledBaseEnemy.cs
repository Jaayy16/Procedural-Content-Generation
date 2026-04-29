using System;
using ProceduralDungeon.Combat;
using ProceduralDungeon.Pooling;
using UnityEngine;
using UnityEngine.Serialization;

namespace ProceduralDungeon.Enemy
{
    public abstract class PooledBaseEnemy : MonoBehaviour, IAttackable, IPoolable
    {
        [SerializeField] protected float maxHP = 30f;
        [SerializeField] protected float detectionRad = 3.5f;
        [SerializeField] protected float moveSpd = 3f;
        [SerializeField] protected LayerMask detectionLayer;
        
        protected Transform playerTransform;
        protected bool isPlayerDetected = false;
        protected Rigidbody2D rb;
        protected bool isAlive = true;
        protected float currentHP;

        protected virtual void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
            currentHP = maxHP;
            FindPlayer();
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            if(!isAlive) return;
            DetectPlayer();
        }

        protected virtual void FixedUpdate()
        {
            if(!isAlive) return;
            
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
            if (playerTransform == null)
            {
                isPlayerDetected = false;
                return;
            }
            
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
            if(!isAlive) return;
            
            currentHP -= damage;

            if (currentHP <= 0)
            {
                Die();
            }
        }

        public virtual float GetHealth()
        {
            return currentHP;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public bool IsAlive()
        {
            return isAlive;
        }

        protected virtual void Die()
        {
            isAlive = false;

            if (this is PooledMeleeEnemy meleeEnemy)
            {
                PoolManager.ReturnMeleeEnemy(meleeEnemy);
            }
            else if (this is PooledRangedEnemy rangedEnemy)
            {
                PoolManager.ReturnRangedEnemy(rangedEnemy);
            }
        }

        protected void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("No player found");
            }
        }

        protected void RotateTowards(Vector2 dir)
        {
            if (dir.magnitude < 0.01f) return;
            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        #region IPoolable Implemented

        public virtual void OnPoolCreated()
        {
            Debug.Log($"[POOL] {gameObject.name} created for pooling");
        }

        public virtual void OnPoolGet()
        {
            isAlive = true;
            currentHP = maxHP;
            rb.linearVelocity = Vector2.zero;
            isPlayerDetected = false;
            Debug.Log($"[POOL] {gameObject.name} has been retrieved from pool");
        }

        public virtual void OnPoolReturn()
        {
            rb.linearVelocity = Vector2.zero;
            isPlayerDetected = true;
            Debug.Log($"[POOL] {gameObject.name} has been returned to pool");
        }

        public void ResetEnemy()
        {
            isAlive = true;
            currentHP = maxHP;
            rb.linearVelocity = Vector2.zero;
            isPlayerDetected = false;
        }

        #endregion
    }
}
