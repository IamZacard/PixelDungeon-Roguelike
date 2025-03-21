using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private bool justFinishedMoving = false;
    private PlayerController controller;

    public bool IsMoving => isMoving;

    public void Setup(PlayerController ctrl)
    {
        controller = ctrl;
        targetPosition = transform.position;
        Debug.Log("PlayerMovement setup complete");
    }

    public void Move(Vector2 direction)
    {
        Vector2 newPos = (Vector2)transform.position + direction;
        Collider2D hit = Physics2D.OverlapPoint(newPos, LayerMask.GetMask("Default"));
        if (hit == null || !hit.enabled) // Safeguard against disabled colliders
        {
            if (IsTileWalkable(newPos))
            {
                targetPosition = newPos;
                isMoving = true;
                justFinishedMoving = false;
            }
            else
            {
                justFinishedMoving = true;
            }
            return;
        }

        if (hit.CompareTag("Enemy"))
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                int damage = Mathf.Max(controller.playerHealth.GetAttack() - enemy.GetDefense(), 0);
                enemy.TakeDamage(damage);
                if (enemy.GetHealth() <= 0)
                {
                    targetPosition = newPos;
                    isMoving = true;
                    justFinishedMoving = false;
                }
                else
                {
                    justFinishedMoving = true;
                }
            }
        }
        else if (IsTileWalkable(newPos))
        {
            targetPosition = newPos;
            isMoving = true;
            justFinishedMoving = false;
        }
        else
        {
            justFinishedMoving = true;
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
                justFinishedMoving = true;
            }
        }
        else
        {
            justFinishedMoving = false;
        }
    }

    public bool IsTileWalkable(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Default"));
        bool walkable = !(hit != null && hit.CompareTag("Wall")); // Only check walls
        Debug.Log($"Checking walkable at {pos}: {walkable}, Hit: {(hit != null ? hit.name : "None")}");
        return walkable;
    }

    public bool JustFinishedMoving() => justFinishedMoving;
}