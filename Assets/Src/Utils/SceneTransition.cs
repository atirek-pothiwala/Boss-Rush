using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransition
{
    public static bool IsLoading { get; private set; }

    private static SceneTransitionRunner runner;

    public static void Load(string sceneName)
    {
        if (IsLoading || string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        EnsureRunner();
        runner.BeginLoad(sceneName);
    }

    internal static void SetLoading(bool value)
    {
        IsLoading = value;
    }

    private static void EnsureRunner()
    {
        if (runner != null)
        {
            return;
        }

        var go = new GameObject(nameof(SceneTransitionRunner));
        Object.DontDestroyOnLoad(go);
        runner = go.AddComponent<SceneTransitionRunner>();
    }
}

public class SceneTransitionRunner : MonoBehaviour
{
    private string pendingScene;

    public void BeginLoad(string sceneName)
    {
        pendingScene = sceneName;
        StopAllCoroutines();
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        if (string.IsNullOrEmpty(pendingScene))
        {
            yield break;
        }

        SceneTransition.SetLoading(true);
        Time.timeScale = 1f;
        PlayerInputManager.ClearAllEvents();

        // Defer past the UI/input callback so WebGL does not invoke destroyed delegates.
        yield return null;

        var scene = pendingScene;
        pendingScene = null;
        SceneManager.LoadScene(scene);
        SceneTransition.SetLoading(false);
    }
}
