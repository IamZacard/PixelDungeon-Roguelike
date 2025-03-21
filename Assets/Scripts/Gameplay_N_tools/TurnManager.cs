using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private PlayerController playerController;
    private bool isInitialized = false;
    private int pendingEnemyTurns;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Subscribe to scene loaded event to reset state
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (playerController != null)
        {
            PlayerController.OnPlayerTurnEnded -= HandlePlayerTurnEnded;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"TurnManager: Scene loaded - {scene.name}");
        // Reset initialization state
        isInitialized = false;
        playerController = null;
        pendingEnemyTurns = 0;
    }

    void Start()
    {
        Debug.Log("TurnManager started, waiting for player initialization...");
    }

    void Update()
    {
        if (!isInitialized)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                Initialize(playerController);
                isInitialized = true;
                Debug.Log("TurnManager initialized via Update polling.");
            }
        }
    }

    public void Initialize(PlayerController playerCtrl)
    {
        if (playerCtrl == null)
        {
            Debug.LogError("TurnManager: Attempted to initialize with null PlayerController!");
            return;
        }

        if (playerController != playerCtrl)
        {
            if (playerController != null)
            {
                PlayerController.OnPlayerTurnEnded -= HandlePlayerTurnEnded;
            }

            playerController = playerCtrl;
            PlayerController.OnPlayerTurnEnded += HandlePlayerTurnEnded;
            Debug.Log("TurnManager initialized with PlayerController");
        }
    }

    public void HandlePlayerTurnEnded()
    {
        if (playerController == null)
        {
            Debug.LogError("playerController is null in HandlePlayerTurnEnded!");
            return;
        }
        var activeEnemies = FindObjectsOfType<EnemyController>()
            .Where(e => e.gameObject.activeInHierarchy)
            .ToList();
        pendingEnemyTurns = activeEnemies.Count;
        Debug.Log($"Starting enemy turns: {pendingEnemyTurns} enemies active");
        if (pendingEnemyTurns == 0)
        {
            playerController.OnEnemyTurnsComplete();
            Debug.Log("No enemies, player turn resumed");
            return;
        }
        foreach (var enemy in activeEnemies)
        {
            enemy.TakeEnemyTurn();
        }
    }

    public void EnemyFinishedTurn()
    {
        pendingEnemyTurns--;
        Debug.Log($"Enemy finished turn, pending: {pendingEnemyTurns}");
        if (pendingEnemyTurns <= 0)
        {
            if (playerController != null)
            {
                playerController.OnEnemyTurnsComplete();
                Debug.Log("All enemies done, player turn resumed");
            }
            else
            {
                Debug.LogError("playerController is null when enemy turns completed!");
            }
        }
    }
}