using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DivineOfficeAdapter : MonoBehaviour
{
    [SerializeField] private HeavenOfficeConfig config;

    private HeavenOfficeView view;
    private GameSessionManager manager;
    private SoulDocumentGenerator generator;
    private HeavenOfficeAnalyticsLog analytics;
    private DivineOfficeBootstrap bootstrap;

    private void Awake()
    {
        view = GetComponent<HeavenOfficeView>();
        if (view == null) view = gameObject.AddComponent<HeavenOfficeView>();

        generator = new SoulDocumentGenerator();
        analytics = new HeavenOfficeAnalyticsLog(config != null && config.enableAnalyticsLog);
        manager = new GameSessionManager(config ?? new HeavenOfficeConfig(), generator, analytics);

        // Wire manager events to view
        manager.ShowDocumentRequested += (doc, current, total, language) => view.ShowDocument(doc, generator, current, total, language);
        manager.UpdateHudRequested += (score, current, total, mistakes, maxMistakes, combo, tier, language) => view.UpdateHud(score, current, total, mistakes, maxMistakes, combo, tier, language);
        manager.SetFeedbackRequested += (text, color, language) => view.SetFeedback(text, color, language);
        manager.ShowFinalPanelRequested += (title, score, correct, mistakes, maxCombo, reason, language) => view.ShowFinalPanel(title, score, correct, mistakes, maxCombo, reason, language);

        // Animation provider
        manager.PlayAnimationProvider = (stamp, hold) => view.PlayStampAnimation(stamp, hold);

        // Bind view callbacks to manager
        view.Bind(manager.OnStampSelected, () => StartCoroutine(manager.ResolveSelectedStampCoroutine()), manager.OnLanguageSelected, manager.StartSession, manager.StartSession);

        // Setup bootstrap and services
        bootstrap = FindObjectOfType<DivineOfficeBootstrap>();
        if (bootstrap == null)
        {
            GameObject go = new GameObject("DivineOfficeBootstrap");
            bootstrap = go.AddComponent<DivineOfficeBootstrap>();
        }

        // Subscribe localization changes to manager and view
        var loc = bootstrap.LocalizationService;
        if (loc != null)
        {
            loc.OnLanguageChanged += OnLanguageChanged;
            // Apply current language to manager/view
            manager.OnLanguageSelected(loc.CurrentLanguage == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian);
            view.ApplyLanguage(loc.CurrentLanguage == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian);
        }
    }

    private void OnLanguageChanged(string code)
    {
        var lang = code == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian;
        manager.OnLanguageSelected(lang);
        view.ApplyLanguage(lang);
    }
}
