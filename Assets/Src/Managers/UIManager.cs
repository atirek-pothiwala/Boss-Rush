using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Menus")]
    public GameObject PauseMenu;
    public GameObject AttributeMenu;

    [Header("Buttons")]
    public GameObject ResumeButton;
    public GameObject NextBossButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI TextStatus;
    [SerializeField] private TextMeshProUGUI textLevelName;

    [Header("Health")]
    public Slider BossHealth;
    public Slider BossStamina;
    public Slider HeroHealth;
    public Slider HeroStamina;

    private void Awake()
    {
        Instance = this;

        PauseMenu.SetActive(false);
        AttributeMenu.SetActive(true);
        textLevelName.text = Constants.Instance.BossName();
    }

    void Update()
    {
        if(!HealthManager.Instance.IsGameOver && !PauseManager.Instance.IsGamePaused)
        {
            if(!PauseMenu.activeSelf) return;
            PauseMenu.SetActive(false);
            return;
        }

        if(PauseMenu.activeSelf) return;
        PauseMenu.SetActive(true);
        if(PauseManager.Instance.IsGamePaused)
        {
            ResumeButton.SetActive(true);
            TextStatus.text = "Paused";
            NextBossButton.SetActive(false);
        }
        else if (HealthManager.Instance.IsHeroDead)
        {
            ResumeButton.SetActive(false);
            TextStatus.text = $"{Constants.Instance.SelectedHeroName()} was defeated by {Constants.Instance.BossName()}";
            NextBossButton.SetActive(false);
        } 
        else if (HealthManager.Instance.IsBossDead)
        {
            ResumeButton.SetActive(false);
            if (Constants.Instance.IsNextLevel)
            {
                TextStatus.text = $"{Constants.Instance.BossName()} defeated!\nNext boss: {Constants.Instance.NextBossName()}";
            }
            else
            {
                TextStatus.text = $"Victory!\n{Constants.Instance.SelectedHeroName()} conquered the Boss Rush!";
            }
            NextBossButton.SetActive(Constants.Instance.IsNextLevel);
        }
    }
}