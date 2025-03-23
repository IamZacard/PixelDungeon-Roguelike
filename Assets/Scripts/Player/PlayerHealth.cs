using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private int attack = 5;
    [SerializeField] private int defense = 2;

    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer playerSprite; // Reference to the player's sprite
    private Color originalColor; // Store the sprite's original color

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Get the SpriteRenderer and store its original color
        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
            if (playerSprite == null)
            {
                Debug.LogError("PlayerHealth: No SpriteRenderer found on player!");
            }
        }
        originalColor = playerSprite != null ? playerSprite.color : Color.white;
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(damage - defense, 0);
        currentHealth -= damageTaken;
        Debug.Log($"Player took {damageTaken} damage! HP: {currentHealth}");

        // Trigger the blink effect if damage is taken
        if (damageTaken > 0 && playerSprite != null)
        {
            StartCoroutine(BlinkRed());
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
        }
        UpdateHealthBar();
    }

    public void GainHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthBar();
        Debug.Log($"Player gained {amount} health! HP: {currentHealth}");
    }

    private void UpdateHealthBar()
    {
        healthBar.value = (float)currentHealth / maxHealth;
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }

    private IEnumerator BlinkRed()
    {
        // Set sprite color to red
        playerSprite.color = Color.red;

        // Wait for 0.3 seconds
        yield return new WaitForSeconds(0.3f);

        // Revert to original color
        playerSprite.color = originalColor;
    }

    public int GetAttack() => attack;
    public int GetHealth() => currentHealth;
    public int GetDefense() => defense;
}