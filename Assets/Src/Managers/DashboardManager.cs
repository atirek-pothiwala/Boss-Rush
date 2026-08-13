using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DashboardManager : MonoBehaviour
{
    [SerializeField] private GameObject[] environmentObjects;
    [SerializeField] private GameObject startButton;
    [SerializeField] private Sprite[] heroPortraits;

    private GameObject heroSelectionPanel;
    private GameObject settingsPanel;
    private GameObject controlsPanel;
    private TMP_FontAsset menuFont;
    private Slider musicSlider;
    private Slider sfxSlider;

    void Start()
    {
        LoadEnvironment();
        CacheMenuFont();
        BuildHeroSelectionPanel();
        BuildSettingsPanel();
        BuildControlsPanel();
        BuildMainMenuButtons();
        TryResumeSavedRun();
    }

    public void ShowHeroSelection()
    {
        SoundManager.Instance.PlayNormalClick();
        HideMenuPanels();
        heroSelectionPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        SoundManager.Instance.PlaySoftClick();
        HideMenuPanels();
        settingsPanel.SetActive(true);
    }

    public void ShowControls()
    {
        SoundManager.Instance.PlaySoftClick();
        HideMenuPanels();
        controlsPanel.SetActive(true);
    }

    public void SelectHero(int heroIndex)
    {
        SoundManager.Instance.PlayHardClick();
        Constants.Instance.SelectHero(heroIndex);
        Constants.Instance.ResetProgress();
        Constants.Instance.PersistRun();
        SceneManager.LoadScene("Fight Level");
    }

    public void ContinueSavedRun()
    {
        if (!GameSave.TryLoadRun(out var heroIndex, out var level)) return;

        SoundManager.Instance.PlayHardClick();
        Constants.Instance.LoadProgress(heroIndex, level);
        SceneManager.LoadScene("Fight Level");
    }

    public void BackToMainMenu()
    {
        SoundManager.Instance.PlaySoftClick();
        HideMenuPanels();
        if (startButton != null)
        {
            startButton.SetActive(true);
        }
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

    private void BuildMainMenuButtons()
    {
        var parent = startButton != null ? startButton.transform.parent : transform;
        CreateTextButton(parent, "Settings", new Vector2(0, -110), new Vector2(180, 36), ShowSettings);
        CreateTextButton(parent, "Controls", new Vector2(0, -155), new Vector2(180, 36), ShowControls);
        CreateTextButton(parent, "Exit", new Vector2(0, -200), new Vector2(180, 36), () => Navigate("Exit"));
    }

    private void TryResumeSavedRun()
    {
        if (!GameSave.TryLoadRun(out _, out var level) || level <= 0) return;
        CreateTextButton(
            startButton != null ? startButton.transform.parent : transform,
            "Continue",
            new Vector2(0, -70),
            new Vector2(180, 36),
            ContinueSavedRun);
    }

    private void HideMenuPanels()
    {
        if (startButton != null) startButton.SetActive(false);
        if (heroSelectionPanel != null) heroSelectionPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    private void LoadEnvironment()
    {
        SoundManager.Instance.ApplySavedVolumes();
        SoundManager.Instance.PlayMenuMusic();
        foreach (var item in environmentObjects)
        {
            Instantiate(item);
        }
    }

    private void CacheMenuFont()
    {
        var existingText = FindFirstObjectByType<TextMeshProUGUI>();
        if (existingText != null)
        {
            menuFont = existingText.font;
        }
    }

    private void BuildHeroSelectionPanel()
    {
        var canvas = GetComponent<RectTransform>();
        heroSelectionPanel = CreateUiObject("HeroSelectionPanel", canvas);
        Stretch(heroSelectionPanel.GetComponent<RectTransform>());

        var title = CreateLabel(heroSelectionPanel.transform, "Choose Your Hero", 42, new Vector2(0, 150));
        title.fontStyle = FontStyles.Bold;

        var cardPositions = new[] { new Vector2(-170, 10), new Vector2(0, 10), new Vector2(170, 10) };
        for (var i = 0; i < Constants.HeroNames.Length; i++)
        {
            var heroIndex = i;
            var portrait = heroPortraits != null && i < heroPortraits.Length ? heroPortraits[i] : null;
            CreateHeroCard(
                heroSelectionPanel.transform,
                Constants.HeroNames[i],
                portrait,
                cardPositions[i],
                () => SelectHero(heroIndex));
        }

        CreateTextButton(heroSelectionPanel.transform, "Back", new Vector2(0, -170), new Vector2(220, 42), BackToMainMenu);
        heroSelectionPanel.SetActive(false);
    }

    private void BuildSettingsPanel()
    {
        var canvas = GetComponent<RectTransform>();
        settingsPanel = CreateUiObject("SettingsPanel", canvas);
        Stretch(settingsPanel.GetComponent<RectTransform>());

        CreateLabel(settingsPanel.transform, "Settings", 42, new Vector2(0, 150)).fontStyle = FontStyles.Bold;
        musicSlider = CreateSlider(settingsPanel.transform, "Music Volume", new Vector2(0, 40), GameSave.MusicVolume, value =>
        {
            GameSave.MusicVolume = value;
            SoundManager.Instance.SetMusicVolume(value);
        });
        sfxSlider = CreateSlider(settingsPanel.transform, "SFX Volume", new Vector2(0, -30), GameSave.SfxVolume, value =>
        {
            GameSave.SfxVolume = value;
            SoundManager.Instance.SetSfxVolume(value);
        });

        CreateTextButton(settingsPanel.transform, "Back", new Vector2(0, -150), new Vector2(220, 42), BackToMainMenu);
        settingsPanel.SetActive(false);
    }

    private void BuildControlsPanel()
    {
        var canvas = GetComponent<RectTransform>();
        controlsPanel = CreateUiObject("ControlsPanel", canvas);
        Stretch(controlsPanel.GetComponent<RectTransform>());

        CreateLabel(controlsPanel.transform, "Controls", 42, new Vector2(0, 170)).fontStyle = FontStyles.Bold;
        CreateLabel(controlsPanel.transform,
            "Move: WASD / Arrow Keys / Left Stick\n" +
            "Run: Shift / RB\n" +
            "Jump: Space / A\n" +
            "Quick Attack: LMB / Y\n" +
            "Power Attack: RMB / X\n" +
            "Special Attack: E / B\n" +
            "Shield: Q / LB\n" +
            "Pause: Escape / Start",
            22, new Vector2(0, 10));

        CreateTextButton(controlsPanel.transform, "Back", new Vector2(0, -170), new Vector2(220, 42), BackToMainMenu);
        controlsPanel.SetActive(false);
    }

    private Slider CreateSlider(Transform parent, string label, Vector2 position, float initialValue, UnityEngine.Events.UnityAction<float> onChanged)
    {
        var row = CreateUiObject("SliderRow", parent);
        var rect = row.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(360, 50);
        rect.anchoredPosition = position;

        var text = CreateLabel(row.transform, label, 22, new Vector2(-90, 0));
        text.alignment = TextAlignmentOptions.Left;
        text.rectTransform.sizeDelta = new Vector2(180, 40);

        var sliderObject = CreateUiObject("Slider", row.transform);
        var sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.sizeDelta = new Vector2(180, 24);
        sliderRect.anchoredPosition = new Vector2(80, 0);

        var background = sliderObject.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.35f);

        var fillArea = CreateUiObject("Fill Area", sliderObject.transform);
        Stretch(fillArea.GetComponent<RectTransform>());

        var fill = CreateUiObject("Fill", fillArea.transform);
        var fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.8f, 0.8f, 0.9f, 0.9f);
        Stretch(fill.GetComponent<RectTransform>());

        var handleArea = CreateUiObject("Handle Slide Area", sliderObject.transform);
        Stretch(handleArea.GetComponent<RectTransform>());

        var handle = CreateUiObject("Handle", handleArea.transform);
        var handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(18, 18);

        var slider = sliderObject.AddComponent<Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = initialValue;
        slider.onValueChanged.AddListener(onChanged);
        return slider;
    }

    private void CreateHeroCard(
        Transform parent,
        string label,
        Sprite portrait,
        Vector2 position,
        UnityEngine.Events.UnityAction onClick)
    {
        var cardObject = CreateUiObject("Card" + label, parent);
        var rect = cardObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(130, 155);
        rect.anchoredPosition = position;

        var background = cardObject.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.35f);
        background.raycastTarget = true;

        var button = cardObject.AddComponent<Button>();
        button.targetGraphic = background;
        button.onClick.AddListener(onClick);

        var portraitObject = CreateUiObject("Portrait", cardObject.transform);
        var portraitRect = portraitObject.GetComponent<RectTransform>();
        portraitRect.anchorMin = new Vector2(0.5f, 1f);
        portraitRect.anchorMax = new Vector2(0.5f, 1f);
        portraitRect.pivot = new Vector2(0.5f, 1f);
        portraitRect.sizeDelta = new Vector2(96, 96);
        portraitRect.anchoredPosition = new Vector2(0, -12);

        var portraitImage = portraitObject.AddComponent<Image>();
        portraitImage.sprite = portrait;
        portraitImage.preserveAspect = true;
        portraitImage.raycastTarget = false;

        var labelObject = CreateUiObject("Label", cardObject.transform);
        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(1f, 0f);
        labelRect.pivot = new Vector2(0.5f, 0f);
        labelRect.sizeDelta = new Vector2(0, 36);
        labelRect.anchoredPosition = new Vector2(0, 8);

        var text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.font = menuFont;
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
    }

    private void CreateTextButton(
        Transform parent,
        string label,
        Vector2 position,
        Vector2 size,
        UnityEngine.Events.UnityAction onClick)
    {
        var buttonObject = CreateUiObject("Btn" + label, parent);
        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var image = buttonObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = true;

        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        var textObject = CreateUiObject("Text", buttonObject.transform);
        Stretch(textObject.GetComponent<RectTransform>());

        var text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.font = menuFont;
        text.fontSize = 30;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
    }

    private TextMeshProUGUI CreateLabel(Transform parent, string textValue, float fontSize, Vector2 position)
    {
        var labelObject = CreateUiObject("Label", parent);
        var rect = labelObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(500, 60);
        rect.anchoredPosition = position;

        var text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = textValue;
        text.font = menuFont;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        return text;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        var uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.transform.SetParent(parent, false);
        uiObject.layer = LayerMask.NameToLayer("UI");
        return uiObject;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
