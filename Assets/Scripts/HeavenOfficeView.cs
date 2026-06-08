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
        scaler.referenceResolution = new Vector2(1600, 900);
        scaler.matchWidthOrHeight = 0.5f;
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        RectTransform root = canvas.GetComponent<RectTransform>();
        Image background = canvas.gameObject.AddComponent<Image>();
        background.color = new Color(0.07f, 0.075f, 0.08f);

        RectTransform top = Panel("TopHud", root, new Color(0.07f, 0.075f, 0.085f, 0.98f), Vector2.zero, Vector2.zero, Vector2.zero);
        top.anchorMin = new Vector2(0f, 1f);
        top.anchorMax = new Vector2(1f, 1f);
        top.offsetMin = new Vector2(0f, -72f);
        top.offsetMax = Vector2.zero;
        top.gameObject.AddComponent<Outline>().effectColor = new Color(0.78f, 0.52f, 0.24f, 0.38f);

        titleText = Label("DIVINE OFFICE", top, 32, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.86f, 0.62f, 0.32f));
        Stretch(titleText.rectTransform, 360, 360, 12, -6);
        Text subtitle = Label("Soul Distribution Department No. 7", top, 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.67f, 0.53f, 0.35f));
        subtitle.rectTransform.anchorMin = new Vector2(0.35f, 0f);
        subtitle.rectTransform.anchorMax = new Vector2(0.65f, 0.45f);
        subtitle.rectTransform.offsetMin = Vector2.zero;
        subtitle.rectTransform.offsetMax = Vector2.zero;

        scoreText = TopLabel("Score: 0", top, 16, 24);
        queueText = TopLabel("Case: 1/12", top, 16, 170);
        timerText = TopLabel("Timer: 12.0", top, 16, 1280);
        mistakesText = TopLabel("Errors: 0/3", top, 16, 1420);
        comboText = TopLabel("Combo: 0", top, 14, 24, -44);
        tierText = TopLabel("Tier 0", top, 14, 170, -44);

        RectTransform timerBar = Panel("TimerBar", top, new Color(0.24f, 0.2f, 0.15f), new Vector2(0.78f, 0f), new Vector2(0.98f, 0f), Vector2.zero);
        timerBar.offsetMin = new Vector2(0f, 12f);
        timerBar.offsetMax = new Vector2(0f, 22f);
        timerFill = Panel("TimerFill", timerBar, new Color(0.82f, 0.52f, 0.22f), new Vector2(0f, 0f), new Vector2(0f, 1f), Vector2.zero).GetComponent<Image>();
        timerFill.rectTransform.pivot = new Vector2(0f, 0.5f);
        Stretch(timerFill.rectTransform, 0, 0, 0, 0);

        RectTransform bottom = Panel("BottomHud", root, new Color(0.07f, 0.075f, 0.085f, 0.98f), Vector2.zero, Vector2.zero, Vector2.zero);
        bottom.anchorMin = Vector2.zero;
        bottom.anchorMax = new Vector2(1f, 0f);
        bottom.offsetMin = Vector2.zero;
        bottom.offsetMax = new Vector2(0f, 84f);
        bottom.gameObject.AddComponent<Outline>().effectColor = new Color(0.78f, 0.52f, 0.24f, 0.3f);
        ruleHintText = Label("Hint: ", bottom, 17, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.82f, 0.68f, 0.46f));
        feedbackText = Label("Decision: ", bottom, 17, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.74f, 0.72f, 0.66f));
        CenterBottomHints();

        RectTransform center = Panel("CentralWorkZone", root, new Color(0.11f, 0.11f, 0.12f), Vector2.zero, Vector2.one, Vector2.zero);
        center.offsetMin = new Vector2(0f, 84f);
        center.offsetMax = new Vector2(0f, -72f);

        BuildOfficeDesk(center);

        soulQueueStrip = Panel("SoulQueue", center, new Color(0.16f, 0.12f, 0.09f, 0.9f), new Vector2(0.02f, 0.26f), new Vector2(0.22f, 0.86f), Vector2.zero).GetComponent<Image>();
        soulQueueStripRect = soulQueueStrip.rectTransform;
        soulQueueStripRect.offsetMin = Vector2.zero;
        soulQueueStripRect.offsetMax = Vector2.zero;
        soulQueueStrip.gameObject.AddComponent<Outline>().effectColor = new Color(0.82f, 0.57f, 0.27f, 0.45f);
        AttachDeskZone(soulQueueStripRect, "soul_queue", false);
        soulQueueStripText = Label("SOUL QUEUE\n\nwaiting souls\nempty sprites", soulQueueStripRect, 16, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.92f, 0.84f, 0.7f));
        Stretch(soulQueueStripText.rectTransform, 12, 12, 12, -12);

        soulCard = Panel("SoulWindow", center, new Color(0.20f, 0.31f, 0.35f, 0.96f), new Vector2(0.48f, 0.66f), new Vector2(0.66f, 0.93f), Vector2.zero).GetComponent<Image>();
        soulRect = soulCard.rectTransform;
        soulRect.offsetMin = Vector2.zero;
        soulRect.offsetMax = Vector2.zero;
        soulCard.gameObject.AddComponent<Outline>().effectColor = new Color(0.82f, 0.57f, 0.27f, 0.45f);
        AttachDeskZone(soulRect, "soul_window", false);
        soulText = Label("CURRENT SOUL", soulRect, 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.93f, 0.91f, 0.82f));
        Stretch(soulText.rectTransform, 8, 8, 8, 8);

        reactionText = Label("", center, 16, FontStyle.Italic, TextAnchor.MiddleCenter, new Color(0.9f, 0.82f, 0.68f));
        reactionText.rectTransform.anchorMin = new Vector2(0.28f, 0.57f);
        reactionText.rectTransform.anchorMax = new Vector2(0.68f, 0.65f);
        reactionText.rectTransform.offsetMin = Vector2.zero;
        reactionText.rectTransform.offsetMax = Vector2.zero;

        documentCard = Panel("SoulDossier", center, new Color(0.94f, 0.86f, 0.67f), new Vector2(0.27f, 0.18f), new Vector2(0.47f, 0.69f), Vector2.zero).GetComponent<Image>();
        documentRect = documentCard.rectTransform;
        documentRect.offsetMin = Vector2.zero;
        documentRect.offsetMax = Vector2.zero;
        documentRect.localEulerAngles = new Vector3(0f, 0f, -3.5f);
        documentCard.gameObject.AddComponent<Outline>().effectColor = new Color(0.18f, 0.12f, 0.06f, 0.58f);
        AttachDeskZone(documentRect, "soul_dossier", false);
        Button documentButton = documentCard.gameObject.AddComponent<Button>();
        documentButton.transition = Selectable.Transition.ColorTint;
        documentButton.onClick.AddListener(() => onStampTargetPressed?.Invoke());

        documentText = Label("", documentRect, 15, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.18f, 0.16f, 0.13f));
        Stretch(documentText.rectTransform, 24, 126, 20, -22);

        photoFrame = Panel("SoulPhotoSlot", documentRect, new Color(0.9f, 0.88f, 0.76f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(92f, 112f)).GetComponent<Image>();
        photoFrame.rectTransform.anchoredPosition = new Vector2(-66f, -78f);
        photoFrame.gameObject.AddComponent<Outline>().effectColor = new Color(0.36f, 0.31f, 0.22f, 0.55f);
        photoText = Label("PHOTO", photoFrame.rectTransform, 16, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.42f, 0.39f, 0.33f));
        Stretch(photoText.rectTransform, 6, 6, 6, 6);

        stampMarkPanel = Panel("StampInkTrace", documentRect, new Color(0.2f, 0.5f, 0.26f, 0.14f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(166f, 78f)).GetComponent<Image>();
        stampMarkRect = stampMarkPanel.rectTransform;
        stampMarkRect.anchorMin = new Vector2(1f, 0f);
        stampMarkRect.anchorMax = new Vector2(1f, 0f);
        stampMarkRect.anchoredPosition = new Vector2(-105f, 68f);
        stampMarkRect.localEulerAngles = new Vector3(0f, 0f, -8f);
        stampMarkPanel.gameObject.AddComponent<Outline>().effectColor = new Color(0.2f, 0.5f, 0.26f, 0.68f);
        stampMarkTopLine = Panel("StampTraceLineTop", stampMarkRect, new Color(0.2f, 0.5f, 0.26f, 0.55f), new Vector2(0.08f, 0.7f), new Vector2(0.92f, 0.7f), new Vector2(0f, 3f)).GetComponent<Image>();
        stampMarkBottomLine = Panel("StampTraceLineBottom", stampMarkRect, new Color(0.2f, 0.5f, 0.26f, 0.55f), new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.28f), new Vector2(0f, 3f)).GetComponent<Image>();
        stampMarkText = Label("", stampMarkRect, 22, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.2f, 0.5f, 0.26f, 0.86f));
        Stretch(stampMarkText.rectTransform, 8, 8, 8, 8);
        stampMarkPanel.gameObject.SetActive(false);

        targetZoneText = Label("STAMP TARGET", center, 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.74f, 0.64f, 0.44f));
        targetZoneText.rectTransform.anchorMin = new Vector2(0.34f, 0.12f);
        targetZoneText.rectTransform.anchorMax = new Vector2(0.47f, 0.18f);
        targetZoneText.rectTransform.offsetMin = Vector2.zero;
        targetZoneText.rectTransform.offsetMax = Vector2.zero;
        Button targetButton = targetZoneText.gameObject.AddComponent<Button>();
        targetButton.onClick.AddListener(() => onStampTargetPressed?.Invoke());

        AddDeskStampButton(center, StampType.Heaven, "HEAVEN", "HEAVEN", new Color(0.22f, 0.58f, 0.32f), -210f);
        AddDeskStampButton(center, StampType.Hell, "HELL", "HELL", new Color(0.66f, 0.16f, 0.12f), -70f);
        AddDeskStampButton(center, StampType.Audit, "AUDIT", "AUDIT", new Color(0.18f, 0.38f, 0.64f), 70f);
        AddDeskStampButton(center, StampType.Appeal, "APPEAL", "APPEAL", new Color(0.46f, 0.26f, 0.62f), 210f);

        BuildDestinationTrays(center);
        BuildFullVersionTeasers(center);
        BuildReincarnationLever(center);
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
        documentRect.offsetMin = Vector2.zero;
        documentRect.offsetMax = Vector2.zero;
        documentRect.localEulerAngles = new Vector3(0f, 0f, -3.5f);
        soulRect.offsetMin = Vector2.zero;
        soulRect.offsetMax = Vector2.zero;
        documentCard.color = new Color(0.94f, 0.86f, 0.67f);
        soulCard.color = new Color(0.20f, 0.31f, 0.35f, 0.96f);
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
        timerText.color = ratio <= 0.25f ? new Color(0.95f, 0.24f, 0.16f) : new Color(0.84f, 0.75f, 0.58f);
        timerFill.color = ratio <= 0.25f ? new Color(0.88f, 0.16f, 0.12f) : new Color(0.82f, 0.52f, 0.22f);
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
        soulCard.color = Color.Lerp(new Color(0.20f, 0.31f, 0.35f, 0.96f), color, 0.35f);
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
            titleText.gameObject.SetActive(true);
        }

        PlaceTopLabel(scoreText, 24f, -16f, 130f);
        PlaceTopLabel(queueText, 170f, -16f, 150f);
        PlaceTopLabel(timerText, 1280f, -16f, 130f);
        PlaceTopLabel(mistakesText, 1420f, -16f, 150f);
        PlaceTopLabel(comboText, 24f, -44f, 130f);
        PlaceTopLabel(tierText, 170f, -44f, 120f);
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
        label.color = new Color(0.84f, 0.75f, 0.58f);
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
        RectTransform skyWindow = Panel("SoulWindowBackdrop", center, new Color(0.28f, 0.36f, 0.42f, 0.55f), new Vector2(0.40f, 0.58f), new Vector2(0.70f, 0.96f), Vector2.zero);
        skyWindow.offsetMin = Vector2.zero;
        skyWindow.offsetMax = Vector2.zero;
        skyWindow.gameObject.AddComponent<Outline>().effectColor = new Color(0.82f, 0.57f, 0.27f, 0.2f);

        RectTransform wall = Panel("StoneArchiveWall", center, new Color(0.17f, 0.16f, 0.15f, 0.8f), new Vector2(0.23f, 0.48f), new Vector2(0.39f, 0.93f), Vector2.zero);
        wall.offsetMin = Vector2.zero;
        wall.offsetMax = Vector2.zero;
        wall.gameObject.AddComponent<Outline>().effectColor = new Color(0.78f, 0.52f, 0.24f, 0.25f);

        RectTransform desk = Panel("MainOfficeDesk", center, new Color(0.29f, 0.18f, 0.10f, 0.96f), new Vector2(0.08f, 0.02f), new Vector2(0.88f, 0.43f), Vector2.zero);
        desk.offsetMin = Vector2.zero;
        desk.offsetMax = Vector2.zero;
        desk.gameObject.AddComponent<Outline>().effectColor = new Color(0.78f, 0.52f, 0.24f, 0.45f);
        AttachDeskZone(desk, "main_desk", false);

        RectTransform archive = Panel("ArchiveTeaser", center, new Color(0.23f, 0.18f, 0.12f, 0.92f), new Vector2(0.25f, 0.72f), new Vector2(0.37f, 0.86f), Vector2.zero);
        archive.pivot = new Vector2(0f, 1f);
        archive.offsetMin = Vector2.zero;
        archive.offsetMax = Vector2.zero;
        archive.gameObject.AddComponent<Outline>().effectColor = new Color(0.82f, 0.57f, 0.27f, 0.45f);
        AttachDeskZone(archive, "archive", false);
        Text archiveText = Label("ARCHIVE\nempty shelf", archive, 13, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.92f, 0.84f, 0.7f));
        Stretch(archiveText.rectTransform, 6, 6, 6, 6);

        RectTransform collection = Panel("CollectionTeaser", center, new Color(0.16f, 0.18f, 0.22f, 0.9f), new Vector2(0.02f, 0.08f), new Vector2(0.13f, 0.24f), Vector2.zero);
        collection.pivot = new Vector2(1f, 1f);
        collection.offsetMin = Vector2.zero;
        collection.offsetMax = Vector2.zero;
        collection.gameObject.AddComponent<Outline>().effectColor = new Color(0.82f, 0.57f, 0.27f, 0.45f);
        AttachDeskZone(collection, "card_collection", false);
        Text collectionText = Label("CARDS\ncollection", collection, 13, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.92f, 0.84f, 0.7f));
        Stretch(collectionText.rectTransform, 6, 6, 6, 6);
    }

    private void BuildReincarnationLever(RectTransform center)
    {
        reincarnationLeverRect = Panel("ReincarnationLever", center, new Color(0.32f, 0.20f, 0.12f, 0.96f), new Vector2(0.73f, 0.06f), new Vector2(0.83f, 0.33f), Vector2.zero);
        reincarnationLeverRect.pivot = new Vector2(1f, 0f);
        reincarnationLeverRect.offsetMin = Vector2.zero;
        reincarnationLeverRect.offsetMax = Vector2.zero;
        reincarnationLeverRect.gameObject.AddComponent<Outline>().effectColor = new Color(0.82f, 0.57f, 0.27f, 0.45f);
        AttachDeskZone(reincarnationLeverRect, "reincarnation_lever", false);

        Button leverButton = reincarnationLeverRect.gameObject.AddComponent<Button>();
        leverButton.transition = Selectable.Transition.ColorTint;
        leverButton.onClick.AddListener(() => onStampTargetPressed?.Invoke());

        RectTransform handle = Panel("LeverHandle", reincarnationLeverRect, new Color(0.82f, 0.18f, 0.1f), new Vector2(0.5f, 0.58f), new Vector2(0.5f, 0.58f), new Vector2(24f, 78f));
        handle.localEulerAngles = new Vector3(0f, 0f, -16f);
        handle.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 1f, 1f, 0.24f);

        reincarnationLeverText = Label("NEW LIFE\nLEVER", reincarnationLeverRect, 13, FontStyle.Bold, TextAnchor.LowerCenter, new Color(0.94f, 0.82f, 0.58f));
        Stretch(reincarnationLeverText.rectTransform, 6, 6, 8, -78);
    }

    private void BuildFullVersionTeasers(RectTransform center)
    {
        RectTransform mirror = Panel("MirrorFullVersionTeaser", center, new Color(0.33f, 0.54f, 0.66f, 0.42f), new Vector2(0.56f, 0.22f), new Vector2(0.64f, 0.45f), Vector2.zero);
        mirror.pivot = new Vector2(0f, 0f);
        mirror.offsetMin = Vector2.zero;
        mirror.offsetMax = Vector2.zero;
        mirror.gameObject.AddComponent<Outline>().effectColor = new Color(0.75f, 0.86f, 1f, 0.26f);
        AttachDeskZone(mirror, "past_life_mirror_teaser", true);
        mirrorTeaserText = Label("PAST LIFE\nMIRROR\nfull version", mirror, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.85f, 0.93f, 1f, 0.72f));
        Stretch(mirrorTeaserText.rectTransform, 6, 6, 6, 6);

        RectTransform scales = Panel("ScalesFullVersionTeaser", center, new Color(0.70f, 0.54f, 0.19f, 0.42f), new Vector2(0.47f, 0.24f), new Vector2(0.55f, 0.43f), Vector2.zero);
        scales.pivot = new Vector2(0f, 0f);
        scales.offsetMin = Vector2.zero;
        scales.offsetMax = Vector2.zero;
        scales.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 0.87f, 0.45f, 0.28f);
        AttachDeskZone(scales, "karma_scales_teaser", true);
        scalesTeaserText = Label("KARMA\nSCALES\nfull version", scales, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, 0.88f, 0.55f, 0.75f));
        Stretch(scalesTeaserText.rectTransform, 6, 6, 6, 6);
    }

    private void BuildDestinationTrays(RectTransform center)
    {
        Transform trayParent = center;
        leftTrayRect = Panel("HeavenPortal", trayParent, new Color(0.10f, 0.48f, 0.30f, 0.86f), new Vector2(0.68f, 0.48f), new Vector2(0.76f, 0.82f), Vector2.zero);
        leftTrayRect.pivot = new Vector2(0f, 0f);
        leftTrayRect.offsetMin = Vector2.zero;
        leftTrayRect.offsetMax = Vector2.zero;
        leftTrayImage = leftTrayRect.GetComponent<Image>();
        if (paradiseDestinationSprite != null)
        {
            leftTrayImage.sprite = paradiseDestinationSprite;
            leftTrayImage.preserveAspect = true;
            leftTrayImage.color = Color.white;
        }
        leftTrayImage.gameObject.AddComponent<Outline>().effectColor = new Color(0.62f, 1f, 0.74f, 0.45f);
        AttachDeskZone(leftTrayRect, "portal_heaven", false);
        Text leftLabel = Label("HEAVEN\nPORTAL", leftTrayRect, 15, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(leftLabel.rectTransform, 6, 6, 6, 6);
        leftLabel.gameObject.SetActive(paradiseDestinationSprite == null);

        rightTrayRect = Panel("HellPortal", trayParent, new Color(0.60f, 0.08f, 0.05f, 0.88f), new Vector2(0.77f, 0.48f), new Vector2(0.85f, 0.82f), Vector2.zero);
        rightTrayRect.pivot = new Vector2(1f, 0f);
        rightTrayRect.offsetMin = Vector2.zero;
        rightTrayRect.offsetMax = Vector2.zero;
        rightTrayImage = rightTrayRect.GetComponent<Image>();
        if (hellDestinationSprite != null)
        {
            rightTrayImage.sprite = hellDestinationSprite;
            rightTrayImage.preserveAspect = true;
            rightTrayImage.color = Color.white;
        }
        rightTrayImage.gameObject.AddComponent<Outline>().effectColor = new Color(1f, 0.42f, 0.32f, 0.45f);
        AttachDeskZone(rightTrayRect, "portal_hell", false);
        Text rightLabel = Label("HELL\nPORTAL", rightTrayRect, 15, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(rightLabel.rectTransform, 6, 6, 6, 6);
        rightLabel.gameObject.SetActive(hellDestinationSprite == null);

        RectTransform auditPortal = Panel("AuditPortal", trayParent, new Color(0.08f, 0.25f, 0.58f, 0.86f), new Vector2(0.86f, 0.48f), new Vector2(0.94f, 0.82f), Vector2.zero);
        auditPortal.offsetMin = Vector2.zero;
        auditPortal.offsetMax = Vector2.zero;
        auditPortal.gameObject.AddComponent<Outline>().effectColor = new Color(0.45f, 0.7f, 1f, 0.45f);
        AttachDeskZone(auditPortal, "portal_audit", false);
        Text auditLabel = Label("AUDIT\nPORTAL", auditPortal, 15, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(auditLabel.rectTransform, 6, 6, 6, 6);

        RectTransform appealPortal = Panel("AppealPortal", trayParent, new Color(0.34f, 0.18f, 0.50f, 0.82f), new Vector2(0.91f, 0.28f), new Vector2(0.985f, 0.47f), Vector2.zero);
        appealPortal.offsetMin = Vector2.zero;
        appealPortal.offsetMax = Vector2.zero;
        appealPortal.gameObject.AddComponent<Outline>().effectColor = new Color(0.72f, 0.47f, 1f, 0.4f);
        AttachDeskZone(appealPortal, "portal_appeal", false);
        Text appealLabel = Label("APPEAL", appealPortal, 13, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(appealLabel.rectTransform, 6, 6, 6, 6);
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

    private void AddDeskStampButton(RectTransform parent, StampType stamp, string label, string mark, Color color, float x)
    {
        RectTransform baseRect = Panel(stamp + "DeskStampBase", parent, new Color(0.21f, 0.13f, 0.08f, 0.95f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(118f, 96f));
        baseRect.pivot = new Vector2(0.5f, 0f);
        baseRect.anchoredPosition = new Vector2(x, 30f);
        baseRect.gameObject.AddComponent<Outline>().effectColor = new Color(0.82f, 0.57f, 0.27f, 0.45f);
        AttachDeskZone(baseRect, "stamp_" + stamp.ToString().ToLowerInvariant(), false);

        RectTransform rect = Panel(stamp + "Stamp", baseRect, color, new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(86f, 66f));
        rect.pivot = new Vector2(0.5f, 0.5f);
        Image image = rect.GetComponent<Image>();
        bool hasStampSprite = stampSprites.TryGetValue(stamp, out Sprite stampSprite);
        if (hasStampSprite)
        {
            image.sprite = stampSprite;
            image.preserveAspect = true;
            image.color = Color.white;
        }

        image.gameObject.AddComponent<Outline>().effectColor = new Color(0f, 0f, 0f, 0.35f);
        Button button = rect.gameObject.AddComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;

        Text text = Label(mark, rect, 14, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(text.rectTransform, 4, 4, 4, 4);
        text.gameObject.SetActive(!hasStampSprite);

        Text plate = Label(label, baseRect, 13, FontStyle.Bold, TextAnchor.LowerCenter, new Color(0.94f, 0.82f, 0.58f));
        Stretch(plate.rectTransform, 4, 4, 4, -68);

        stampButtons[stamp] = button;
        stampImages[stamp] = image;
        stampTexts[stamp] = text;
        stampColors[stamp] = color;
    }

    private void AttachDeskZone(RectTransform rect, string zoneId, bool premiumTeaser)
    {
        if (rect == null) return;

        Image image = rect.GetComponent<Image>();
        MainOfficeDeskZone zone = rect.gameObject.GetComponent<MainOfficeDeskZone>();
        if (zone == null)
        {
            zone = rect.gameObject.AddComponent<MainOfficeDeskZone>();
        }

        zone.Configure(zoneId, premiumTeaser, image);
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
        Text label = Label(text, parent, size, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.84f, 0.75f, 0.58f));
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
