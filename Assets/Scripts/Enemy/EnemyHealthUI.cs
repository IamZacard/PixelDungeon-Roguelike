using UnityEngine;
using UnityEngine.UI; // Required for Slider
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("Enemy Health UI")]
    [SerializeField] private GameObject healthUIPrefab; // Assigned in Inspector, not instantiated
    [SerializeField] private TextMeshProUGUI nameText;  // UI text for the enemy’s name
    [SerializeField] private TextMeshProUGUI healthText; // UI text for the enemy’s health
    [SerializeField] private Slider healthSlider;       // Slider for the health bar

    private EnemyController enemy;
    private Transform playerTransform;

    void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        else Debug.LogWarning("EnemyHealthUI: Player not found! UI visibility may not work.");

        // Get EnemyController from the same GameObject
        enemy = GetComponent<EnemyController>();
        if (enemy != null)
        {
            Setup(enemy); // Initialize UI with this enemy
            if (healthUIPrefab != null)
            {
                healthUIPrefab.SetActive(false); // Start hidden
            }
            else
            {
                Debug.LogError("healthUIPrefab is not assigned in EnemyHealthUI!");
            }
        }
        else
        {
            Debug.LogError("EnemyController not found on this GameObject!");
        }
    }

    void Update()
    {
        if (playerTransform == null || healthUIPrefab == null) return;

        // Show/hide UI based on player proximity (1 cell away)
        float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);
        if (distanceToPlayer <= 1f) // 1 unit away
        {
            healthUIPrefab.SetActive(true);
        }
        else
        {
            healthUIPrefab.SetActive(false);
        }
    }

    public void Setup(EnemyController enemyController)
    {
        enemy = enemyController;
        /*if (nameText != null)
        {
            nameText.text = enemy.gameObject.name;
        }*/
        UpdateHealth(); // Set initial health display
    }

    public void UpdateHealth()
    {
        if (enemy == null) return;

        // Update the health text
        if (healthText != null)
        {
            healthText.text = $"{enemy.GetHealth()}/{enemy.GetMaxHealth()}";
        }
        else
        {
            Debug.LogWarning("healthText is not assigned in EnemyHealthUI");
        }

        // Update the health slider
        if (healthSlider != null)
        {
            if (enemy.GetMaxHealth() > 0)
            {
                float healthPercentage = (float)enemy.GetHealth() / enemy.GetMaxHealth();
                healthSlider.value = healthPercentage;
            }
            else
            {
                healthSlider.value = 0;
                Debug.LogWarning("enemy.GetMaxHealth() is zero, cannot calculate health percentage");
            }
        }
        else
        {
            Debug.LogWarning("healthSlider is not assigned in EnemyHealthUI");
        }
    }
}