using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Cinemachine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Assigned in Inspector")]
    [SerializeField] private LevelSettings settings;          // Level configuration (e.g., size, tile types)
    [SerializeField] private TilemapManager tilemapManager;   // Handles tile placement
    [SerializeField] private GridGenerator gridGenerator;     // Generates the grid layout
    [SerializeField] private EntitySpawner entitySpawner;     // Spawns entities like the player
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // Camera to follow the player

    private void OnEnable()
    {
        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Start level generation after scene is fully loaded
        StartCoroutine(InitializeLevel());
    }

    private IEnumerator InitializeLevel()
    {
        // Wait one frame to ensure all objects are active
        yield return null;

        // Clear previous level data
        entitySpawner.ClearEntities();
        tilemapManager.ClearTilemaps();

        // Generate the grid
        char[,] grid = gridGenerator.GenerateGrid(settings);

        // Place tiles based on the grid
        tilemapManager.SetTiles(grid, settings);

        // Spawn entities and pass the camera reference
        entitySpawner.SpawnEntities(grid, settings, virtualCamera);
    }
}