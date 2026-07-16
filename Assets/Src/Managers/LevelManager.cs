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
    }

    void Start()
    {
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
        SoundManager.Instance.PlayBattleMusic();
        foreach (var item in environmentObjects)
        {
            Instantiate(item);
        }
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