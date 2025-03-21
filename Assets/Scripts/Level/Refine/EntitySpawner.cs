using UnityEngine;
using Cinemachine;
public class EntitySpawner : MonoBehaviour
{
    private GameObject playerInstance;
    private CinemachineVirtualCamera virtualCamera;

    public bool AssignVirtualCameraDynamically()
    {
        Debug.Log("EntitySpawner: Assigning VirtualCamera...");
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogWarning("CinemachineVirtualCamera not found dynamically!");
            return false;
        }
        Debug.Log($"CinemachineVirtualCamera found: {virtualCamera.gameObject.name}");
        return true;
    }

    public void SpawnEntities(char[,] grid, LevelSettings settings)
    {
        // Spawn enemies (already marked as 'N' in the grid)
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                if (grid[y, x] == 'N')
                {
                    if (settings.enemyPrefab != null)
                    {
                        Vector3 worldPos = new Vector3(x + 0.5f, y + 0.5f, 0);
                        Instantiate(settings.enemyPrefab, worldPos, Quaternion.identity);
                    }
                    else
                    {
                        Debug.LogError("EntitySpawner: Enemy prefab not assigned!");
                    }
                }
            }
        }

        // Spawn player
        SpawnPlayer(settings);
    }

    private void SpawnPlayer(LevelSettings settings)
    {
        Debug.Log("EntitySpawner: Spawning player...");
        if (settings.playerPrefab == null)
        {
            Debug.LogError("EntitySpawner: Player prefab not assigned!");
            return;
        }
        if (virtualCamera == null)
        {
            Debug.LogError("EntitySpawner: Cinemachine Virtual Camera not assigned!");
            return;
        }

        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }

        playerInstance = Instantiate(settings.playerPrefab, new Vector3(1.5f, 1.5f, 0), Quaternion.identity);
        playerInstance.tag = "Player";
        PlayerController playerController = playerInstance.GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("EntitySpawner: Player prefab does not have a PlayerController component!");
            return;
        }

        Vector3 centerPosition = new Vector3(settings.gridWidth / 2f, settings.gridHeight / 2f, -10f);
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
            Debug.LogError("EntitySpawner: No TurnManager found in the scene!");
        }

        GameHandler gameHandler = FindObjectOfType<GameHandler>();
        if (gameHandler != null)
        {
            gameHandler.InitializePlayer();
        }
        else
        {
            Debug.LogError("EntitySpawner: No GameHandler found in the scene!");
        }

        Debug.Log("EntitySpawner: Player spawned successfully.");
    }
}