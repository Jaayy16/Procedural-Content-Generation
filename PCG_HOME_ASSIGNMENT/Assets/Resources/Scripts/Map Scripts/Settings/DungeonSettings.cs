using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;

namespace ProceduralPlatformer.Settings
{
    [System.Serializable]
    public class PlatformerSettings
    {
        //Fixed Constraints
        [FoldoutGroup("Fixed Constraints")]
        [Tooltip("The initial seed for random level generation. Changing this will result in a different level layout.")]
        [SerializeField]
        private int seed = 12345;

        [FoldoutGroup("Fixed Constraints")]
        [Tooltip("The width of the level in blocks. Changing this will result in a wider or narrower level.")]
        [SerializeField]
        private int width = 50;

        [FoldoutGroup("Fixed Constraints")]
        [Tooltip("The dirt depth of the level in blocks. Changing this will result in a deeper or shallower level.")]
        [SerializeField]
        private int dirtDepth = 6;

        [FoldoutGroup("Fixed Constraints")]
        [Tooltip("The sky background height filler depth in blocks. Changing this will result in a taller or shorter sky background.")]
        [SerializeField]
        private int backgroundFillerDepth = 10;

        //Randomised Contraints
        [FoldoutGroup("Randomised Constraints")]
        [Tooltip("Minimum surface height in blocks.")]
        [SerializeField]
        private int minSurfaceHeight = 5;
        
        [FoldoutGroup("Randomised Constraints")]
        [Tooltip("Maximum surface height in blocks.")]
        [SerializeField]
        private int maxSurfaceHeight = 10;

        [FoldoutGroup("Randomised Constraints")]
        [Tooltip("Maximum height variation in between sections")]
        [SerializeField]
        private int maxHeightVariation = 1;

        [FoldoutGroup("Randomised Constraints")]
        [Tooltip("Minimum width for each section in blocks.")]
        [SerializeField]
        private int minSectionWidth = 3;
        
        //Decoration Settings
        
        [FoldoutGroup("Decoration Settings")]
        [Tooltip("Probability of spawning a flower on the surface")]
        [Range(0f, 0.1f)]
        [SerializeField]
        private float flowerSpawnChance = 0.05f;
        
        [FoldoutGroup("Decoration Settings")]
        [Tooltip("Probability of spawning a bush on the surface")]
        [Range(0f, 0.1f)]
        [SerializeField]
        private float bushSpawnChance = 0.05f;
        
        [FoldoutGroup("Decoration Settings")]
        [Tooltip("Probability of spawning a tree on the surface")]
        [Range(0f, 0.1f)]
        [SerializeField]
        private float treeSpawnChance = 0.05f;
        
        [FoldoutGroup("Decoration Settings")]
        [Tooltip("Maximum number of adjacent flowers in a bunch")]
        [SerializeField]
        private int maxFlowerBunchCount = 3;
        
        //Tiles
        [FoldoutGroup("Tile References")] 
        [Tooltip("Tile used for grass at the top surface.")] 
        [SerializeField]
        private TileBase grassTile;
        
        [FoldoutGroup("Tile References")] 
        [Tooltip("Tile used for grass left corner at the top surface.")] 
        [SerializeField]
        private TileBase grassLeftTile;
        
        [FoldoutGroup("Tile References")] 
        [Tooltip("Tile used for grass right corner at the top surface.")] 
        [SerializeField]
        private TileBase grassRightTile;
        
        [FoldoutGroup("Tile References")] 
        [Tooltip("Dirt used below the grass")] 
        [SerializeField]
        private TileBase dirtTile;
        
        [FoldoutGroup("Tile References")] 
        [Tooltip("Water tile used to fill in an area instead of the dirt.")] 
        [SerializeField]
        private TileBase waterTile;

        [FoldoutGroup("Tile References")] 
        [Tooltip("Tile used to fill the sky background.")] 
        [SerializeField]
        private TileBase skyBackgroundTile;
        
        [FoldoutGroup("Tile References")] 
        [Tooltip("Tile used for flower decoration on the surface.")] 
        [SerializeField]
        private TileBase flowerTile;
        
        [FoldoutGroup("Tile References")] 
        [Tooltip("Tile used for bush decoration on the surface.")] 
        [SerializeField]
        private TileBase bushTile;
        
        [FoldoutGroup("Tile References")] 
        [Tooltip("Tile used for tree decoration on the surface.")] 
        [SerializeField]
        private TileBase treeBottomTile;
        
        [FoldoutGroup("Tile References")] 
        [Tooltip("Tile used for tree decoration on the surface.")] 
        [SerializeField]
        private TileBase treeTopTile;
        
        public int Seed => seed;

        public int Width => width;

        public int DirtDepth => dirtDepth;

        public int BackgroundFillerDepth => backgroundFillerDepth;

        public int MinSurfaceHeight => minSurfaceHeight;

        public int MaxSurfaceHeight => maxSurfaceHeight;

        public int MaxHeightVariation => maxHeightVariation;

        public int MinSectionWidth => minSectionWidth;

        public TileBase GrassTile => grassTile;

        public TileBase DirtTile => dirtTile;

        public TileBase SkyBackgroundTile => skyBackgroundTile;
        
        public TileBase GrassLeftTile => grassLeftTile;

        public TileBase GrassRightTile => grassRightTile;

        public TileBase WaterTile => waterTile;

        public float FlowerSpawnChance => flowerSpawnChance;

        public float BushSpawnChance => bushSpawnChance;

        public float TreeSpawnChance => treeSpawnChance;

        public int MaxFlowerBunchCount => maxFlowerBunchCount;

        public TileBase FlowerTile => flowerTile;

        public TileBase BushTile => bushTile;

        public TileBase TreeBottomTile => treeBottomTile;

        public TileBase TreeTopTile => treeTopTile;
    }
}