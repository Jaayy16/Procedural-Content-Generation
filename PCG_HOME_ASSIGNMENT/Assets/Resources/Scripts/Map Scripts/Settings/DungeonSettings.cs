using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace ProceduralDungeon.Settings
{
    [System.Serializable]
    public class DungeonSettings
    {
        public enum RoomShapes
        {
            Square,
            Hexagon,
            Circle
        }
        
        /// <summary>
        /// Dungeon Generation 
        /// </summary>
        [FoldoutGroup("Dungeon Dimensions")] [SerializeField]
        private int dungeonWidth = 80;
        
        [FoldoutGroup("Dungeon Dimensions")] [SerializeField]
        private int dungeonHeight = 60;
        
        /// <summary>
        /// Room Generation 
        /// </summary>
        
        [FoldoutGroup("Room Generation")] [SerializeField]
        private int minRoomWidth = 5;

        [FoldoutGroup("Room Generation")] [SerializeField]
        private int maxRoomWidth = 15;

        [FoldoutGroup("Room Generation")] [SerializeField]
        private int minRoomHeight = 5;

        [FoldoutGroup("Room Generation")] [SerializeField]
        private int maxRoomHeight = 20;

        [FoldoutGroup("Room Generation")] [SerializeField]
        private int maxRooms = 20;

        [FoldoutGroup("Room Generation")] [SerializeField]
        private int roomSpacing = 10;
        
        /// <summary>
        /// Corridor Generation 
        /// </summary>
        
        [FoldoutGroup("Corridor Generation")] [SerializeField]
        private int corridorWidth = 5;

        /// <summary>
        /// Decoration Settings
        /// </summary>
        
        [FoldoutGroup("Decoration Generation")] [SerializeField]
        private bool enableDecoration = true;
        [FoldoutGroup("Decoration Generation")] [SerializeField]
        public float spawnChance = 0.15f;
        [FoldoutGroup("Decoration Generation")] [SerializeField]
        private int minDistFromCentre = 2;
        [FoldoutGroup("Decoration Generation")] [SerializeField]
        private float decorationChance = 0.15f;
        
        /// <summary>
        /// Seed Generation 
        /// </summary>
        
        [FoldoutGroup("Seed Generation")] [SerializeField]
        private int seed = 12345;

        ///<summary>
        ///Tile Variance
        /// </summary>
        
        [FoldoutGroup("Tile Variance")] [SerializeField]
        private bool enableFloorVariance = true;
        
        [FoldoutGroup("Tile Variance")] [SerializeField]
        private bool enableWallVariance = true;

        [FoldoutGroup("Tile Variance")] [SerializeField]
        private float noiseScale = 1f;
        
        [FoldoutGroup("Tile Variance")] [SerializeField]
        private TileBase[] floorVariants = new TileBase[1];
        
        [FoldoutGroup("Tile Variance")] [SerializeField]
        private TileBase[] wallVariants = new TileBase[1];
        
        [FoldoutGroup("Tile Variance")] [SerializeField, Range(0, 1)]
        private float varianceThreshold = 0.5f;
        
        /// <summary>
        /// Tile Refrences 
        /// </summary>

        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase wallTile;
        
        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase floorTile; 
        
        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase[] trapTile = new TileBase[1];
        
        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase[] decorationTiles = new TileBase[1];
        
        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase spawnTile;
        
        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase levelExitTile;
        
        /// <summary>
        ///  Public Properties
        /// <summary>
        
        //Pass Dungeon-Related Variables
        public int DungeonWidth  => dungeonWidth;
        public int DungeonHeight => dungeonHeight;
        
        //Pass Room-Related Variables
        public int MinRoomWidth => minRoomWidth;
        public int MaxRoomWidth => maxRoomWidth;
        public int MinRoomHeight => minRoomHeight;
        public int MaxRoomHeight => maxRoomHeight;
        public int MaxRooms => maxRooms;
        public int RoomSpacing => roomSpacing;
        
        //Pass Corridor-Related Variables
        public int CorridorWidth => corridorWidth;
        
        //Pass Decoration-Related Variables
        public bool EnableDecoration => enableDecoration;
        public int MinDistFromCentre => minDistFromCentre;
        public float DecorationChance => decorationChance;
        public int Seed => seed;
        
        //Pass Perlin-Noise Related Variables
        public bool EnableFloorVariance => enableFloorVariance;
        public bool EnableWallVariance => enableWallVariance;
        public float NoiseScale => noiseScale;
        public TileBase[] FloorVariants => floorVariants;
        public TileBase[] WallVariants => wallVariants;
        public float VarianceThreshold => varianceThreshold;
        
        //Pass Tile-Related Variables
        public TileBase WallTile => wallTile;
        public TileBase FloorTile => floorTile;
        public TileBase[] TrapTile => trapTile;
        public TileBase[] DecorationTiles => decorationTiles;
        public TileBase SpawnTile => spawnTile;
        public TileBase LevelExitTile => levelExitTile;

    }
}
