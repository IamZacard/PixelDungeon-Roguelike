using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    public void AssignTilemapsDynamically()
    {
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        groundTilemap = null;
        wallTilemap = null;

        foreach (Tilemap tm in tilemaps)
        {
            string tmName = tm.gameObject.name.ToLower();
            if (tmName.Contains("ground"))
                groundTilemap = tm;
            else if (tmName.Contains("wall"))
                wallTilemap = tm;
        }

        if (groundTilemap == null) Debug.LogWarning("TilemapManager: groundTilemap not found!");
        if (wallTilemap == null) Debug.LogWarning("TilemapManager: wallTilemap not found!");
    }

    public void ClearTilemaps()
    {
        if (groundTilemap != null) groundTilemap.ClearAllTiles();
        if (wallTilemap != null) wallTilemap.ClearAllTiles();
    }

    public void SetTiles(char[,] grid, LevelSettings settings)
    {
        if (groundTilemap == null || wallTilemap == null)
        {
            Debug.LogError("TilemapManager: Tilemaps missing!");
            return;
        }

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (grid[y, x] == '#')
                    wallTilemap.SetTile(pos, settings.wallTile);
                else if (grid[y, x] == '.')
                    groundTilemap.SetTile(pos, settings.groundTile);
            }
        }
    }
}