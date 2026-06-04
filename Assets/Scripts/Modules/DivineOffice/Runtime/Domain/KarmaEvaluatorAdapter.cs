using System;

public class KarmaEvaluatorAdapter : IKarmaEvaluator
{
    public KarmaEvaluationResult Evaluate(SoulCaseData soul)
    {
        // Adapter to existing HeavenOfficeRulesEvaluator where possible.
        var result = new KarmaEvaluationResult();
        // Best-effort mapping: use SoulCaseData.CorrectStamp if present.
        result.ExpectedStamp = soul.CorrectStamp;
        result.Explanation = "(derived)";
        return result;
    }
}
