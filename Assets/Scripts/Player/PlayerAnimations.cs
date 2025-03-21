using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("Player needs a SpriteRenderer!");
        Debug.Log("PlayerAnimations initialized");
    }

    public void UpdateFacing(Vector2 direction)
    {
        if (spriteRenderer == null) return;
        if (direction.x < 0) spriteRenderer.flipX = true; // Left
        else if (direction.x > 0) spriteRenderer.flipX = false; // Right
        Debug.Log($"Facing updated: {direction}, flipX: {spriteRenderer.flipX}");
    }
}