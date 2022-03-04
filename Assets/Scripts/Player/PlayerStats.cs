using System;
using UnityEngine;

[Serializable]
public class PlayerStats : MonoBehaviour
{
    public float StabDamage = 1f;
    public float ThrowDamage = 1f;
    public float ThrowInterval = 1f;
    public float Speed = 1f;

    public void Upgrade(Upgrade upgrade)
    {
        switch (upgrade.Type)
        {
            case UpgradeType.StabDamage:
                this.StabDamage += upgrade.Value;
                break;

            case UpgradeType.ThrowDamage:
                this.ThrowDamage += upgrade.Value;
                break;

            case UpgradeType.Speed:
                this.Speed += upgrade.Value;
                break;

            case UpgradeType.ThrowInterval:
                this.ThrowInterval *= upgrade.Value;
                break;

            default:
                throw new System.NotImplementedException();
        }
    }
}
