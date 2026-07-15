using UnityEngine;
using UnityEngine.SceneManagement;

public class DashboardManager : MonoBehaviour
{
    [SerializeField] private GameObject[] environmentObjects;

    void Start()
    {
        LoadEnvironment();
    }

    public void Navigate(string name)
    {
        SoundManager.Instance.PlayHardClick();
        if (name.Equals("Exit"))
        {
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        } 
        else
        {
            SceneManager.LoadScene(name);   
        }
    }

    private void LoadEnvironment()
    {
        SoundManager.Instance.PlayMenuMusic();
        foreach (var item in environmentObjects)
        {
            Instantiate(item);
        }
    }
}
