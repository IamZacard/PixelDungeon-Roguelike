using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.Die();
            }
        }
    }
}