using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public PlayerMovement playerMovement;
    public PlayerAnimations playerAnimations;
    public PlayerHealth playerHealth;
    public PlayerInteraction playerInteraction;
    public Collider2D playerCollider;

    [SerializeField] private LevelSettings levelSettings; // Assign in Inspector or load dynamically

    private bool isPlayerTurn = true;
    private bool isSceneTransitioning = false;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>() ?? gameObject.AddComponent<PlayerMovement>();
        playerAnimations = GetComponent<PlayerAnimations>() ?? gameObject.AddComponent<PlayerAnimations>();
        playerHealth = GetComponent<PlayerHealth>() ?? gameObject.AddComponent<PlayerHealth>();
        playerInteraction = GetComponent<PlayerInteraction>() ?? gameObject.AddComponent<PlayerInteraction>();

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
            if (playerCollider == null) Debug.LogError("Player needs a Collider2D!");
        }

        if (levelSettings == null)
        {
            levelSettings = Resources.Load<LevelSettings>("LevelSettings");
            if (levelSettings == null) Debug.LogError("LevelSettings not found in Resources!");
        }

        playerMovement.Setup(this);
        playerInteraction.Setup(this, levelSettings);

        Debug.Log("PlayerController initialized");
    }

    void Update()
    {
        if (isSceneTransitioning) return;

        if (isPlayerTurn && !playerMovement.IsMoving)
        {
            Vector2 direction = Vector2.zero;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) direction = Vector2.up;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) direction = Vector2.down;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) direction = Vector2.left;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) direction = Vector2.right;

            if (direction != Vector2.zero)
            {
                isPlayerTurn = false;
                playerMovement.Move(direction);
                playerAnimations.UpdateFacing(direction);
            }
            else if (Input.GetKeyDown(KeyCode.F)) // Stay-in-place move
            {
                isPlayerTurn = false;
                playerMovement.SkipMove(); // New method for F key
            }
        }

        if (playerMovement.JustFinishedMoving())
        {
            Debug.Log("Move finished, handling tile effect");
            playerInteraction.HandleTileEffect();
            Debug.Log("Tile effect handled, ending turn");
            EndPlayerTurn();
        }
    }

    void EndPlayerTurn()
    {
        if (isSceneTransitioning) return;
        StartCoroutine(ProcessEnemyTurns());
    }

    IEnumerator ProcessEnemyTurns()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>().Where(e => e.gameObject.activeInHierarchy).ToArray();
        Debug.Log($"Processing {enemies.Length} enemy turns.");

        foreach (EnemyController enemy in enemies)
        {
            yield return StartCoroutine(enemy.TakeTurnCoroutine());
        }

        Debug.Log("All enemy turns finished. Player's turn resumed.");
        isPlayerTurn = true;
    }
}