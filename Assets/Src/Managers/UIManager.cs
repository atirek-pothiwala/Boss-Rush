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

    private Color defaultStatusColor = Color.white;
    private float defaultStatusFontSize = 36f;
    private FontStyles defaultStatusFontStyle = FontStyles.Normal;
    private bool victorySaveCleared;

    private void Awake()
    {
        Instance = this;

        CacheStatusTextDefaults();
        EnsurePauseMenuVisible();
        PauseMenu.SetActive(false);
        AttributeMenu.SetActive(true);
        textLevelName.text = Constants.Instance.BossName();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Update()
    {
        // Do not gate on SceneTransition.IsLoading here — that flag must not block
        // pause/game-over UI during normal gameplay after a scene has loaded.

        var healthManager = HealthManager.Instance;
        var pauseManager = PauseManager.Instance;
        if (healthManager == null || pauseManager == null) return;

        var shouldShowOverlay = healthManager.IsGameOver || pauseManager.IsGamePaused;
        if (!shouldShowOverlay)
        {
            victorySaveCleared = false;
            if (PauseMenu.activeSelf)
            {
                PauseMenu.SetActive(false);
            }

            return;
        }

        EnsurePauseMenuVisible();
        if (!PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(true);
        }

        RefreshOverlay(healthManager, pauseManager);
    }

    private void RefreshOverlay(HealthManager healthManager, PauseManager pauseManager)
    {
        if (healthManager.IsHeroDead)
        {
            ResumeButton.SetActive(false);
            NextBossButton.SetActive(false);
            ApplyVictoryStyle(false);
            TextStatus.text = $"{Constants.Instance.SelectedHeroName()} was defeated by {Constants.Instance.BossName()}";
            return;
        }

        if (healthManager.IsBossDead)
        {
            ResumeButton.SetActive(false);

            var hasMoreBosses = Constants.Instance.HasMoreBosses;
            ApplyVictoryStyle(!hasMoreBosses);
            TextStatus.text = Constants.Instance.GetBossDefeatedStatusMessage();
            NextBossButton.SetActive(hasMoreBosses);

            if (!hasMoreBosses && !victorySaveCleared)
            {
                Constants.Instance.CompleteRun();
                victorySaveCleared = true;
            }

            return;
        }

        if (pauseManager.IsGamePaused)
        {
            ResumeButton.SetActive(true);
            NextBossButton.SetActive(false);
            ApplyVictoryStyle(false);
            TextStatus.text = "Paused";
        }
    }

    private void CacheStatusTextDefaults()
    {
        if (TextStatus == null) return;

        defaultStatusColor = TextStatus.color;
        defaultStatusFontSize = TextStatus.fontSize;
        defaultStatusFontStyle = TextStatus.fontStyle;
    }

    private void ApplyVictoryStyle(bool isVictory)
    {
        if (TextStatus == null) return;

        if (isVictory)
        {
            TextStatus.fontStyle = FontStyles.Bold;
            TextStatus.fontSize = 42f;
            TextStatus.color = new Color(1f, 0.85f, 0.2f);
            return;
        }

        TextStatus.fontStyle = defaultStatusFontStyle;
        TextStatus.fontSize = defaultStatusFontSize;
        TextStatus.color = defaultStatusColor;
    }

    private void EnsurePauseMenuVisible()
    {
        if (PauseMenu == null) return;

        var rect = PauseMenu.GetComponent<RectTransform>();
        if (rect != null && rect.localScale == Vector3.zero)
        {
            rect.localScale = Vector3.one;
        }
    }
}
