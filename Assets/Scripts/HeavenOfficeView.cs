using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Font = TMPro.TMP_FontAsset;
using Text = TMPro.TextMeshProUGUI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

public class HeavenOfficeView : MonoBehaviour
{
    private readonly Dictionary<StampType, Button> stampButtons = new Dictionary<StampType, Button>();
    private readonly Dictionary<StampType, Image> stampImages = new Dictionary<StampType, Image>();
    private readonly Dictionary<StampType, Text> stampTexts = new Dictionary<StampType, Text>();
    private readonly Dictionary<StampType, Color> stampColors = new Dictionary<StampType, Color>();
    private readonly Dictionary<StampType, Sprite> stampSprites = new Dictionary<StampType, Sprite>();
    private readonly List<Sprite> avatarSprites = new List<Sprite>();
    private Sprite paradiseDestinationSprite;
    private Sprite hellDestinationSprite;

    [SerializeField] private Font font;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text queueText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text mistakesText;
    [SerializeField] private Text comboText;
    [SerializeField] private Text tierText;
    [SerializeField] private Text soulText;
    [SerializeField] private Text soulQueueStripText;
    [SerializeField] private Text reactionText;
    [SerializeField] private Text documentText;
    [SerializeField] private Text stampMarkText;
    [SerializeField] private Text targetZoneText;
    [SerializeField] private Text reincarnationLeverText;
    [SerializeField] private Text mirrorTeaserText;
    [SerializeField] private Text scalesTeaserText;
    [SerializeField] private Text ruleHintText;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Text finalTitleText;
    [SerializeField] private Text finalStatsText;
    [SerializeField] private Text startTitleText;
    [SerializeField] private Text startSubtitleText;
    [SerializeField] private Text startButtonText;
    [SerializeField] private Text restartButtonText;
    [SerializeField] private GameObject startButtonObject;
    [SerializeField] private Image timerFill;
    [SerializeField] private Image soulCard;
    [SerializeField] private Image soulQueueStrip;
    [SerializeField] private Image documentCard;
    [SerializeField] private Image photoFrame;
    [SerializeField] private Image stampMarkPanel;
    [SerializeField] private Image stampMarkTopLine;
    [SerializeField] private Image stampMarkBottomLine;
    [SerializeField] private Image heldStampImage;
    [SerializeField] private Image leftTrayImage;
    [SerializeField] private Image rightTrayImage;
    [SerializeField] private RectTransform documentRect;
    [SerializeField] private RectTransform soulRect;
    [SerializeField] private RectTransform soulQueueStripRect;
    [SerializeField] private RectTransform reincarnationLeverRect;
    [SerializeField] private RectTransform stampMarkRect;
    [SerializeField] private RectTransform heldStampRect;
    [SerializeField] private RectTransform leftTrayRect;
    [SerializeField] private RectTransform rightTrayRect;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Text photoText;
    [SerializeField] private Text heldStampText;

    private Action<StampType> onStampSelected;
    private Action onStampTargetPressed;
    private Action<HeavenOfficeLanguage> onLanguageSelected;
    private Action onStart;
    private Action onRestart;
    private HeavenOfficeLanguage currentLanguage = HeavenOfficeLanguage.Russian;

    public void BuildIfNeeded(bool createUiAtRuntime)
    {
        if (!createUiAtRuntime)
        {
            return;
        }

        font = Resources.Load<Font>("Shrifts/Rubik-Bold SDF");
        if (font == null)
        {
            font = Resources.Load<Font>("Fonts & Materials/LiberationSans SDF");
        }

        LoadReferenceSprites();

        Canvas existingCanvas = GetComponentInChildren<Canvas>();
        if (existingCanvas != null && scoreText != null && documentRect != null)
        {
            return;
        }

        if (existingCanvas != null)
        {
            if (Application.isPlaying)
            {
                Destroy(existingCanvas.gameObject);
            }
            else
            {
                DestroyImmediate(existingCanvas.gameObject);
            }
        }

        EnsureEventSystem();

        Canvas canvas = CreateGameObject<Canvas>("HeavenOfficeCanvas", transform);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        RectTransform root = canvas.GetComponent<RectTransform>();
        Image background = canvas.gameObject.AddComponent<Image>();
        background.color = new Color(0.86f, 0.93f, 0.98f);

        RectTransform top = Panel("TopPanel", root, new Color(0.96f, 0.9f, 0.72f), Vector2.zero, Vector2.zero, Vector2.zero);
        top.anchorMin = new Vector2(0f, 1f);
        top.anchorMax = new Vector2(1f, 1f);
        top.offsetMin = new Vector2(0f, -84f);
        top.offsetMax = Vector2.zero;
        titleText = Label("Heaven Office / Божественная канцелярия", top, 25, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.22f, 0.18f, 0.12f));
        Stretch(titleText.rectTransform, 18, 0, 0, -36);

        scoreText = TopLabel("Счёт: 0", top, 20, 450);
        queueText = TopLabel("Очередь: 1/12", top, 20, 640);
        timerText = TopLabel("Таймер: 12.0", top, 20, 850);
        mistakesText = TopLabel("Ошибки: 0/3", top, 20, 1060);
        comboText = TopLabel("Серия: 0", top, 17, 450, -42);
        tierText = TopLabel("Tier 0", top, 17, 640, -42);

        RectTransform timerBar = Panel("TimerBar", top, new Color(0.72f, 0.77f, 0.84f), Vector2.zero, new Vector2(1f, 0f), Vector2.zero);
        timerBar.offsetMin = new Vector2(12f, 9f);
        timerBar.offsetMax = new Vector2(-12f, 25f);
        timerFill = Panel("TimerFill", timerBar, new Color(0.2f, 0.58f, 0.82f), new Vector2(0f, 0f), new Vector2(0f, 1f), Vector2.zero).GetComponent<Image>();
        timerFill.rectTransform.pivot = new Vector2(0f, 0.5f);
        Stretch(timerFill.rectTransform, 0, 0, 0, 0);

        RectTransform bottom = Panel("BottomPanel", root, new Color(0.91f, 0.94f, 0.96f), Vector2.zero, Vector2.zero, Vector2.zero);
        bottom.anchorMin = Vector2.zero;
        bottom.anchorMax = new Vector2(1f, 0f);
        bottom.offsetMin = Vector2.zero;
        bottom.offsetMax = new Vector2(0f, 92f);
        ruleHintText = Label("Подсказка правила: ", bottom, 20, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.16f, 0.22f, 0.28f));
        Stretch(ruleHintText.rectTransform, 18, 18, 42, -4);
        feedbackText = Label("Последнее решение: ", bottom, 20, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.28f, 0.28f, 0.32f));
        Stretch(feedbackText.rectTransform, 18, 18, 2, -46);
        CenterBottomHints();

        RectTransform left = Panel("LeftStampPanel", root, new Color(0.78f, 0.87f, 0.94f), Vector2.zero, Vector2.zero, Vector2.zero);
        left.anchorMin = Vector2.zero;
        left.anchorMax = new Vector2(0f, 1f);
        left.offsetMin = new Vector2(0f, 92f);
        left.offsetMax = new Vector2(230f, -84f);
        RectTransform right = Panel("RightStampPanel", root, new Color(0.78f, 0.87f, 0.94f), Vector2.one, Vector2.one, Vector2.zero);
        right.anchorMin = new Vector2(1f, 0f);
        right.anchorMax = Vector2.one;
        right.offsetMin = new Vector2(-230f, 92f);
        right.offsetMax = new Vector2(0f, -84f);
        RectTransform center = Panel("CentralWorkZone", root, new Color(0.93f, 0.96f, 0.98f), Vector2.zero, Vector2.one, Vector2.zero);
        center.offsetMin = new Vector2(230f, 92f);
        center.offsetMax = new Vector2(-230f, -84f);

        BuildOfficeDesk(center);

        AddStampButton(left, StampType.Heaven, "Рай", "HEAVEN", new Color(0.22f, 0.68f, 0.35f), -92, 0f);
        AddStampButton(left, StampType.Appeal, "Апелляция", "APPEAL", new Color(0.94f, 0.68f, 0.16f), -248, 0f);
        AddStampButton(right, StampType.Hell, "Ад", "HELL", new Color(0.78f, 0.18f, 0.14f), -92, 0f);
        AddStampButton(right, StampType.Audit, "Проверка", "AUDIT", new Color(0.28f, 0.48f, 0.68f), -248, 0f);

        soulQueueStrip = Panel("SoulQueueStrip", center, new Color(0.16f, 0.22f, 0.29f, 0.88f), new Vector2(0.12f, 1f), new Vector2(0.88f, 1f), new Vector2(0f, 46f)).GetComponent<Image>();
        soulQueueStripRect = soulQueueStrip.rectTransform;
        soulQueueStripRect.pivot = new Vector2(0.5f, 1f);
        soulQueueStripRect.anchoredPosition = new Vector2(0f, -10f);
        soulQueueStrip.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 0.3f);
        soulQueueStripText = Label("SOUL QUEUE  |  waiting for first case", soulQueueStripRect, 17, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(soulQueueStripText.rectTransform, 12, 12, 4, -4);

        soulCard = Panel("SoulWindow", center, new Color(0.72f, 0.84f, 0.98f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280f, 84f)).GetComponent<Image>();
        soulRect = soulCard.rectTransform;
        soulRect.anchoredPosition = new Vector2(0f, -92f);
        soulCard.gameObject.AddComponent<Outline>().effectColor = new Color(0.2f, 0.38f, 0.58f, 0.55f);
        soulText = Label("Душа", soulRect, 20, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(soulText.rectTransform, 8, 8, 8, 8);

        reactionText = Label("", center, 18, FontStyle.Italic, TextAnchor.MiddleCenter, new Color(0.18f, 0.25f, 0.32f));
        reactionText.rectTransform.anchorMin = new Vector2(0.18f, 0.68f);
        reactionText.rectTransform.anchorMax = new Vector2(0.82f, 0.77f);
        reactionText.rectTransform.offsetMin = Vector2.zero;
        reactionText.rectTransform.offsetMax = Vector2.zero;

        documentCard = Panel("SoulDossier", center, new Color(1f, 0.97f, 0.84f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(500f, 360f)).GetComponent<Image>();
        documentRect = documentCard.rectTransform;
        documentRect.anchoredPosition = new Vector2(0f, -50f);
        documentCard.gameObject.AddComponent<Outline>().effectColor = new Color(0.38f, 0.3f, 0.16f, 0.28f);
        Button documentButton = documentCard.gameObject.AddComponent<Button>();
        documentButton.transition = Selectable.Transition.ColorTint;
        documentButton.onClick.AddListener(() => onStampTargetPressed?.Invoke());

        documentText = Label("", documentRect, 17, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.18f, 0.16f, 0.13f));
        Stretch(documentText.rectTransform, 26, 142, 22, -24);

        photoFrame = Panel("SoulPhotoSlot", documentRect, new Color(0.9f, 0.88f, 0.76f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(104f, 124f)).GetComponent<Image>();
        photoFrame.rectTransform.anchoredPosition = new Vector2(-78f, -86f);
        photoFrame.gameObject.AddComponent<Outline>().effectColor = new Color(0.36f, 0.31f, 0.22f, 0.55f);
        photoText = Label("ФОТО", photoFrame.rectTransform, 18, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.42f, 0.39f, 0.33f));
        Stretch(photoText.rectTransform, 6, 6, 6, 6);

        stampMarkPanel = Panel("StampInkTrace", documentRect, new Color(0.2f, 0.5f, 0.26f, 0.14f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(184f, 88f)).GetComponent<Image>();
        stampMarkRect = stampMarkPanel.rectTransform;
        stampMarkRect.anchorMin = new Vector2(1f, 0f);
        stampMarkRect.anchorMax = new Vector2(1f, 0f);
        stampMarkRect.anchoredPosition = new Vector2(-118f, 74f);
        stampMarkRect.localEulerAngles = new Vector3(0f, 0f, -8f);
        stampMarkPanel.gameObject.AddComponent<Outline>().effectColor = new Color(0.2f, 0.5f, 0.26f, 0.68f);
        stampMarkTopLine = Panel("StampTraceLineTop", stampMarkRect, new Color(0.2f, 0.5f, 0.26f, 0.55f), new Vector2(0.08f, 0.7f), new Vector2(0.92f, 0.7f), new Vector2(0f, 3f)).GetComponent<Image>();
        stampMarkBottomLine = Panel("StampTraceLineBottom", stampMarkRect, new Color(0.2f, 0.5f, 0.26f, 0.55f), new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.28f), new Vector2(0f, 3f)).GetComponent<Image>();
        stampMarkText = Label("", stampMarkRect, 24, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.2f, 0.5f, 0.26f, 0.86f));
        Stretch(stampMarkText.rectTransform, 8, 8, 8, 8);
        stampMarkPanel.gameObject.SetActive(false);

        targetZoneText = Label("ЗОНА УДАРА ПЕЧАТЬЮ", center, 18, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.42f, 0.42f, 0.46f));
        targetZoneText.rectTransform.anchorMin = new Vector2(0.3f, 0.06f);
        targetZoneText.rectTransform.anchorMax = new Vector2(0.7f, 0.15f);
        targetZoneText.rectTransform.offsetMin = Vector2.zero;
        targetZoneText.rectTransform.offsetMax = Vector2.zero;
        Button targetButton = targetZoneText.gameObject.AddComponent<Button>();
        targetButton.onClick.AddListener(() => onStampTargetPressed?.Invoke());

        BuildReincarnationLever(center);
        BuildFullVersionTeasers(center);
        BuildDestinationTrays(center);
        BuildHeldStamp(root);
        BuildFinalPanel(root);
        BuildStartPanel(root);
        CompactTopPanel();
    }

    public void Bind(Action<StampType> stampSelected, Action stampTargetPressed, Action<HeavenOfficeLanguage> languageSelected, Action start, Action restart)
    {
        onStampSelected = stampSelected;
        onStampTargetPressed = stampTargetPressed;
        onLanguageSelected = languageSelected;
        onStart = start;
        onRestart = restart;
        foreach (var pair in stampButtons)
        {
            StampType stamp = pair.Key;
            pair.Value.onClick.RemoveAllListeners();
            pair.Value.onClick.AddListener(() => onStampSelected?.Invoke(stamp));
        }
    }

    public void ShowDocument(SoulDocumentData document, SoulDocumentGenerator generator, int current, int total, HeavenOfficeLanguage language)
    {
        currentLanguage = language;
        documentRect.anchoredPosition = new Vector2(0f, -50f);
        soulRect.anchoredPosition = new Vector2(0f, -92f);
        documentCard.color = new Color(1f, 0.97f, 0.84f);
        soulCard.color = new Color(0.72f, 0.84f, 0.98f);
        if (stampMarkPanel != null)
        {
            stampMarkPanel.gameObject.SetActive(false);
        }
        if (photoText != null)
        {
            Sprite avatar = GetAvatarFor(document.soulName);
            photoFrame.sprite = avatar;
            photoFrame.preserveAspect = true;
            photoFrame.color = avatar != null ? Color.white : new Color(0.9f, 0.88f, 0.76f);
            photoText.gameObject.SetActive(avatar == null);
            photoText.text = BuildPhotoInitials(document.soulName) + "\n\nФОТО";
        }
        HideHeldStamp();
        reactionText.text = language == HeavenOfficeLanguage.English ? "The soul awaits the office decision." : "Душа ожидает решения канцелярии.";
        targetZoneText.text = language == HeavenOfficeLanguage.English ? "STAMP TARGET ZONE" : "ЗОНА УДАРА ПЕЧАТЬЮ";
        if (soulQueueStripText != null)
        {
            soulQueueStripText.text = language == HeavenOfficeLanguage.English
                ? $"SOUL QUEUE  |  current {current}/{total}  |  next souls waiting"
                : $"ОЧЕРЕДЬ ДУШ  |  дело {current}/{total}  |  следующие души ждут";
        }

        if (reincarnationLeverText != null)
        {
            reincarnationLeverText.text = language == HeavenOfficeLanguage.English ? "REINCARNATE" : "РЕИНКАРНАЦИЯ";
        }

        if (mirrorTeaserText != null)
        {
            mirrorTeaserText.text = language == HeavenOfficeLanguage.English ? "MIRROR\nSOON" : "ЗЕРКАЛО\nПОЗЖЕ";
        }

        if (scalesTeaserText != null)
        {
            scalesTeaserText.text = language == HeavenOfficeLanguage.English ? "SCALES\nSOON" : "ВЕСЫ\nПОЗЖЕ";
        }

        string tags = document.tags.Count == 0
            ? (language == HeavenOfficeLanguage.English ? "no special notes" : "особых пометок нет")
            : string.Join("\n", document.tags.Select(tag => "• " + generator.GetTagDescription(tag, language)));

        soulText.text = language == HeavenOfficeLanguage.English ? $"{document.soulName}\ncase {current}/{total}" : $"{document.soulName}\nдело {current}/{total}";
        documentText.text = language == HeavenOfficeLanguage.English
            ? $"{DocTitle("SOUL DOCUMENT")}\n{DocHead("Name")}: {document.soulName}\n{DocHead("Age")}: {document.age}\n{DocHead("Life")}: {document.lifeSummary}\n\n{DocHead("Good acts")}:\n{BulletList(document.goodActs)}\n\n{DocHead("Bad acts")}:\n{BulletList(document.badActs)}\n\n{DocHead("Special notes")}:\n{tags}"
            : $"{DocTitle("ДОКУМЕНТ ДУШИ")}\n{DocHead("Имя")}: {document.soulName}\n{DocHead("Возраст")}: {document.age}\n{DocHead("Жизнь")}: {document.lifeSummary}\n\n{DocHead("Хорошие поступки")}:\n{BulletList(document.goodActs)}\n\n{DocHead("Плохие поступки")}:\n{BulletList(document.badActs)}\n\n{DocHead("Особые приметы")}:\n{tags}";
    }

    public void UpdateHud(int score, int current, int total, int mistakes, int maxMistakes, int combo, int tier, HeavenOfficeLanguage language)
    {
        currentLanguage = language;
        scoreText.text = language == HeavenOfficeLanguage.English ? $"Score: {score}" : $"Счёт: {score}";
        queueText.text = language == HeavenOfficeLanguage.English ? $"Queue: {current}/{total}" : $"Очередь: {current}/{total}";
        mistakesText.text = language == HeavenOfficeLanguage.English ? $"Errors: {mistakes}/{maxMistakes}" : $"Ошибки: {mistakes}/{maxMistakes}";
        comboText.text = language == HeavenOfficeLanguage.English ? $"Combo: {combo}" : $"Серия: {combo}";
        tierText.text = $"Tier {tier}";
        if (soulQueueStripText != null)
        {
            soulQueueStripText.text = language == HeavenOfficeLanguage.English
                ? $"SOUL QUEUE  |  current {current}/{total}  |  office pressure stable"
                : $"ОЧЕРЕДЬ ДУШ  |  дело {current}/{total}  |  канцелярия держит темп";
        }
    }

    public void UpdateTimer(float remaining, float limit)
    {
        float ratio = Mathf.Clamp01(remaining / Mathf.Max(0.01f, limit));
        timerText.text = currentLanguage == HeavenOfficeLanguage.English ? $"Timer: {Mathf.Max(0f, remaining):0.0}" : $"Таймер: {Mathf.Max(0f, remaining):0.0}";
        timerText.color = ratio <= 0.25f ? new Color(0.86f, 0.12f, 0.1f) : new Color(0.18f, 0.22f, 0.26f);
        timerFill.color = ratio <= 0.25f ? new Color(0.88f, 0.16f, 0.12f) : new Color(0.2f, 0.58f, 0.82f);
        timerFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
    }

    public void SetRuleHint(string hint)
    {
        ruleHintText.text = (currentLanguage == HeavenOfficeLanguage.English ? "Hint: " : "Подсказка: ") + hint;
    }

    public void SetFeedback(string feedback, Color color, HeavenOfficeLanguage language = HeavenOfficeLanguage.Russian)
    {
        currentLanguage = language;
        feedbackText.text = (language == HeavenOfficeLanguage.English ? "Decision: " : "Решение: ") + feedback;
        feedbackText.color = color;
    }

    public void SetStampAvailability(IReadOnlyCollection<StampType> available)
    {
        foreach (StampType stamp in stampButtons.Keys.ToList())
        {
            bool enabled = available.Contains(stamp);
            stampButtons[stamp].interactable = enabled;
            Color baseColor = stampColors[stamp];
            bool usesSprite = stampSprites.ContainsKey(stamp);
            stampImages[stamp].color = enabled
                ? (usesSprite ? Color.white : baseColor)
                : (usesSprite ? new Color(0.45f, 0.45f, 0.45f, 0.38f) : new Color(baseColor.r, baseColor.g, baseColor.b, 0.28f));
            stampTexts[stamp].color = enabled ? Color.white : new Color(1f, 1f, 1f, 0.45f);
        }
    }

    public void HighlightSelectedStamp(StampType? selected)
    {
        foreach (StampType stamp in stampButtons.Keys.ToList())
        {
            if (selected.HasValue && selected.Value == stamp)
            {
                stampImages[stamp].transform.localScale = Vector3.one * 1.08f;
                stampImages[stamp].GetComponent<Outline>().effectColor = Color.white;
                stampImages[stamp].GetComponent<Outline>().effectDistance = new Vector2(5f, -5f);
            }
            else
            {
                stampImages[stamp].transform.localScale = Vector3.one;
                stampImages[stamp].GetComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.28f);
                stampImages[stamp].GetComponent<Outline>().effectDistance = new Vector2(2f, -2f);
            }
        }
    }

    public IEnumerator PlayStampAnimation(StampType stamp, float holdTime)
    {
        Button button = stampButtons[stamp];
        RectTransform rect = button.GetComponent<RectTransform>();
        Vector3 startScale = rect.localScale;
        Color color = stampColors[stamp];

        for (float t = 0f; t < 0.16f; t += Time.deltaTime)
        {
            float pulse = Mathf.Lerp(1.08f, 0.86f, t / 0.16f);
            rect.localScale = Vector3.one * pulse;
            yield return null;
        }

        rect.localScale = startScale;
        HideHeldStamp();
        stampMarkPanel.gameObject.SetActive(true);
        stampMarkPanel.color = new Color(color.r, color.g, color.b, 0.12f);
        stampMarkPanel.GetComponent<Outline>().effectColor = new Color(color.r, color.g, color.b, 0.72f);
        stampMarkTopLine.color = new Color(color.r, color.g, color.b, 0.55f);
        stampMarkBottomLine.color = new Color(color.r, color.g, color.b, 0.55f);
        stampMarkText.text = StampMark(stamp);
        stampMarkText.color = new Color(color.r, color.g, color.b, 0.86f);
        stampMarkRect.localScale = Vector3.one * 1.35f;

        for (float t = 0f; t < 0.18f; t += Time.deltaTime)
        {
            stampMarkRect.localScale = Vector3.Lerp(Vector3.one * 1.35f, Vector3.one, t / 0.18f);
            yield return null;
        }

        stampMarkRect.localScale = Vector3.one;
        reactionText.text = ReactionFor(stamp);
        soulCard.color = Color.Lerp(new Color(0.72f, 0.84f, 0.98f), color, 0.35f);
        yield return new WaitForSeconds(holdTime);
    }

    public void ShowSpoiledStamp()
    {
        stampMarkText.text = "ПРОСРОЧЕНО";
        stampMarkText.color = new Color(0.45f, 0.32f, 0.25f, 0.95f);
        stampMarkPanel.gameObject.SetActive(true);
        stampMarkPanel.color = new Color(0.45f, 0.32f, 0.25f, 0.12f);
        stampMarkPanel.GetComponent<Outline>().effectColor = new Color(0.45f, 0.32f, 0.25f, 0.7f);
        stampMarkTopLine.color = new Color(0.45f, 0.32f, 0.25f, 0.55f);
        stampMarkBottomLine.color = new Color(0.45f, 0.32f, 0.25f, 0.55f);
        documentCard.color = new Color(0.78f, 0.74f, 0.66f);
        reactionText.text = currentLanguage == HeavenOfficeLanguage.English ? "The document gathers bureaucratic dust." : "Документ покрывается бюрократической пылью.";
    }

    public IEnumerator PlayExitAnimation(StampType? stamp, float duration)
    {
        Vector2 documentStart = documentRect.anchoredPosition;
        Vector2 soulStart = soulRect.anchoredPosition;
        Vector2 documentTarget = GetTrayTarget(stamp);
        Vector2 soulTarget = soulStart + (documentTarget - documentStart) * 0.45f;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            documentRect.anchoredPosition = Vector2.Lerp(documentStart, documentTarget, k);
            soulRect.anchoredPosition = Vector2.Lerp(soulStart, soulTarget, k);
            yield return null;
        }
    }

    public void ShowFinalPanel(string title, int score, int correct, int mistakes, int maxCombo, SessionEndReason reason, HeavenOfficeLanguage language)
    {
        finalPanel.SetActive(true);
        finalTitleText.text = title;
        finalStatsText.text = language == HeavenOfficeLanguage.English
            ? $"Final score: {score}\nCorrect decisions: {correct}\nErrors: {mistakes}\nBest combo: {maxCombo}\nReason: {SessionReasonLabel(reason, language)}"
            : $"Итоговый счёт: {score}\nВерных решений: {correct}\nОшибок: {mistakes}\nЛучшая серия: {maxCombo}\nПричина: {SessionReasonLabel(reason, language)}";
    }

    public void HideFinalPanel()
    {
        if (finalPanel != null)
        {
            finalPanel.SetActive(false);
        }
    }

    public void ShowStartMenu()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(true);
            if (startButtonObject != null)
            {
                startButtonObject.SetActive(false);
            }
        }
    }

    public void HideStartMenu()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }
    }

    public void ShowHeldStamp(StampType stamp)
    {
        if (heldStampRect == null) return;

        if (stampSprites.TryGetValue(stamp, out Sprite sprite))
        {
            heldStampImage.sprite = sprite;
            heldStampImage.preserveAspect = true;
            heldStampImage.color = Color.white;
            heldStampText.gameObject.SetActive(false);
        }
        else
        {
            heldStampImage.sprite = null;
            heldStampImage.color = stampColors[stamp];
            heldStampText.gameObject.SetActive(true);
        }
        heldStampText.text = StampMark(stamp);
        heldStampRect.gameObject.SetActive(true);
        UpdateHeldStampPosition();
    }

    public void HideHeldStamp()
    {
        if (heldStampRect != null)
        {
            heldStampRect.gameObject.SetActive(false);
        }
    }

    public void UpdateHeldStampPosition()
    {
        if (heldStampRect == null || !heldStampRect.gameObject.activeSelf) return;

        heldStampRect.position = GetPointerPosition() + new Vector2(28f, -24f);
    }

    public void ApplyLanguage(HeavenOfficeLanguage language)
    {
        currentLanguage = language;
        if (startSubtitleText != null)
        {
            startSubtitleText.text = language == HeavenOfficeLanguage.English ? "Heavenly Office" : "Божественная канцелярия";
        }

        if (startButtonText != null)
        {
            startButtonText.text = language == HeavenOfficeLanguage.English ? "START" : "СТАРТ";
        }

        if (restartButtonText != null)
        {
            restartButtonText.text = language == HeavenOfficeLanguage.English ? "Restart" : "Начать заново";
        }

        if (startButtonObject != null)
        {
            startButtonObject.SetActive(true);
        }
    }

    private void CompactTopPanel()
    {
        if (titleText != null)
        {
            titleText.gameObject.SetActive(false);
        }

        PlaceTopLabel(scoreText, 110f, -16f, 210f);
        PlaceTopLabel(queueText, 330f, -16f, 230f);
        PlaceTopLabel(timerText, 575f, -16f, 230f);
        PlaceTopLabel(mistakesText, 830f, -16f, 230f);
        PlaceTopLabel(comboText, 110f, -44f, 180f);
        PlaceTopLabel(tierText, 330f, -44f, 150f);
    }

    private void CenterBottomHints()
    {
        ConfigureBottomLabel(ruleHintText, 0.48f, 1f, true);
        ConfigureBottomLabel(feedbackText, 0f, 0.52f, false);
    }

    private void ConfigureBottomLabel(Text label, float yMin, float yMax, bool bold)
    {
        if (label == null) return;

        label.text = bold ? "Подсказка: " : "Решение: ";
        label.fontSize = 18;
        label.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        label.alignment = TextAlignmentOptions.Center;
        label.rectTransform.anchorMin = new Vector2(0.2f, yMin);
        label.rectTransform.anchorMax = new Vector2(0.8f, yMax);
        label.rectTransform.offsetMin = Vector2.zero;
        label.rectTransform.offsetMax = Vector2.zero;
    }

    private void PlaceTopLabel(Text label, float x, float y, float width)
    {
        if (label == null) return;

        label.rectTransform.sizeDelta = new Vector2(width, 30f);
        label.rectTransform.anchoredPosition = new Vector2(x, y);
    }

    private Vector2 GetPointerPosition()
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }

        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }

        return Input.mousePosition;
#endif
    }

    private Vector2 GetTrayTarget(StampType? stamp)
    {
        RectTransform tray = (stamp == StampType.Hell || stamp == StampType.Audit || !stamp.HasValue) ? rightTrayRect : leftTrayRect;
        RectTransform parent = documentRect != null ? documentRect.parent as RectTransform : null;
        if (tray != null && parent != null)
        {
            Vector3 local = parent.InverseTransformPoint(tray.position);
            return new Vector2(local.x, local.y);
        }

        if (stamp == StampType.Hell || stamp == StampType.Audit || !stamp.HasValue)
        {
            return rightTrayRect != null ? rightTrayRect.anchoredPosition : new Vector2(330f, -230f);
        }

        return leftTrayRect != null ? leftTrayRect.anchoredPosition : new Vector2(-330f, -230f);
    }

    private void LoadReferenceSprites()
    {
        avatarSprites.Clear();
        stampSprites.Clear();
        paradiseDestinationSprite = null;
        hellDestinationSprite = null;

        Texture2D avatarSheet = Resources.Load<Texture2D>("HeavenOffice/avatar_sheet");
        if (avatarSheet != null)
        {
            int columns = 5;
            int rows = 2;
            int cellWidth = avatarSheet.width / columns;
            int cellHeight = avatarSheet.height / rows;
            for (int row = rows - 1; row >= 0; row--)
            {
                for (int column = 0; column < columns; column++)
                {
                    Rect rect = new Rect(column * cellWidth + 8, row * cellHeight + 8, cellWidth - 16, cellHeight - 16);
                    avatarSprites.Add(Sprite.Create(avatarSheet, rect, new Vector2(0.5f, 0.5f), 100f));
                }
            }
        }

        Sprite[] slicedStampSprites = Resources.LoadAll<Sprite>("HeavenOffice/stamp_sheet");
        if (slicedStampSprites != null && slicedStampSprites.Length >= 4)
        {
            ApplySlicedStampSprites(slicedStampSprites);
        }
        else
        {
            Texture2D stampSheet = Resources.Load<Texture2D>("HeavenOffice/stamp_sheet");
            if (stampSheet != null)
            {
                int half = stampSheet.width / 2;
                int inset = 18;
                stampSprites[StampType.Hell] = Sprite.Create(stampSheet, new Rect(inset, half + inset, half - inset * 2, half - inset * 2), new Vector2(0.5f, 0.5f), 100f);
                stampSprites[StampType.Heaven] = Sprite.Create(stampSheet, new Rect(half + inset, half + inset, half - inset * 2, half - inset * 2), new Vector2(0.5f, 0.5f), 100f);
                stampSprites[StampType.Appeal] = Sprite.Create(stampSheet, new Rect(inset, inset, half - inset * 2, half - inset * 2), new Vector2(0.5f, 0.5f), 100f);
                stampSprites[StampType.Audit] = Sprite.Create(stampSheet, new Rect(half + inset, inset, half - inset * 2, half - inset * 2), new Vector2(0.5f, 0.5f), 100f);
            }
        }

        Texture2D destinationSheet = Resources.Load<Texture2D>("HeavenOffice/destination_sheet");
        if (destinationSheet != null)
        {
            int halfWidth = destinationSheet.width / 2;
            int inset = 18;
            hellDestinationSprite = Sprite.Create(destinationSheet, new Rect(inset, inset, halfWidth - inset * 2, destinationSheet.height - inset * 2), new Vector2(0.5f, 0.5f), 100f);
            paradiseDestinationSprite = Sprite.Create(destinationSheet, new Rect(halfWidth + inset, inset, halfWidth - inset * 2, destinationSheet.height - inset * 2), new Vector2(0.5f, 0.5f), 100f);
        }
    }

    private void ApplySlicedStampSprites(Sprite[] slicedSprites)
    {
        Sprite[] ordered = slicedSprites
            .OrderByDescending(sprite => sprite.rect.y)
            .ThenBy(sprite => sprite.rect.x)
            .ToArray();

        stampSprites[StampType.Hell] = FindStampSprite(ordered, "hell", 0);
        stampSprites[StampType.Heaven] = FindStampSprite(ordered, "heaven", 1);
        stampSprites[StampType.Appeal] = FindStampSprite(ordered, "apl", 2, "appeal");
        stampSprites[StampType.Audit] = FindStampSprite(ordered, "audit", 3);
    }

    private Sprite FindStampSprite(Sprite[] orderedSprites, string keyword, int fallbackIndex, string alternateKeyword = null)
    {
        Sprite found = orderedSprites.FirstOrDefault(sprite =>
            sprite.name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
            || (!string.IsNullOrEmpty(alternateKeyword) && sprite.name.IndexOf(alternateKeyword, StringComparison.OrdinalIgnoreCase) >= 0));

        if (found != null)
        {
            return found;
        }

        return orderedSprites[Mathf.Clamp(fallbackIndex, 0, orderedSprites.Length - 1)];
    }

    private void BuildOfficeDesk(RectTransform center)
    {
        RectTransform desk = Panel("MainOfficeDesk", center, new Color(0.62f, 0.48f, 0.32f, 0.38f), new Vector2(0f, 0f), new Vector2(1f, 0.58f), Vector2.zero);
        desk.offsetMin = new Vector2(18f, 12f);
        desk.offsetMax = new Vector2(-18f, 0f);

        RectTransform archive = Panel("ArchiveTeaser", center, new Color(0.38f, 0.34f, 0.28f, 0.5f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(104f, 50f));
        archive.pivot = new Vector2(0f, 1f);
        archive.anchoredPosition = new Vector2(18f, -68f);
        archive.gameObject.AddComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.12f);
        Text archiveText = Label("ARCHIVE", archive, 13, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 0.72f));
        Stretch(archiveText.rectTransform, 6, 6, 6, 6);

        RectTransform collection = Panel("CollectionTeaser", center, new Color(0.32f, 0.42f, 0.54f, 0.5f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(104f, 50f));
        collection.pivot = new Vector2(1f, 1f);
        collection.anchoredPosition = new Vector2(-18f, -68f);
        collection.gameObject.AddComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.12f);
        Text collectionText = Label("CARDS", collection, 13, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 0.72f));
        Stretch(collectionText.rectTransform, 6, 6, 6, 6);
    }

    private void BuildReincarnationLever(RectTransform center)
    {
        reincarnationLeverRect = Panel("ReincarnationLever", center, new Color(0.35f, 0.22f, 0.15f, 0.94f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(106f, 142f));
        reincarnationLeverRect.pivot = new Vector2(1f, 0f);
        reincarnationLeverRect.anchoredPosition = new Vector2(-34f, 102f);
        reincarnationLeverRect.gameObject.AddComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.35f);

        Button leverButton = reincarnationLeverRect.gameObject.AddComponent<Button>();
        leverButton.transition = Selectable.Transition.ColorTint;
        leverButton.onClick.AddListener(() => onStampTargetPressed?.Invoke());

        RectTransform handle = Panel("LeverHandle", reincarnationLeverRect, new Color(0.82f, 0.18f, 0.1f), new Vector2(0.5f, 0.58f), new Vector2(0.5f, 0.58f), new Vector2(24f, 78f));
        handle.localEulerAngles = new Vector3(0f, 0f, -16f);
        handle.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 0.24f);

        reincarnationLeverText = Label("LEVER", reincarnationLeverRect, 13, FontStyle.Bold, TextAnchor.LowerCenter, Color.white);
        Stretch(reincarnationLeverText.rectTransform, 6, 6, 8, -78);
    }

    private void BuildFullVersionTeasers(RectTransform center)
    {
        RectTransform mirror = Panel("MirrorFullVersionTeaser", center, new Color(0.5f, 0.72f, 0.82f, 0.28f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(90f, 74f));
        mirror.pivot = new Vector2(0f, 0f);
        mirror.anchoredPosition = new Vector2(34f, 104f);
        mirror.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 0.14f);
        mirrorTeaserText = Label("MIRROR", mirror, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 0.58f));
        Stretch(mirrorTeaserText.rectTransform, 6, 6, 6, 6);

        RectTransform scales = Panel("ScalesFullVersionTeaser", center, new Color(0.78f, 0.64f, 0.25f, 0.3f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(90f, 74f));
        scales.pivot = new Vector2(0f, 0f);
        scales.anchoredPosition = new Vector2(34f, 188f);
        scales.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 0.14f);
        scalesTeaserText = Label("SCALES", scales, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 0.58f));
        Stretch(scalesTeaserText.rectTransform, 6, 6, 6, 6);
    }

    private void BuildDestinationTrays(RectTransform center)
    {
        Canvas canvas = center.GetComponentInParent<Canvas>();
        Transform trayParent = canvas != null ? canvas.transform : center;
        leftTrayRect = Panel("LeftDestinationTray", trayParent, new Color(0.18f, 0.56f, 0.28f, 0.82f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(150f, 150f));
        leftTrayRect.pivot = new Vector2(0f, 0f);
        leftTrayRect.anchoredPosition = new Vector2(14f, 14f);
        leftTrayImage = leftTrayRect.GetComponent<Image>();
        if (paradiseDestinationSprite != null)
        {
            leftTrayImage.sprite = paradiseDestinationSprite;
            leftTrayImage.preserveAspect = true;
            leftTrayImage.color = Color.white;
        }
        leftTrayImage.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 0.55f);
        Text leftLabel = Label("PARADISE\nOUT", leftTrayRect, 15, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(leftLabel.rectTransform, 6, 6, 6, 6);
        leftLabel.gameObject.SetActive(paradiseDestinationSprite == null);

        rightTrayRect = Panel("RightDestinationTray", trayParent, new Color(0.78f, 0.1f, 0.08f, 0.82f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(150f, 150f));
        rightTrayRect.pivot = new Vector2(1f, 0f);
        rightTrayRect.anchoredPosition = new Vector2(-14f, 14f);
        rightTrayImage = rightTrayRect.GetComponent<Image>();
        if (hellDestinationSprite != null)
        {
            rightTrayImage.sprite = hellDestinationSprite;
            rightTrayImage.preserveAspect = true;
            rightTrayImage.color = Color.white;
        }
        rightTrayImage.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 0.55f);
        Text rightLabel = Label("HELL\nOUT", rightTrayRect, 15, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(rightLabel.rectTransform, 6, 6, 6, 6);
        rightLabel.gameObject.SetActive(hellDestinationSprite == null);
    }

    private void BuildHeldStamp(RectTransform root)
    {
        heldStampRect = Panel("HeldStampCursor", root, new Color(0.22f, 0.68f, 0.35f, 0.9f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(92f, 74f));
        heldStampImage = heldStampRect.GetComponent<Image>();
        heldStampImage.raycastTarget = false;
        heldStampImage.gameObject.AddComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.35f);
        heldStampText = Label("STAMP", heldStampRect, 15, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        heldStampText.raycastTarget = false;
        Stretch(heldStampText.rectTransform, 6, 6, 6, 6);
        heldStampRect.gameObject.SetActive(false);
    }

    private void BuildStartPanel(RectTransform root)
    {
        startPanel = Panel("StartPanel", root, new Color(0.86f, 0.93f, 0.98f, 0.98f), Vector2.zero, Vector2.one, Vector2.zero).gameObject;
        Stretch(startPanel.GetComponent<RectTransform>(), 0, 0, 0, 0);

        RectTransform card = Panel("StartCard", startPanel.transform, new Color(0.97f, 0.95f, 0.84f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(500f, 330f));
        card.gameObject.AddComponent<Outline>().effectColor = new Color(0.74f, 0.58f, 0.25f, 0.55f);

        startTitleText = Label("HEAVEN OFFICE", card, 36, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.18f, 0.2f, 0.24f));
        startTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
        startTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
        startTitleText.rectTransform.sizeDelta = new Vector2(0f, 78f);
        startTitleText.rectTransform.anchoredPosition = new Vector2(0f, -70f);

        startSubtitleText = Label("Choose language / Выберите язык", card, 24, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.36f, 0.28f, 0.14f));
        startSubtitleText.rectTransform.anchorMin = new Vector2(0f, 0.64f);
        startSubtitleText.rectTransform.anchorMax = new Vector2(1f, 0.78f);
        startSubtitleText.rectTransform.offsetMin = new Vector2(24f, 0f);
        startSubtitleText.rectTransform.offsetMax = new Vector2(-24f, 0f);

        RectTransform englishButtonRect = Panel("EnglishButton", card, new Color(0.22f, 0.52f, 0.74f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(170f, 52f));
        englishButtonRect.anchoredPosition = new Vector2(-95f, 0f);
        Button englishButton = englishButtonRect.gameObject.AddComponent<Button>();
        englishButton.onClick.AddListener(() => onLanguageSelected?.Invoke(HeavenOfficeLanguage.English));
        Text englishText = Label("English", englishButtonRect, 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(englishText.rectTransform, 6, 6, 6, 6);

        RectTransform russianButtonRect = Panel("RussianButton", card, new Color(0.74f, 0.52f, 0.22f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(170f, 52f));
        russianButtonRect.anchoredPosition = new Vector2(95f, 0f);
        Button russianButton = russianButtonRect.gameObject.AddComponent<Button>();
        russianButton.onClick.AddListener(() => onLanguageSelected?.Invoke(HeavenOfficeLanguage.Russian));
        Text russianText = Label("Русский", russianButtonRect, 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(russianText.rectTransform, 6, 6, 6, 6);

        RectTransform startButtonRect = Panel("StartButton", card, new Color(0.22f, 0.52f, 0.74f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(220f, 64f));
        startButtonRect.anchoredPosition = new Vector2(0f, 72f);
        startButtonObject = startButtonRect.gameObject;
        Button startButton = startButtonRect.gameObject.AddComponent<Button>();
        startButton.onClick.AddListener(() => onStart?.Invoke());
        startButtonText = Label("START", startButtonRect, 26, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(startButtonText.rectTransform, 6, 6, 6, 6);
        startButtonObject.SetActive(false);
    }

    private void AddStampButton(RectTransform parent, StampType stamp, string label, string mark, Color color, float y, float x)
    {
        RectTransform rect = Panel(stamp + "Stamp", parent, color, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(132f, 132f));
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x, y);
        Image image = rect.GetComponent<Image>();
        bool hasStampSprite = stampSprites.TryGetValue(stamp, out Sprite stampSprite);
        if (hasStampSprite)
        {
            image.sprite = stampSprite;
            image.preserveAspect = true;
            image.color = Color.white;
            rect.sizeDelta = new Vector2(132f, 132f);
        }
        image.gameObject.AddComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.28f);
        Button button = rect.gameObject.AddComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;
        Text text = Label($"Печать\n«{label}»\n{mark}", rect, 19, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(text.rectTransform, 8, 8, 8, 8);

        text.gameObject.SetActive(!hasStampSprite);
        stampButtons[stamp] = button;
        stampImages[stamp] = image;
        stampTexts[stamp] = text;
        stampColors[stamp] = color;
    }

    private void BuildFinalPanel(RectTransform root)
    {
        finalPanel = Panel("FinalPanel", root, new Color(0.12f, 0.16f, 0.2f, 0.88f), new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero).gameObject;
        Stretch(finalPanel.GetComponent<RectTransform>(), 0, 0, 0, 0);
        RectTransform card = Panel("FinalCard", finalPanel.transform, new Color(0.97f, 0.95f, 0.88f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(450f, 330f));
        finalTitleText = Label("Смена завершена", card, 30, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.18f, 0.16f, 0.13f));
        finalTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
        finalTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
        finalTitleText.rectTransform.sizeDelta = new Vector2(0f, 70f);
        finalTitleText.rectTransform.anchoredPosition = new Vector2(0f, -48f);

        finalStatsText = Label("", card, 22, FontStyle.Normal, TextAnchor.UpperCenter, new Color(0.2f, 0.2f, 0.22f));
        finalStatsText.rectTransform.anchorMin = new Vector2(0f, 0.25f);
        finalStatsText.rectTransform.anchorMax = new Vector2(1f, 0.75f);
        finalStatsText.rectTransform.offsetMin = new Vector2(20f, 0f);
        finalStatsText.rectTransform.offsetMax = new Vector2(-20f, 0f);

        RectTransform restartRect = Panel("RestartButton", card, new Color(0.24f, 0.52f, 0.76f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(210f, 58f));
        restartRect.anchoredPosition = new Vector2(0f, 50f);
        Button restartButton = restartRect.gameObject.AddComponent<Button>();
        restartButton.onClick.AddListener(() => onRestart?.Invoke());
        restartButtonText = Label("Начать заново", restartRect, 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(restartButtonText.rectTransform, 4, 4, 4, 4);
        finalPanel.SetActive(false);
    }

    private Text TopLabel(string text, RectTransform parent, int size, float x, float y = -18f)
    {
        Text label = Label(text, parent, size, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.18f, 0.22f, 0.26f));
        label.rectTransform.anchorMin = new Vector2(0f, 1f);
        label.rectTransform.anchorMax = new Vector2(0f, 1f);
        label.rectTransform.sizeDelta = new Vector2(220f, 32f);
        label.rectTransform.anchoredPosition = new Vector2(x, y);
        return label;
    }

    private Text Label(string text, Transform parent, int size, FontStyle style, TextAnchor anchor, Color color)
    {
        Text label = CreateGameObject<Text>("Text", parent);
        label.text = text;
        label.font = font;
        label.fontSize = size;
        label.fontStyle = ToTmpFontStyle(style);
        label.alignment = ToTmpAlignment(anchor);
        label.color = color;
        label.enableWordWrapping = true;
        label.overflowMode = TextOverflowModes.Truncate;
        label.raycastTarget = false;
        return label;
    }

    private FontStyles ToTmpFontStyle(FontStyle style)
    {
        switch (style)
        {
            case FontStyle.Bold: return FontStyles.Bold;
            case FontStyle.Italic: return FontStyles.Italic;
            case FontStyle.BoldAndItalic: return FontStyles.Bold | FontStyles.Italic;
            default: return FontStyles.Normal;
        }
    }

    private TextAlignmentOptions ToTmpAlignment(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft: return TextAlignmentOptions.MidlineLeft;
            case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
            case TextAnchor.MiddleRight: return TextAlignmentOptions.MidlineRight;
            case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
            default: return TextAlignmentOptions.Center;
        }
    }

    private RectTransform Panel(string name, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
    {
        Image image = CreateGameObject<Image>(name, parent);
        image.color = color;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = size;
        return rect;
    }

    private T CreateGameObject<T>(string name, Transform parent) where T : Component
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.AddComponent<T>();
    }

    private void Stretch(RectTransform rect, float left, float right, float bottom, float top)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, top);
    }

    private string BulletList(List<string> items)
    {
        return string.Join("\n", items.Select(item => "• " + item));
    }

    private string DocHead(string text)
    {
        return $"<b><size=18><color=#5B4A25>{text}</color></size></b>";
    }

    private string DocTitle(string text)
    {
        return $"<b><size=18><color=#3F3522>{text}</color></size></b>";
    }

    private string BuildPhotoInitials(string soulName)
    {
        if (string.IsNullOrWhiteSpace(soulName)) return "?";

        string[] parts = soulName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();

        return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpperInvariant();
    }

    private Sprite GetAvatarFor(string soulName)
    {
        if (avatarSprites.Count == 0) return null;

        int hash = string.IsNullOrEmpty(soulName) ? 0 : soulName.GetHashCode();
        hash = hash == int.MinValue ? 0 : Mathf.Abs(hash);
        return avatarSprites[hash % avatarSprites.Count];
    }

    private string StampMark(StampType stamp)
    {
        switch (stamp)
        {
            case StampType.Heaven: return "РАЙ";
            case StampType.Hell: return "АД";
            case StampType.Appeal: return "АПЕЛЛЯЦИЯ";
            case StampType.Audit: return "ПРОВЕРКА";
            default: return stamp.ToString().ToUpperInvariant();
        }
    }

    private string ReactionFor(StampType stamp)
    {
        if (currentLanguage == HeavenOfficeLanguage.English)
        {
            switch (stamp)
            {
                case StampType.Heaven: return "The soul smiles with relief.";
                case StampType.Hell: return "The soul nervously vanishes into a red portal.";
                case StampType.Appeal: return "The soul takes a ticket and waits.";
                case StampType.Audit: return "The case is sent to the blue audit archive.";
                default: return "The office marks the case.";
            }
        }

        switch (stamp)
        {
            case StampType.Heaven: return "Душа облегчённо улыбается.";
            case StampType.Hell: return "Душа нервно исчезает в красном портале.";
            case StampType.Appeal: return "Душа получает номерок и уходит ждать.";
            case StampType.Audit: return "Дело отправлено в синий архив проверки.";
            default: return "Канцелярия ставит отметку.";
        }
    }

    private string SessionReasonLabel(SessionEndReason reason, HeavenOfficeLanguage language)
    {
        if (language == HeavenOfficeLanguage.English)
        {
            switch (reason)
            {
                case SessionEndReason.QueueCompleted: return "queue completed";
                case SessionEndReason.TooManyMistakes: return "too many errors";
                case SessionEndReason.SessionTimerExpired: return "shift timer expired";
                case SessionEndReason.ManualRestart: return "manual restart";
                default: return reason.ToString();
            }
        }

        switch (reason)
        {
            case SessionEndReason.QueueCompleted: return "очередь обработана";
            case SessionEndReason.TooManyMistakes: return "слишком много ошибок";
            case SessionEndReason.SessionTimerExpired: return "истёк таймер смены";
            case SessionEndReason.ManualRestart: return "ручной перезапуск";
            default: return reason.ToString();
        }
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
        eventSystem.AddComponent<InputSystemUIInputModule>();
#else
        eventSystem.AddComponent<StandaloneInputModule>();
#endif
    }
}
