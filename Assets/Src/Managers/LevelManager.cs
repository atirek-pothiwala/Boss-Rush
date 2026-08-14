using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [SerializeField] private GameObject[] environmentObjects;
    [SerializeField] private GameObject[] heroes;
    [SerializeField] private GameObject[] bosses;

    private void Awake()
    {
        Instance = this;

        if (GameSave.TryLoadRun(out var heroIndex, out var level))
        {
            Constants.Instance.LoadProgress(heroIndex, level);
        }
    }

    void Start()
    {
        LoadEnvironment();
        LoadFighters();
        Constants.Instance.PersistRun();
    }

    private void LoadFighters()
    {
        int heroIndex = Mathf.Clamp(Constants.Instance.SelectedHeroIndex, 0, heroes.Length - 1);
        GameObject hero = Instantiate(heroes[heroIndex]);
        GameObject boss = Instantiate(bosses[Constants.Instance.CurrentLevel]);

        var playerController = hero.GetComponent<PlayerController>();
        HeroStats.Apply(heroIndex, playerController, HealthManager.Instance);
        CameraManager.Instance.Initialize(hero.transform, boss.transform);
    }

    private void LoadEnvironment()
    {
        SoundManager.Instance.PlayBattleMusicForLevel(Constants.Instance.CurrentLevel);
        foreach (var item in environmentObjects)
        {
            Instantiate(item);
        }
    }
}
