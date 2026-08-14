using System.Runtime.InteropServices;
using UnityEngine;

public static class WebGLHelper
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CloseBrowserTab();
#endif

    public static void CloseTab()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        CloseBrowserTab();
#else
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#endif
    }
}
