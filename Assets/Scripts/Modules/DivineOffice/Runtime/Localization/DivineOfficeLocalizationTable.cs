using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DivineOffice/LocalizationTable")]
public class DivineOfficeLocalizationTable : ScriptableObject
{
    public string LanguageCode = "ru";
    public List<LocalizationEntry> Entries = new List<LocalizationEntry>();
}

[System.Serializable]
public class LocalizationEntry
{
    public string Key;
    [TextArea]
    public string Value;
}
