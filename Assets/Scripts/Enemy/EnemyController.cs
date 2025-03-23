using UnityEngine;
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
    private Vector2 targetPosition;
    private bool isMoving = false;

    private SpriteRenderer spriteRenderer;
    private EnemyHealthUI healthUI;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("EnemyController: No SpriteRenderer found on enemy!");
        }
        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemyController: Player not found!");
        }

        targetPosition = transform.position;
        currentHealth = maxHealth;

        healthUI = GetComponent<EnemyHealthUI>();
        if (healthUI != null)
        {
            healthUI.Setup(this);
        }
        else
        {
            Debug.LogError("EnemyHealthUI component not found on this GameObject!");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public IEnumerator TakeTurnCoroutine()
    {
        if (currentHealth <= 0 || !gameObject.activeInHierarchy) // Added active check
        {
            yield break;
        }

        if (playerTransform == null)
        {
            yield break;
        }

        Vector2 playerPos = playerTransform.position;
        Vector2 enemyPos = transform.position;

        if (IsOrthogonallyAdjacent(playerPos, enemyPos))
        {
            Debug.Log($"{gameObject.name} attacks the player.");
            AttackPlayer();
            yield return null;
        }
        else
        {
            Debug.Log($"{gameObject.name} is attempting to move.");
            Move();

            while (isMoving)
            {
                yield return null;
            }
        }

        yield return null;
    }

    private bool IsOrthogonallyAdjacent(Vector2 playerPos, Vector2 enemyPos)
    {
        float deltaX = Mathf.Abs(playerPos.x - enemyPos.x);
        float deltaY = Mathf.Abs(playerPos.y - enemyPos.y);
        return (Mathf.Approximately(deltaX, 1f) && Mathf.Approximately(deltaY, 0f)) ||
               (Mathf.Approximately(deltaX, 0f) && Mathf.Approximately(deltaY, 1f));
    }

    private void AttackPlayer()
    {
        if (playerTransform == null) return;

        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            int damage = Mathf.Max(attack - playerHealth.GetTotalDefense(), 0);
            playerHealth.TakeDamage(damage);
            Debug.Log($"{gameObject.name} attacks player for {damage} damage!");
        }
    }

    private void Move()
    {
        if (playerTransform == null) return;

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
            newPos = enemyPos + moveDirection;
            UpdateFacing(moveDirection);
        }
        else
        {
            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            Vector2 randomDir = directions[Random.Range(0, directions.Length)];
            newPos = (Vector2)transform.position + randomDir;
            UpdateFacing(randomDir);
        }

        if (IsWalkable(newPos))
        {
            targetPosition = newPos;
            isMoving = true;
        }
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(damage - defense, 0);
        currentHealth -= damageTaken;
        Debug.Log($"{gameObject.name} took {damageTaken} damage! HP: {currentHealth}");

        if (damageTaken > 0 && spriteRenderer != null)
        {
            StartCoroutine(BlinkRed());
        }

        if (healthUI != null)
        {
            healthUI.UpdateHealth();
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        Debug.Log($"{name} is dead!");
    }

    private bool IsWalkable(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Default"));
        if (hit != null && (hit.CompareTag("Wall") ||
                            (hit.CompareTag("Enemy") && hit.gameObject != gameObject) ||
                            hit.CompareTag("Trap") ||
                            hit.CompareTag("Player")))
        {
            return false;
        }
        return true;
    }

    public void UpdateFacing(Vector2 direction)
    {
        if (spriteRenderer == null) return;
        if (direction.x < 0) spriteRenderer.flipX = true;
        else if (direction.x > 0) spriteRenderer.flipX = false;
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = originalColor;
    }

    public int GetDefense() => defense;
    public int GetHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}