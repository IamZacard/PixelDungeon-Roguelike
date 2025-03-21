using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    [SerializeField] private int attack = 10;
    [SerializeField] private int defense = 1;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Movement and References")]
    private Transform playerTransform;
    private Tilemap encountersTilemap;
    private Vector2 targetPosition;
    private bool isMoving = false;

    private SpriteRenderer spriteRenderer;

    [Header("Enemy Health UI")]
    private EnemyHealthUI healthUI;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        else Debug.LogWarning("EnemyController: Player not found! UI and movement may not work.");

        // Set up tilemap and position
        encountersTilemap = FindObjectOfType<TilemapManager>().encountersTilemap;
        targetPosition = transform.position;
        currentHealth = maxHealth;

        // Link to EnemyHealthUI on the same GameObject
        healthUI = GetComponent<EnemyHealthUI>();
        if (healthUI == null)
        {
            Debug.LogError("EnemyHealthUI component not found on this GameObject!");
        }
        else
        {
            healthUI.Setup(this); // Initialize the UI with this enemy
        }
    }

    void Update()
    {
        if (playerTransform == null) return; // Skip if player isn’t found

        // Handle movement
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
                Debug.Log("Enemy movement finished");
                TurnManager.Instance.EnemyFinishedTurn();
            }
        }
    }

    public void TakeEnemyTurn()
    {
        if (currentHealth <= 0)
        {
            TurnManager.Instance.EnemyFinishedTurn();
            return;
        }
        if (isMoving) return;

        Vector2 playerPos = playerTransform.position;
        Vector2 enemyPos = transform.position;

        // Check if player is orthogonally adjacent (1 unit away, no diagonals)
        if (IsOrthogonallyAdjacent(playerPos, enemyPos))
        {
            AttackPlayer();
            TurnManager.Instance.EnemyFinishedTurn();
        }
        else
        {
            Move();
            if (!isMoving)
            {
                TurnManager.Instance.EnemyFinishedTurn();
            }
        }
    }

    private bool IsOrthogonallyAdjacent(Vector2 playerPos, Vector2 enemyPos)
    {
        float deltaX = Mathf.Abs(playerPos.x - enemyPos.x);
        float deltaY = Mathf.Abs(playerPos.y - enemyPos.y);

        // Enemy can attack if exactly 1 unit away horizontally or vertically, but not diagonally
        bool isAdjacent = (deltaX == 1f && deltaY == 0f) || (deltaX == 0f && deltaY == 1f);
        if (isAdjacent)
        {
            Debug.Log($"Enemy is orthogonally adjacent to player: deltaX={deltaX}, deltaY={deltaY}");
        }
        return isAdjacent;
    }

    private void AttackPlayer()
    {
        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            int damage = Mathf.Max(attack - playerHealth.GetDefense(), 0);
            playerHealth.TakeDamage(damage);
            Debug.Log($"Enemy attacks player for {damage} damage!");
        }
    }

    private void Move()
    {
        int roll = Random.Range(1, 101);
        Vector2 newPos;
        if (roll >= 65)
        {
            Vector2 playerPos = playerTransform.position;
            Vector2 enemyPos = transform.position;
            float deltaX = playerPos.x - enemyPos.x;
            float deltaY = playerPos.y - enemyPos.y;
            Vector2 moveDirection = Mathf.Abs(deltaX) > Mathf.Abs(deltaY) ?
                (deltaX > 0 ? Vector2.right : Vector2.left) :
                (deltaY > 0 ? Vector2.up : Vector2.down);
            newPos = (Vector2)transform.position + moveDirection;

            UpdateFacing(moveDirection);
            Debug.Log($"Enemy attempting to move toward player: {newPos}");
        }
        else
        {
            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            Vector2 randomDir = directions[Random.Range(0, directions.Length)];
            newPos = (Vector2)transform.position + randomDir;

            UpdateFacing(randomDir); // Update facing for random movement too
            Debug.Log($"Enemy attempting random move: {newPos}");
        }

        if (IsWalkable(newPos))
        {
            targetPosition = newPos;
            isMoving = true;
        }
        else
        {
            Debug.Log("Enemy movement blocked");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthUI != null)
        {
            healthUI.UpdateHealth(); // Update the health display
        }
        else
        {
            Debug.LogWarning("healthUI is null in TakeDamage!");
        }
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        isMoving = false;
        StartCoroutine(DeactivateAtEndOfFrame());
    }

    private IEnumerator DeactivateAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
        Debug.Log("Enemy deactivated at end of frame");
    }

    private bool IsWalkable(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Default"));
        if (hit != null && (hit.CompareTag("Wall") || (hit.CompareTag("Enemy") && hit.gameObject != gameObject)))
        {
            return false;
        }
        Vector3Int tilePos = encountersTilemap.WorldToCell(pos);
        TileBase tile = encountersTilemap.GetTile(tilePos);
        return tile == null;
    }

    public void UpdateFacing(Vector2 direction)
    {
        if (spriteRenderer == null) return;
        if (direction.x < 0) spriteRenderer.flipX = true; // Left
        else if (direction.x > 0) spriteRenderer.flipX = false; // Right
        Debug.Log($"Enemy facing updated: {direction}, flipX: {spriteRenderer.flipX}");
    }

    public int GetDefense() => defense;
    public int GetHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsMoving => isMoving;
}