using System;
using System.Collections.Generic;

[Serializable]
public class SoulDocumentData
{
    public string soulName;
    public int age;
    public string lifeSummary;
    public List<string> goodActs = new List<string>();
    public List<string> badActs = new List<string>();
    public List<SoulDocumentTag> tags = new List<SoulDocumentTag>();
    public StampType expectedStamp;
    public string ruleExplanation;
    public int difficultyTier;
    public float timeLimit;

    public string CaseType
    {
        get
        {
            if (tags.Contains(SoulDocumentTag.ArchiveError)) return "archive_error";
            if (tags.Contains(SoulDocumentTag.IncompleteSignature)) return "appeal_form";
            if (tags.Contains(SoulDocumentTag.SelfishGoodActs) || tags.Contains(SoulDocumentTag.ForgivenBadAct)) return "weighted_morality";
            return "basic_morality";
        }
    }
}
