using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public UpgradeType Type;
    public string LabelText;
    public float Value;
    public Sprite spriteImage;
}

public enum UpgradeType
{
    StabDamage,
    ThrowDamage,
    Speed,
    ThrowInterval,
    HealthIncrease
}
