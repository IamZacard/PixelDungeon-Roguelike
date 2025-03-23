using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    // Method for the "Play" button
    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    // Method for the "Play Hardcore" button (empty for now)
    public void PlayHardcore()
    {
        // Add hardcore mode logic here later
        Debug.Log("Play Hardcore button clicked - functionality to be implemented");
    }
}