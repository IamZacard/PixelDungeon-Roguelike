using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using System.Collections;

public class OldLevelGenerator : MonoBehaviour
{
    // Grid settings
    public int gridWidth = 30;
    public int gridHeight = 30;
    public int numRooms = 4;

    // Room size ranges
    public Vector2Int smallRoomMinSize = new Vector2Int(4, 4);
    public Vector2Int smallRoomMaxSize = new Vector2Int(6, 6);
    public Vector2Int largeRoomMinSize = new Vector2Int(7, 7);
    public Vector2Int largeRoomMaxSize = new Vector2Int(10, 10);

    // Tilemap settings (will be assigned dynamically)
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public Tilemap encountersTilemap;
    public TileBase groundTile;
    public TileBase wallTile;
    public TileBase escapeTile;
    public TileBase trapTile;
    public TileBase enemyTile;
    public TileBase coinTile;
    public TileBase potionTile;

    // Player settings
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    // Cinemachine settings
    public CinemachineVirtualCamera virtualCamera;

    private char[,] grid;
    private List<RectInt> rooms = new List<RectInt>();
    private GameObject playerInstance;
    private bool canProceed = true; // Flag to control execution

    void Awake()
    {
        Debug.Log("LevelGenerator: Awake called.");
        AssignVirtualCameraDynamically();
        ValidateReferences();

        if (!canProceed)
        {
            Debug.LogError("LevelGenerator: Cannot proceed due to missing references. Check errors above.");
            enabled = false; // Disable the script to prevent further execution
        }
    }

    void Start()
    {
        Debug.Log("LevelGenerator: Start called.");
        if (canProceed)
        {
            StartCoroutine(InitializeLevel());
        }
        else
        {
            Debug.LogError("LevelGenerator: Start skipped due to missing references.");
        }
    }

    private IEnumerator InitializeLevel()
    {
        Debug.Log("LevelGenerator: Initializing level...");
        yield return new WaitForEndOfFrame();

        AssignTilemapsDynamically();
        ClearTilemaps();

        GenerateLevel();
        SetTiles();
        SpawnPlayer();
    }

    private void AssignTilemapsDynamically()
    {
        Debug.Log("LevelGenerator: Assigning Tilemaps...");
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        groundTilemap = null;
        wallTilemap = null;
        encountersTilemap = null;

        foreach (Tilemap tm in tilemaps)
        {
            string tmName = tm.gameObject.name.ToLower();
            if (tmName.Contains("ground"))
            {
                groundTilemap = tm;
                Debug.Log($"Assigned groundTilemap: {tm.gameObject.name}");
            }
            else if (tmName.Contains("wall"))
            {
                wallTilemap = tm;
                Debug.Log($"Assigned wallTilemap: {tm.gameObject.name}");
            }
            else if (tmName.Contains("encounter"))
            {
                encountersTilemap = tm;
                Debug.Log($"Assigned encountersTilemap: {tm.gameObject.name}");
            }
        }

        if (groundTilemap == null) Debug.LogWarning("groundTilemap not found dynamically!");
        if (wallTilemap == null) Debug.LogWarning("wallTilemap not found dynamically!");
        if (encountersTilemap == null) Debug.LogWarning("encountersTilemap not found dynamically!");
    }

    private void ClearTilemaps()
    {
        Debug.Log("LevelGenerator: Clearing Tilemaps...");
        if (groundTilemap != null) groundTilemap.ClearAllTiles();
        if (wallTilemap != null) wallTilemap.ClearAllTiles();
        if (encountersTilemap != null) encountersTilemap.ClearAllTiles();
        Debug.Log("Tilemaps cleared.");
    }

    private void AssignVirtualCameraDynamically()
    {
        Debug.Log("LevelGenerator: Assigning VirtualCamera...");
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogWarning("CinemachineVirtualCamera not found dynamically!");
        }
        else
        {
            Debug.Log($"CinemachineVirtualCamera found: {virtualCamera.gameObject.name}");
        }
    }

    private void ValidateReferences()
    {
        Debug.Log("LevelGenerator: Validating references...");
        canProceed = true; // Reset flag

        if (groundTile == null)
        {
            Debug.LogError("groundTile not assigned!");
            canProceed = false;
        }
        if (wallTile == null)
        {
            Debug.LogError("wallTile not assigned!");
            canProceed = false;
        }
        if (escapeTile == null)
        {
            Debug.LogError("escapeTile not assigned!");
            canProceed = false;
        }
        if (trapTile == null)
        {
            Debug.LogError("trapTile not assigned!");
            canProceed = false;
        }
        if (enemyTile == null)
        {
            Debug.LogError("enemyTile not assigned!");
            canProceed = false;
        }
        if (coinTile == null)
        {
            Debug.LogError("coinTile not assigned!");
            canProceed = false;
        }
        if (potionTile == null)
        {
            Debug.LogError("potionTile not assigned!");
            canProceed = false;
        }
        if (playerPrefab == null)
        {
            Debug.LogError("playerPrefab not assigned!");
            canProceed = false;
        }
        if (enemyPrefab == null)
        {
            Debug.LogError("enemyPrefab not assigned!");
            canProceed = false;
        }
        if (virtualCamera == null)
        {
            Debug.LogError("virtualCamera not assigned!");
            canProceed = false;
        }

        if (canProceed)
        {
            Debug.Log("LevelGenerator: All references validated successfully.");
        }
    }

    void GenerateLevel()
    {
        if (groundTilemap == null || wallTilemap == null || encountersTilemap == null)
        {
            Debug.LogError("Cannot generate level: Tilemaps missing!");
            return;
        }

        grid = new char[gridHeight, gridWidth];
        for (int y = 0; y < gridHeight; y++)
            for (int x = 0; x < gridWidth; x++)
                grid[y, x] = ' '; // Empty space

        for (int i = 0; i < numRooms; i++)
        {
            Vector2Int minSize = i < 2 ? smallRoomMinSize : largeRoomMinSize;
            Vector2Int maxSize = i < 2 ? smallRoomMaxSize : largeRoomMaxSize;
            int width = Random.Range(minSize.x, maxSize.x + 1);
            int height = Random.Range(minSize.y, maxSize.y + 1);
            int x = Random.Range(1, gridWidth - width - 1);
            int y = Random.Range(1, gridHeight - height - 1);

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

        for (int i = 1; i < rooms.Count; i++)
        {
            ConnectRooms(rooms[i - 1], rooms[i]);
        }

        ConnectStartingPoint(rooms[0]);
        PlaceSpecialTiles();
        PlaceWallsAroundFloors();
    }

    bool DoesRoomOverlap(RectInt newRoom)
    {
        foreach (RectInt room in rooms)
        {
            if (newRoom.Overlaps(room))
                return true;
        }
        return false;
    }

    void CarveRoom(RectInt room)
    {
        for (int y = room.y; y < room.y + room.height; y++)
            for (int x = room.x; x < room.x + room.width; x++)
                grid[y, x] = '.';
    }

    void ConnectRooms(RectInt roomA, RectInt roomB)
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

    void ConnectStartingPoint(RectInt firstRoom)
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

    void CreateHorizontalCorridor(int xStart, int xEnd, int y)
    {
        int minX = Mathf.Min(xStart, xEnd);
        int maxX = Mathf.Max(xStart, xEnd);
        for (int x = minX; x <= maxX; x++)
            grid[y, x] = '.';
    }

    void CreateVerticalCorridor(int yStart, int yEnd, int x)
    {
        int minY = Mathf.Min(yStart, yEnd);
        int maxY = Mathf.Max(yStart, yEnd);
        for (int y = minY; y <= maxY; y++)
            grid[y, x] = '.';
    }

    void PlaceSpecialTiles()
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

    void PlaceTileInRoom(RectInt room, char tileType, int count)
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

    void PlaceWallsAroundFloors()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[y, x] == '.' || grid[y, x] == 'E' || grid[y, x] == 'T' || grid[y, x] == 'N' || grid[y, x] == 'C' || grid[y, x] == 'P')
                {
                    if (y > 0 && grid[y - 1, x] == ' ') grid[y - 1, x] = '#';
                    if (y < gridHeight - 1 && grid[y + 1, x] == ' ') grid[y + 1, x] = '#';
                    if (x > 0 && grid[y, x - 1] == ' ') grid[y, x - 1] = '#';
                    if (x < gridWidth - 1 && grid[y, x + 1] == ' ') grid[y, x + 1] = '#';
                    if (y > 0 && x > 0 && grid[y - 1, x - 1] == ' ') grid[y - 1, x - 1] = '#';
                    if (y > 0 && x < gridWidth - 1 && grid[y - 1, x + 1] == ' ') grid[y - 1, x + 1] = '#';
                    if (y < gridHeight - 1 && x > 0 && grid[y + 1, x - 1] == ' ') grid[y + 1, x - 1] = '#';
                    if (y < gridHeight - 1 && x < gridWidth - 1 && grid[y + 1, x + 1] == ' ') grid[y + 1, x + 1] = '#';
                }
            }
        }
    }

    void SetTiles()
    {
        if (groundTilemap == null || wallTilemap == null || encountersTilemap == null)
        {
            Debug.LogError("Cannot set tiles: Tilemaps missing!");
            return;
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (grid[y, x] == '#')
                {
                    wallTilemap.SetTile(pos, wallTile);
                }
                else if (grid[y, x] == '.' || grid[y, x] == 'E' || grid[y, x] == 'T' || grid[y, x] == 'N' || grid[y, x] == 'C' || grid[y, x] == 'P')
                {
                    groundTilemap.SetTile(pos, groundTile);
                    if (grid[y, x] == 'E')
                        encountersTilemap.SetTile(pos, escapeTile);
                    else if (grid[y, x] == 'T')
                        encountersTilemap.SetTile(pos, trapTile);
                    else if (grid[y, x] == 'N')
                    {
                        if (enemyPrefab != null)
                        {
                            Vector3 worldPos = encountersTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0);
                            Instantiate(enemyPrefab, worldPos, Quaternion.identity);
                        }
                        else
                        {
                            Debug.LogError("Enemy prefab not assigned!");
                        }
                    }
                    else if (grid[y, x] == 'C')
                        encountersTilemap.SetTile(pos, coinTile);
                    else if (grid[y, x] == 'P')
                        encountersTilemap.SetTile(pos, potionTile);
                }
            }
        }
    }

    void SpawnPlayer()
    {
        Debug.Log("LevelGenerator: Spawning player...");
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned!");
            return;
        }
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera not assigned!");
            return;
        }

        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }

        playerInstance = Instantiate(playerPrefab, new Vector3(1.5f, 1.5f, 0), Quaternion.identity);
        playerInstance.tag = "Player";
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("Player prefab does not have a PlayerController component!");
            return;
        }

        Vector3 centerPosition = new Vector3(gridWidth / 2f, gridHeight / 2f, -10f);
        virtualCamera.transform.position = centerPosition;
        virtualCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //virtualCamera.Follow = playerInstance.transform;

        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.Initialize(playerController);
        }
        else
        {
            Debug.LogError("No TurnManager found in the scene!");
        }

        GameHandler gameHandler = FindObjectOfType<GameHandler>();
        if (gameHandler != null)
        {
            gameHandler.InitializePlayer();
        }
        else
        {
            Debug.LogError("No GameHandler found in the scene!");
        }

        Debug.Log("Player spawned successfully.");
    }
}