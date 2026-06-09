using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MainOfficeExperienceSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/MainOfficeExperience.unity";
    private const string PrefabPath = "Assets/Prefabs/HeavenOfficeMainOfficeEditable.prefab";

    [MenuItem("Heaven Office/Main Office/Create Editable Main Office Scene")]
    public static void CreateEditableMainOfficeScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("MainOfficeExperienceSceneBuilder: skipped scene creation during Play Mode.");
            return;
        }

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "MainOfficeExperience";

        GameObject root = new GameObject("HeavenOfficeMainOfficeEditable");
        HeavenOfficeGameController controller = root.AddComponent<HeavenOfficeGameController>();
        HeavenOfficeView view = root.AddComponent<HeavenOfficeView>();
        root.AddComponent<MainOfficeEditableSceneBootstrap>();

        view.BuildIfNeeded(true);
        view.PrepareMainOfficeEditorPreview();

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(view);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EnsureSceneInBuildSettings(ScenePath);

        Selection.activeGameObject = root;
        Debug.Log($"Main office editable scene created: {ScenePath}");
    }

    [MenuItem("Heaven Office/Main Office/Open Editable Main Office Scene")]
    public static void OpenEditableMainOfficeScene()
    {
        if (!System.IO.File.Exists(ScenePath))
        {
            CreateEditableMainOfficeScene();
            return;
        }

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
    }

    [MenuItem("Heaven Office/Main Office/Rebuild Current Main Office Layout")]
    public static void RebuildCurrentMainOfficeLayout()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("MainOfficeExperienceSceneBuilder: skipped layout rebuild during Play Mode.");
            return;
        }

        HeavenOfficeView view = Object.FindObjectOfType<HeavenOfficeView>();
        if (view == null)
        {
            GameObject root = new GameObject("HeavenOfficeMainOfficeEditable");
            root.AddComponent<HeavenOfficeGameController>();
            root.AddComponent<MainOfficeEditableSceneBootstrap>();
            view = root.AddComponent<HeavenOfficeView>();
        }
        else
        {
            Canvas existingCanvas = view.GetComponentInChildren<Canvas>();
            if (existingCanvas != null)
            {
                Object.DestroyImmediate(existingCanvas.gameObject);
            }
        }

        view.BuildIfNeeded(true);
        view.PrepareMainOfficeEditorPreview();
        EditorUtility.SetDirty(view);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    [MenuItem("Heaven Office/Main Office/Save Current Layout As Prefab")]
    public static void SaveCurrentLayoutAsPrefab()
    {
        HeavenOfficeView view = Object.FindObjectOfType<HeavenOfficeView>();
        if (view == null)
        {
            Debug.LogWarning("MainOfficeExperienceSceneBuilder: no HeavenOfficeView found in current scene.");
            return;
        }

        Canvas canvas = view.GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("MainOfficeExperienceSceneBuilder: no HeavenOfficeCanvas found to save as prefab.");
            return;
        }

        string directory = System.IO.Path.GetDirectoryName(PrefabPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        PrefabUtility.SaveAsPrefabAsset(view.gameObject, PrefabPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Main office layout prefab saved: {PrefabPath}");
    }

    private static void EnsureSceneInBuildSettings(string path)
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        if (scenes.Any(scene => scene.path == path))
        {
            return;
        }

        EditorBuildSettings.scenes = scenes
            .Concat(new[] { new EditorBuildSettingsScene(path, true) })
            .ToArray();
    }

    [InitializeOnLoadMethod]
    private static void BuildWhenMainOfficeSceneOpens()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || scene.name != "MainOfficeExperience")
            {
                return;
            }

            HeavenOfficeView view = Object.FindObjectOfType<HeavenOfficeView>();
            if (view == null || view.GetComponentInChildren<Canvas>() == null)
            {
                RebuildCurrentMainOfficeLayout();
                EnsureSceneInBuildSettings(ScenePath);
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
        };
    }
}
