using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    private char[,] grid;
    private List<RectInt> rooms = new List<RectInt>();

    public char[,] GenerateGrid(LevelSettings settings)
    {
        grid = new char[settings.gridHeight, settings.gridWidth];
        for (int y = 0; y < settings.gridHeight; y++)
            for (int x = 0; x < settings.gridWidth; x++)
                grid[y, x] = ' '; // Empty space

        // Generate rooms
        for (int i = 0; i < settings.numRooms; i++)
        {
            Vector2Int minSize = i < 2 ? settings.smallRoomMinSize : settings.largeRoomMinSize;
            Vector2Int maxSize = i < 2 ? settings.smallRoomMaxSize : settings.largeRoomMaxSize;
            int width = Random.Range(minSize.x, maxSize.x + 1);
            int height = Random.Range(minSize.y, maxSize.y + 1);
            int x = Random.Range(1, settings.gridWidth - width - 1);
            int y = Random.Range(1, settings.gridHeight - height - 1);

            RectInt newRoom = new RectInt(x, y, width, height);
            if (!DoesRoomOverlap(newRoom))
            {
                rooms.Add(newRoom);
                CarveRoom(newRoom);
            }
            else
            {
                i--;
            }
        }

        // Connect rooms with corridors
        for (int i = 1; i < rooms.Count; i++)
        {
            ConnectRooms(rooms[i - 1], rooms[i]);
        }

        ConnectStartingPoint(rooms[0]);
        PlaceSpecialTiles();
        PlaceWallsAroundFloors();

        return grid;
    }

    private bool DoesRoomOverlap(RectInt newRoom)
    {
        foreach (RectInt room in rooms)
        {
            if (newRoom.Overlaps(room))
                return true;
        }
        return false;
    }

    private void CarveRoom(RectInt room)
    {
        for (int y = room.y; y < room.y + room.height; y++)
            for (int x = room.x; x < room.x + room.width; x++)
                grid[y, x] = '.';
    }

    private void ConnectRooms(RectInt roomA, RectInt roomB)
    {
        Vector2Int pointA = new Vector2Int(
            Random.Range(roomA.x, roomA.x + roomA.width),
            Random.Range(roomA.y, roomA.y + roomA.height)
        );
        Vector2Int pointB = new Vector2Int(
            Random.Range(roomB.x, roomB.x + roomB.width),
            Random.Range(roomB.y, roomB.y + roomB.height)
        );

        if (Random.value < 0.5f)
        {
            CreateHorizontalCorridor(pointA.x, pointB.x, pointA.y);
            CreateVerticalCorridor(pointA.y, pointB.y, pointB.x);
        }
        else
        {
            CreateVerticalCorridor(pointA.y, pointB.y, pointA.x);
            CreateHorizontalCorridor(pointA.x, pointB.x, pointB.y);
        }
    }

    private void ConnectStartingPoint(RectInt firstRoom)
    {
        Vector2Int roomPoint = new Vector2Int(
            Random.Range(firstRoom.x, Mathf.Min(firstRoom.x + firstRoom.width, firstRoom.x + 3)),
            Random.Range(firstRoom.y, Mathf.Min(firstRoom.y + firstRoom.height, firstRoom.y + 3))
        );

        if (Random.value < 0.5f)
        {
            CreateHorizontalCorridor(1, roomPoint.x, 1);
            CreateVerticalCorridor(1, roomPoint.y, roomPoint.x);
        }
        else
        {
            CreateVerticalCorridor(1, roomPoint.y, 1);
            CreateHorizontalCorridor(1, roomPoint.x, roomPoint.y);
        }

        grid[1, 1] = '.'; // Ensure (1,1) is a floor tile for player spawn
    }

    private void CreateHorizontalCorridor(int xStart, int xEnd, int y)
    {
        int minX = Mathf.Min(xStart, xEnd);
        int maxX = Mathf.Max(xStart, xEnd);
        for (int x = minX; x <= maxX; x++)
            grid[y, x] = '.';
    }

    private void CreateVerticalCorridor(int yStart, int yEnd, int x)
    {
        int minY = Mathf.Min(yStart, yEnd);
        int maxY = Mathf.Max(yStart, yEnd);
        for (int y = minY; y <= maxY; y++)
            grid[y, x] = '.';
    }

    private void PlaceSpecialTiles()
    {
        int escapeRoomIndex = Random.Range(0, rooms.Count);
        PlaceTileInRoom(rooms[escapeRoomIndex], 'E', 1);

        for (int i = 0; i < rooms.Count; i++)
        {
            RectInt room = rooms[i];
            bool isSmallRoom = i < 2;
            int trapCount = isSmallRoom ? 1 : 3;
            int enemyCount = isSmallRoom ? 1 : 2;

            PlaceTileInRoom(room, 'T', trapCount);
            PlaceTileInRoom(room, 'N', enemyCount);
            PlaceTileInRoom(room, 'C', 1);
            if (Random.value < 0.5f)
                PlaceTileInRoom(room, 'P', 1);
        }
    }

    private void PlaceTileInRoom(RectInt room, char tileType, int count)
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        for (int y = room.y; y < room.y + room.height; y++)
        {
            for (int x = room.x; x < room.x + room.width; x++)
            {
                if (grid[y, x] == '.' && !(x == 1 && y == 1))
                {
                    availablePositions.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < count && availablePositions.Count > 0; i++)
        {
            int index = Random.Range(0, availablePositions.Count);
            Vector2Int pos = availablePositions[index];
            grid[pos.y, pos.x] = tileType;
            availablePositions.RemoveAt(index);
        }
    }

    private void PlaceWallsAroundFloors()
    {
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                if (grid[y, x] == '.' || grid[y, x] == 'E' || grid[y, x] == 'T' || grid[y, x] == 'N' || grid[y, x] == 'C' || grid[y, x] == 'P')
                {
                    if (y > 0 && grid[y - 1, x] == ' ') grid[y - 1, x] = '#';
                    if (y < grid.GetLength(0) - 1 && grid[y + 1, x] == ' ') grid[y + 1, x] = '#';
                    if (x > 0 && grid[y, x - 1] == ' ') grid[y, x - 1] = '#';
                    if (x < grid.GetLength(1) - 1 && grid[y, x + 1] == ' ') grid[y, x + 1] = '#';
                    if (y > 0 && x > 0 && grid[y - 1, x - 1] == ' ') grid[y - 1, x - 1] = '#';
                    if (y > 0 && x < grid.GetLength(1) - 1 && grid[y - 1, x + 1] == ' ') grid[y - 1, x + 1] = '#';
                    if (y < grid.GetLength(0) - 1 && x > 0 && grid[y + 1, x - 1] == ' ') grid[y + 1, x - 1] = '#';
                    if (y < grid.GetLength(0) - 1 && x < grid.GetLength(1) - 1 && grid[y + 1, x + 1] == ' ') grid[y + 1, x + 1] = '#';
                }
            }
        }
    }
}