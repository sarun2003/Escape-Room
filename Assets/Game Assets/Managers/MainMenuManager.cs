using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main Menu Manager
/// Attach to a GameObject called "MenuManager" in your Menu scene.
/// Wire up the Play and Quit buttons in the Inspector via OnClick events.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Exact name of your game scene — must match the scene file name")]
    public string gameSceneName = "scene1";

    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Called by Play button's OnClick event
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Called by Quit button's OnClick event
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
