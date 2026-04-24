using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;

namespace ProceduralDungeon.Player
{

    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField, Range(0, 10)]
        private float movementSpeed = 5f;

        [SerializeField] private float waterSlowMultiplier = 0.25f;

        [Header("TileMaps")] [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private Tilemap floorTilemap;
        [SerializeField] private Tilemap decorationTilemap;

        [Header("Object Detection")] [SerializeField]
        private TileBase[] waterTiles;

        [SerializeField] private float gridCellSize = 1f;

        private Vector2 inputDirection;
        private Rigidbody2D rb;
        private float currentSpeed;
        private bool isOnWater = false;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            
        }

        // Update is called once per frame
        void Update()
        {
            inputDirection = GetInputMovement();
        }

        private void FixedUpdate()
        {
            if (rb == null) return;

            Vector2 dir = rb.position + inputDirection * movementSpeed * Time.fixedDeltaTime;
            
            if (CanMoveTo(dir))
            {
                rb.linearVelocity = inputDirection * GetCurrentSpeed();
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }

            CheckWaterStatus();
        }

        private Vector2 GetInputMovement()
        {
           Vector2 input = Vector2.zero;
           
           input.x = Input.GetAxisRaw("Horizontal");
           input.y = Input.GetAxisRaw("Vertical");
           
           return input.normalized;
        }

        private float GetCurrentSpeed()
        {
            return isOnWater ? movementSpeed * waterSlowMultiplier : movementSpeed;
        }

        private bool CanMoveTo(Vector2 direction)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(direction, 0.3f);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    return false;
                }
            }
            return true;
        }

        private void CheckWaterStatus()
        {
            Vector3Int playerCell = floorTilemap.WorldToCell(transform.position);
            isOnWater = false;
            
            TileBase tile = decorationTilemap.GetTile(playerCell);

            if (tile != null && isWaterTile(tile))
            {
                isOnWater = true;
                return;
            }
            
            tile = floorTilemap.GetTile(playerCell);
            if (tile != null && isWaterTile(tile))
            {
                isOnWater = true;
                return;
            }

            tile = wallTilemap.GetTile(playerCell);
            if (tile != null && isWaterTile(tile))
            {
                isOnWater = true;
            }
        }

        private bool isWaterTile(TileBase tile)
        {
            if (tile == null || waterTiles == null) return false;

            foreach (TileBase waterTile in waterTiles)
            {
                if(tile == waterTile) return true;
            }
            
            return false;
        }

        public bool GetIsOnWater() => isOnWater;
    }
}
