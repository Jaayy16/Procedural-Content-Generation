using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralDungeon.Combat
{
    public class DetectionScript : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 2.5f;
        [SerializeField] private LayerMask detectionMask;
        
        private List<IAttackable> detectedEnemies = new List<IAttackable>();

        public IAttackable GetClosestTarget()
        {
            if(detectedEnemies.Count == 0) return null;
            
            IAttackable closest = null;
            float closestDist =  float.MaxValue;

            foreach (var enemy in detectedEnemies)
            {
                if (enemy == null) continue;
                
                float dist = Vector2.Distance(transform.position, enemy.GetTransform().position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = enemy;
                }
            }
            
            return closest;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & detectionMask) != 0)
            {
                IAttackable isAttackable = other.GetComponent<IAttackable>();
                if (isAttackable != null && !detectedEnemies.Contains(isAttackable))
                {
                    detectedEnemies.Add(isAttackable);
                } 
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            IAttackable isAttackable = other.GetComponent<IAttackable>();
            if (isAttackable != null)
            {
                detectedEnemies.Remove(isAttackable);
            }
        }
    }
}
