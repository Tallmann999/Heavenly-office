using System;
using UnityEngine;

[Serializable]
public class HeavenOfficeConfig
{
    [Header("Session")]
    [Min(1)] public int sessionSoulCount = 12;
    [Min(0)] public int maxMistakeCount = 3;
    [Min(1)] public int difficultyRampStep = 4;

    [Header("Score")]
    [Min(0)] public int baseScoreReward = 100;
    [Min(0)] public int mistakePenalty = 75;
    [Min(0f)] public float comboScoreMultiplier = 0.1f;

    [Header("Difficulty")]
    [Min(0.1f)] public float reactionDelay = 0.55f;
    [Min(0.1f)] public float exitAnimationTime = 0.45f;

    [Header("Prototype")]
    public bool enableAnalyticsLog = true;
    public bool createUiAtRuntime = true;
}
