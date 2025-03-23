using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public Tilemap groundTilemap { get; private set; }
    public Tilemap wallTilemap { get; private set; }
    public Tilemap encountersTilemap { get; private set; }

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
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    public void SetTiles(char[,] grid, LevelSettings settings)
    {
        for (int y = 0; y < settings.gridHeight; y++)
        {
            for (int x = 0; x < settings.gridWidth; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (grid[x, y] == '.') // Floor
                {
                    groundTilemap.SetTile(pos, settings.groundTile);
                }
                else if (grid[x, y] == '#') // Wall
                {
                    wallTilemap.SetTile(pos, settings.wallTile);
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