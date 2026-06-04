using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public static class DivineOfficeTestContentGenerator
{
    private const string BasePath = "Assets/Resources/DivineOffice/ScriptableObjects";

    [MenuItem("Tools/DivineOffice/Generate Test Content")]
    public static void Generate()
    {
        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
            AssetDatabase.Refresh();
        }

        GenerateLocalizationTables();
        GenerateReincarnations();
        GenerateSoulCases();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("DivineOffice test content generated.");
    }

    private static void GenerateLocalizationTables()
    {
        // RU
        var ruPath = Path.Combine(BasePath, "loc_ru.asset");
        var ru = ScriptableObject.CreateInstance<DivineOfficeLocalizationTable>();
        ru.LanguageCode = "ru";
        ru.Entries = new List<LocalizationEntry>
        {
            new LocalizationEntry { Key = "ui.title", Value = "Божественная канцелярия" },
            new LocalizationEntry { Key = "ui.start", Value = "Старт" },
            new LocalizationEntry { Key = "ui.next_soul", Value = "Следующая душа" },
            new LocalizationEntry { Key = "ui.collection_title", Value = "Архив карт" },
            new LocalizationEntry { Key = "ui.world_state_title", Value = "Состояние миров" },

            new LocalizationEntry { Key = "soul.dobry_starik.name", Value = "Добрый старик" },
            new LocalizationEntry { Key = "soul.dobry_starik.summary", Value = "Прожил честную жизнь, помогал соседям." },
            new LocalizationEntry { Key = "soul.dobry_starik.good_1", Value = "Кормил бездомных" },
            new LocalizationEntry { Key = "soul.dobry_starik.bad_1", Value = "Иногда воровал яблоки" },

            new LocalizationEntry { Key = "soul.greedy_merchant.name", Value = "Жадный купец" },
            new LocalizationEntry { Key = "soul.greedy_merchant.summary", Value = "Заработал состояние, обманывая клиентов." },
            new LocalizationEntry { Key = "soul.greedy_merchant.good_1", Value = "Поддержал городской рынок" },
            new LocalizationEntry { Key = "soul.greedy_merchant.bad_1", Value = "Обманывал в весах" },

            new LocalizationEntry { Key = "soul.soldier_repented.name", Value = "Солдат с раскаянием" },
            new LocalizationEntry { Key = "soul.soldier_repented.summary", Value = "Участвовал в боях, позже искал искупления." },

            new LocalizationEntry { Key = "soul.archive_error.name", Value = "Архивная ошибка" },
            new LocalizationEntry { Key = "soul.archive_error.summary", Value = "Документы содержат несоответствия и ошибки." },

            new LocalizationEntry { Key = "soul.reptiloid_taxman.name", Value = "Рептилоид-налоговик" },
            new LocalizationEntry { Key = "soul.reptiloid_taxman.summary", Value = "Суровый сборщик налогов межпланетной империи." },

            // Reincarnations RU
            new LocalizationEntry { Key = "reincarnation.angel_gardener.name", Value = "Ангел-садовник" },
            new LocalizationEntry { Key = "reincarnation.ordinary_human.name", Value = "Обычный человек" },
            new LocalizationEntry { Key = "reincarnation.monk_student.name", Value = "Монах-ученик" },
            new LocalizationEntry { Key = "reincarnation.dung_beetle.name", Value = "Навозный жук" },
            new LocalizationEntry { Key = "reincarnation.swamp_toad.name", Value = "Болотная жаба" },
            new LocalizationEntry { Key = "reincarnation.demon_intern.name", Value = "Демон-стажёр" },
            new LocalizationEntry { Key = "reincarnation.cosmic_slug.name", Value = "Космическая слизь" },
            new LocalizationEntry { Key = "reincarnation.guardian_firefly.name", Value = "Огненный светлячок" }
        };

        AssetDatabase.CreateAsset(ru, ruPath);

        // EN
        var enPath = Path.Combine(BasePath, "loc_en.asset");
        var en = ScriptableObject.CreateInstance<DivineOfficeLocalizationTable>();
        en.LanguageCode = "en";
        en.Entries = new List<LocalizationEntry>
        {
            new LocalizationEntry { Key = "ui.title", Value = "Divine Office" },
            new LocalizationEntry { Key = "ui.start", Value = "Start" },
            new LocalizationEntry { Key = "ui.next_soul", Value = "Next Soul" },
            new LocalizationEntry { Key = "ui.collection_title", Value = "Card Archive" },
            new LocalizationEntry { Key = "ui.world_state_title", Value = "World State" },

            new LocalizationEntry { Key = "soul.dobry_starik.name", Value = "Kind Old Man" },
            new LocalizationEntry { Key = "soul.dobry_starik.summary", Value = "Lived honestly, helped neighbors." },
            new LocalizationEntry { Key = "soul.dobry_starik.good_1", Value = "Fed the homeless" },
            new LocalizationEntry { Key = "soul.dobry_starik.bad_1", Value = "Occasionally stole apples" },

            new LocalizationEntry { Key = "soul.greedy_merchant.name", Value = "Greedy Merchant" },
            new LocalizationEntry { Key = "soul.greedy_merchant.summary", Value = "Made a fortune by cheating customers." },
            new LocalizationEntry { Key = "soul.greedy_merchant.good_1", Value = "Supported the city market" },
            new LocalizationEntry { Key = "soul.greedy_merchant.bad_1", Value = "Cheated on scales" },

            new LocalizationEntry { Key = "soul.soldier_repented.name", Value = "Soldier Repented" },
            new LocalizationEntry { Key = "soul.soldier_repented.summary", Value = "Fought in battles, later sought redemption." },

            new LocalizationEntry { Key = "soul.archive_error.name", Value = "Archive Error" },
            new LocalizationEntry { Key = "soul.archive_error.summary", Value = "Documents contain inconsistencies and errors." },

            new LocalizationEntry { Key = "soul.reptiloid_taxman.name", Value = "Reptiloid Taxman" },
            new LocalizationEntry { Key = "soul.reptiloid_taxman.summary", Value = "A stern taxman of an interplanetary empire." },

            // Reincarnations EN
            new LocalizationEntry { Key = "reincarnation.angel_gardener.name", Value = "Angel Gardener" },
            new LocalizationEntry { Key = "reincarnation.ordinary_human.name", Value = "Ordinary Human" },
            new LocalizationEntry { Key = "reincarnation.monk_student.name", Value = "Monk Student" },
            new LocalizationEntry { Key = "reincarnation.dung_beetle.name", Value = "Dung Beetle" },
            new LocalizationEntry { Key = "reincarnation.swamp_toad.name", Value = "Swamp Toad" },
            new LocalizationEntry { Key = "reincarnation.demon_intern.name", Value = "Demon Intern" },
            new LocalizationEntry { Key = "reincarnation.cosmic_slug.name", Value = "Cosmic Slug" },
            new LocalizationEntry { Key = "reincarnation.guardian_firefly.name", Value = "Guardian Firefly" }
        };

        AssetDatabase.CreateAsset(en, enPath);
    }

    private static void GenerateReincarnations()
    {
        var list = new List<(string id, StampType[] allowed, string world)> {
            ("angel_gardener", new[]{ StampType.Heaven }, "Heaven"),
            ("ordinary_human", new[]{ StampType.Heaven, StampType.Hell }, "HumanWorld"),
            ("monk_student", new[]{ StampType.Appeal }, "KarmaArchive"),
            ("dung_beetle", new[]{ StampType.Hell }, "Hell"),
            ("swamp_toad", new[]{ StampType.Hell }, "HumanWorld"),
            ("demon_intern", new[]{ StampType.Hell }, "Hell"),
            ("cosmic_slug", new[]{ StampType.Appeal, StampType.Audit }, "StrangeWorld"),
            ("guardian_firefly", new[]{ StampType.Heaven }, "Heaven"),
        };

        foreach (var item in list)
        {
            string path = Path.Combine(BasePath, $"reincarnation_{item.id}.asset");
            var so = ScriptableObject.CreateInstance<ReincarnationData>();
            so.Id = item.id;
            so.NameKey = $"reincarnation.{item.id}.name";
            so.DescriptionKey = $"reincarnation.{item.id}.description";
            so.AllowedStamps = item.allowed;
            so.WorldTarget = item.world;
            AssetDatabase.CreateAsset(so, path);
        }
    }

    private static void GenerateSoulCases()
    {
        var souls = new List<(string id, string namekey, StampType correct, string reinc)> {
            ("dobry_starik", "soul.dobry_starik.name", StampType.Heaven, "angel_gardener"),
            ("greedy_merchant", "soul.greedy_merchant.name", StampType.Hell, "dung_beetle"),
            ("soldier_repented", "soul.soldier_repented.name", StampType.Appeal, "monk_student"),
            ("archive_error", "soul.archive_error.name", StampType.Audit, ""),
            ("reptiloid_taxman", "soul.reptiloid_taxman.name", StampType.Appeal, "cosmic_slug")
        };

        foreach (var s in souls)
        {
            string path = Path.Combine(BasePath, $"soul_{s.id}.asset");
            var so = ScriptableObject.CreateInstance<SoulCaseData>();
            so.Id = s.id;
            so.NameKey = s.namekey;
            so.LifeSummaryKey = $"soul.{s.id}.summary";
            so.GoodActKeys = new List<string> { $"soul.{s.id}.good_1" };
            so.BadActKeys = new List<string> { $"soul.{s.id}.bad_1" };
            so.CorrectStamp = s.correct;
            so.CorrectReincarnationId = s.reinc;
            so.CardRewardId = $"card_{s.id}";
            AssetDatabase.CreateAsset(so, path);
        }
    }
}
