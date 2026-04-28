using System.Collections;
using UnityEngine;
using ProceduralDungeon.Generator;
using ProceduralDungeon.Player;
using ProceduralDungeon.Settings;
using ProceduralDungeon.Spawning;
using UnityEngine.Tilemaps;

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
    [SerializeField] private DungeonSettings dungeonSettings;

    [SerializeField] private EnemySpawnerSettings enemySettings;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (dungeonGenerator != null)
        {
            Debug.Log($"Generating Dungeon");
            dungeonGenerator.GenerateDungeon();
        }

        StartCoroutine(SpawnWhenWorldReady());
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
