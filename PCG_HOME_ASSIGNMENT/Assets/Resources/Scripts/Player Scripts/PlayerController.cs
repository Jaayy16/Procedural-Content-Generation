using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;

namespace ProceduralDungeon.Player
{

    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField, Range(0, 20)]
        private float movementSpeed = 5f;

        [SerializeField] private float waterSlowMultiplier = 0.25f;

        [Header("TileMaps")] 
        [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private Tilemap floorTilemap;
        [SerializeField] private Tilemap decorationTilemap;
        [SerializeField] private Tilemap portalTilemap;

        [Header("Object To Detect")] 
        [SerializeField] private TileBase[] waterTiles;
        [SerializeField] private TileBase[] lavaTiles;
        
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

            if (wallTilemap == null)
            {
                wallTilemap = GameObject.Find("Walls").GetComponent<Tilemap>();
            }

            if (floorTilemap == null)
            {
                floorTilemap = GameObject.Find("Floor").GetComponent<Tilemap>();
            }

            if (decorationTilemap == null)
            {
                decorationTilemap = GameObject.Find("Decorations").GetComponent<Tilemap>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            inputDirection = GetInputMovement();
            CheckIfOnExit();
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

        private void CheckIfOnExit()
        {
            if (portalTilemap == null) return;

            Vector3Int playerCell = portalTilemap.WorldToCell(transform.position);
            TileBase tile = portalTilemap.GetTile(playerCell);

            if (tile != null)
            {
                OnExitReached(tile);
            }
        }

        private void OnExitReached(TileBase tile)
        {
            //For now it will quit the application
            Application.Quit();
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

        public void SetTilemaps(Tilemap Floor, Tilemap Wall, Tilemap Decoration, Tilemap portal)
        {
            floorTilemap = Floor;
            wallTilemap = Wall;
            decorationTilemap = Decoration;
            portalTilemap = portal;
        }
    }
}
