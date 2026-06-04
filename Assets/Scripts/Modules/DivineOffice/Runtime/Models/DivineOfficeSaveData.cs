using System;
using System.Collections.Generic;

[Serializable]
public class DivineOfficeSaveData
{
    public int Version = 1;
    public string SelectedLanguage = "ru";
    public int CurrentDay = 1;
    public List<string> ProcessedSoulIds = new List<string>();
    public List<string> UnlockedCardIds = new List<string>();
    public int KarmaPoints = 0;
    public int OfficeCoins = 0;
}
