using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class HeavenOfficeSceneBuilder
{
    [MenuItem("Heaven Office/Rebuild Editable Scene")]
    public static void RebuildEditableScene()
    {
        HeavenOfficeGameController controller = Object.FindObjectOfType<HeavenOfficeGameController>();
        if (controller == null)
        {
            GameObject game = new GameObject("HeavenOfficeGame");
            controller = game.AddComponent<HeavenOfficeGameController>();
        }

        HeavenOfficeView view = controller.GetComponent<HeavenOfficeView>();
        if (view == null)
        {
            view = controller.gameObject.AddComponent<HeavenOfficeView>();
        }

        view.BuildIfNeeded(true);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(view);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    [InitializeOnLoadMethod]
    private static void BuildWhenSampleSceneOpens()
    {
        EditorApplication.delayCall += () =>
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || scene.name != "SampleScene")
            {
                return;
            }

            HeavenOfficeGameController controller = Object.FindObjectOfType<HeavenOfficeGameController>();
            HeavenOfficeView view = controller != null ? controller.GetComponent<HeavenOfficeView>() : null;
            if (controller == null || view == null || controller.GetComponentInChildren<Canvas>() == null)
            {
                RebuildEditableScene();
            }
        };
    }
}
