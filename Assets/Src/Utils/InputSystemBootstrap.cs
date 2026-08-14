using UnityEngine;
using UnityEngine.InputSystem;

public static class InputSystemBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Configure()
    {
        // Keep reading input while Time.timeScale is 0 (pause menu).
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }
}
