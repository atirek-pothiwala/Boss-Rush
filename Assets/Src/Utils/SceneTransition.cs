using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransition
{
    public static bool IsLoading { get; private set; }

    public static void Load(string sceneName)
    {
        if (IsLoading || string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        IsLoading = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        // Synchronous load: reset immediately so gameplay UI (pause menu) is not blocked.
        IsLoading = false;
    }
}
