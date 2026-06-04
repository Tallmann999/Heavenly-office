using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public static class DivineOfficePrefabAssembler
{
    private const string PrefabPath = "Assets/Content/DivineOffice/Prefabs/UI";

    [MenuItem("Tools/DivineOffice/Assemble UI Prefabs")]
    public static void AssemblePrefabs()
    {
        if (!Directory.Exists(PrefabPath)) Directory.CreateDirectory(PrefabPath);

        CreateMainScreenPrefab();
        CreateReincarnationResultPrefab();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("DivineOffice UI prefabs assembled.");
    }

    private static GameObject CreateBaseCanvas(string rootName)
    {
        var canvasGO = new GameObject(rootName);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        return canvasGO;
    }

    private static void AddBackground(Transform parent)
    {
        var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(parent, false);
        var rt = bg.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = bg.GetComponent<Image>();
        img.color = new Color(0.95f, 0.95f, 0.95f);
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, int size, TextAlignmentOptions align)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var txt = go.GetComponent<TextMeshProUGUI>();
        txt.fontSize = size;
        txt.alignment = align;
        txt.text = name;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 80);
        return txt;
    }

    private static void AddLocalizedBinder(TextMeshProUGUI tmp, string key)
    {
        var binder = tmp.gameObject.AddComponent<LocalizedTextBinder>();
        binder.Key = key;
    }

    private static void CreateMainScreenPrefab()
    {
        var canvasGO = CreateBaseCanvas("PF_DivineOfficeMainScreen");
        AddBackground(canvasGO.transform);

        var header = CreateText(canvasGO.transform, "Title", 36, TextAlignmentOptions.Center);
        header.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        header.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        header.rectTransform.pivot = new Vector2(0.5f, 1f);
        header.rectTransform.anchoredPosition = new Vector2(0f, -40f);
        AddLocalizedBinder(header, "ui.title");

        var startBtn = new GameObject("StartButton", typeof(RectTransform), typeof(Image), typeof(Button));
        startBtn.transform.SetParent(canvasGO.transform, false);
        var btnRt = startBtn.GetComponent<RectTransform>();
        btnRt.sizeDelta = new Vector2(240, 80);
        btnRt.anchorMin = new Vector2(0.5f, 0f);
        btnRt.anchorMax = new Vector2(0.5f, 0f);
        btnRt.anchoredPosition = new Vector2(0f, 80f);
        var img = startBtn.GetComponent<Image>(); img.color = new Color(0.22f, 0.6f, 0.86f);

        var btnText = CreateText(startBtn.transform, "StartText", 24, TextAlignmentOptions.Center);
        btnText.rectTransform.anchoredPosition = Vector2.zero;
        AddLocalizedBinder(btnText, "ui.start");

        // Save prefab
        string path = Path.Combine(PrefabPath, "PF_DivineOfficeMainScreen.prefab");
        PrefabUtility.SaveAsPrefabAsset(canvasGO, path);
        Object.DestroyImmediate(canvasGO);
    }

    private static void CreateReincarnationResultPrefab()
    {
        var canvasGO = CreateBaseCanvas("PF_ReincarnationResultScreen");
        AddBackground(canvasGO.transform);

        var title = CreateText(canvasGO.transform, "ResultTitle", 30, TextAlignmentOptions.Center);
        title.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        title.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        title.rectTransform.pivot = new Vector2(0.5f, 1f);
        title.rectTransform.anchoredPosition = new Vector2(0f, -40f);
        AddLocalizedBinder(title, "ui.result_title");

        var original = CreateText(canvasGO.transform, "OriginalSoul", 22, TextAlignmentOptions.Left);
        original.rectTransform.anchorMin = new Vector2(0.1f, 0.65f);
        original.rectTransform.anchorMax = new Vector2(0.4f, 0.9f);
        original.rectTransform.anchoredPosition = Vector2.zero;
        AddLocalizedBinder(original, "result.original_label");

        var newForm = CreateText(canvasGO.transform, "NewForm", 22, TextAlignmentOptions.Left);
        newForm.rectTransform.anchorMin = new Vector2(0.6f, 0.65f);
        newForm.rectTransform.anchorMax = new Vector2(0.9f, 0.9f);
        newForm.rectTransform.anchoredPosition = Vector2.zero;
        AddLocalizedBinder(newForm, "result.newform_label");

        var reason = CreateText(canvasGO.transform, "Reason", 20, TextAlignmentOptions.Center);
        reason.rectTransform.anchorMin = new Vector2(0.1f, 0.4f);
        reason.rectTransform.anchorMax = new Vector2(0.9f, 0.6f);
        reason.rectTransform.anchoredPosition = Vector2.zero;
        AddLocalizedBinder(reason, "result.reason");

        var nextBtn = new GameObject("NextButton", typeof(RectTransform), typeof(Image), typeof(Button));
        nextBtn.transform.SetParent(canvasGO.transform, false);
        var nbRt = nextBtn.GetComponent<RectTransform>();
        nbRt.sizeDelta = new Vector2(220, 70);
        nbRt.anchorMin = new Vector2(0.5f, 0f);
        nbRt.anchorMax = new Vector2(0.5f, 0f);
        nbRt.anchoredPosition = new Vector2(0f, 80f);
        var nimg = nextBtn.GetComponent<Image>(); nimg.color = new Color(0.24f, 0.65f, 0.28f);

        var nextText = CreateText(nextBtn.transform, "NextText", 20, TextAlignmentOptions.Center);
        nextText.rectTransform.anchoredPosition = Vector2.zero;
        AddLocalizedBinder(nextText, "ui.next_soul");

        string path = Path.Combine(PrefabPath, "PF_ReincarnationResultScreen.prefab");
        PrefabUtility.SaveAsPrefabAsset(canvasGO, path);
        Object.DestroyImmediate(canvasGO);
    }
}
