using UnityEngine;

public static class HeavenOfficeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (UnityEngine.Object.FindFirstObjectByType<HeavenOfficeGameController>() != null
            || UnityEngine.Object.FindFirstObjectByType<HeavenOfficeAdapter>() != null
            || UnityEngine.Object.FindFirstObjectByType<DivineOfficeFlowController>() != null)
        {
            return;
        }

        GameObject game = new GameObject("DivineOfficeDemo");
        game.AddComponent<HeavenOfficeView>();
        game.AddComponent<DivineOfficeFlowController>();
    }
}
