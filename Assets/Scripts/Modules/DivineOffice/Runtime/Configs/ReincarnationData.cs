using UnityEngine;

[CreateAssetMenu(menuName = "DivineOffice/ReincarnationData")]
public class ReincarnationData : ScriptableObject
{
    public string Id;
    public string NameKey;
    public string DescriptionKey;
    public Sprite Icon;
    public StampType[] AllowedStamps;
    public string WorldTarget; // simple string for prototype
}
