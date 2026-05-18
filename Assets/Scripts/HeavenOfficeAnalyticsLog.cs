using System.Collections.Generic;
using UnityEngine;

public class HeavenOfficeAnalyticsLog
{
    public readonly List<string> Events = new List<string>();
    private readonly bool enabled;

    public HeavenOfficeAnalyticsLog(bool enabled)
    {
        this.enabled = enabled;
    }

    public void Log(string eventName, SoulDocumentData document = null, StampType? selected = null, DecisionResultType? result = null, float reactionTime = 0f, int tier = 0)
    {
        if (!enabled) return;

        string payload = eventName;
        if (document != null)
        {
            payload += $" | case={document.CaseType} tier={tier} selected={selected?.ToString() ?? "None"} expected={document.expectedStamp} time={reactionTime:0.00} result={result?.ToString() ?? "None"} good={document.goodActs.Count} bad={document.badActs.Count} tags={string.Join(",", document.tags)}";
        }

        Events.Add(payload);
        Debug.Log("[HeavenOfficeAnalytics] " + payload);
    }
}
