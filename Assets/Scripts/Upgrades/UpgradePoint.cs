using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text label;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private CanvasGroup textCanvas;

    private bool isUpgradeTime;
    private PlayerController player;

    private Upgrade currentUpgrade;

    public void SetUp(Upgrade upgrade)
    {
        this.currentUpgrade = upgrade;

        this.image.sprite = this.currentUpgrade.spriteImage;
        this.label.text = this.currentUpgrade.LabelText;

        this.canvas.alpha = 1;
        this.collider2d.enabled = true;
        this.isUpgradeTime = true;
    }

    public void Hide()
    {
        this.canvas.alpha = 0;
        this.isUpgradeTime = false;
        this.collider2d.enabled = false;
    }

    private void Update()
    {
        if (this.player != null && Input.GetButtonDown("Stab"))
        {
            this.player.GiveUpgrade(this.currentUpgrade);
            RoundManager.UpgradeChosen();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag(Tags.PLAYER))
            return;
        if (!this.isUpgradeTime || this.textCanvas.alpha > 0)
            return;

        if (this.player == null)
        {
            var p = collision.GetComponent<PlayerController>();
            this.player = p;
        }

        this.textCanvas.alpha = 1;        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(Tags.PLAYER))
            return;

        this.player = null;

        if (this.textCanvas.alpha > 0)
            this.textCanvas.alpha = 0;
    }
}
