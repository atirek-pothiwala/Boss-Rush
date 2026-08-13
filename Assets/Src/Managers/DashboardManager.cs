using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DashboardManager : MonoBehaviour
{
    [SerializeField] private GameObject[] environmentObjects;
    [SerializeField] private GameObject startButton;

    private GameObject heroSelectionPanel;
    private TMP_FontAsset menuFont;

    void Start()
    {
        LoadEnvironment();
        CacheMenuFont();
        BuildHeroSelectionPanel();
    }

    public void ShowHeroSelection()
    {
        SoundManager.Instance.PlayHardClick();
        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        heroSelectionPanel.SetActive(true);
    }

    public void SelectHero(int heroIndex)
    {
        SoundManager.Instance.PlayHardClick();
        Constants.Instance.SelectHero(heroIndex);
        Constants.Instance.ResetProgress();
        SceneManager.LoadScene("Fight Level");
    }

    public void BackToMainMenu()
    {
        SoundManager.Instance.PlayHardClick();
        heroSelectionPanel.SetActive(false);

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

    private void LoadEnvironment()
    {
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
        var panelRect = heroSelectionPanel.GetComponent<RectTransform>();
        Stretch(panelRect);

        var title = CreateLabel(heroSelectionPanel.transform, "Choose Your Hero", 42, new Vector2(0, 120));
        title.fontStyle = FontStyles.Bold;

        var buttonY = 35f;
        for (var i = 0; i < Constants.HeroNames.Length; i++)
        {
            var heroIndex = i;
            CreateHeroButton(
                heroSelectionPanel.transform,
                Constants.HeroNames[i],
                new Vector2(0, buttonY - i * 70),
                () => SelectHero(heroIndex));
        }

        CreateHeroButton(
            heroSelectionPanel.transform,
            "Back",
            new Vector2(0, -170),
            BackToMainMenu);

        heroSelectionPanel.SetActive(false);
    }

    private void CreateHeroButton(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        var buttonObject = CreateUiObject("Btn" + label, parent);
        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(220, 42);
        rect.anchoredPosition = position;

        var image = buttonObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = true;

        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        var textObject = CreateUiObject("Text", buttonObject.transform);
        var textRect = textObject.GetComponent<RectTransform>();
        Stretch(textRect);

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
