using UnityEngine;
using UnityEngine.UI;

public class PauseMenu: MonoBehaviour
{
    public GameObject PausePanel;
    public bool isPaused = false;

    public void Start()
    {
        PausePanel.SetActive(false);
        isPaused = false;
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Exit Game");
    }
    public void Settings()
    {
        
        Debug.Log("Settings clicked");
    }
}