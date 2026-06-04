using System;

public interface IDivineOfficeLocalizationService
{
    event Action<string> OnLanguageChanged; // language code
    string CurrentLanguage { get; }
    string Get(string key);
    void SetLanguage(string languageCode);
}
