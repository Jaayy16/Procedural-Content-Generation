using System.Linq;
using UnityEngine;
using ProceduralPlatformer.Settings;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;

namespace ProceduralPlatformer.Generators
{ 
    public enum TopTileType
    {
        Normal,
        LeftCorner,
        RightCorner,
        Water
    }
    
    [ExecuteAlways]
    public class PlatformerGenerator : MonoBehaviour
    {
        [SerializeField] private PlatformerSettings platformerSettings;
        [SerializeField] private Tilemap terrainTilemap;
        [SerializeField] private Tilemap backgroundTilemap;
        [SerializeField] private Tilemap decorationTilemap;

        [Button("Generate Platformer")]
        public void GeneratePlatformer()
        {
            //0. Reset Tilemaps
            terrainTilemap.ClearAllTiles();
            backgroundTilemap.ClearAllTiles();
            decorationTilemap.ClearAllTiles();
            
            //1. Randomise Terrain
            int[] heights = GenerateHeights();
            //Debug.Log(string.Join(", ", heights));

            //2. Check Top Tile Types
            TopTileType[] topTileTypes = DetermineTopTileTypes(heights);
            
            //3. Render Terrain
            RenderTiles(heights,topTileTypes);

            //4. Render the sky background
            RenderSkyBackground();
            
            //5. Render decorations (TODO)
            RenderDecorations(heights, topTileTypes);
        }

        private void RenderDecorations(int[] heights, TopTileType[] topTileTypes)
        {
            System.Random RandDecorations = new System.Random(platformerSettings.Seed + 12876);

            for (int x = 0; x < platformerSettings.Width; x++)
            {
                if (topTileTypes[x] == TopTileType.Normal && RandDecorations.NextDouble() < platformerSettings.FlowerSpawnChance)
                {
                    Vector3Int pos = new Vector3Int(x, heights[x] + 1, 0);
                    decorationTilemap.SetTile(pos, platformerSettings.FlowerTile);
                }

                if (topTileTypes[x] == TopTileType.Normal && RandDecorations.NextDouble() < platformerSettings.BushSpawnChance)
                {
                    Vector3Int pos = new Vector3Int(x, heights[x] + 1, 0);
                    decorationTilemap.SetTile(pos, platformerSettings.BushTile);
                }

                if (topTileTypes[x] == TopTileType.Normal && RandDecorations.NextDouble() < platformerSettings.TreeSpawnChance)
                {
                    Vector3Int pos = new Vector3Int(x, heights[x] + 2, 0);
                    decorationTilemap.SetTile(pos, platformerSettings.TreeTopTile);
                    pos = new Vector3Int(x, heights[x] + 1, 0);
                    decorationTilemap.SetTile(pos, platformerSettings.TreeBottomTile);
                    
                }
            }
        }

        private int[] GenerateHeights()
        {
            int width = platformerSettings.Width;
            int minSurfaceHeight = platformerSettings.MinSurfaceHeight;
            int maxSurfaceHeight = platformerSettings.MaxSurfaceHeight;
            int maxHeightVariation = platformerSettings.MaxHeightVariation;
            int minSectionWidth = platformerSettings.MinSectionWidth;


            int[] heights = new int[width];
            System.Random rng = new System.Random(platformerSettings.Seed);
            int i = 0;

            int currentHeight = rng.Next(minSurfaceHeight, maxSurfaceHeight+1);

            while (i < width)
            {
                int remainingWidth = width - i;
                int sectionWidth = (remainingWidth < minSectionWidth) ? 
                    remainingWidth : rng.Next(minSectionWidth, Mathf.Min(minSectionWidth + 3, remainingWidth) + 1);
                if (i > 0)
                {
                    int delta = rng.Next(-maxHeightVariation, maxHeightVariation + 1);
                    currentHeight = (int) Mathf.Clamp(heights[i-1] + delta, minSurfaceHeight, maxSurfaceHeight);
                }
                for (int j = 0; j <sectionWidth && i < width; j++)
                {
                    heights[i] = currentHeight;
                    i++;
                }
            }
            return heights;
        }

        private void RenderTiles(int[] heights, TopTileType[] topTileTypes)
        {
            int width = platformerSettings.Width;
            int dirtDepth = platformerSettings.DirtDepth;
            TileBase grassTile = platformerSettings.GrassTile;
            TileBase dirtTile = platformerSettings.DirtTile;
            for (int x = 0; x < width; x++)
            {
                int currentHeight = heights[x];
                Vector3Int pos = new Vector3Int(x, currentHeight, 0);
                //Rendering the top tile
                terrainTilemap.SetTile(pos, GetTopTile(topTileTypes[x]));

                //Place the dirt
                for (int y = currentHeight - 1; y >= currentHeight - dirtDepth; y--)
                {
                    pos.y = y;
                    terrainTilemap.SetTile(pos, dirtTile);
                }
            }
        }
        
        private TopTileType[] DetermineTopTileTypes(int[] heights)
        {
            int width = platformerSettings.Width;
            TopTileType[] tileTypes = new TopTileType[width];
 
            for (int x = 0; x < width; x++)
            {
                tileTypes[x] = TopTileType.Normal;
 
                if (x == 0 || (heights[x-1] < heights[x]))
                {
                    tileTypes[x] = TopTileType.LeftCorner;
                }
                else if ((x == width - 1) || (heights[x] > heights[x + 1]))
                {
                    tileTypes[x] = TopTileType.RightCorner;
                }
            }
            return tileTypes;
        }

        private TileBase GetTopTile(TopTileType type)
        {
            switch (type)
            {
                case TopTileType.Normal:
                    return platformerSettings.GrassTile;
                case TopTileType.LeftCorner:
                    return platformerSettings.GrassLeftTile;
                case TopTileType.RightCorner:
                    return platformerSettings.GrassRightTile;
                case TopTileType.Water:
                    return platformerSettings.WaterTile;
                default:
                    return platformerSettings.GrassTile;
            }
        }

        private void RenderSkyBackground()
        {
            for (int x=0; x<platformerSettings.Width; x++)
            {
                for (int y=platformerSettings.MinSurfaceHeight; y<(platformerSettings.BackgroundFillerDepth+platformerSettings.MaxSurfaceHeight); y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    backgroundTilemap.SetTile(pos, platformerSettings.SkyBackgroundTile);
                }
            }
        }

        [Button("Reset Tilemaps")]
        public void ResetTerrain()
        {
            if (terrainTilemap == null)
            {
                Debug.LogWarning("Terrain tilemap not defined.");
                return;
            }
            terrainTilemap.ClearAllTiles();
            backgroundTilemap.ClearAllTiles();
            decorationTilemap.ClearAllTiles();
            Debug.Log("Tilemap reset successfully.");
        }
    }
}