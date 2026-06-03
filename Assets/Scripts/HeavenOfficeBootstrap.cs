using UnityEngine;

public static class HeavenOfficeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (UnityEngine.Object.FindFirstObjectByType<HeavenOfficeAdapter>() != null)
        {
            return;
        }

        GameObject game = new GameObject("HeavenOfficeGame");
        game.AddComponent<HeavenOfficeAdapter>();
    }
}
