using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public GameObject equipment;
    public GameObject inventory;
    private PlayerHealth playerHealth; // No need for public if we're finding it dynamically

    private void Start()
    {
        if (equipment != null) equipment.SetActive(false);
        if (inventory != null) inventory.SetActive(false);
    }

    private void OnDestroy()
    {
        // Unsubscribe if we found the player
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath -= RestartScene;
        }
    }

    void Update()
    {
        // Try to find PlayerHealth if we haven't yet
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                // Subscribe once we find it
                playerHealth.OnPlayerDeath += RestartScene;
                Debug.Log("GameHandler: Found and subscribed to PlayerHealth!");
            }
        }

        // Existing input handling
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.Die();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (equipment != null)
            {
                equipment.SetActive(!equipment.activeSelf);
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (inventory != null)
            {
                inventory.SetActive(!inventory.activeSelf);
            }
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CloseEquipment()
    {
        if (equipment != null)
        {
            equipment.SetActive(false);
        }
    }

    public void CloseInventory()
    {
        if (inventory != null)
        {
            inventory.SetActive(false);
        }
    }
}