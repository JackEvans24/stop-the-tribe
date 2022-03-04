using UnityEngine;

public class Dinosaur : MonoBehaviour
{
    [SerializeField] private HUDUpdater hud;
    [SerializeField] private int healthRequired = 10;

    private int currentHealth = 0;

    private void Start()
    {
        this.hud.UpdateDinoHealth(this.currentHealth - this.healthRequired);
    }

    public void AddHealth()
    {
        if (this.currentHealth >= this.healthRequired)
            return;

        this.currentHealth = Mathf.Min(this.healthRequired, this.currentHealth + 1);

        this.hud.UpdateDinoHealth(this.currentHealth - this.healthRequired);

        if (this.currentHealth >= this.healthRequired)
            GameManager.GameOver();
    }
}
