using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [SerializeField] private GameObject[] environmentObjects;
    [SerializeField] private GameObject[] heroes;
    [SerializeField] private GameObject[] bosses;

    private void Awake()
    {
        Instance = this;

        LoadEnvironment();
        LoadFighters();
    }

    private void LoadFighters()
    {
        GameObject hero = Instantiate(heroes[0]);
        GameObject boss = Instantiate(bosses[0]);
        CameraManager.Instance.Initialize(hero.transform, boss.transform);
    }

    private void LoadEnvironment()
    {
        foreach (var item in environmentObjects)
        {
            Instantiate(item);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("Game Page");
    }

    public void MainMenu()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("Menu Page");
    }
}