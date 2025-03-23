using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] private int baseMaxHealth = 100;
    private int currentHealth;
    [SerializeField] private int baseAttack = 5;
    [SerializeField] private int baseDefense = 2;

    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private GameObject attackParticleEffect;
    private Color originalColor;

    // Event for when player dies
    public event Action OnPlayerDeath;

    // Reference to PlayerInventory
    private PlayerInventory playerInventory;

    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerHealth: PlayerInventory component not found on the same GameObject!");
        }
        else
        {
            playerInventory.OnEquipmentChanged += OnEquipmentChanged;
        }

        currentHealth = baseMaxHealth + (playerInventory?.equipment.GetTotalHealthBonus() ?? 0);
        UpdateHealthBar();

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

    void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnEquipmentChanged -= OnEquipmentChanged;
        }
    }

    public void TakeDamage(int damage)
    {
        int totalDefense = GetTotalDefense();
        int damageTaken = Mathf.Max(damage - totalDefense, 0);
        currentHealth -= damageTaken;
        Debug.Log($"Player took {damageTaken} damage! HP: {currentHealth}/{CurrentMaxHealth}");

        PlayAttackParticleEffect();

        if (damageTaken > 0 && playerSprite != null)
        {
            StartCoroutine(BlinkRed());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        UpdateHealthBar();
    }

    public void GainHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > CurrentMaxHealth)
            currentHealth = CurrentMaxHealth;
        UpdateHealthBar();
        Debug.Log($"Player gained {amount} health! HP: {currentHealth}/{CurrentMaxHealth}");
    }

    private void Die()
    {
        Debug.Log("Player died!");
        OnPlayerDeath?.Invoke();
    }

    private void UpdateHealthBar()
    {
        healthBar.value = (float)currentHealth / CurrentMaxHealth;
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{CurrentMaxHealth}";
        }
    }

    private IEnumerator BlinkRed()
    {
        playerSprite.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        playerSprite.color = originalColor;
    }

    private void PlayAttackParticleEffect()
    {
        if (attackParticleEffect != null)
        {
            Instantiate(attackParticleEffect, transform.position, Quaternion.identity);
        }
    }

    private void OnEquipmentChanged(ItemType type, Item item)
    {
        if (currentHealth > CurrentMaxHealth)
        {
            currentHealth = CurrentMaxHealth;
        }
        UpdateHealthBar();
    }

    // Stat getters
    public int GetTotalAttack()
    {
        return baseAttack + (playerInventory?.equipment.GetTotalAttackBonus() ?? 0);
    }

    public int GetTotalDefense()
    {
        return baseDefense + (playerInventory?.equipment.GetTotalDefenseBonus() ?? 0);
    }

    public int CurrentMaxHealth
    {
        get { return baseMaxHealth + (playerInventory?.equipment.GetTotalHealthBonus() ?? 0); }
    }

    // Original getters for base stats (if needed elsewhere) commented, coz no need for now.
    //public int GetBaseAttack() => baseAttack;
    //public int GetBaseDefense() => baseDefense;
    public int GetHealth() => currentHealth;
}