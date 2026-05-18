using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HeavenOfficeRulesEvaluator
{
    public static RuleEvaluation Evaluate(SoulDocumentData document, IReadOnlyCollection<StampType> availableStamps)
    {
        if (document.tags.Contains(SoulDocumentTag.ArchiveError) && availableStamps.Contains(StampType.Audit))
        {
            return new RuleEvaluation
            {
                expectedStamp = StampType.Audit,
                explanation = "Архивная ошибка имеет высший приоритет: нужна печать «Проверка»."
            };
        }

        if (document.tags.Contains(SoulDocumentTag.IncompleteSignature) && availableStamps.Contains(StampType.Appeal))
        {
            return new RuleEvaluation
            {
                expectedStamp = StampType.Appeal,
                explanation = "Неполная подпись отправляет дело на апелляцию."
            };
        }

        float goodWeight = document.goodActs.Count;
        float badWeight = document.badActs.Count;

        if (document.tags.Contains(SoulDocumentTag.SelfishGoodActs))
        {
            goodWeight = Mathf.Max(0f, goodWeight - 1f);
        }

        if (document.tags.Contains(SoulDocumentTag.ForgivenBadAct))
        {
            badWeight = Mathf.Max(0f, badWeight - 1f);
        }

        if (Mathf.Approximately(goodWeight, badWeight))
        {
            if (availableStamps.Contains(StampType.Appeal))
            {
                return new RuleEvaluation
                {
                    expectedStamp = StampType.Appeal,
                    explanation = "После пересчёта баланс равный: дело уходит на апелляцию."
                };
            }

            return new RuleEvaluation
            {
                expectedStamp = StampType.Heaven,
                explanation = "Ранний регламент спорные дела не выдаёт; этот случай трактуется мягко."
            };
        }

        return new RuleEvaluation
        {
            expectedStamp = goodWeight > badWeight ? StampType.Heaven : StampType.Hell,
            explanation = goodWeight > badWeight
                ? "Добрых поступков больше: ставь «Рай»."
                : "Плохих поступков больше: ставь «Ад»."
        };
    }

    public static List<StampType> GetAvailableStamps(int tier)
    {
        if (tier < 2)
        {
            return new List<StampType> { StampType.Heaven, StampType.Hell };
        }

        return new List<StampType> { StampType.Heaven, StampType.Hell, StampType.Appeal, StampType.Audit };
    }
}
