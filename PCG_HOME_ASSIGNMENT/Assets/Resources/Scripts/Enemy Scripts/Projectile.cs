using ProceduralDungeon.Combat;
using UnityEngine;

namespace ProceduralDungeon.Projectile
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 4f;
        PlayerController player;
        private int dmg = 5;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                player = other.GetComponent<PlayerController>();
                player.TakeDamage(dmg);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
