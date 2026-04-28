using System;
using ProceduralDungeon.Generator;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
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
        [SerializeField] private Tilemap biomeTileMap;

        [Header("Object To Detect")] 
        [SerializeField] private TileBase[] waterTiles;
        [SerializeField] private TileBase[] lavaTiles;
                
        [Header("Lava Settings")] [SerializeField , Range(0,5)]
        private float lavaDmgPerSecond =1f;
                
        [SerializeField] private float gridCellSize = 1f;
        
        private Vector2 inputDirection;
        private Rigidbody2D rb;
        private float currentSpeed;
        
        private bool isOnWater = false;
        private bool isOnLava = false;
        private float lavaDmgTimer = 0f;
        private BiomeType currentBiome = BiomeType.Normal;
        
        private float playerHealth = 100f;

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

            if (biomeTileMap == null)
            {
                biomeTileMap = GameObject.Find("Biomes").GetComponent<Tilemap>();
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
                  CheckLavaStatus();
                  ApplyLavaDamage();
              }
                
        //Movements functions
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

        //Checks if player is able to leave the current dungeon
        private void CheckIfOnExit()
        {
            if (portalTilemap == null) return;

            Vector3Int playerCell = portalTilemap.WorldToCell(transform.position);
            TileBase tile = portalTilemap.GetTile(playerCell);

            if (tile != null && tile.name == "Dungeon Tileset_199")
            {
                OnExitReached(tile);
            }
        }

        private void OnExitReached(TileBase tile)
        {
            //For now, it will quit the application
            Application.Quit();
            Debug.Log("Exit Reached");
        }
        
        //Applies water slowness
        private void CheckWaterStatus()
        {
            Vector3Int playerCell = floorTilemap.WorldToCell(transform.position);
            isOnWater = false;

            if (biomeTileMap != null)
            {
                TileBase tile = biomeTileMap.GetTile(playerCell);
                
                if (tile != null && IsWaterTile(tile)) 
                { 
                    isOnWater = true;
                    return;
                }
            }
            
        }

        private bool IsWaterTile(TileBase tile)
        {
            if (tile == null || waterTiles == null) return false;

            foreach (TileBase waterTile in waterTiles)
            {
                if(tile == waterTile) return true;
            }
            
            return false;
        }
        
        //Applies lava Damage
        private void CheckLavaStatus()
        {
            Vector3Int playerCell = floorTilemap.WorldToCell(transform.position);
            isOnLava = false;

            if (biomeTileMap != null)
            {
                TileBase tile = biomeTileMap.GetTile(playerCell);

                if (tile != null && IsLavaTile(tile))
                {
                    isOnLava = true;
                    return;
                }
            }
        }

        private bool IsLavaTile(TileBase tile)
        {
            if(tile == null || lavaTiles == null) return false;

            foreach (TileBase lavaTile in lavaTiles)
            {
                if (tile == lavaTile) return true;
            }
            
            return false;
        }

        private void ApplyLavaDamage()
        {
            if (!isOnLava)
            {
                lavaDmgTimer = 0f;
                return;
            }
            
            lavaDmgTimer += Time.fixedDeltaTime;

            if (lavaDmgTimer >= 1f)
            {
                playerHealth -= lavaDmgPerSecond;
                lavaDmgTimer = 0f;
                Debug.Log($"Player too DMG! Health: {playerHealth}");

                if (playerHealth <= 0)
                {
                    Die();
                }
            }
            
        }

        private void Die()
        {
            Debug.Log("Player Died");
            
            Destroy(this.gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void UpdateBiomeStatus()
        {
            Vector3Int playerCell = biomeTileMap.WorldToCell(transform.position);
            BiomeType detectedBiome = BiomeType.Normal;
            
            TileBase tile = biomeTileMap.GetTile(playerCell);

            if (tile != null)
            {
                if (IsLavaTile(tile))
                {
                    detectedBiome = BiomeType.Molten;
                }
                else if (IsWaterTile(tile))
                {
                    detectedBiome = BiomeType.Flooded;
                }
            }
            
            currentBiome = detectedBiome;
        }

        public BiomeType GetCurrentBiome()
        {
            return currentBiome;
        }
        
        public void SetTilemaps(Tilemap Floor, Tilemap Wall, Tilemap Decoration, Tilemap portal, Tilemap biome)
        {
            floorTilemap = Floor;
            wallTilemap = Wall;
            decorationTilemap = Decoration;
            portalTilemap = portal;
            biomeTileMap = biome;

            if (portal != null)
            {
                portalTilemap = portal;
            }

        }
    
        public bool GetIsOnWater() => isOnWater;
        public bool GetIsOnLava() => isOnLava;
    }
}
