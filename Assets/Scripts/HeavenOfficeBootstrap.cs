using UnityEngine;

public static class HeavenOfficeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (UnityEngine.Object.FindFirstObjectByType<HeavenOfficeModeSelector>() != null
            || UnityEngine.Object.FindFirstObjectByType<HeavenOfficeAdapter>() != null
            || UnityEngine.Object.FindFirstObjectByType<DivineOfficeFlowController>() != null)
        {
            return;
        }

        GameObject selector = new GameObject("HeavenOfficeModeSelector");
        selector.AddComponent<HeavenOfficeModeSelector>();
    }
}
