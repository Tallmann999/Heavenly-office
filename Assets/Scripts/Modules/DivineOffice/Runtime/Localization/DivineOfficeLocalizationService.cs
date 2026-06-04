using System;
using System.Collections.Generic;
using UnityEngine;

public class DivineOfficeLocalizationService : IDivineOfficeLocalizationService
{
    public event Action<string> OnLanguageChanged;
    public string CurrentLanguage { get; private set; } = "ru";

    private readonly Dictionary<string, string> table = new Dictionary<string, string>();

    public DivineOfficeLocalizationService()
    {
    }

    public void LoadTable(DivineOfficeLocalizationTable soTable)
    {
        if (soTable == null) return;
        if (soTable.LanguageCode == null) return;
        if (soTable.LanguageCode != CurrentLanguage) return;

        table.Clear();
        foreach (var entry in soTable.Entries)
        {
            if (!string.IsNullOrEmpty(entry.Key)) table[entry.Key] = entry.Value;
        }
    }

    public string Get(string key)
    {
        return table.ContainsKey(key) ? table[key] : key;
    }

    public void SetLanguage(string languageCode)
    {
        if (languageCode == CurrentLanguage) return;
        CurrentLanguage = languageCode;
        OnLanguageChanged?.Invoke(CurrentLanguage);
    }
}
