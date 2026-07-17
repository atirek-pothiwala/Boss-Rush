using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public bool IsGamePaused { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerInputManager.OnPauseEvent += OnPause;
    }

    private void OnDisable()
    {
        PlayerInputManager.OnPauseEvent -= OnPause;
    }

    void OnPause(bool value)
    {
        if (IsGamePaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        SoundManager.Instance.PlayHardClick();
        
        IsGamePaused = true;
        Time.timeScale = 0;
    }

    public void NextBoss()
    {
        SoundManager.Instance.PlayHardClick();
        
        IsGamePaused = false;
        Time.timeScale = 1;        

        Constants.Instance.NextLevel();
        SceneManager.LoadScene("Fight Level");
    }

    public void ResumeGame()
    {
        SoundManager.Instance.PlayHardClick();

        IsGamePaused = false;
        Time.timeScale = 1;
    }

     public void RestartGame()
    {
        SoundManager.Instance.PlayHardClick();

        Time.timeScale = 1;
        SceneManager.LoadScene("Fight Level");
    }

    public void MainMenu()
    {
        SoundManager.Instance.PlayHardClick();
        SoundManager.Instance.PlayMenuMusic();

        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

}