using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private LevelSettings levelSettings; // Assign this in the Inspector
    private PlayerHealth playerHealth;
    private PlayerInteraction playerInteraction;
    private GameObject[] enemies;
    private LevelGenerator levelGenerator; // Cache the LevelGenerator reference
    private bool playerInitialized = false;
    private float lastWarningTime = 0f;
    private float warningInterval = 1f;

    void Awake()
    {
        // Singleton pattern
        GameHandler[] handlers = FindObjectsOfType<GameHandler>();
        if (handlers.Length > 1)
        {
            Debug.Log("GameHandler: Multiple instances found, destroying this one.");
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("GameHandler: Awake completed, subscribed to sceneLoaded.");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("GameHandler: OnDestroy called, unsubscribed from sceneLoaded.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameHandler: Scene loaded - {scene.name}");
        // Reset state
        playerInitialized = false;
        playerHealth = null;
        playerInteraction = null;
        enemies = new GameObject[0];
        lastWarningTime = 0f;

        // Validate LevelSettings
        if (levelSettings == null)
        {
            Debug.LogError("GameHandler: LevelSettings is not assigned in the Inspector!");
        }
        else
        {
            Debug.Log("GameHandler: LevelSettings assigned successfully.");
        }
    }

    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"GameHandler started. Found {enemies.Length} enemies.");
    }

    void Update()
    {
        if (!playerInitialized)
        {
            Debug.Log("GameHandler: Player not initialized yet, calling InitializePlayer.");
            InitializePlayer();
        }

        if (playerInitialized)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("GameHandler: R key pressed, restarting scene.");
                RestartScene();
            }

            if (playerHealth.GetHealth() <= 0)
            {
                Debug.Log("GameHandler: Player health <= 0, restarting scene.");
                RestartScene();
            }

            UpdateEnemies();
            Debug.Log($"GameHandler: Checking win condition - AreAllEnemiesDead: {AreAllEnemiesDead()}, IsPlayerOnEscapeTile: {IsPlayerOnEscapeTile()}");

            if (AreAllEnemiesDead() && IsPlayerOnEscapeTile())
            {
                Debug.Log("GameHandler: Win condition met! Proceeding to next level.");
                ProceedToNextLevel();
            }
            else
            {
                Debug.Log("GameHandler: Win condition not met.");
            }
        }
    }

    public void InitializePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerInteraction = player.GetComponent<PlayerInteraction>();
            if (playerHealth == null || playerInteraction == null)
            {
                Debug.LogError("PlayerHealth or PlayerInteraction not found on Player object!");
            }
            else
            {
                playerInitialized = true;
                Debug.Log("Player initialized successfully in GameHandler.");
            }
        }
        else
        {
            if (Time.time - lastWarningTime >= warningInterval)
            {
                Debug.LogWarning("Player object with 'Player' tag not found yet. Waiting...");
                lastWarningTime = Time.time;
            }
        }
    }

    private void UpdateEnemies()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"GameHandler: UpdateEnemies - Found {enemies.Length} enemies.");
    }

    private bool AreAllEnemiesDead()
    {
        bool allDead = enemies.Length == 0;
        Debug.Log($"GameHandler: AreAllEnemiesDead - Result: {allDead}, Enemy count: {enemies.Length}");
        return allDead;
    }

    private bool IsPlayerOnEscapeTile()
    {
        if (playerInteraction == null)
        {
            Debug.LogWarning("GameHandler: IsPlayerOnEscapeTile - playerInteraction is null!");
            return false;
        }

        Vector3Int tilePos = playerInteraction.GetTilePosition();
        Tilemap encountersTilemap = playerInteraction.GetEncountersTilemap();

        Debug.Log($"GameHandler: IsPlayerOnEscapeTile - Tile Position: {tilePos}, EncountersTilemap: {(encountersTilemap != null ? "Found" : "Null")}, LevelSettings: {(levelSettings != null ? "Found" : "Null")}");

        if (encountersTilemap != null && levelSettings != null)
        {
            TileBase currentTile = encountersTilemap.GetTile(tilePos);
            bool isEscapeTile = currentTile == levelSettings.escapeTile;
            Debug.Log($"GameHandler: IsPlayerOnEscapeTile - Current Tile: {(currentTile != null ? currentTile.name : "Null")}, Escape Tile: {(levelSettings.escapeTile != null ? levelSettings.escapeTile.name : "Null")}, IsEscapeTile: {isEscapeTile}");
            return isEscapeTile;
        }

        Debug.LogWarning("GameHandler: IsPlayerOnEscapeTile - encountersTilemap or levelSettings is null!");
        return false;
    }

    private void RestartScene()
    {
        Debug.Log("Restarting scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ProceedToNextLevel()
    {
        Debug.Log("All enemies dead and player on escape tile! Proceeding to next level...");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"GameHandler: Loading next scene with index {nextSceneIndex}.");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels available! Restarting from first level.");
            SceneManager.LoadScene(0);
        }
    }
}