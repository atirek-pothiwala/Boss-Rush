using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public bool IsGamePaused { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        PlayerInputManager.OnPauseEvent -= OnPause;

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnEnable()
    {
        PlayerInputManager.OnPauseEvent += OnPause;
    }

    private void OnDisable()
    {
        PlayerInputManager.OnPauseEvent -= OnPause;
    }

    void OnPause(bool isPressed)
    {
        if (!isPressed || SceneTransition.IsLoading) return;

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
        if (SceneTransition.IsLoading) return;

        SoundManager.Instance.PlayHardClick();
        
        IsGamePaused = false;

        Constants.Instance.NextLevel();
        SceneTransition.Load("Fight Level");
    }

    public void ResumeGame()
    {
        SoundManager.Instance.PlayHardClick();

        IsGamePaused = false;
        Time.timeScale = 1;
    }

     public void RestartGame()
    {
        if (SceneTransition.IsLoading) return;

        SoundManager.Instance.PlayHardClick();

        IsGamePaused = false;
        Constants.Instance.ResetProgress();
        SceneTransition.Load("Fight Level");
    }

    public void MainMenu()
    {
        if (SceneTransition.IsLoading) return;

        SoundManager.Instance.PlayHardClick();
        SoundManager.Instance.PlayMenuMusic();

        IsGamePaused = false;
        Time.timeScale = 1;

        if (HealthManager.Instance != null && HealthManager.Instance.IsHeroDead)
        {
            Constants.Instance.ResetProgress();
        }

        SceneTransition.Load("Main Menu");
    }

}
