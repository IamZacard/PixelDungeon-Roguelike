using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("Enemy Health UI")]
    [SerializeField] private GameObject healthUIPrefab;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;

    private EnemyController enemy;
    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        else Debug.LogWarning("EnemyHealthUI: Player not found!");

        enemy = GetComponent<EnemyController>();
        if (enemy != null)
        {
            Setup(enemy);
            if (healthUIPrefab != null)
            {
                healthUIPrefab.SetActive(false);
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

        float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);
        if (distanceToPlayer <= 1f)
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
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        if (enemy == null) return;

        if (healthText != null)
        {
            healthText.text = $"{enemy.GetHealth()}/{enemy.GetMaxHealth()}";
        }

        if (healthSlider != null)
        {
            if (enemy.GetMaxHealth() > 0)
            {
                healthSlider.value = (float)enemy.GetHealth() / enemy.GetMaxHealth();
            }
            else
            {
                healthSlider.value = 0;
            }
        }
    }
}