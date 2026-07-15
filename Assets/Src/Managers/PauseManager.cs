using UnityEngine;

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
        UIManager.Instance.PauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        SoundManager.Instance.PlayHardClick();

        IsGamePaused = false;
        Time.timeScale = 1;
        UIManager.Instance.PauseMenu.SetActive(false);
    }
}