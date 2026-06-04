using UnityEngine;

[DisallowMultipleComponent]
public class DivineOfficeFlowController : MonoBehaviour
{
    public DivineOfficeSaveService SaveService;
    public DivineOfficeLocalizationService LocalizationService;

    private void Awake()
    {
        // Ensure services exist
        if (SaveService == null) SaveService = new DivineOfficeSaveService();
        if (LocalizationService == null) LocalizationService = new DivineOfficeLocalizationService();
    }

    private void Start()
    {
        // Load save and initialize flow
        DivineOfficeSaveData data = SaveService.Load();
        LocalizationService.SetLanguage(data.SelectedLanguage);

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
        var souls = Resources.LoadAll<SoulCaseData>("DivineOffice/ScriptableObjects");
        if (souls != null && souls.Length > 0)
        {
            // Build a simple SoulDocumentData from first SO and show via existing HeavenOfficeView
            var first = souls[0];
            var doc = new SoulDocumentData();
            var lang = LocalizationService.CurrentLanguage == "en" ? HeavenOfficeLanguage.English : HeavenOfficeLanguage.Russian;
            doc.soulName = LocalizationService.Get(first.NameKey);
            doc.age = 0;
            doc.lifeSummary = LocalizationService.Get(first.LifeSummaryKey);
            foreach (var g in first.GoodActKeys) doc.goodActs.Add(LocalizationService.Get(g));
            foreach (var b in first.BadActKeys) doc.badActs.Add(LocalizationService.Get(b));
            doc.expectedStamp = first.CorrectStamp;
            doc.ruleExplanation = "";
            doc.difficultyTier = 0;
            doc.timeLimit = 15f;

            // Find view and show
            var view = FindObjectOfType<HeavenOfficeView>();
            if (view != null)
            {
                var generator = new SoulDocumentGenerator();
                view.BuildIfNeeded(true);
                view.Bind(null, null, null, null, null);
                view.ShowDocument(doc, generator, 1, souls.Length, lang);
                view.UpdateHud(0, 1, souls.Length, 0, 3, 0, doc.difficultyTier, lang);
            }
        }
    }
}
