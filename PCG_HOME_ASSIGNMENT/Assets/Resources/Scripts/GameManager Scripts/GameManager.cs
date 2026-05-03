using System.Collections;
using System.Collections.Generic;
using ProceduralDungeon.Combat;
using ProceduralDungeon.Enemy;
using UnityEngine;
using ProceduralDungeon.Generator;
using ProceduralDungeon.Settings;
using UnityEngine.Tilemaps;
using ProceduralDungeon.Player;

public class GameManager : MonoBehaviour
{    
    [SerializeField] private Camera mainCamera; 
    [SerializeField] private GameObject playerPrefab;
    
    [SerializeField] Tilemap portalTileMap;
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap decorationTilemap;
    [SerializeField] Tilemap trapTilemap;
    [SerializeField] Tilemap biomeTileMap;
    
    [SerializeField] private DungeonGenerator dungeonGenerator;    
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private DungeonSettings dungeonSettings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (dungeonGenerator != null)
        {
            Debug.Log($"Generating Dungeon");
            dungeonGenerator.GenerateDungeon();
            
            SpawnEnemiesInRooms();
        }

        StartCoroutine(SpawnWhenWorldReady());
    }

    private void SpawnEnemiesInRooms()
    {
        if (enemySpawner == null)
        {
            Debug.LogError("[GM] enemy spawner not assigned!");
            return;
        }

        if (dungeonGenerator == null)
        {
            Debug.LogError("[GM] dungeonGenerator not assigned!");
            return;
        }

        List<DungeonGenerator.Room> rooms = dungeonGenerator.GetGeneratedRooms();

        if (rooms == null || rooms.Count == 0)
        {
            Debug.LogError("[GM] no rooms generated!");
            return;
        }
        
        Debug.Log($"[GM] Spawning Enemies in {rooms.Count} rooms");

        for (int i = 0; i < rooms.Count; i++)
        {
            DungeonGenerator.Room currentRoom = rooms[i];

            if (currentRoom.isSpawnRoom || currentRoom.isEndRoom)
            {
                continue;
            }
            
            Rect roomBounds = new Rect(currentRoom.x, currentRoom.y, currentRoom.width, currentRoom.height);
            
            enemySpawner.SpawnEnemiesInRoom(roomBounds);
        }
    }
    
    private IEnumerator SpawnWhenWorldReady()
    {
        yield return new WaitForEndOfFrame();
        
        GameObject playerInstance = SpawnPlayer();

        if (playerInstance != null)
        {
            SetupCamera(playerInstance.transform);
        }
    }

    private GameObject SpawnPlayer()
    {
        if (playerPrefab == null || portalTileMap == null)
        {
            Debug.LogError("No player prefab or portalTileMap assigned!");
            return null;
        }

        if (dungeonSettings == null || dungeonSettings.SpawnTile == null)
        {
            Debug.LogError("No Dungeon settings or spawn tile assigned!");
        }

        BoundsInt bounds = portalTileMap.cellBounds;
        Vector3 spawnWorldPos = Vector3.zero;
        bool foundSpawn = false;
        
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = portalTileMap.GetTile(pos);

            if (tile == dungeonSettings.SpawnTile)
            {
                spawnWorldPos += portalTileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0f);
                foundSpawn = true;
                break;
            }
        }

        if (!foundSpawn)
        {
            Debug.LogError("No Spawn tile found!");
            return null;
        }
        
        GameObject playerInstance = Instantiate(playerPrefab, spawnWorldPos, Quaternion.identity);
        
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.SetTilemaps(floorTilemap, wallTilemap, decorationTilemap, portalTileMap, biomeTileMap);
        }
        else
        {
            Debug.LogError("Player controller cannot be found or instantiated!");
        }
        
        Debug.Log($"Player Spawned at {spawnWorldPos}");
        return playerInstance;
    }

    private void SetupPlayerCombat(Transform playerTransform)
    {
        PlayerCombat combat = playerTransform.GetComponent<PlayerCombat>();

        if (combat == null)
        {
            combat =  playerTransform.gameObject.AddComponent<PlayerCombat>();
        }
        
        Debug.Log("[GM] Player combat Initialized");
    }
    
    private void SetupCamera(Transform playerTransform)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        CameraController cameraController = mainCamera.GetComponent<CameraController>();
        
        if (cameraController != null)
        {
            cameraController = mainCamera.gameObject.AddComponent<CameraController>();
        }
        
        cameraController.SetPlayerTransform(playerTransform);
    }
}
