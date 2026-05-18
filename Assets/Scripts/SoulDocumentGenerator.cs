using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoulDocumentGenerator
{
    private readonly System.Random random = new System.Random();
    private readonly string[] names =
    {
        "Аглая Синицына", "Пётр Ветров", "Марта Звонарёва", "Григорий Листков",
        "Нина Протоколова", "Семён Лампадный", "Ирина Перьева", "Фома Печаткин",
        "Леонид Скрепкин", "Вера Облачная", "Клим Резолюций", "Алла Нимбова"
    };

    private readonly string[] summaries =
    {
        "жил тихо, но часто спорил с квитанциями",
        "работал бухгалтером и верил в силу аккуратной папки",
        "держал маленькую пекарню рядом с трамвайной остановкой",
        "чинил чужие зонты и свои оправдания",
        "руководил домовым чатом с подозрительной энергией",
        "учил детей математике и взрослых терпению",
        "водил автобус и коллекционировал потерянные варежки",
        "писал заявления красивым почерком",
        "работал в архиве и боялся пустых полок",
        "сажал цветы там, где раньше стояли таблички «нельзя»",
        "жил быстро, но чеки хранил в идеальном порядке",
        "пел в хоре и иногда фальшивил по принципиальным причинам"
    };

    private readonly string[] goodActs =
    {
        "вернул потерянный кошелёк", "помог соседке донести покупки",
        "уступал место в переполненном автобусе", "не списывал на экзамене, хотя мог",
        "кормил бездомных животных", "помирил двух старых друзей",
        "платил налоги без театральных вздохов", "сдал найденный телефон в бюро находок",
        "посадил деревья во дворе", "защитил стажёра на совещании",
        "починил лифт и не оставил записку с упрёком", "покупал лекарства пожилому соседу",
        "признал ошибку в отчёте", "спас семейный фотоальбом из пожара",
        "не шумел во время чужого сна", "перевёл бабушку через цифровой банк",
        "делился зонтом у проходной", "отдал премию на лечение коллеги",
        "обучал новичков без сарказма", "оставлял честные чаевые"
    };

    private readonly string[] badActs =
    {
        "парковался на месте для ангелов", "врал в налоговой декларации",
        "шумел перфоратором в 7 утра", "крал офисные печенья и винил стажёра",
        "перебивал всех на семейных советах", "подделывал подписи в журнале выдачи нимбов",
        "оставлял тележку посреди парковки", "продавал сломанные зонты как антиквариат",
        "не возвращал книги в библиотеку", "распускал слухи в очереди",
        "прятал чужие кружки в архиве", "портил общий чайник на работе",
        "обещал помочь и исчезал до понедельника", "рисовал усы на портретах директоров",
        "накручивал отзывы собственной пекарне", "спорил с врачами по комментариям",
        "выдавал чужие идеи за свои", "забывал выключать музыку ночью",
        "ставил срочно на каждую бумагу", "сваливал ошибки на принтер"
    };

    private readonly Dictionary<SoulDocumentTag, string> tagDescriptions = new Dictionary<SoulDocumentTag, string>
    {
        { SoulDocumentTag.SelfishGoodActs, "часть добрых поступков совершена ради выгоды" },
        { SoulDocumentTag.ForgivenBadAct, "один плохой поступок прощён по заявлению пострадавшего" },
        { SoulDocumentTag.IncompleteSignature, "подпись земного архива неполная" },
        { SoulDocumentTag.ArchiveError, "обнаружена ошибка в архивной карточке" },
        { SoulDocumentTag.UrgentCase, "срочное дело: очередь нервничает" }
    };

    public List<SoulDocumentData> BuildSession(HeavenOfficeConfig config, HeavenOfficeLanguage language)
    {
        var result = new List<SoulDocumentData>();
        for (int i = 0; i < config.sessionSoulCount; i++)
        {
            int tier = Mathf.Clamp(i / Mathf.Max(1, config.difficultyRampStep), 0, 3);
            result.Add(CreateDocument(tier, config, i, language));
        }

        return result;
    }

    public string GetTagDescription(SoulDocumentTag tag)
    {
        return tagDescriptions.TryGetValue(tag, out string description) ? description : "особых пометок нет";
    }

    public string GetTagDescription(SoulDocumentTag tag, HeavenOfficeLanguage language)
    {
        if (language == HeavenOfficeLanguage.English)
        {
            switch (tag)
            {
                case SoulDocumentTag.SelfishGoodActs: return "some good deeds were done for profit";
                case SoulDocumentTag.ForgivenBadAct: return "one bad deed was officially forgiven";
                case SoulDocumentTag.IncompleteSignature: return "earth archive signature is incomplete";
                case SoulDocumentTag.ArchiveError: return "archive card contains an error";
                case SoulDocumentTag.UrgentCase: return "urgent case: the queue is restless";
                default: return "no special notes";
            }
        }

        return GetTagDescription(tag);
    }

    private SoulDocumentData CreateDocument(int tier, HeavenOfficeConfig config, int index, HeavenOfficeLanguage language)
    {
        var available = HeavenOfficeRulesEvaluator.GetAvailableStamps(tier);
        for (int attempt = 0; attempt < 40; attempt++)
        {
            SoulDocumentData document = BuildCandidate(tier, config, index + attempt, language);
            RuleEvaluation evaluation = HeavenOfficeRulesEvaluator.Evaluate(document, available);
            document.expectedStamp = evaluation.expectedStamp;
            document.ruleExplanation = BuildHint(document, evaluation.explanation, language);

            if (available.Contains(document.expectedStamp) && (tier >= 2 || document.expectedStamp == StampType.Heaven || document.expectedStamp == StampType.Hell))
            {
                return document;
            }
        }

        SoulDocumentData fallback = BuildSimpleCandidate(tier, config, index, language);
        RuleEvaluation fallbackEvaluation = HeavenOfficeRulesEvaluator.Evaluate(fallback, available);
        fallback.expectedStamp = fallbackEvaluation.expectedStamp;
        fallback.ruleExplanation = BuildHint(fallback, fallbackEvaluation.explanation, language);
        return fallback;
    }

    private SoulDocumentData BuildCandidate(int tier, HeavenOfficeConfig config, int seedOffset, HeavenOfficeLanguage language)
    {
        int goodCount = tier == 0 ? random.Next(2, 4) : random.Next(1, 4);
        int badCount = tier == 0 ? random.Next(0, 2) : random.Next(1, 4);

        if (tier == 0 && goodCount == badCount)
        {
            goodCount++;
        }

        var tags = new List<SoulDocumentTag>();
        if (tier >= 1 && random.NextDouble() < 0.45)
        {
            tags.Add(random.NextDouble() < 0.5 ? SoulDocumentTag.SelfishGoodActs : SoulDocumentTag.ForgivenBadAct);
        }

        if (tier >= 2 && random.NextDouble() < 0.45)
        {
            tags.Clear();
            tags.Add(random.NextDouble() < 0.5 ? SoulDocumentTag.IncompleteSignature : SoulDocumentTag.ArchiveError);
        }

        if (tier >= 3 && random.NextDouble() < 0.45 && !tags.Contains(SoulDocumentTag.UrgentCase))
        {
            tags.Add(SoulDocumentTag.UrgentCase);
        }

        return new SoulDocumentData
        {
            soulName = PickName(seedOffset, language),
            age = random.Next(35, 91),
            lifeSummary = PickSummary(language),
            goodActs = Pick(GetGoodActs(language), goodCount),
            badActs = Pick(GetBadActs(language), badCount),
            tags = tags,
            difficultyTier = tier,
            timeLimit = CalculateTimeLimit(config, tier, tags)
        };
    }

    private SoulDocumentData BuildSimpleCandidate(int tier, HeavenOfficeConfig config, int index, HeavenOfficeLanguage language)
    {
        bool heaven = index % 2 == 0;
        var tags = new List<SoulDocumentTag>();
        return new SoulDocumentData
        {
            soulName = GetNames(language)[index % GetNames(language).Length],
            age = random.Next(35, 91),
            lifeSummary = GetSummaries(language)[index % GetSummaries(language).Length],
            goodActs = Pick(GetGoodActs(language), heaven ? 3 : 1),
            badActs = Pick(GetBadActs(language), heaven ? 1 : 3),
            tags = tags,
            difficultyTier = tier,
            timeLimit = CalculateTimeLimit(config, tier, tags)
        };
    }

    private float CalculateTimeLimit(HeavenOfficeConfig config, int tier, List<SoulDocumentTag> tags)
    {
        float time = config.documentReadTimeLimit;
        if (tier >= 2)
        {
            time *= config.tierTwoTimeMultiplier;
        }

        if (tags.Contains(SoulDocumentTag.UrgentCase))
        {
            time *= config.urgentTimeMultiplier;
        }

        return Mathf.Max(4f, time);
    }

    private string BuildHint(SoulDocumentData document, string evaluation, HeavenOfficeLanguage language)
    {
        if (language == HeavenOfficeLanguage.English)
        {
            if (document.tags.Contains(SoulDocumentTag.ArchiveError)) return "Archive error: use Audit.";
            if (document.tags.Contains(SoulDocumentTag.IncompleteSignature)) return "Incomplete signature: use Appeal.";
            if (document.tags.Contains(SoulDocumentTag.SelfishGoodActs)) return "Selfish good deed: ignore one good act.";
            if (document.tags.Contains(SoulDocumentTag.ForgivenBadAct)) return "Forgiven bad deed: ignore one bad act.";
            if (document.tags.Contains(SoulDocumentTag.UrgentCase)) return "Urgent case: less time, same rule.";
            return "More good acts means Heaven. More bad acts means Hell.";
        }

        if (document.tags.Contains(SoulDocumentTag.ArchiveError)) return "Пометка «Архивная ошибка» отправляет дело на «Проверку».";
        if (document.tags.Contains(SoulDocumentTag.IncompleteSignature)) return "Пометка «Неполная подпись» отправляет дело на «Апелляцию».";
        if (document.tags.Contains(SoulDocumentTag.SelfishGoodActs)) return "Особая пометка «Корысть»: один добрый поступок не учитывается.";
        if (document.tags.Contains(SoulDocumentTag.ForgivenBadAct)) return "Пометка «Прощённый поступок»: один плохой поступок не учитывается.";
        if (document.tags.Contains(SoulDocumentTag.UrgentCase)) return "Срочное дело: времени меньше, правило морали прежнее.";
        return "Базовое правило: если хороших поступков больше, ставь «Рай». Если плохих больше, ставь «Ад».";
    }

    private readonly string[] englishNames =
    {
        "Agatha Quill", "Peter Gale", "Martha Bell", "Gregory Ledger",
        "Nina Clerk", "Simon Halo", "Irene Feather", "Thomas Stamp",
        "Leonid Binder", "Vera Cloud", "Clement Forms", "Alice Nimbus"
    };

    private readonly string[] englishSummaries =
    {
        "kept receipts in perfect order", "worked as an accountant and trusted folders",
        "ran a bakery near the tram stop", "fixed umbrellas and excuses",
        "managed a building chat with suspicious energy", "taught children math and adults patience",
        "drove a bus and collected lost mittens", "wrote beautiful applications",
        "worked in an archive and feared empty shelves", "planted flowers where signs said no",
        "lived fast but filed every receipt", "sang in a choir and sometimes argued with melody"
    };

    private readonly string[] englishGoodActs =
    {
        "returned a lost wallet", "helped a neighbor carry groceries",
        "gave up a seat on a crowded bus", "did not cheat on an exam",
        "fed stray animals", "reconciled two old friends",
        "paid taxes without dramatic sighs", "turned in a found phone",
        "planted trees in the yard", "protected an intern at a meeting",
        "fixed an elevator without leaving a complaint", "bought medicine for an elderly neighbor",
        "admitted a mistake in a report", "saved a family photo album from fire",
        "kept quiet during someone else's sleep", "helped a grandmother with online banking",
        "shared an umbrella at the gate", "donated a bonus for a coworker's treatment",
        "trained newcomers without sarcasm", "left honest tips"
    };

    private readonly string[] englishBadActs =
    {
        "parked in a spot reserved for angels", "lied on a tax declaration",
        "used a drill at 7 a.m.", "stole office cookies and blamed an intern",
        "interrupted every family council", "forged signatures in the halo log",
        "left a shopping cart in the road", "sold broken umbrellas as antiques",
        "did not return library books", "spread rumors in the queue",
        "hid other people's mugs in the archive", "broke the shared office kettle",
        "promised help and vanished until Monday", "drew mustaches on director portraits",
        "boosted reviews for their own bakery", "argued with doctors in comment sections",
        "claimed other people's ideas", "forgot to turn off music at night",
        "marked every paper as urgent", "blamed mistakes on the printer"
    };

    private string[] GetNames(HeavenOfficeLanguage language)
    {
        return language == HeavenOfficeLanguage.English ? englishNames : names;
    }

    private string[] GetSummaries(HeavenOfficeLanguage language)
    {
        return language == HeavenOfficeLanguage.English ? englishSummaries : summaries;
    }

    private string[] GetGoodActs(HeavenOfficeLanguage language)
    {
        return language == HeavenOfficeLanguage.English ? englishGoodActs : goodActs;
    }

    private string[] GetBadActs(HeavenOfficeLanguage language)
    {
        return language == HeavenOfficeLanguage.English ? englishBadActs : badActs;
    }

    private string PickName(int seedOffset, HeavenOfficeLanguage language)
    {
        string[] pool = GetNames(language);
        return pool[(seedOffset + random.Next(pool.Length)) % pool.Length];
    }

    private string PickSummary(HeavenOfficeLanguage language)
    {
        string[] pool = GetSummaries(language);
        return pool[random.Next(pool.Length)];
    }

    private List<string> Pick(string[] source, int count)
    {
        return source.OrderBy(_ => random.Next()).Take(Mathf.Clamp(count, 1, 3)).ToList();
    }
}
