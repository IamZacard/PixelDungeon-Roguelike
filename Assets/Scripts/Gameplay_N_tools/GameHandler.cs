using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameHandler : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerInteraction playerInteraction;
    private GameObject[] enemies;
    private bool playerInitialized = false;
    private float lastWarningTime = 0f;
    private float warningInterval = 1f;

    void Awake()
    {
        // Singleton pattern
        GameHandler[] handlers = FindObjectsOfType<GameHandler>();
        if (handlers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
            InitializePlayer();
        }

        if (playerInitialized)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartScene();
            }

            if (playerHealth.GetHealth() <= 0)
            {
                RestartScene();
            }

            UpdateEnemies();
            if (AreAllEnemiesDead() && IsPlayerOnEscapeTile())
            {
                ProceedToNextLevel();
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
    }

    private bool AreAllEnemiesDead()
    {
        return enemies.Length == 0;
    }

    private bool IsPlayerOnEscapeTile()
    {
        if (playerInteraction == null) return false;

        Vector3Int tilePos = playerInteraction.GetTilePosition();
        Tilemap encountersTilemap = playerInteraction.GetEncountersTilemap();

        LevelSettings levelSettings = FindObjectOfType<LevelSettings>();
        //LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();

        if (encountersTilemap != null && levelSettings != null)
        {
            TileBase currentTile = encountersTilemap.GetTile(tilePos);
            return currentTile == levelSettings.escapeTile;
        }
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
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels available! Restarting from first level.");
            SceneManager.LoadScene(0);
        }
    }
}