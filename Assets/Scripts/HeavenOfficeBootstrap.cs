using UnityEngine;

public static class HeavenOfficeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (UnityEngine.Object.FindObjectOfType<HeavenOfficeGameController>() != null)
        {
            return;
        }

        GameObject game = new GameObject("HeavenOfficeGame");
        game.AddComponent<HeavenOfficeGameController>();
    }
}
