using UnityEngine;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private LevelSettings settings;
    private List<GameObject> spawnedEntities = new List<GameObject>();

    public void SpawnEntities(Dictionary<Vector2Int, char> specialTiles)
    {
        ClearEntities();

        foreach (var kvp in specialTiles)
        {
            Vector3 worldPos = new Vector3(kvp.Key.x + 0.5f, kvp.Key.y + 0.5f, 0); // Center of tile
            switch (kvp.Value)
            {
                case 'P':
                    SpawnPrefab(settings.playerPrefab, worldPos, "Player");
                    break;
                case 'E':
                    SpawnPrefab(settings.enemyPrefab, worldPos, "Enemy");
                    break;
                case 'C':
                    SpawnPrefab(settings.coinPrefab, worldPos, "Coin");
                    break;
                case 'H':
                    SpawnPrefab(settings.healthPotionPrefab, worldPos, "Potion");
                    break;
                case 'X':
                    SpawnPrefab(settings.exitPrefab, worldPos, "Exit");
                    break;
                case 'T': 
                    SpawnPrefab(settings.trapPrefab, worldPos, "Trap");
                    break;
            }
        }
        Debug.Log($"Spawned {spawnedEntities.Count} entities");
    }

    private void SpawnPrefab(GameObject prefab, Vector3 position, string tag)
    {
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            spawnedEntities.Add(instance);
        }
        else
        {
            Debug.LogWarning($"Prefab for {tag} is not assigned in LevelSettings!");
        }
    }

    public void ClearEntities()
    {
        foreach (GameObject entity in spawnedEntities)
        {
            if (entity != null)
                Destroy(entity);
        }
        spawnedEntities.Clear();
    }
}