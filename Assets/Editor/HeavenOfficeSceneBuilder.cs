using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class HeavenOfficeSceneBuilder
{
    [MenuItem("Heaven Office/Rebuild Editable Scene")]
    public static void RebuildEditableScene()
    {
        Debug.Log("Legacy editable scene rebuild is disabled. Opening MainOfficeExperience instead.");
        MainOfficeExperienceSceneBuilder.OpenEditableMainOfficeScene();
    }

    [InitializeOnLoadMethod]
    private static void BuildWhenSampleSceneOpens()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || scene.name != "SampleScene")
            {
                return;
            }

            Debug.Log("SampleScene legacy auto-rebuild is disabled. Use MainOfficeExperience for Steam Demo.");
        };
    }
}
