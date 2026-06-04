using UnityEngine;

[DisallowMultipleComponent]
public class DivineOfficeBootstrap : MonoBehaviour
{
    public DivineOfficeFlowController FlowController { get; private set; }
    public DivineOfficeLocalizationService LocalizationService => FlowController?.LocalizationService;
    public DivineOfficeSaveService SaveService => FlowController != null ? FlowController.GetComponent<DivineOfficeSaveService>() ?? FlowController.SaveService : null;

    private void Awake()
    {
        FlowController = FindObjectOfType<DivineOfficeFlowController>();
        if (FlowController == null)
        {
            GameObject go = new GameObject("DivineOfficeFlow");
            FlowController = go.AddComponent<DivineOfficeFlowController>();
            // attach Save and Localization services if not assigned
            if (FlowController.SaveService == null) FlowController.SaveService = new DivineOfficeSaveService();
            if (FlowController.LocalizationService == null) FlowController.LocalizationService = new DivineOfficeLocalizationService();
        }

        // Ensure object persists to make services available to binders
        DontDestroyOnLoad(FlowController.gameObject);
    }
}
