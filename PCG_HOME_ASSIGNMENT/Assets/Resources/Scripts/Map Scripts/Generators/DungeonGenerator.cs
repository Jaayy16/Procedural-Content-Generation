using System;
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
            public DungeonSettings.RoomShapes shape;
            public bool isSpawnRoom;
            public bool isEndRoom;

            public Room(int x, int y, int width, int height, DungeonSettings.RoomShapes shape, bool isSpawnRoom,
                bool isEndRoom)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.shape = shape;
                this.isSpawnRoom = isSpawnRoom;
                this.isEndRoom = isEndRoom;
            }

            public Vector2Int GetCentre()
            {
                return new Vector2Int(x + width / 2, y + height / 2);
            }

            public bool IsInRoom(int xPer, int yPer)
            {
                return xPer > x && xPer < x + width - 1 && yPer > y && yPer < y + height - 1;
            }

            public bool IsPointInRoom(int xPer, int yPer)
            {
                switch (shape)
                {
                    case DungeonSettings.RoomShapes.Square:
                        return xPer >= x && xPer < x + width && yPer >= y && yPer < y + height;
                    case DungeonSettings.RoomShapes.Hexagon:
                        return IsPointInHex(xPer, yPer);
                    case DungeonSettings.RoomShapes.Circle:
                        return IsPointInCircle(xPer, yPer);
                    default:
                        return false;
                }
            }

            private bool IsPointInHex(int xPer, int yPer)
            {
                int xCentre = x + width / 2;
                int yCentre = y + height / 2;
                int xRad = width / 2;
                int yRad = height / 2;

                int xDeg = Mathf.Abs(xPer - xCentre);
                int yDeg = Mathf.Abs(yPer - yCentre);

                if (xDeg > xRad || yDeg > yRad) return false;
                return yDeg < yRad * (1f - (float)xDeg / xRad);
            }

            private bool IsPointInCircle(int xPer, int yPer)
            {
                int xCentre = x + width / 2;
                int yCentre = y + height / 2;
                int xRad = width / 2;
                int yRad = height / 2;

                if (xRad == 0 || yRad == 0) return false;

                float xDeg = xPer - xCentre;
                float yDeg = yPer - yCentre;
                float norm = (xDeg * xDeg) / (xRad * xRad) + (yDeg * yDeg) / (yRad * yRad);
                return norm <= 1f;
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

            bool[,] tileData = PaintDungeonTiles(rooms);

            GenerateDecorations(rooms, tileData);
        }

        [Button("Reset Dungeon")]
        public void ResetDungeon()
        {
            if (floorTileMap != null || wallTileMap != null || trapTileMap != null || decorationTileMap != null)
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

            int spawnW = rng.Next(dungeonSettings.MinRoomWidth, dungeonSettings.MaxRoomWidth + 1);
            int spawnH = rng.Next(dungeonSettings.MinRoomHeight, dungeonSettings.MaxRoomHeight + 1);

            DungeonSettings.RoomShapes spawnShape = DungeonSettings.RoomShapes.Square;

            Room spawnRoom = new Room(5, dungeonSettings.DungeonHeight / 2 - spawnH / 2, spawnW, spawnH, spawnShape,
                true, false);

            rooms.Add(spawnRoom);

            Debug.Log($"Spawning at {spawnW}x{spawnH}");

            for (int i = 1; i < dungeonSettings.MaxRooms - 1; i++)
            {
                int roomW = rng.Next(dungeonSettings.MinRoomWidth, dungeonSettings.MaxRoomWidth + 1);
                int roomH = rng.Next(dungeonSettings.MinRoomHeight, dungeonSettings.MaxRoomHeight + 1);

                int xPos = 5 + i * (dungeonSettings.MinRoomWidth + dungeonSettings.RoomSpacing);

                int yVar = rng.Next(-5, 6);
                int yPos = dungeonSettings.DungeonHeight / 2 - roomH / 2 + yVar;

                yPos = Mathf.Clamp(yPos, 1, dungeonSettings.DungeonHeight - roomH - 1);

                if (xPos + roomW >= dungeonSettings.DungeonWidth - 10)
                {
                    break;
                }

                DungeonSettings.RoomShapes roomShape = (DungeonSettings.RoomShapes)rng.Next(0, 3);

                Room newRoom = new Room(xPos, yPos, roomW, roomH, roomShape, false, false);
                rooms.Add(newRoom);
            }

            int endW = rng.Next(dungeonSettings.MinRoomWidth, dungeonSettings.MaxRoomWidth + 1);
            int endH = rng.Next(dungeonSettings.MinRoomHeight, dungeonSettings.MaxRoomHeight + 1);

            int endX = dungeonSettings.DungeonWidth - endW - 5;

            DungeonSettings.RoomShapes endShape = DungeonSettings.RoomShapes.Square;

            Room endRoom = new Room(endX, endH, endW, endH, endShape, false, true);
            rooms.Add(endRoom);

            Debug.Log($"End Room at {endW}x{endH}");

            return rooms;
        }

        private void GenerateCorridors(List<Room> rooms)
        {
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                Vector2Int startRoom = rooms[i].GetCentre();
                Vector2Int endRoom = rooms[i + 1].GetCentre();

                CreateHorizontalCorridor(startRoom.x, endRoom.x, startRoom.y, rooms);
                CreateVerticalCorridor(startRoom.y, endRoom.y, endRoom.x, rooms);
            }
        }

        private void CreateHorizontalCorridor(int xStart, int xEnd, int yCentre, List<Room> rooms)
        {
            int xMin = Mathf.Min(xStart, xEnd);
            int xMax = Mathf.Max(xStart, xEnd);

            for (int x = xMin; x <= xMax; x++)
            {
                bool inRoom = false;
                foreach (Room room in rooms)
                {
                    if (room.IsPointInRoom(x, yCentre) && room.IsInRoom(x, yCentre))
                    {
                        inRoom = true;
                        break;
                    }
                }

                if (!inRoom)
                {
                    CarveCorridor(x, yCentre);
                }
            }
        }

        private void CreateVerticalCorridor(int yStart, int yEnd, int xCentre, List<Room> rooms)
        {
            int yMin = Mathf.Min(yStart, yEnd);
            int yMax = Mathf.Max(yStart, yEnd);

            for (int y = yMin; y <= yMax; y++)
            {
                bool inRoom = false;
                foreach (Room room in rooms)
                {
                    if (room.IsPointInRoom(xCentre, y) && room.IsInRoom(xCentre, y))
                    {
                        inRoom = true;
                        break;
                    }
                }

                if (!inRoom)
                {
                    CarveCorridor(y, xCentre);
                }
            }
        }

        private void CarveCorridor(int xCentre, int yCentre)
        {
            int corridorWidth = dungeonSettings.CorridorWidth;
            int corridorHalf = corridorWidth / 2;

            for (int offsetX = -corridorHalf; offsetX <= corridorHalf; offsetX++)
            {
                for (int offsetY = -corridorHalf; offsetY <= corridorHalf; offsetY++)
                {
                    int x = xCentre + offsetX;
                    int y = yCentre + offsetY;

                    if (x >= 0 && x < dungeonSettings.DungeonWidth && y >= 0 && y < dungeonSettings.DungeonHeight)
                    {
                        CreateTile(x, y);
                    }
                }
            }
        }

        private void GenerateDecorations(List<Room> rooms, bool[,] tileData)
        {
            if (!dungeonSettings.EnableDecoration ||
                dungeonSettings.DecorationTiles == null ||
                dungeonSettings.DecorationTiles.Length == 0)
            {
                return;
            }

            System.Random rng = new System.Random(dungeonSettings.Seed + 1);
            int decoCount = 0;

            for (int x = 0; x < dungeonSettings.DungeonWidth; x++)
            {
                for (int y = 0; y < dungeonSettings.DungeonHeight; y++)
                {
                    if (!tileData[x, y]) continue;

                    if (!IsValidDecoSpot(x, y, rooms)) continue;

                    if (decorationTileMap.GetTile(new Vector3Int(x, y, 0)) != null) continue;

                    if (rng.NextDouble() < dungeonSettings.DecorationChance)
                    {
                        TileBase decoTile =
                            dungeonSettings.DecorationTiles[rng.Next(0, dungeonSettings.DecorationTiles.Length)];

                        decorationTileMap.SetTile(new Vector3Int(x, y, 0), decoTile);
                        decoCount++;
                    }

                }
            }

            Debug.Log($"Place {decoCount} decorations");
        }

        private bool IsValidDecoSpot(int x, int y, List<Room> rooms)
        {
            int minDist = dungeonSettings.MinDistFromCentre;
            foreach (Room room in rooms)
            {
                if (room.isEndRoom || room.isSpawnRoom)
                {
                    Vector2Int roomCentre = room.GetCentre();
                    float dist = Vector2Int.Distance(new Vector2Int(x, y), roomCentre);

                    if (dist < minDist) return false;
                }
            }

            return true;
        }

        private void CreateTile(int x, int y)
        {
        }

        private bool[,] PaintDungeonTiles(List<Room> rooms)
        {
            //Marks Rooms
            bool[,] created = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];
            bool[,] isRoomTile = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];
            bool[,] isMainFloor = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];

            foreach (Room room in rooms)
            {
                for (int x = room.x - 1; x < room.x + room.width; x++)
                {
                    for (int y = room.y - 1; y < room.y + room.height; y++)
                    {
                        if (x >= 0 && x < dungeonSettings.DungeonWidth && y >= 0 && y < dungeonSettings.DungeonHeight)
                        {
                            if (room.IsPointInRoom(x, y))
                            {
                                created[x, y] = true;
                                isRoomTile[x, y] = true;

                                if (room.IsInRoom(x, y))
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

                for (int x = room.x - 1; x < room.x + room.width + 1; x++)
                {
                    for (int y = room.y - 1; y < room.y + room.height + 1; y++)
                    {
                        if (x >= 0 && x < dungeonSettings.DungeonWidth &&
                            y >= 0 && y < dungeonSettings.DungeonHeight)
                        {

                            if (!isRoomTile[x, y] && !created[x, y])
                            {
                                bool adjToRoom = false;

                                for (int xDeg = -1; xDeg <= 1; xDeg++)
                                {
                                    for (int yDeg = -1; yDeg <= 1; yDeg++)
                                    {
                                        if (xDeg == 0 && yDeg == 0) continue;
                                        int xN = x + xDeg;
                                        int yN = y + yDeg;

                                        if (xN >= 0 && xN < dungeonSettings.DungeonWidth && yN >= 0 &&
                                            yN < dungeonSettings.DungeonHeight)
                                        {
                                            if (isRoomTile[xN, yN] && isMainFloor[xN, yN])
                                            {
                                                adjToRoom = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (adjToRoom) break;
                                }

                                if (adjToRoom)
                                {
                                    created[x, y] = true;
                                    isMainFloor[x, y] = false;
                                }
                            }
                        }
                    }
                }
            }

            //Marks corridors
            int corridorWidth = dungeonSettings.CorridorWidth;
            int corridorHalf = corridorWidth / 2;
            int floorWidth = corridorWidth - 2;
            int floorHalf = floorWidth / 2;

            bool[,] corridorCreated = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];
            bool[,] isCorridorFloor = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];

            for (int i = 0; i < rooms.Count - 1; i++)
            {
                Vector2Int startRoom = rooms[i].GetCentre();
                Vector2Int endRoom = rooms[i + 1].GetCentre();

                int xMin = Mathf.Min(startRoom.x, endRoom.x);
                int xMax = Mathf.Max(startRoom.x, endRoom.x);

                for (int x = xMin; x <= xMax; x++)
                {
                    for (int yOffset = -corridorHalf; yOffset <= corridorHalf; yOffset++)
                    {
                        int y = startRoom.y + yOffset;

                        if (y >= 0 && y < dungeonSettings.DungeonHeight)
                        {
                            bool inRoom = false;
                            foreach (Room room in rooms)
                            {
                                if (room.IsPointInRoom(x, y) && room.IsInRoom(x, y))
                                {
                                    inRoom = true;
                                    break;
                                }
                            }

                            if (!inRoom)
                            {
                                created[x, y] = true;
                                corridorCreated[x, y] = true;

                                if (yOffset >= -floorHalf && yOffset <= floorHalf)
                                {
                                    isMainFloor[x, y] = true;
                                    isCorridorFloor[x, y] = true;
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
                    for (int xOffset = -corridorHalf; xOffset <= corridorHalf; xOffset++)
                    {
                        int x = endRoom.x + xOffset;

                        if (x >= 0 && x < dungeonSettings.DungeonWidth)
                        {
                            bool inRoom = false;
                            foreach (Room room in rooms)
                            {
                                if (room.IsPointInRoom(x, y) && room.IsInRoom(x, y))
                                {
                                    inRoom = true;
                                    break;
                                }
                            }

                            if (!inRoom)
                            {
                                created[x, y] = true;
                                corridorCreated[x, y] = true;

                                if (xOffset >= -floorHalf && xOffset <= floorHalf)
                                {
                                    isMainFloor[x, y] = true;
                                    isCorridorFloor[x, y] = true;
                                }
                                else
                                {
                                    isMainFloor[x, y] = false;
                                }
                            }
                        }
                    }
                }
            }

            // Fixes issue with some room corridors not having walls on all sides
            for (int x = 0; x < dungeonSettings.DungeonWidth; x++)
            {
                for (int y = 0; y < dungeonSettings.DungeonHeight; y++)
                {
                    if (corridorCreated[x, y] && isMainFloor[x, y])
                    {
                        for (int xDeg = -1; xDeg <= 1; xDeg++)
                        {
                            for (int yDeg = -1; yDeg <= 1; yDeg++)
                            {
                                if (xDeg == 0 && yDeg == 0) continue;

                                int xNeighbour = x + xDeg;
                                int yNeighbour = y + yDeg;

                                if (xNeighbour >= 0 && xNeighbour < dungeonSettings.DungeonWidth &&
                                    yNeighbour >= 0 && yNeighbour < dungeonSettings.DungeonHeight)
                                {
                                    if (!created[xNeighbour, yNeighbour] && !isRoomTile[xNeighbour, yNeighbour])
                                    {
                                        created[xNeighbour, yNeighbour] = true;
                                        isMainFloor[xNeighbour, yNeighbour] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Gets all marked tiles and paints them accordingly
            for (int x = 0; x < dungeonSettings.DungeonWidth; x++)
            {
                for (int y = 0; y < dungeonSettings.DungeonHeight; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if (isMainFloor[x, y])
                    {
                        floorTileMap.SetTile(pos, dungeonSettings.FloorTile);
                    }
                    else if (isCorridorFloor[x, y])
                    {
                        floorTileMap.SetTile(pos, dungeonSettings.FloorTile);
                    }
                    else if (created[x, y])
                    {
                        floorTileMap.SetTile(pos, dungeonSettings.WallTile);
                    }
                }
            }

            //takes all floor tiles (corridor and room) and combines them into a boolean array to pass to generateDecorations
            bool[,] combined = new bool[dungeonSettings.DungeonWidth, dungeonSettings.DungeonHeight];

            for (int x = 0; x < dungeonSettings.DungeonWidth; x++)
            {
                for (int y = 0; y < dungeonSettings.DungeonHeight; y++)
                {
                    combined[x, y] = isMainFloor[x, y] || isCorridorFloor[x, y];
                }
            }

            return combined;
        }
    }
}