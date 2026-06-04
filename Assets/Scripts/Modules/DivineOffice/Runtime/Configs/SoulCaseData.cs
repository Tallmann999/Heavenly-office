using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DivineOffice/SoulCaseData")]
public class SoulCaseData : ScriptableObject
{
    public string Id;
    public string NameKey;
    public Sprite Portrait;
    public string LifeSummaryKey;
    public List<string> GoodActKeys = new List<string>();
    public List<string> BadActKeys = new List<string>();
    public List<string> IntentionKeys = new List<string>();
    public List<string> HiddenTagKeys = new List<string>();
    public StampType CorrectStamp;
    public string CorrectReincarnationId;
    public string CardRewardId;
}
