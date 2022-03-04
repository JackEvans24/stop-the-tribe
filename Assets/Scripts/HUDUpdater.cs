using TMPro;
using UnityEngine;

public class HUDUpdater : MonoBehaviour
{
    [SerializeField] private TMP_Text playerHealth;
    [SerializeField] private TMP_Text dinoHealth;
    [SerializeField] private TMP_Text roundLabel;

    public void UpdatePlayerHealth(int health)
    {
        var text = "";
        if (health <= 0)
            text = "X_X";
        else
            text = $"you: {health}";

        this.playerHealth.text = text;
    }

    public void UpdateDinoHealth(int health)
    {
        var text = "";
        if (health >= 0)
            text = "he's alive!";
        else
            text = $"rex: {health}";

        this.dinoHealth.text = text;
    }

    public void UpdateRoundText(int round)
    {
        this.roundLabel.text = $"round: {Mathf.Max(1, round)}";
    }
}
