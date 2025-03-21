using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] private int maxHealth = 100; // Set in Inspector
    private int currentHealth;
    [SerializeField] private int attack = 5;      // Base attack power
    [SerializeField] private int defense = 2;     // Damage reduction    

    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText; // Change to TMP

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(damage - defense, 0); // Minimum damage is 0
        currentHealth -= damageTaken;
        Debug.Log($"Player took {damageTaken} damage! HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
        }

        UpdateHealthBar();
    }

    public void GainHealth(int amount)
    {
        currentHealth += amount;
        Debug.Log($"Player got {amount}! HP: {currentHealth}");
        if (currentHealth <= 0) Debug.Log("Player died!");
        else if (currentHealth > maxHealth) currentHealth = maxHealth; Debug.Log("Player has max currentHealth!");
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.value = (float)currentHealth / maxHealth;
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}"; // Optional: Show as "current/max"
        }
    }

    public int GetAttack() => attack;
    public int GetHealth() => currentHealth;
    public int GetDefense() => defense; // For enemy damage calculation
}