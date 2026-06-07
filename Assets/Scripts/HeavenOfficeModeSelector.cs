using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

[DisallowMultipleComponent]
public class HeavenOfficeModeSelector : MonoBehaviour
{
    private Canvas canvas;

    private void Awake()
    {
        DisableSceneModeViews();
        EnsureEventSystem();
        BuildModeMenu();
    }

    private void BuildModeMenu()
    {
        canvas = new GameObject("HeavenOfficeModeCanvas").AddComponent<Canvas>();
        canvas.transform.SetParent(transform, false);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;

        canvas.gameObject.AddComponent<GraphicRaycaster>();

        Image background = canvas.gameObject.AddComponent<Image>();
        background.color = new Color(0.84f, 0.9f, 0.94f);

        RectTransform root = canvas.GetComponent<RectTransform>();
        RectTransform panel = Panel("ModePanel", root, new Color(0.96f, 0.94f, 0.84f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(640f, 390f));
        panel.gameObject.AddComponent<Outline>().effectColor = new Color(0.64f, 0.52f, 0.28f, 0.55f);

        TextMeshProUGUI title = Label("Heavenly Office", panel, 42, FontStyles.Bold, TextAlignmentOptions.Center, new Color(0.16f, 0.19f, 0.22f));
        Anchor(title.rectTransform, new Vector2(0f, 0.74f), new Vector2(1f, 0.94f), 30f, 0f);

        TextMeshProUGUI subtitle = Label("Choose mode / Выберите режим", panel, 24, FontStyles.Bold, TextAlignmentOptions.Center, new Color(0.36f, 0.3f, 0.18f));
        Anchor(subtitle.rectTransform, new Vector2(0f, 0.62f), new Vector2(1f, 0.74f), 36f, 0f);

        AddModeButton(panel, "Steam Demo", "New Divine Office flow", new Vector2(0f, 18f), new Color(0.22f, 0.52f, 0.74f), HeavenOfficeMode.DivineOfficeDemo);
        AddModeButton(panel, "Legacy Office Loop", "First playable iteration", new Vector2(0f, -86f), new Color(0.68f, 0.48f, 0.22f), HeavenOfficeMode.LegacyOfficeLoop);
    }

    private void AddModeButton(RectTransform parent, string title, string subtitle, Vector2 position, Color color, HeavenOfficeMode mode)
    {
        RectTransform buttonRect = Panel(title + "Button", parent, color, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(430f, 76f));
        buttonRect.anchoredPosition = position;

        Button button = buttonRect.gameObject.AddComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        button.onClick.AddListener(() => Launch(mode));

        TextMeshProUGUI label = Label(title + "\n<size=18>" + subtitle + "</size>", buttonRect, 24, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
        Stretch(label.rectTransform, 10f, 10f, 8f, 8f);
    }

    private void Launch(HeavenOfficeMode mode)
    {
        if (canvas != null)
        {
            Destroy(canvas.gameObject);
        }

        if (mode == HeavenOfficeMode.LegacyOfficeLoop)
        {
            GameObject game = new GameObject("LegacyOfficeLoop");
            game.AddComponent<HeavenOfficeAdapter>();
        }
        else
        {
            GameObject game = new GameObject("DivineOfficeDemo");
            game.AddComponent<HeavenOfficeView>();
            game.AddComponent<DivineOfficeFlowController>();
        }

        Destroy(gameObject);
    }

    private RectTransform Panel(string name, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = size;
        return rect;
    }

    private TextMeshProUGUI Label(string text, Transform parent, int size, FontStyles style, TextAlignmentOptions alignment, Color color)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        TextMeshProUGUI label = go.AddComponent<TextMeshProUGUI>();
        label.font = Resources.Load<TMP_FontAsset>("Shrifts/Rubik-Bold SDF") ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        label.text = text;
        label.fontSize = size;
        label.fontStyle = style;
        label.alignment = alignment;
        label.color = color;
        label.enableWordWrapping = true;
        label.raycastTarget = false;
        return label;
    }

    private void Anchor(RectTransform rect, Vector2 min, Vector2 max, float horizontalPadding, float verticalPadding)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = new Vector2(horizontalPadding, verticalPadding);
        rect.offsetMax = new Vector2(-horizontalPadding, -verticalPadding);
    }

    private void Stretch(RectTransform rect, float left, float right, float bottom, float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
        eventSystem.AddComponent<InputSystemUIInputModule>();
#else
        eventSystem.AddComponent<StandaloneInputModule>();
#endif
    }

    private void DisableSceneModeViews()
    {
        foreach (HeavenOfficeView existingView in FindObjectsOfType<HeavenOfficeView>(true))
        {
            existingView.gameObject.SetActive(false);
        }
    }
}
