using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransition
{
    public static bool IsLoading { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ResetLoadingFlag()
    {
        IsLoading = false;
    }

    internal static void BeginLoadingForTests()
    {
        IsLoading = true;
    }

    internal static void ResetLoadingFlagForTests()
    {
        IsLoading = false;
    }

    public static void Load(string sceneName)
    {
        if (IsLoading || string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        IsLoading = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
