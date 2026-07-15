using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Menus")]
    public GameObject PauseMenu;
    public GameObject AttributeMenu;
    public GameObject ResumeButton;

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
    }

    void Update()
    {
        if (HealthManager.Instance.IsGameOver)
        {
            PauseMenu.SetActive(true);
            ResumeButton.SetActive(false);
            return;
        }
    }
}