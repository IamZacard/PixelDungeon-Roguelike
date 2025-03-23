using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "Level/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
{
    public int gridWidth = 30;
    public int gridHeight = 30;
    public int numRooms = 4;
    public Vector2Int roomMinSize = new Vector2Int(4, 4);
    public Vector2Int roomMaxSize = new Vector2Int(10, 10);
    public TileBase groundTile;
    public TileBase wallTile;

    // New tiles for encounters
    /*public TileBase playerTile;
    public TileBase enemyTile;
    public TileBase coinTile;
    public TileBase healthPotionTile;
    public TileBase exitTile;*/

    // Prefabs for spawning
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject trapPrefab;
    public GameObject coinPrefab;
    public GameObject healthPotionPrefab;
    public GameObject exitPrefab;
}