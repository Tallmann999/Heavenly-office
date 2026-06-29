using System.Collections.Generic;

public sealed class SoulContentDefinition
{
    public string Id;
    public SoulRarity Rarity;
    public int DemoDay;
    public int DemoQueuePosition;
    public StampType CorrectStamp;
    public string CorrectReincarnationId;
    public string RuName;
    public string EnName;
    public string RuSummary;
    public string EnSummary;
    public string RuGoodAct;
    public string EnGoodAct;
    public string RuBadAct;
    public string EnBadAct;
    public string RuIntention;
    public string EnIntention;
    public string RuHiddenTag;
    public string EnHiddenTag;

    public int JudgePointReward
    {
        get
        {
            switch (Rarity)
            {
                case SoulRarity.Rare:
                    return 15;
                case SoulRarity.Epic:
                    return 35;
                case SoulRarity.Legendary:
                    return 70;
                default:
                    return 10;
            }
        }
    }

    public int OfficeCoinReward
    {
        get
        {
            switch (Rarity)
            {
                case SoulRarity.Rare:
                    return 7;
                case SoulRarity.Epic:
                    return 12;
                case SoulRarity.Legendary:
                    return 20;
                default:
                    return 5;
            }
        }
    }

    public string NameKey => $"soul.{Id}.name";
    public string SummaryKey => $"soul.{Id}.summary";
    public string GoodActKey => $"soul.{Id}.good_1";
    public string BadActKey => $"soul.{Id}.bad_1";
    public string IntentionKey => $"soul.{Id}.intention_1";
    public string HiddenTagKey => $"soul.{Id}.hidden_1";

    public IEnumerable<LocalizationEntry> BuildLocalizationEntries(string languageCode)
    {
        bool english = languageCode == "en";
        yield return new LocalizationEntry { Key = NameKey, Value = english ? EnName : RuName };
        yield return new LocalizationEntry { Key = SummaryKey, Value = english ? EnSummary : RuSummary };
        yield return new LocalizationEntry { Key = GoodActKey, Value = english ? EnGoodAct : RuGoodAct };
        yield return new LocalizationEntry { Key = BadActKey, Value = english ? EnBadAct : RuBadAct };
        yield return new LocalizationEntry { Key = IntentionKey, Value = english ? EnIntention : RuIntention };
        yield return new LocalizationEntry { Key = HiddenTagKey, Value = english ? EnHiddenTag : RuHiddenTag };
    }
}
