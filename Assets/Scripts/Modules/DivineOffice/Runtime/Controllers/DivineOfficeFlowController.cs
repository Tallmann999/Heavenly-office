using UnityEngine;

[DisallowMultipleComponent]
public class DivineOfficeFlowController : MonoBehaviour
{
    public DivineOfficeSaveService SaveService;
    public DivineOfficeLocalizationService LocalizationService;

    private DivineOfficeSaveData saveData;
    private SoulCaseData[] soulSOs;
    private int currentIndex = 0;
    private StampType? selectedStamp;
    private HeavenOfficeView view;
    private ReincarnationData[] reincarnationSOs;
    private bool sessionStarted;


    private void Awake()
    {
        // Ensure services exist
        if (SaveService == null) SaveService = new DivineOfficeSaveService();
        if (LocalizationService == null) LocalizationService = new DivineOfficeLocalizationService();
    }

    private void Start()
    {
        // Load save and initialize flow
        saveData = SaveService.Load();
        LocalizationService.SetLanguage(saveData.SelectedLanguage);

        view = FindObjectOfType<HeavenOfficeView>();

        // Load localization tables and SO content from Resources
        var locTables = Resources.LoadAll<DivineOfficeLocalizationTable>("DivineOffice/ScriptableObjects");
        foreach (var t in locTables)
        {
            if (t.LanguageCode == LocalizationService.CurrentLanguage)
            {
                LocalizationService.LoadTable(t);
                break;
            }
        }

        // Load soul cases
        soulSOs = Resources.LoadAll<SoulCaseData>("DivineOffice/ScriptableObjects");
        reincarnationSOs = Resources.LoadAll<ReincarnationData>("DivineOffice/ScriptableObjects");
        if (soulSOs == null || soulSOs.Length == 0)
        {
            Debug.LogWarning("No SoulCaseData found in Resources/DivineOffice/ScriptableObjects");
            return;
        }

        // Ensure view exists and bind controls
        if (view != null)
        {
            view.BuildIfNeeded(true);
            view.Bind(OnStampSelected, OnStampTargetPressed, OnLanguageSelected, OnStartRequested, OnRestartRequested);
            view.ApplyLanguage(LocalizationService.CurrentLanguage == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian);
            view.ShowStartMenu();
        }

        // Find first unprocessed soul
        currentIndex = 0;
        while (currentIndex < soulSOs.Length && saveData.ProcessedSoulIds.Contains(soulSOs[currentIndex].Id)) currentIndex++;
    }

    private void ShowCurrentSoul()
    {
        if (currentIndex >= soulSOs.Length)
        {
            // End of queue
            if (view != null) view.ShowFinalPanel(LocalizationService.Get("ui.shift_complete"), 0, 0, 0, 0, SessionEndReason.QueueCompleted, LocalizationService.CurrentLanguage == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian);
            return;
        }

        var so = soulSOs[currentIndex];
        var doc = new SoulDocumentData();
        var lang = LocalizationService.CurrentLanguage == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian;
        doc.soulName = LocalizationService.Get(so.NameKey);
        doc.age = 0;
        doc.lifeSummary = LocalizationService.Get(so.LifeSummaryKey);
        foreach (var g in so.GoodActKeys) doc.goodActs.Add(LocalizationService.Get(g));
        foreach (var b in so.BadActKeys) doc.badActs.Add(LocalizationService.Get(b));
        doc.expectedStamp = so.CorrectStamp;
        doc.ruleExplanation = LocalizationService.Get("ui.rule_hint");
        doc.difficultyTier = 0;
        doc.timeLimit = 15f;

        if (view != null)
        {
            var generator = new SoulDocumentGenerator();
            view.ShowDocument(doc, generator, currentIndex + 1, soulSOs.Length, lang);
            view.UpdateHud(0, currentIndex + 1, soulSOs.Length, 0, 3, 0, doc.difficultyTier, lang);
            view.SetFeedback(LocalizationService.Get("ui.waiting_for_stamp"), new Color(0.25f, 0.27f, 0.3f), lang);
        }
        selectedStamp = null;
    }

    private void OnStampSelected(StampType stamp)
    {
        if (!sessionStarted) return;

        selectedStamp = stamp;
        if (view != null)
        {
            view.SetFeedback(LocalizationService.Get("ui.stamp_selected"), new Color(0.1f, 0.48f, 0.2f), LocalizationService.CurrentLanguage == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian);
        }
    }

    private void OnStampTargetPressed()
    {
        if (!sessionStarted) return;
        if (!selectedStamp.HasValue) return;

        var so = soulSOs[currentIndex];
        bool correct = selectedStamp.Value == so.CorrectStamp;

        // Update saveData
        if (!saveData.ProcessedSoulIds.Contains(so.Id)) saveData.ProcessedSoulIds.Add(so.Id);
        if (correct && !string.IsNullOrEmpty(so.CardRewardId) && !saveData.UnlockedCardIds.Contains(so.CardRewardId))
        {
            saveData.UnlockedCardIds.Add(so.CardRewardId);
            // reward points for unlocking a card
            saveData.KarmaPoints += 10;
            saveData.OfficeCoins += 5;
        }

        // Persist
        SaveService.Save(saveData);

        // Show result screen (try to use prefab in Resources)
        var resultPrefab = Resources.Load<GameObject>("DivineOffice/Prefabs/UI/PF_ReincarnationResultScreen");
        if (resultPrefab != null)
        {
            var inst = Instantiate(resultPrefab);
            // attempt to find TMP fields and fill dynamic content
            var original = inst.transform.Find("OriginalSoul")?.GetComponent<TMPro.TextMeshProUGUI>();
            var newForm = inst.transform.Find("NewForm")?.GetComponent<TMPro.TextMeshProUGUI>();
            var reason = inst.transform.Find("Reason")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (original != null) original.text = LocalizationService.Get(so.NameKey);
            // find reincarnation SO
            ReincarnationData rso = null;
            if (!string.IsNullOrEmpty(so.CorrectReincarnationId) && reincarnationSOs != null)
            {
                foreach (var r in reincarnationSOs) if (r.Id == so.CorrectReincarnationId) { rso = r; break; }
            }
            if (newForm != null) newForm.text = rso != null ? LocalizationService.Get(rso.NameKey) : "";
            if (reason != null) reason.text = LocalizationService.Get(so.LifeSummaryKey);
        }
        else
        {
            // fallback: use view feedback
            if (view != null)
            {
                var lang = LocalizationService.CurrentLanguage == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian;
                string feedbackKey = correct ? "ui.decision_correct" : "ui.decision_wrong";
                view.SetFeedback(LocalizationService.Get(feedbackKey), correct ? new Color(0.1f, 0.48f, 0.2f) : new Color(0.68f, 0.12f, 0.1f), lang);
            }
        }

        currentIndex++;
        ShowCurrentSoul();
    }

    private void OnLanguageSelected(HeavenOfficeLanguage lang)
    {
        string code = lang == HeavenOfficeLanguage.English ? "en" : "ru";
        LocalizationService.SetLanguage(code);
        LoadLocalizationTable();
        saveData.SelectedLanguage = code;
        SaveService.Save(saveData);
        view?.ApplyLanguage(lang);
    }

    private void OnStartRequested()
    {
        sessionStarted = true;
        view?.HideStartMenu();
        view?.HideFinalPanel();
        ShowCurrentSoul();
    }

    private void OnRestartRequested()
    {
        // reset progress
        saveData = new DivineOfficeSaveData();
        SaveService.ResetSave();
        SaveService.Save(saveData);
        currentIndex = 0;
        sessionStarted = true;
        view?.HideStartMenu();
        view?.HideFinalPanel();
        ShowCurrentSoul();
    }

    private void LoadLocalizationTable()
    {
        var locTables = Resources.LoadAll<DivineOfficeLocalizationTable>("DivineOffice/ScriptableObjects");
        foreach (var t in locTables)
        {
            if (t.LanguageCode == LocalizationService.CurrentLanguage)
            {
                LocalizationService.LoadTable(t);
                return;
            }
        }
    }
}
