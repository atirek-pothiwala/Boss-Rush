using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchControlsHud : MonoBehaviour
{
    private bool moveLeft;
    private bool moveRight;
    private bool isRunning;

    public static bool ShouldShow()
    {
        if (Application.isMobilePlatform) return true;
        if (SystemInfo.deviceType == DeviceType.Handheld) return true;
        return Input.touchSupported && Screen.width < 1200;
    }

    void Awake()
    {
        if (!ShouldShow())
        {
            Destroy(gameObject);
            return;
        }

        BuildHud();
    }

    void Update()
    {
        if (!enabled) return;

        float horizontal = 0f;
        if (moveLeft) horizontal -= 1f;
        if (moveRight) horizontal += 1f;

        PlayerInputManager.SetTouchMovement(new Vector2(horizontal, 0f));
        PlayerInputManager.SetTouchRun(isRunning && horizontal != 0f);
    }

    void OnDisable()
    {
        PlayerInputManager.SetTouchMovement(Vector2.zero);
        PlayerInputManager.SetTouchRun(false);
    }

    private void BuildHud()
    {
        var canvasObject = new GameObject("TouchControlsCanvas", typeof(RectTransform));
        canvasObject.transform.SetParent(transform, false);

        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        var root = canvasObject.GetComponent<RectTransform>();

        CreateTouchButton(root, "MoveLeft", new Vector2(70, 70), new Vector2(90, 90), "◀", OnMoveLeftDown, OnMoveLeftUp);
        CreateTouchButton(root, "MoveRight", new Vector2(170, 70), new Vector2(90, 90), "▶", OnMoveRightDown, OnMoveRightUp);
        CreateTouchButton(root, "Run", new Vector2(120, 155), new Vector2(90, 44), "RUN", _ => isRunning = true, _ => isRunning = false);

        CreateTouchButton(root, "Jump", new Vector2(650, 95), new Vector2(80, 80), "JUMP", _ => PlayerInputManager.SetTouchJump(true), _ => PlayerInputManager.SetTouchJump(false));
        CreateTouchButton(root, "Quick", new Vector2(560, 170), new Vector2(72, 72), "Q", _ => PlayerInputManager.SetTouchQuickAttack(true), _ => PlayerInputManager.SetTouchQuickAttack(false));
        CreateTouchButton(root, "Heavy", new Vector2(650, 170), new Vector2(72, 72), "H", _ => PlayerInputManager.SetTouchHeavyAttack(true), _ => PlayerInputManager.SetTouchHeavyAttack(false));
        CreateTouchButton(root, "Special", new Vector2(740, 170), new Vector2(72, 72), "S", _ => PlayerInputManager.SetTouchSpecialAttack(true), _ => PlayerInputManager.SetTouchSpecialAttack(false));
        CreateTouchButton(root, "Shield", new Vector2(740, 95), new Vector2(72, 72), "SH", _ => PlayerInputManager.SetTouchShield(true), _ => PlayerInputManager.SetTouchShield(false));
    }

    private void OnMoveLeftDown(BaseEventData _) => moveLeft = true;
    private void OnMoveLeftUp(BaseEventData _) => moveLeft = false;
    private void OnMoveRightDown(BaseEventData _) => moveRight = true;
    private void OnMoveRightUp(BaseEventData _) => moveRight = false;

    private static void CreateTouchButton(
        RectTransform parent,
        string name,
        Vector2 anchoredPosition,
        Vector2 size,
        string label,
        UnityEngine.Events.UnityAction<BaseEventData> onDown,
        UnityEngine.Events.UnityAction<BaseEventData> onUp)
    {
        var buttonObject = new GameObject(name, typeof(RectTransform));
        buttonObject.transform.SetParent(parent, false);
        buttonObject.layer = LayerMask.NameToLayer("UI");

        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        var image = buttonObject.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.18f);
        image.raycastTarget = true;

        var textObject = new GameObject("Label", typeof(RectTransform));
        textObject.transform.SetParent(buttonObject.transform, false);
        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = label.Length > 2 ? 18 : 28;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(1f, 1f, 1f, 0.9f);
        text.raycastTarget = false;

        var trigger = buttonObject.AddComponent<EventTrigger>();
        AddTrigger(trigger, EventTriggerType.PointerDown, onDown);
        AddTrigger(trigger, EventTriggerType.PointerUp, onUp);
        AddTrigger(trigger, EventTriggerType.PointerExit, onUp);
    }

    private static void AddTrigger(
        EventTrigger trigger,
        EventTriggerType type,
        UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
}
