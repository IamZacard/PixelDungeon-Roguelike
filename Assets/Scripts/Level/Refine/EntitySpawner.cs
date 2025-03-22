using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
public class EntitySpawner : MonoBehaviour
{
    private GameObject playerInstance;
    private CinemachineVirtualCamera virtualCamera;
    private List<GameObject> spawnedEntities = new List<GameObject>();

    public bool AssignVirtualCameraDynamically()
    {
        Debug.Log("EntitySpawner: Assigning VirtualCamera...");
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogWarning("EntitySpawner: CinemachineVirtualCamera not found dynamically!");
            return false;
        }
        Debug.Log($"EntitySpawner: CinemachineVirtualCamera found: {virtualCamera.gameObject.name}");
        return true;
    }

    public void ClearEntities()
    {
        Debug.Log($"EntitySpawner: Clearing {spawnedEntities.Count} entities...");
        foreach (GameObject entity in spawnedEntities)
        {
            if (entity != null)
            {
                Debug.Log($"EntitySpawner: Destroying entity {entity.name}");
                Destroy(entity);
            }
        }
        spawnedEntities.Clear();
        playerInstance = null;
        Debug.Log("EntitySpawner: Entities cleared.");
    }

    public void SpawnEntities(char[,] grid, LevelSettings settings)
    {
        Debug.Log("EntitySpawner: Spawning entities...");
        Vector3? playerSpawnPosition = null;

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 worldPos = new Vector3(x + 0.5f, y + 0.5f, 0);
                if (grid[y, x] == 'N')
                {
                    if (settings.enemyPrefab != null)
                    {
                        GameObject enemy = Instantiate(settings.enemyPrefab, worldPos, Quaternion.identity);
                        spawnedEntities.Add(enemy);
                        Debug.Log($"EntitySpawner: Spawned enemy at {worldPos}");
                    }
                    else
                    {
                        Debug.LogError("EntitySpawner: Enemy prefab not assigned!");
                    }
                }
                else if (grid[y, x] == 'P')
                {
                    playerSpawnPosition = worldPos;
                    Debug.Log($"EntitySpawner: Found player spawn position at {worldPos}");
                }
            }
        }

        if (playerSpawnPosition.HasValue)
        {
            SpawnPlayer(settings, playerSpawnPosition.Value);
        }
        else
        {
            Debug.LogWarning("EntitySpawner: No player spawn position ('P') found in the grid! Spawning at default position.");
            SpawnPlayer(settings, new Vector3(1.5f, 1.5f, 0));
        }
    }

    private void SpawnPlayer(LevelSettings settings, Vector3 spawnPosition)
    {
        Debug.Log($"EntitySpawner: Spawning player at position {spawnPosition}...");
        if (settings.playerPrefab == null)
        {
            Debug.LogError("EntitySpawner: Player prefab not assigned!");
            return;
        }

        if (virtualCamera == null)
        {
            if (!AssignVirtualCameraDynamically())
            {
                Debug.LogError("EntitySpawner: Cinemachine Virtual Camera not found after reassignment!");
                return;
            }
        }

        if (playerInstance != null)
        {
            Debug.Log("EntitySpawner: Destroying old player instance...");
            Destroy(playerInstance);
            playerInstance = null;
        }

        playerInstance = Instantiate(settings.playerPrefab, spawnPosition, Quaternion.identity);
        playerInstance.tag = "Player";
        spawnedEntities.Add(playerInstance);
        Debug.Log("EntitySpawner: Player instantiated.");

        PlayerController playerController = playerInstance.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("EntitySpawner: Player prefab does not have a PlayerController component!");
            return;
        }

        if (virtualCamera != null)
        {
            Vector3 centerPosition = new Vector3(settings.gridWidth / 2f, settings.gridHeight / 2f, -10f);
            virtualCamera.transform.position = centerPosition;
            virtualCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            virtualCamera.Follow = playerInstance.transform;
            Debug.Log("EntitySpawner: Camera set up to follow player.");
        }

        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.Initialize(playerController);
            Debug.Log("EntitySpawner: TurnManager initialized.");
        }
        else
        {
            Debug.LogError("EntitySpawner: No TurnManager found in the scene!");
        }

        GameHandler gameHandler = FindObjectOfType<GameHandler>();
        if (gameHandler != null)
        {
            gameHandler.InitializePlayer();
            Debug.Log("EntitySpawner: GameHandler initialized.");
        }
        else
        {
            Debug.LogError("EntitySpawner: No GameHandler found in the scene!");
        }

        Debug.Log("EntitySpawner: Player spawned successfully.");
    }
}