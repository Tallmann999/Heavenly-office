using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSessionManager
{
    public event Action<SoulDocumentData,int,int,HeavenOfficeLanguage> ShowDocumentRequested;
    public event Action<int,int,int,int,int,int,HeavenOfficeLanguage> UpdateHudRequested;
    public event Action<string,Color,HeavenOfficeLanguage> SetFeedbackRequested;
    public event Action<string,int,int,int,int,SessionEndReason,HeavenOfficeLanguage> ShowFinalPanelRequested;

    // Provider for playing view animations (adapter must set this)
    public Func<StampType, float, IEnumerator> PlayAnimationProvider;

    private readonly HeavenOfficeConfig config;
    private readonly SoulDocumentGenerator generator;
    private readonly HeavenOfficeAnalyticsLog analytics;

    private List<SoulDocumentData> queue = new List<SoulDocumentData>();
    private SoulDocumentData currentDocument;
    private StampType? selectedStamp;
    private int currentIndex;
    private int score;
    private int mistakes;
    private int combo;
    private int maxCombo;
    private HeavenOfficeLanguage language = HeavenOfficeLanguage.Russian;
    private bool sessionEnded = true;

    public GameSessionManager(HeavenOfficeConfig config, SoulDocumentGenerator generator, HeavenOfficeAnalyticsLog analytics)
    {
        this.config = config ?? new HeavenOfficeConfig();
        this.generator = generator ?? new SoulDocumentGenerator();
        this.analytics = analytics ?? new HeavenOfficeAnalyticsLog(false);
    }

    public void StartSession()
    {
        selectedStamp = null;
        sessionEnded = false;
        currentIndex = 0;
        score = 0;
        mistakes = 0;
        combo = 0;
        maxCombo = 0;
        queue = generator.BuildSession(config, language);
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
        selectedStamp = null;
        var available = HeavenOfficeRulesEvaluator.GetAvailableStamps(currentDocument.difficultyTier);
        ShowDocumentRequested?.Invoke(currentDocument, currentIndex + 1, queue.Count, language);
        UpdateHudRequested?.Invoke(score, currentIndex + 1, queue.Count, mistakes, config.maxMistakeCount, combo, currentDocument.difficultyTier, language);
        SetFeedbackRequested?.Invoke(language == HeavenOfficeLanguage.English ? "Waiting for a stamp." : "Ожидается печать.", new Color(0.25f, 0.27f, 0.3f), language);
        analytics.Log("document_shown", currentDocument, null, null, 0f, currentDocument.difficultyTier);
    }

    public void OnStampSelected(StampType stamp)
    {
        if (sessionEnded || currentDocument == null) return;
        var available = HeavenOfficeRulesEvaluator.GetAvailableStamps(currentDocument.difficultyTier);
        if (!available.Contains(stamp))
        {
            SetFeedbackRequested?.Invoke(language == HeavenOfficeLanguage.English ? "This stamp is locked by the current tier." : "Эта печать пока закрыта регламентом сложности.", new Color(0.35f, 0.35f, 0.4f), language);
            return;
        }

        selectedStamp = stamp;
        SetFeedbackRequested?.Invoke(language == HeavenOfficeLanguage.English ? "Stamp selected." : "Печать выбрана.", new Color(0.1f, 0.48f, 0.2f), language);
        analytics.Log("stamp_selected", currentDocument, stamp, null, 0f, currentDocument.difficultyTier);
    }

    public IEnumerator ResolveSelectedStampCoroutine()
    {
        if (sessionEnded || currentDocument == null || !selectedStamp.HasValue) yield break;
        StampType stamp = selectedStamp.Value;
        analytics.Log("stamp_applied", currentDocument, stamp, null, 0f, currentDocument.difficultyTier);

        if (PlayAnimationProvider != null)
        {
            yield return PlayAnimationProvider(stamp, config.reactionDelay);
        }

        bool correct = stamp == currentDocument.expectedStamp;
        if (correct)
        {
            int gain = CalculateScoreGain();
            score += gain;
            combo++;
            maxCombo = Math.Max(maxCombo, combo);
            SetFeedbackRequested?.Invoke(language == HeavenOfficeLanguage.English ? $"Correct: {stamp}. +{gain} points." : $"Верно: {stamp}. +{gain} очков.", new Color(0.1f, 0.48f, 0.2f), language);
            analytics.Log("decision_correct", currentDocument, stamp, DecisionResultType.Correct, 0f, currentDocument.difficultyTier);
        }
        else
        {
            Penalize();
            SetFeedbackRequested?.Invoke(language == HeavenOfficeLanguage.English ? $"Wrong: needed {currentDocument.expectedStamp}. -{config.mistakePenalty}." : $"Ошибка: нужна печать «{currentDocument.expectedStamp}». -{config.mistakePenalty}.", new Color(0.68f, 0.12f, 0.1f), language);
            analytics.Log("decision_wrong", currentDocument, stamp, DecisionResultType.WrongStamp, 0f, currentDocument.difficultyTier);
        }

        UpdateHudRequested?.Invoke(score, currentIndex + 1, queue.Count, mistakes, config.maxMistakeCount, combo, currentDocument.difficultyTier, language);

        // small exit delay
        yield return new WaitForSeconds(config.exitAnimationTime);

        AdvanceOrEnd();
    }

    private int CalculateScoreGain()
    {
        int baseReward = config.baseScoreReward;
        int comboBonus = Mathf.RoundToInt(config.baseScoreReward * config.comboScoreMultiplier * combo);
        return baseReward + comboBonus;
    }

    private void Penalize()
    {
        score = Math.Max(0, score - config.mistakePenalty);
        mistakes++;
        if (combo > 0)
        {
            analytics.Log("combo_broken", currentDocument, selectedStamp, null, 0f, currentDocument.difficultyTier);
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
        analytics.Log("session_end");
        ShowFinalPanelRequested?.Invoke(language == HeavenOfficeLanguage.English ? "Shift Complete" : "Смена завершена", score, 0, mistakes, maxCombo, reason, language);
    }

    public void OnLanguageSelected(HeavenOfficeLanguage selectedLanguage)
    {
        language = selectedLanguage;
    }
}
