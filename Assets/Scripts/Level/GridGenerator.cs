using UnityEngine;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
    private char[,] grid;
    private List<RectInt> rooms = new List<RectInt>();

    private RectInt startingRoom;
    private Dictionary<Vector2Int, char> specialTiles = new Dictionary<Vector2Int, char>();

    public char[,] GenerateGrid(LevelSettings settings, out Dictionary<Vector2Int, char> outSpecialTiles)
    {
        if (settings == null)
        {
            Debug.LogError("Settings is null in GenerateGrid");
            outSpecialTiles = null;
            return null;
        }
        grid = new char[settings.gridHeight, settings.gridWidth];
        specialTiles.Clear();
        for (int y = 0; y < settings.gridHeight; y++)
            for (int x = 0; x < settings.gridWidth; x++)
                grid[y, x] = ' ';

        Debug.Log($"Generating {settings.numRooms} rooms...");
        for (int i = 0; i < settings.numRooms; i++)
        {
            int width = Random.Range(settings.roomMinSize.x, settings.roomMaxSize.x + 1);
            int height = Random.Range(settings.roomMinSize.y, settings.roomMaxSize.y + 1);
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
                i--; // Retry if overlap
            }
        }
        Debug.Log($"Generated {rooms.Count} rooms");

        for (int i = 1; i < rooms.Count; i++)
        {
            ConnectRooms(rooms[i - 1], rooms[i]);
        }

        PlaceWalls();
        if (rooms.Count > 0)
            startingRoom = rooms[0];
        PlaceSpecialTiles(settings);

        // Convert corridor tiles ('C') back to ground tiles ('.') for tilemap compatibility
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                if (grid[y, x] == 'C')
                    grid[y, x] = '.';
            }
        }

        int groundCount = 0;
        int wallCount = 0;
        for (int y = 0; y < grid.GetLength(0); y++)
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                if (grid[y, x] == '.') groundCount++; // Now only '.' counts as ground
                else if (grid[y, x] == '#') wallCount++;
            }
        Debug.Log($"Grid generated: {groundCount} ground tiles, {wallCount} wall tiles, {specialTiles.Count} special positions");

        outSpecialTiles = specialTiles;
        return grid;
    }

    private bool DoesRoomOverlap(RectInt newRoom)
    {
        foreach (RectInt room in rooms)
            if (newRoom.Overlaps(room))
                return true;
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
        Vector2Int pointA = new Vector2Int(roomA.x + roomA.width / 2, roomA.y + roomA.height / 2);
        Vector2Int pointB = new Vector2Int(roomB.x + roomB.width / 2, roomB.y + roomB.height / 2);

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

    private void CreateHorizontalCorridor(int xStart, int xEnd, int y)
    {
        int minX = Mathf.Min(xStart, xEnd);
        int maxX = Mathf.Max(xStart, xEnd);
        for (int x = minX; x <= maxX; x++)
            grid[y, x] = 'C'; // Mark corridors with 'C'
    }

    private void CreateVerticalCorridor(int yStart, int yEnd, int x)
    {
        int minY = Mathf.Min(yStart, yEnd);
        int maxY = Mathf.Max(yStart, yEnd);
        for (int y = minY; y <= maxY; y++)
            grid[y, x] = 'C'; // Mark corridors with 'C'
    }

    private void PlaceWalls()
    {
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                if (grid[y, x] == '.' || grid[y, x] == 'C') // Walls around rooms and corridors
                {
                    for (int dy = -1; dy <= 1; dy++)
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int ny = y + dy;
                            int nx = x + dx;
                            if (ny >= 0 && ny < grid.GetLength(0) && nx >= 0 && nx < grid.GetLength(1) && grid[ny, nx] == ' ')
                                grid[ny, nx] = '#';
                        }
                }
            }
        }
    }

    private void PlaceSpecialTiles(LevelSettings settings)
    {
        if (rooms.Count == 0)
        {
            Debug.LogError("No rooms generated, cannot place special tiles.");
            return;
        }

        if (!PlaceSpecialTileInRoom(rooms[0], 'P'))
        {
            Debug.LogError("Failed to place player in starting room.");
            return;
        }

        if (rooms.Count > 1)
        {
            int exitRoomIndex = Random.Range(1, rooms.Count);
            if (!PlaceSpecialTileInRoom(rooms[exitRoomIndex], 'X'))
                Debug.LogError("Failed to place exit in room.");
        }
        else
        {
            if (!PlaceSpecialTileInRoom(rooms[0], 'X'))
                Debug.LogError("Failed to place exit in same room as player.");
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            RectInt room = rooms[i];
            bool isSmallRoom = room.width <= 6 && room.height <= 6;
            int enemyCount = isSmallRoom ? Random.Range(1, 2) : Random.Range(1, 3);
            for (int e = 0; e < enemyCount; e++)
                PlaceSpecialTileInRoom(room, 'E');
            if (Random.value < 0.65f)
                PlaceSpecialTileInRoom(room, 'C');
            if (Random.value < 0.7f)
                PlaceSpecialTileInRoom(room, 'H');

            int trapCount = isSmallRoom ? Random.Range(0, 2) : Random.Range(1, 3);
            for (int t = 0; t < trapCount; t++)
                PlaceTrapInRoom(room);
        }
    }

    private bool PlaceSpecialTileInRoom(RectInt room, char tileType)
    {
        int attempts = 0;
        const int maxAttempts = 100;
        while (attempts < maxAttempts)
        {
            Vector2Int pos = GetRandomPositionInRoom(room);
            if (grid[pos.y, pos.x] == '.' && !specialTiles.ContainsKey(pos))
            {
                specialTiles[pos] = tileType;
                return true;
            }
            attempts++;
        }
        Debug.LogWarning($"Could not place '{tileType}' in room after {maxAttempts} attempts.");
        return false;
    }

    private bool PlaceTrapInRoom(RectInt room)
    {
        int attempts = 0;
        const int maxAttempts = 100;
        while (attempts < maxAttempts)
        {
            Vector2Int pos = GetRandomPositionInRoom(room);
            if (grid[pos.y, pos.x] == '.' && !specialTiles.ContainsKey(pos) && !IsAdjacentToCorridor(pos))
            {
                specialTiles[pos] = 'T';
                return true;
            }
            attempts++;
        }
        Debug.LogWarning($"Could not place trap in room after {maxAttempts} attempts.");
        return false;
    }

    private bool IsAdjacentToCorridor(Vector2Int pos)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = pos + dir;
            if (checkPos.x >= 0 && checkPos.x < grid.GetLength(1) && checkPos.y >= 0 && checkPos.y < grid.GetLength(0))
            {
                if (grid[checkPos.y, checkPos.x] == 'C')
                    return true;
            }
        }
        return false;
    }

    private Vector2Int GetRandomPositionInRoom(RectInt room)
    {
        int x = Random.Range(room.x, room.x + room.width);
        int y = Random.Range(room.y, room.y + room.height);
        return new Vector2Int(x, y);
    }
}