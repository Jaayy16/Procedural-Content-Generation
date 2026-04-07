using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using ProceduralDungeon.Settings;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;

namespace ProceduralDungeon.Generator
{
    [ExecuteAlways]
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private DungeonSettings dungeonSettings;
        [SerializeField] private Tilemap floorTileMap;
        [SerializeField] private Tilemap wallTileMap;
        [SerializeField] private Tilemap trapTileMap;
        [SerializeField] private Tilemap decorationTileMap;

        [System.Serializable]
        private struct Room
        {
            public int x, y;
            public int width, height;

            public Room(int x, int y, int width, int height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }

            public Vector2Int GetCenter()
            {
                return new Vector2Int(x + width / 2, y + height / 2);
            }

            public bool Overlaps(Room room, int padding = 1)
            {
                return !(x + width + padding < room.x || room.x + room.width + padding < x ||
                         y + height + padding < room.y || room.y + room.height + padding < y);
            }
        }

        [Button("Generate Dungeon")]
        public void GenerateDungeon()
        {
            floorTileMap.ClearAllTiles();
            wallTileMap.ClearAllTiles();
            decorationTileMap.ClearAllTiles();
            trapTileMap.ClearAllTiles();

            List<Room> rooms = GenerateRooms();
            Debug.Log($"Generated {rooms.Count} rooms");

            GenerateCorridors(rooms);

            PaintDungeonTiles(rooms);
        }

        [Button("Reset Dungeon")]
        public void ResetDungeon()
        {
            if (floorTileMap != null || wallTileMap != null|| trapTileMap != null|| decorationTileMap != null) 
            {
                Debug.Log("Reset Dungeon");
                floorTileMap.ClearAllTiles();
                wallTileMap.ClearAllTiles();
                decorationTileMap.ClearAllTiles();
                trapTileMap.ClearAllTiles();
            }
            else
            {
                Debug.Log("Dungeon Tilemap is null");
            }
        }

        private List<Room> GenerateRooms()
        {
            List<Room> rooms = new List<Room>();
            System.Random rng = new System.Random(dungeonSettings.Seed);

            int atmpts = 0;
            int maxAtmpts = 10;

            while (rooms.Count < dungeonSettings.MaxRooms && atmpts < maxAtmpts)
            {
                atmpts++;

                int roomWidth = rng.Next(dungeonSettings.MinRoomWidth, dungeonSettings.MaxRoomWidth + 1);

                int roomHeight = rng.Next(dungeonSettings.MinRoomHeight, dungeonSettings.MaxRoomHeight + 1);

                int x = rng.Next(1, dungeonSettings.DungeonWidth - roomWidth - 1);
                int y = rng.Next(1, dungeonSettings.DungeonHeight - roomHeight - 1);

                Room room = new Room(x, y, roomWidth, roomHeight);

                bool overlaps = false;

                foreach (Room existingRoom in rooms)
                {
                    if (room.Overlaps(existingRoom, padding: 2))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    rooms.Add(room);
                    atmpts = 0;
                }
            }

            return rooms;
        }

        private void GenerateCorridors(List<Room> rooms)
        {
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                Vector2Int startRoom = rooms[i].GetCenter();
                Vector2Int endRoom = rooms[i + 1].GetCenter();

                CreateHorizontalCorridor(startRoom.x, endRoom.x, startRoom.y);
                CreateVerticalCorridor(startRoom.y, endRoom.y, endRoom.x);
            }
        }

        private void CreateHorizontalCorridor(int xStart, int xEnd, int yCenter)
        {
            int xMin = Mathf.Min(xStart, xEnd);
            int xMax = Mathf.Max(xStart, xEnd);

            for (int x = xMin; x <= xMax; x++)
            {
                CreateTile(x, yCenter - 1);
                CreateTile(x, yCenter);
                CreateTile(x, yCenter + 1);
            }
        }

        private void CreateVerticalCorridor(int yStart, int yEnd, int xCenter)
        {
            int yMin = Mathf.Min(yStart, yEnd);
            int yMax = Mathf.Max(yStart, yEnd);

            for (int y = yMin; y <= yMax; y++)
            {
                CreateTile(xCenter, y - 1);
                CreateTile(xCenter, y);
                CreateTile(xCenter, y + 1);
            }
        }

        private void CreateTile(int x, int y)
        {
            if (x >= 0 && x < dungeonSettings.DungeonWidth && y >= 0 && y < dungeonSettings.DungeonHeight)
            {
            }
        }

        private void PaintDungeonTiles(List<Room> rooms)
        {
            bool[,] created = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];
            bool[,] isRoomTile = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];
            bool[,] isMainFloor = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];

            foreach (Room room in rooms)
            {
                for (int x = room.x; x < room.x + room.width; x++)
                {
                    for (int y = room.y; y < room.y + room.height; y++)
                    {
                        if (x >= 0 && x < dungeonSettings.DungeonWidth && y >= 0 && y < dungeonSettings.DungeonHeight)
                        {
                            created[x, y] = true;
                            isMainFloor[x, y] = true;
                            isRoomTile[x, y] = true;

                            if (x > room.x && x < room.x + room.width - 1 && y > room.y && y < room.y + room.height - 1)
                            {
                                isMainFloor[x, y] = true;
                            }
                            else
                            {
                                isMainFloor[x, y] = false;
                            }
                        }
                    }
                }

                for (int x = room.x; x < room.x + room.width; x++)
                {
                    if (room.y >= 0 && room.y < dungeonSettings.DungeonHeight)
                    {
                        created[x, room.y] = true;
                        isRoomTile[x, room.y] = true;
                    }

                    if (room.y + room.height - 1 >= 0 && room.y + room.height - 1 < dungeonSettings.DungeonHeight)
                    {
                        created[x, room.y + room.height - 1] = true;
                        isRoomTile[x, room.y + room.height - 1] = true;
                    }
                }

                for (int y = room.y; y < room.y + room.height; y++)
                {
                    if (room.x >= 0 && room.x < dungeonSettings.DungeonWidth)
                    {
                        created[room.x, y] = true;
                        isRoomTile[room.x, y] = true;
                    }

                    if (room.x + room.width - 1 >= 0 && room.x + room.width - 1 < dungeonSettings.DungeonWidth)
                    {
                        created[room.x + room.width - 1, y] = true;
                        isRoomTile[room.x + room.width - 1, y] = true;
                    }
                }
            }

            int corridorWidth = dungeonSettings.CorridorWidth;
            int corridorHalf =corridorWidth / 2;
            
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                Vector2Int startRoom = rooms[i].GetCenter();
                Vector2Int endRoom = rooms[i + 1].GetCenter();

                int xMin = Mathf.Min(startRoom.x, endRoom.x);
                int xMax = Mathf.Max(startRoom.x, endRoom.x);

                for (int x = xMin; x <= xMax; x++)
                {
                    for (int yPadding = -corridorHalf; yPadding <= corridorHalf; yPadding++)
                    {
                        int y = startRoom.y + yPadding;

                        if (y >= 0 && y < dungeonSettings.DungeonHeight)
                        {
                            if (!isRoomTile[x, y])
                            {
                                created[x, y] = true;

                                if (yPadding == 0)
                                {
                                    isMainFloor[x, y] = true;
                                }
                                else
                                {
                                    isMainFloor[x, y] = false;
                                }
                            }
                        }
                    }
                }

                int yMin = Mathf.Min(startRoom.y, endRoom.y);
                int yMax = Mathf.Max(startRoom.y, endRoom.y);

                for (int y = yMin; y <= yMax; y++)
                {
                    for (int xPadding = -corridorHalf; xPadding <= corridorHalf; xPadding++)
                    {
                        int x = endRoom.x + xPadding;

                        if (x >= 0 && x < dungeonSettings.DungeonWidth)
                        {
                            if (!isRoomTile[x, y])
                            {
                                isMainFloor[x, y] = true;
                            }
                            else
                            {
                                isMainFloor[x, y] = false;  
                            }
                        }
                    }
                }
    
                for (int x = 0; x < dungeonSettings.DungeonWidth; x++)
                {
                    for (int y = 0; y < dungeonSettings.DungeonHeight; y++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, 0);
                        
                        if (isMainFloor[x, y])
                        {
                            floorTileMap.SetTile(pos,dungeonSettings.FloorTile);
                        }
                        else if (created[x, y])
                        {
                            floorTileMap.SetTile(pos,dungeonSettings.WallTile);
                        }
                    }
                }
            }
        }
    }
}