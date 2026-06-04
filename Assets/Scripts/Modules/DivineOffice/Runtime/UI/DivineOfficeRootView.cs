using UnityEngine;
using TMPro;

public class DivineOfficeRootView : MonoBehaviour
{
    public TextMeshProUGUI TitleText;

    private void Awake()
    {
        if (TitleText == null)
        {
            Debug.Log("DivineOfficeRootView: TitleText not assigned (placeholder)");
        }
    }

    public void ApplyTitle(string title)
    {
        if (TitleText != null) TitleText.text = title;
    }
}
