using System;
using System.Collections;

public interface IHeavenOfficeView
{
    void Bind(Action<StampType> stampSelected, Action stampTargetPressed, Action<HeavenOfficeLanguage> languageSelected, Action start, Action restart);
    void ShowDocument(SoulDocumentData document, SoulDocumentGenerator generator, int current, int total, HeavenOfficeLanguage language);
    void UpdateHud(int score, int current, int total, int mistakes, int maxMistakes, int combo, int tier, HeavenOfficeLanguage language);
    IEnumerator PlayStampAnimation(StampType stamp, float holdTime);
    void SetFeedback(string feedback, UnityEngine.Color color, HeavenOfficeLanguage language);
    void ShowStartMenu();
    void HideStartMenu();
    void ShowFinalPanel(string title, int score, int correct, int mistakes, int maxCombo, SessionEndReason reason, HeavenOfficeLanguage language);
}
