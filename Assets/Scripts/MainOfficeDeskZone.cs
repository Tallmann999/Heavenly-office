using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MainOfficeDeskZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string zoneId;
    [SerializeField] private bool premiumTeaser;
    [SerializeField] private Image targetImage;

    private Color baseColor;
    private Vector3 baseScale;

    public string ZoneId => zoneId;
    public bool PremiumTeaser => premiumTeaser;

    public void Configure(string id, bool isPremiumTeaser, Image image)
    {
        zoneId = id;
        premiumTeaser = isPremiumTeaser;
        targetImage = image;
        CacheBaseState();
    }

    private void Awake()
    {
        CacheBaseState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage == null) return;

        targetImage.color = Color.Lerp(baseColor, Color.white, premiumTeaser ? 0.12f : 0.2f);
        transform.localScale = baseScale * (premiumTeaser ? 1.015f : 1.035f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage == null) return;

        targetImage.color = baseColor;
        transform.localScale = baseScale;
    }

    private void CacheBaseState()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (targetImage != null)
        {
            baseColor = targetImage.color;
        }

        baseScale = transform.localScale;
    }
}
