using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class HeavenOfficeAdapter : MonoBehaviour
{
    [SerializeField] private HeavenOfficeConfig config = new HeavenOfficeConfig();

    private HeavenOfficeView view;
    private GameSessionManager manager;
    private SoulDocumentGenerator generator;
    private HeavenOfficeAnalyticsLog analytics;

    private void Awake()
    {
        if (config == null)
        {
            config = new HeavenOfficeConfig();
        }

        view = GetComponent<HeavenOfficeView>();
        if (view == null)
        {
            view = gameObject.AddComponent<HeavenOfficeView>();
        }

        view.BuildIfNeeded(config.createUiAtRuntime);

        generator = new SoulDocumentGenerator();
        analytics = new HeavenOfficeAnalyticsLog(config.enableAnalyticsLog);
        manager = new GameSessionManager(config, generator, analytics);

        // Wire manager events to view
        manager.ShowDocumentRequested += (doc, current, total, language) => view.ShowDocument(doc, generator, current, total, language);
        manager.UpdateHudRequested += (score, current, total, mistakes, maxMistakes, combo, tier, language) => view.UpdateHud(score, current, total, mistakes, maxMistakes, combo, tier, language);
        manager.SetFeedbackRequested += (text, color, language) => view.SetFeedback(text, color, language);
        manager.ShowFinalPanelRequested += (title, score, correct, mistakes, maxCombo, reason, language) => view.ShowFinalPanel(title, score, correct, mistakes, maxCombo, reason, language);

        // Provide animation provider
        manager.PlayAnimationProvider = (stamp, hold) => view.PlayStampAnimation(stamp, hold);

        // Bind view callbacks to manager
        view.Bind(manager.OnStampSelected, () => StartCoroutine(manager.ResolveSelectedStampCoroutine()), OnLanguageSelected, manager.StartSession, manager.StartSession);
    }

    private void Start()
    {
        view.ShowStartMenu();
    }

    private void OnLanguageSelected(HeavenOfficeLanguage language)
    {
        manager.OnLanguageSelected(language);
        view.ApplyLanguage(language);
    }
}
