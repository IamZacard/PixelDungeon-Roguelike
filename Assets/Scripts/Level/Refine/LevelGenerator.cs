using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelSettings settings;
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private EntitySpawner entitySpawner;

    private bool canProceed = true;

    public LevelSettings GetLevelSettings()
    {
        return settings;
    }

    void Awake()
    {
        Debug.Log("LevelGenerator: Awake called.");
        ValidateComponents();

        if (!canProceed)
        {
            Debug.LogError("LevelGenerator: Cannot proceed due to missing components. Check errors above.");
            enabled = false;
            return;
        }

        if (!entitySpawner.AssignVirtualCameraDynamically())
        {
            Debug.LogError("LevelGenerator: Failed to assign CinemachineVirtualCamera.");
            canProceed = false;
        }

        ValidateSettings();
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
            Debug.LogError("LevelGenerator: Start skipped due to missing references or components.");
        }
    }

    private IEnumerator InitializeLevel()
    {
        Debug.Log("LevelGenerator: Initializing level...");
        yield return new WaitForEndOfFrame();

        tilemapManager.AssignTilemapsDynamically();
        tilemapManager.ClearTilemaps();

        char[,] grid = gridGenerator.GenerateGrid(settings);
        tilemapManager.SetTiles(grid, settings);
        entitySpawner.SpawnEntities(grid, settings);
    }

    private void ValidateComponents()
    {
        Debug.Log("LevelGenerator: Validating components...");
        canProceed = true;

        if (settings == null)
        {
            Debug.LogError("LevelGenerator: LevelSettings not assigned!");
            canProceed = false;
        }
        if (tilemapManager == null)
        {
            Debug.LogError("LevelGenerator: TilemapManager not assigned!");
            canProceed = false;
        }
        if (gridGenerator == null)
        {
            Debug.LogError("LevelGenerator: GridGenerator not assigned!");
            canProceed = false;
        }
        if (entitySpawner == null)
        {
            Debug.LogError("LevelGenerator: EntitySpawner not assigned!");
            canProceed = false;
        }

        if (canProceed)
        {
            Debug.Log("LevelGenerator: All components validated successfully.");
        }
    }

    private void ValidateSettings()
    {
        Debug.Log("LevelGenerator: Validating settings...");
        if (settings.groundTile == null)
        {
            Debug.LogError("LevelGenerator: groundTile not assigned in LevelSettings!");
            canProceed = false;
        }
        if (settings.wallTile == null)
        {
            Debug.LogError("LevelGenerator: wallTile not assigned in LevelSettings!");
            canProceed = false;
        }
        if (settings.escapeTile == null)
        {
            Debug.LogError("LevelGenerator: escapeTile not assigned in LevelSettings!");
            canProceed = false;
        }
        if (settings.trapTile == null)
        {
            Debug.LogError("LevelGenerator: trapTile not assigned in LevelSettings!");
            canProceed = false;
        }
        if (settings.enemyTile == null)
        {
            Debug.LogError("LevelGenerator: enemyTile not assigned in LevelSettings!");
            canProceed = false;
        }
        if (settings.coinTile == null)
        {
            Debug.LogError("LevelGenerator: coinTile not assigned in LevelSettings!");
            canProceed = false;
        }
        if (settings.potionTile == null)
        {
            Debug.LogError("LevelGenerator: potionTile not assigned in LevelSettings!");
            canProceed = false;
        }
        if (settings.playerPrefab == null)
        {
            Debug.LogError("LevelGenerator: playerPrefab not assigned in LevelSettings!");
            canProceed = false;
        }
        if (settings.enemyPrefab == null)
        {
            Debug.LogError("LevelGenerator: enemyPrefab not assigned in LevelSettings!");
            canProceed = false;
        }

        if (canProceed)
        {
            Debug.Log("LevelGenerator: All settings validated successfully.");
        }
    }
}