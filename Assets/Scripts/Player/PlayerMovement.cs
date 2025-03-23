using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject attackParticleEffect; // Particle effect for attacking
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
                PlayAttackParticleEffect(newPos); // Play particle effect at enemy's position                               

                if (enemy.GetHealth() <= 0)
                {
                    targetPosition = newPos; // Move into enemy's position if killed
                    isMoving = true;
                    justFinishedMoving = false;
                }
                else
                {
                    justFinishedMoving = true; // Stay put if enemy survives
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

    public void SkipMove() // For F key
    {
        Debug.Log("Player skipped move (F pressed)");
        justFinishedMoving = true; // Trigger turn end without moving
    }

    private void PlayAttackParticleEffect(Vector2 enemyPos)
    {
        if (attackParticleEffect != null)
        {
            attackParticleEffect.transform.position = enemyPos; // Set position to enemy's location
            Instantiate(attackParticleEffect, enemyPos, Quaternion.identity);
            Debug.Log($"Attack particle effect played at {enemyPos}");
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
        bool walkable = !(hit != null && hit.CompareTag("Wall"));
        Debug.Log($"Checking walkable at {pos}: {walkable}, Hit: {(hit != null ? hit.name : "None")}");
        return walkable;
    }

    public bool JustFinishedMoving() => justFinishedMoving;
}