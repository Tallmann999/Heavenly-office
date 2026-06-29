using UnityEngine;
using UnityEngine.SceneManagement;

public static class HeavenOfficeBootstrap
{
    private const string StartupSceneName = "MainOfficeExperience";
    private static bool startupSceneLoadRequested;
    private static bool demoBootstrapped;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoadedHandler()
    {
        startupSceneLoadRequested = false;
        demoBootstrapped = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (SceneManager.GetActiveScene().name != StartupSceneName)
        {
            LoadStartupScene();
            return;
        }

        BootstrapDemo();
    }

    private static void LoadStartupScene()
    {
        if (startupSceneLoadRequested)
        {
            return;
        }

        startupSceneLoadRequested = true;
        SceneManager.LoadScene(StartupSceneName);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != StartupSceneName)
        {
            return;
        }

        BootstrapDemo();
    }

    private static void BootstrapDemo()
    {
        if (demoBootstrapped)
        {
            return;
        }

        if (UnityEngine.Object.FindFirstObjectByType<DivineOfficeFlowController>() != null)
        {
            demoBootstrapped = true;
            return;
        }

        GameObject game = new GameObject("DivineOfficeDemo");
        game.AddComponent<HeavenOfficeView>();
        game.AddComponent<DivineOfficeFlowController>();
        demoBootstrapped = true;
    }
}
