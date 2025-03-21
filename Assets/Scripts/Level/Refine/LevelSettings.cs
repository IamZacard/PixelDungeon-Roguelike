using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "Level/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
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

    // Tile settings
    public TileBase groundTile;
    public TileBase wallTile;
    public TileBase escapeTile;
    public TileBase trapTile;
    public TileBase enemyTile;
    public TileBase coinTile;
    public TileBase potionTile;

    // Prefabs
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
}