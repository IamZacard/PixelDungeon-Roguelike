using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class PlayerInteraction : MonoBehaviour
{
    private LevelSettings levelSettings;
    private PlayerController controller;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerAnimations playerAnimations;
    private Collider2D playerCollider;

    [SerializeField] private int wealth = 0;
    [SerializeField] private float interactionRadius = 0.2f;

    private Animator exitAnimator; // Reference to the exit's Animator
    private bool hasCheckedEnemies = false; // Prevent repeated enemy checks

    public void Setup(PlayerController ctrl, LevelSettings settings)
    {
        controller = ctrl;
        levelSettings = settings;

        playerHealth = ctrl.GetComponent<PlayerHealth>();
        playerMovement = ctrl.GetComponent<PlayerMovement>();
        playerAnimations = ctrl.GetComponent<PlayerAnimations>();
        playerCollider = ctrl.GetComponent<Collider2D>();

        if (levelSettings == null)
        {
            Debug.LogError("LevelSettings not provided to PlayerInteraction!");
            return;
        }
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component missing on PlayerController!");
            return;
        }
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component missing on PlayerController!");
            return;
        }
        if (playerAnimations == null)
        {
            Debug.LogError("PlayerAnimations component missing on PlayerController!");
            return;
        }
        if (playerCollider == null)
        {
            Debug.LogError("Player needs a Collider2D component!");
            return;
        }

        // Find the exit's Animator at setup
        GameObject exitObject = GameObject.FindWithTag("Exit");
        if (exitObject != null)
        {
            exitAnimator = exitObject.GetComponent<Animator>();
            if (exitAnimator == null)
            {
                Debug.LogError("Exit object is missing an Animator component!");
            }
        }
        else
        {
            Debug.LogError("No object with 'Exit' tag found in the scene!");
        }

        Debug.Log("PlayerInteraction setup complete");
    }

    void Update()
    {
        // Check enemy status continuously, but only update animation once
        if (!hasCheckedEnemies && exitAnimator != null)
        {
            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            if (enemies.Length == 0 || enemies.All(e => !e.gameObject.activeInHierarchy))
            {
                Debug.Log("All enemies are dead! Exit is now open.");
                exitAnimator.SetBool("Opened", true);
                hasCheckedEnemies = true; // Prevent repeated updates
            }
        }
    }

    public void HandleTileEffect()
    {
        if (playerHealth == null || playerMovement == null || playerAnimations == null)
        {
            Debug.LogError("PlayerInteraction not fully set up! Cannot handle tile effects.");
            return;
        }

        Vector2 pos = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, interactionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Exit"))
            {
                OnEscapeTile(pos);
            }
            else if (hit.CompareTag("Trap"))
            {
                OnTrapTile(pos);
            }
            else if (hit.CompareTag("Coin"))
            {
                OnCoinTile(pos);
                Destroy(hit.gameObject);
            }
            else if (hit.CompareTag("Potion"))
            {
                OnPotionTile(pos);
                Destroy(hit.gameObject);
            }
        }

        if (hits.Length == 0)
        {
            Debug.Log("No interactive objects detected at player position");
        }
    }

    #region Interaction Effects
    private void OnEscapeTile(Vector2 pos)
    {
        // Check if all enemies are dead when player reaches the exit
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        if (enemies.Length == 0 || enemies.All(e => !e.gameObject.activeInHierarchy))
        {
            Debug.Log("Player reached the open exit! Proceeding to next level...");
            StartCoroutine(LoadNextSceneAfterDelay(1f)); // Delay for animation or transition effect
        }
        else
        {
            Debug.Log("Cannot proceed: Some enemies are still alive!");
        }
    }

    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1); // Load next scene in build order
    }

    private void OnTrapTile(Vector2 pos)
    {
        Debug.Log("Player stepped on a trap!");
        playerHealth.TakeDamage(10);

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 jumpDirection = directions[Random.Range(0, directions.Length)];
        Vector2 newPos = (Vector2)transform.position + jumpDirection;

        if (playerMovement.IsTileWalkable(newPos))
        {
            playerMovement.Move(jumpDirection);
            playerAnimations.UpdateFacing(jumpDirection);
        }
        else
        {
            Debug.Log("Trap jump blocked! Trying opposite direction.");
            Vector2 oppositeDirection = -jumpDirection;
            Vector2 oppositePos = (Vector2)transform.position + oppositeDirection;

            if (playerMovement.IsTileWalkable(oppositePos))
            {
                transform.position = oppositePos;
            }
            else
            {
                Debug.Log("Opposite direction blocked! Player stays put.");
            }
        }
    }

    private void OnCoinTile(Vector2 pos)
    {
        Debug.Log("Player picked up a coin!");
        GetWealthy(50);
    }

    private void OnPotionTile(Vector2 pos)
    {
        Debug.Log("Player picked up a potion!");
        playerHealth.GainHealth(30);
    }
    #endregion

    #region Wealth Management
    private void GetWealthy(int amount)
    {
        wealth += amount;
        Debug.Log($"Player gained {amount} wealth! Total wealth: {wealth}");
    }

    public int Wealth => wealth;
    #endregion
}