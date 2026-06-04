using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTextBinder : MonoBehaviour
{
    public string Key;
    private TextMeshProUGUI text;
    private IDivineOfficeLocalizationService localizationService;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        // Try to find a runtime service instance (will be wired by controller/adapter)
        localizationService = FindObjectOfType<DivineOfficeBootstrap>()?.LocalizationService;
        Refresh();
    }

    private void OnEnable()
    {
        if (localizationService != null) localizationService.OnLanguageChanged += OnLangChanged;
    }

    private void OnDisable()
    {
        if (localizationService != null) localizationService.OnLanguageChanged -= OnLangChanged;
    }

    private void OnLangChanged(string code)
    {
        Refresh();
    }

    public void Refresh()
    {
        if (text == null || localizationService == null || string.IsNullOrEmpty(Key)) return;
        text.text = localizationService.Get(Key);
    }
}
