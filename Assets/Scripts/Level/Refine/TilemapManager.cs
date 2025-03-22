using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] public Tilemap groundTilemap { get; private set; }
    [SerializeField] public Tilemap wallTilemap { get; private set; }
    [SerializeField] public Tilemap encountersTilemap { get; private set; }

    public void AssignTilemapsDynamically()
    {
        Debug.Log("TilemapManager: Assigning Tilemaps...");
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

    public void ClearTilemaps()
    {
        Debug.Log("TilemapManager: Clearing Tilemaps...");
        if (groundTilemap != null) groundTilemap.ClearAllTiles();
        if (wallTilemap != null) wallTilemap.ClearAllTiles();
        if (encountersTilemap != null) encountersTilemap.ClearAllTiles();
        Debug.Log("Tilemaps cleared.");
    }

    public void SetTiles(char[,] grid, LevelSettings settings)
    {
        if (groundTilemap == null || wallTilemap == null || encountersTilemap == null)
        {
            Debug.LogError("TilemapManager: Cannot set tiles: Tilemaps missing!");
            return;
        }

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (grid[y, x] == '#')
                {
                    wallTilemap.SetTile(pos, settings.wallTile);
                }
                else if (grid[y, x] == '.' || grid[y, x] == 'E' || grid[y, x] == 'T' || grid[y, x] == 'N' || grid[y, x] == 'C' || grid[y, x] == 'P')
                {
                    groundTilemap.SetTile(pos, settings.groundTile);
                    if (grid[y, x] == 'E')
                        encountersTilemap.SetTile(pos, settings.escapeTile);
                    else if (grid[y, x] == 'T')
                        encountersTilemap.SetTile(pos, settings.trapTile);
                    else if (grid[y, x] == 'C')
                        encountersTilemap.SetTile(pos, settings.coinTile);
                    else if (grid[y, x] == 'P')
                        encountersTilemap.SetTile(pos, settings.potionTile);
                }
            }
        }
    }

    // Add a getter for encountersTilemap
    public Tilemap GetEncountersTilemap()
    {
        return encountersTilemap;
    }
}