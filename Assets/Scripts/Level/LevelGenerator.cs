using UnityEngine;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelSettings settings;
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private EntitySpawner entitySpawner;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private bool canProceed = true;

    void Awake()
    {
        ValidateComponents();
        if (!canProceed)
        {
            Debug.LogError("LevelGenerator: Cannot proceed due to missing components!");
            enabled = false;
        }
    }

    void Start()
    {
        if (canProceed)
            StartCoroutine(GenerateLevel());
    }

    void OnEnable()
    {
        AssignComponents();
    }

    private void AssignComponents()
    {
        // Dynamically assign all components to handle scene restarts
        if (settings == null)
        {
            settings = Resources.Load<LevelSettings>("LevelSettings");
            if (settings == null)
                Debug.LogWarning("LevelGenerator: LevelSettings not found in Resources!");
            else
                Debug.Log("LevelGenerator: LevelSettings assigned dynamically");
        }

        if (tilemapManager == null)
        {
            tilemapManager = FindObjectOfType<TilemapManager>();
            if (tilemapManager == null)
                Debug.LogWarning("LevelGenerator: TilemapManager not found in scene!");
            else
                Debug.Log("LevelGenerator: TilemapManager assigned dynamically");
        }

        if (gridGenerator == null)
        {
            gridGenerator = FindObjectOfType<GridGenerator>();
            if (gridGenerator == null)
                Debug.LogWarning("LevelGenerator: GridGenerator not found in scene!");
            else
                Debug.Log("LevelGenerator: GridGenerator assigned dynamically");
        }

        if (entitySpawner == null)
        {
            entitySpawner = FindObjectOfType<EntitySpawner>();
            if (entitySpawner == null)
                Debug.LogWarning("LevelGenerator: EntitySpawner not found in scene!");
            else
                Debug.Log("LevelGenerator: EntitySpawner assigned dynamically");
        }

        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualCamera == null)
                Debug.LogWarning("LevelGenerator: CinemachineVirtualCamera not found in scene!");
            else
                Debug.Log("LevelGenerator: CinemachineVirtualCamera assigned dynamically");
        }
    }

    private IEnumerator GenerateLevel()
    {
        yield return new WaitForEndOfFrame(); // Ensure all scene objects are loaded
        tilemapManager.AssignTilemapsDynamically();
        tilemapManager.ClearTilemaps();

        Debug.Log("Generating grid...");
        char[,] grid = gridGenerator.GenerateGrid(settings, out Dictionary<Vector2Int, char> specialTiles);
        if (grid == null)
        {
            Debug.LogError("Grid generation failed!");
            yield break;
        }
        //char[,] grid = gridGenerator.GenerateGrid(settings);
        tilemapManager.SetTiles(grid, settings);

        Debug.Log("Spawning entities...");
        entitySpawner.SpawnEntities(specialTiles);

        // Center the camera
        Vector3 centerPosition = new Vector3(settings.gridWidth / 2f, settings.gridHeight / 2f - 2f, -10f);
        virtualCamera.transform.position = centerPosition;
        virtualCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Debug.Log($"Camera positioned at {centerPosition}, rotation set to {virtualCamera.transform.rotation.eulerAngles}");
    }

    private void ValidateComponents()
    {
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
        if (settings.groundTile == null || settings.wallTile == null)
        {
            Debug.LogError("LevelGenerator: Ground or wall tiles not assigned in LevelSettings!");
            canProceed = false;
        }
    }
}