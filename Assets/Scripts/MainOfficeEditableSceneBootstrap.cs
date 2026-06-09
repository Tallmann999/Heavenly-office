using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class MainOfficeEditableSceneBootstrap : MonoBehaviour
{
    [SerializeField] private bool buildWhenOpened = true;

    private void OnEnable()
    {
        if (Application.isPlaying || !buildWhenOpened)
        {
            return;
        }

        HeavenOfficeView view = GetComponent<HeavenOfficeView>();
        if (view == null)
        {
            view = gameObject.AddComponent<HeavenOfficeView>();
        }

        if (GetComponent<HeavenOfficeGameController>() == null)
        {
            gameObject.AddComponent<HeavenOfficeGameController>();
        }

        if (view.GetComponentInChildren<Canvas>() == null)
        {
            view.BuildIfNeeded(true);
            view.PrepareMainOfficeEditorPreview();
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }
}
