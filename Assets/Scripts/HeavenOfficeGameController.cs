using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class HeavenOfficeGameController : MonoBehaviour
{
    [SerializeField] private HeavenOfficeConfig config = new HeavenOfficeConfig();

    private HeavenOfficeView view;
    private SoulDocumentGenerator generator;
    private HeavenOfficeAnalyticsLog analytics;
    private List<SoulDocumentData> queue = new List<SoulDocumentData>();
    private SoulDocumentData currentDocument;
    private StampType? selectedStamp;
    private float remainingTime;
    private bool inputLocked;
    private bool sessionEnded;
    private int currentIndex;
    private int score;
    private int mistakes;
    private int correctDecisions;
    private int combo;
    private int maxCombo;
    private HeavenOfficeLanguage language = HeavenOfficeLanguage.Russian;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        generator = new SoulDocumentGenerator();
        analytics = new HeavenOfficeAnalyticsLog(config.enableAnalyticsLog);
        view = GetComponent<HeavenOfficeView>();
        if (view == null)
        {
            view = gameObject.AddComponent<HeavenOfficeView>();
        }
    }

    [ContextMenu("Rebuild Editable UI")]
    private void RebuildEditableUiFromInspector()
    {
        if (view == null)
        {
            view = GetComponent<HeavenOfficeView>();
        }

        if (view == null)
        {
            view = gameObject.AddComponent<HeavenOfficeView>();
        }

        view.BuildIfNeeded(config.createUiAtRuntime);
    }

    private void Start()
    {
        view.BuildIfNeeded(config.createUiAtRuntime);
        view.Bind(OnStampSelected, OnStampTargetPressed, OnLanguageSelected, StartSession, RestartSession);
        sessionEnded = true;
        inputLocked = true;
        view.ShowStartMenu();
    }

    private void Update()
    {
        HandleHotkeys();
        view.UpdateHeldStampPosition();

        if (sessionEnded || inputLocked || currentDocument == null)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        view.UpdateTimer(remainingTime, currentDocument.timeLimit);

        if (remainingTime <= 0f)
        {
            StartCoroutine(ResolveTimeExpired());
        }
    }

    private void StartSession()
    {
        StopAllCoroutines();
        selectedStamp = null;
        inputLocked = false;
        sessionEnded = false;
        currentIndex = 0;
        score = 0;
        mistakes = 0;
        correctDecisions = 0;
        combo = 0;
        maxCombo = 0;
        queue = generator.BuildSession(config, language);
        view.HideStartMenu();
        view.HideFinalPanel();
        analytics.Log("session_start");
        ShowCurrentDocument();
    }

    private void ShowCurrentDocument()
    {
        if (currentIndex >= queue.Count)
        {
            EndSession(SessionEndReason.QueueCompleted);
            return;
        }

        currentDocument = queue[currentIndex];
        remainingTime = currentDocument.timeLimit;
        selectedStamp = null;
        inputLocked = false;
        var available = HeavenOfficeRulesEvaluator.GetAvailableStamps(currentDocument.difficultyTier);
        view.ShowDocument(currentDocument, generator, currentIndex + 1, queue.Count, language);
        view.UpdateHud(score, currentIndex + 1, queue.Count, mistakes, config.maxMistakeCount, combo, currentDocument.difficultyTier, language);
        view.UpdateTimer(remainingTime, currentDocument.timeLimit);
        view.SetRuleHint(currentDocument.ruleExplanation);
        view.SetStampAvailability(available);
        view.HighlightSelectedStamp(null);
        view.SetFeedback(language == HeavenOfficeLanguage.English ? "Waiting for a stamp." : "Ожидается печать.", new Color(0.25f, 0.27f, 0.3f), language);
        analytics.Log("document_shown", currentDocument, null, null, 0f, currentDocument.difficultyTier);
    }

    private void OnStampSelected(StampType stamp)
    {
        if (inputLocked || sessionEnded || currentDocument == null) return;

        var available = HeavenOfficeRulesEvaluator.GetAvailableStamps(currentDocument.difficultyTier);
        if (!available.Contains(stamp))
        {
            view.SetFeedback(language == HeavenOfficeLanguage.English ? "This stamp is locked by the current tier." : "Эта печать пока закрыта регламентом сложности.", new Color(0.35f, 0.35f, 0.4f), language);
            return;
        }

        selectedStamp = stamp;
        view.HighlightSelectedStamp(stamp);
        view.ShowHeldStamp(stamp);
        analytics.Log("stamp_selected", currentDocument, stamp, null, currentDocument.timeLimit - remainingTime, currentDocument.difficultyTier);
    }

    private void OnStampTargetPressed()
    {
        if (inputLocked || sessionEnded || currentDocument == null) return;

        if (!selectedStamp.HasValue)
        {
            view.SetFeedback(language == HeavenOfficeLanguage.English ? "Choose a stamp first." : "Сначала выбери печать.", new Color(0.45f, 0.34f, 0.12f), language);
            return;
        }

        StartCoroutine(ResolveStamp(selectedStamp.Value));
    }

    private IEnumerator ResolveStamp(StampType stamp)
    {
        inputLocked = true;
        view.HideHeldStamp();
        float reactionTime = currentDocument.timeLimit - remainingTime;
        analytics.Log("stamp_applied", currentDocument, stamp, null, reactionTime, currentDocument.difficultyTier);
        yield return view.PlayStampAnimation(stamp, config.reactionDelay);

        bool correct = stamp == currentDocument.expectedStamp;
        if (correct)
        {
            int gain = CalculateScoreGain();
            score += gain;
            combo++;
            correctDecisions++;
            maxCombo = Mathf.Max(maxCombo, combo);
            view.SetFeedback(language == HeavenOfficeLanguage.English ? $"Correct: {StampLabel(stamp)}. +{gain} points." : $"Верно: {StampLabel(stamp)}. +{gain} очков.", new Color(0.1f, 0.48f, 0.2f), language);
            analytics.Log("decision_correct", currentDocument, stamp, DecisionResultType.Correct, reactionTime, currentDocument.difficultyTier);
        }
        else
        {
            Penalize();
            view.SetFeedback(language == HeavenOfficeLanguage.English ? $"Wrong: needed {StampLabel(currentDocument.expectedStamp)}. -{config.mistakePenalty}." : $"Ошибка: нужна печать «{StampLabel(currentDocument.expectedStamp)}». -{config.mistakePenalty}.", new Color(0.68f, 0.12f, 0.1f), language);
            analytics.Log("decision_wrong", currentDocument, stamp, DecisionResultType.WrongStamp, reactionTime, currentDocument.difficultyTier);
        }

        view.UpdateHud(score, currentIndex + 1, queue.Count, mistakes, config.maxMistakeCount, combo, currentDocument.difficultyTier, language);
        yield return view.PlayExitAnimation(currentDocument.expectedStamp, config.exitAnimationTime);
        AdvanceOrEnd();
    }

    private IEnumerator ResolveTimeExpired()
    {
        inputLocked = true;
        remainingTime = 0f;
        view.UpdateTimer(0f, currentDocument.timeLimit);
        view.ShowSpoiledStamp();
        Penalize();
        view.SetFeedback(language == HeavenOfficeLanguage.English ? $"Time expired. -{config.mistakePenalty}." : $"Время вышло. -{config.mistakePenalty}.", new Color(0.55f, 0.2f, 0.16f), language);
        analytics.Log("time_expired", currentDocument, null, DecisionResultType.TimeExpired, currentDocument.timeLimit, currentDocument.difficultyTier);
        view.UpdateHud(score, currentIndex + 1, queue.Count, mistakes, config.maxMistakeCount, combo, currentDocument.difficultyTier, language);
        yield return new WaitForSeconds(config.reactionDelay);
        yield return view.PlayExitAnimation(null, config.exitAnimationTime);
        AdvanceOrEnd();
    }

    private int CalculateScoreGain()
    {
        float remainingRatio = Mathf.Clamp01(remainingTime / Mathf.Max(0.01f, currentDocument.timeLimit));
        int speedBonus = remainingRatio >= 1f - config.fastDecisionWindow
            ? Mathf.RoundToInt(config.fastDecisionBonus * remainingRatio)
            : 0;
        int comboBonus = Mathf.RoundToInt(config.baseScoreReward * config.comboScoreMultiplier * combo);
        return config.baseScoreReward + speedBonus + comboBonus;
    }

    private void Penalize()
    {
        score = Mathf.Max(0, score - config.mistakePenalty);
        mistakes++;
        if (combo > 0)
        {
            analytics.Log("combo_broken", currentDocument, selectedStamp, null, currentDocument.timeLimit - remainingTime, currentDocument.difficultyTier);
        }

        combo = 0;
    }

    private void AdvanceOrEnd()
    {
        if (mistakes >= config.maxMistakeCount)
        {
            EndSession(SessionEndReason.TooManyMistakes);
            return;
        }

        currentIndex++;
        if (currentIndex >= queue.Count)
        {
            EndSession(SessionEndReason.QueueCompleted);
            return;
        }

        ShowCurrentDocument();
    }

    private void EndSession(SessionEndReason reason)
    {
        sessionEnded = true;
        inputLocked = true;
        currentDocument = null;
        view.HideHeldStamp();
        analytics.Log("session_end");
        string title = reason == SessionEndReason.TooManyMistakes
            ? (language == HeavenOfficeLanguage.English ? "Shift Failed" : "Смена провалена")
            : (language == HeavenOfficeLanguage.English ? "Shift Complete" : "Смена завершена");
        view.ShowFinalPanel(title, score, correctDecisions, mistakes, maxCombo, reason, language);
    }

    private void OnLanguageSelected(HeavenOfficeLanguage selectedLanguage)
    {
        language = selectedLanguage;
        view.ApplyLanguage(language);
    }

    private void RestartSession()
    {
        analytics.Log("game_restarted");
        StartSession();
    }

    private void HandleHotkeys()
    {
        if (sessionEnded || inputLocked) return;

#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.digit1Key.wasPressedThisFrame) OnStampSelected(StampType.Heaven);
        if (keyboard.digit2Key.wasPressedThisFrame) OnStampSelected(StampType.Hell);
        if (keyboard.digit3Key.wasPressedThisFrame) OnStampSelected(StampType.Appeal);
        if (keyboard.digit4Key.wasPressedThisFrame) OnStampSelected(StampType.Audit);
        if (keyboard.spaceKey.wasPressedThisFrame) OnStampTargetPressed();
#else
        if (Input.GetKeyDown(KeyCode.Alpha1)) OnStampSelected(StampType.Heaven);
        if (Input.GetKeyDown(KeyCode.Alpha2)) OnStampSelected(StampType.Hell);
        if (Input.GetKeyDown(KeyCode.Alpha3)) OnStampSelected(StampType.Appeal);
        if (Input.GetKeyDown(KeyCode.Alpha4)) OnStampSelected(StampType.Audit);
        if (Input.GetKeyDown(KeyCode.Space)) OnStampTargetPressed();
#endif
    }

    private string StampLabel(StampType stamp)
    {
        if (language == HeavenOfficeLanguage.English)
        {
            switch (stamp)
            {
                case StampType.Heaven: return "Heaven";
                case StampType.Hell: return "Hell";
                case StampType.Appeal: return "Appeal";
                case StampType.Audit: return "Audit";
                default: return stamp.ToString();
            }
        }

        switch (stamp)
        {
            case StampType.Heaven: return "Рай";
            case StampType.Hell: return "Ад";
            case StampType.Appeal: return "Апелляция";
            case StampType.Audit: return "Проверка";
            default: return stamp.ToString();
        }
    }
}
