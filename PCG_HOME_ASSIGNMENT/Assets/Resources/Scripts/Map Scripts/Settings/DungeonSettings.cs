using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace ProceduralDungeon.Settings
{
    [System.Serializable]
    public class DungeonSettings
    {
        
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

        /// <summary>
        /// Corridor Generation 
        /// </summary>
        
        [FoldoutGroup("Corridor Generation")] [SerializeField]
        private int corridorWidth = 5;

        /// <summary>
        /// Seed Generation 
        /// </summary>
        
        [FoldoutGroup("Seed Generation")] [SerializeField]
        private int seed = 12345;

        /// <summary>
        /// Tile Refrences 
        /// </summary>

        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase wallTile;
        
        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase floorTile; 
        
        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase trapTile;
        
        [FoldoutGroup("Tile References")] [SerializeField]
        private TileBase decorativeTile; //To be changed and extended further depending on decorations
        
        /// <summary>
        ///  Public Properties
        /// <summary>
        
        public int DungeonWidth  => dungeonWidth;
        public int DungeonHeight => dungeonHeight;
        public int MinRoomWidth => minRoomWidth;
        public int MaxRoomWidth => maxRoomWidth;
        public int MinRoomHeight => minRoomHeight;
        public int MaxRoomHeight => maxRoomHeight;
        public int MaxRooms => maxRooms;
        public int CorridorWidth => corridorWidth;
        public int Seed => seed;
        public TileBase WallTile => wallTile;
        public TileBase FloorTile => floorTile;
        public TileBase TrapTile => trapTile;
        public TileBase DecorativeTile => decorativeTile; //To be changed and extended further depending on decorations
    }
}
