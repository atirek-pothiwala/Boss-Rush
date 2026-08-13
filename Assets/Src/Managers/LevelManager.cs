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
    }

    void Start()
    {
        LoadEnvironment();
        LoadFighters();
    }

    private void LoadFighters()
    {
        int heroIndex = Mathf.Clamp(Constants.Instance.SelectedHeroIndex, 0, heroes.Length - 1);
        GameObject hero = Instantiate(heroes[heroIndex]);
        GameObject boss = Instantiate(bosses[Constants.Instance.CurrentLevel]);
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
}