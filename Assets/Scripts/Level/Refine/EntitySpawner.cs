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
        // Destroy all spawned entities
        foreach (GameObject entity in spawnedEntities)
        {
            if (entity != null)
            {
                Destroy(entity);
            }
        }
        spawnedEntities.Clear();
    }

    public void SpawnEntities(char[,] grid, LevelSettings settings, CinemachineVirtualCamera virtualCamera)
    {
        // Example: Spawn a player at a specific grid position
        for (int y = 0; y < settings.gridHeight; y++)
        {
            for (int x = 0; x < settings.gridWidth; x++)
            {
                if (grid[x, y] == 'P') // 'P' marks player spawn point
                {
                    GameObject player = Instantiate(settings.playerPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    spawnedEntities.Add(player);

                    // Set the camera to follow the player
                    if (virtualCamera != null)
                    {
                        virtualCamera.Follow = player.transform;
                    }
                    return; // Exit after spawning the player
                }
            }
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