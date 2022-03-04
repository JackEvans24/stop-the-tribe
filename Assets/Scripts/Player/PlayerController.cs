using Jevansmassive.Utils.Extensions;
using Jevansmassive.Utils.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Raycaster2D[] groundChecks;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private GameObject spear;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] protected AudioSource hurtSound;
    [SerializeField] private HUDUpdater hud;
    [SerializeField] protected ParticleSystem blood;

    [Header("Movement")]
    [SerializeField] private float speed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float jumpInputLag = 0.1f;
    [SerializeField] private float floatThreshold = 1f;
    [SerializeField] private float extraGravity = 5f;

    [Header("Combat")]
    [SerializeField] private Attack stab;
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected Transform throwPoint;
    [SerializeField] protected float spearTimeout = 2f;

    [Header("Recoil")]
    [SerializeField] protected float recoilTime = 0.3f;
    [SerializeField] protected float recoilMultiplier = 200f;

    [Header("Recovery")]
    [SerializeField] protected float invincibilityTime = 0.5f;
    [SerializeField] protected float invincibilityFlashInterval = 0.05f;
    [SerializeField] protected float invincibilityAlpha = 0.5f;

    private int currentHealth;
    private float currentMovement;
    private bool facingLeft;
    private bool isGrounded;
    private bool jumpHeld;
    private float timeSinceJump;
    private float currentRecoilTime;
    private float currentInvincibilityTime;
    private float currentSpearTime;

    private void Awake()
    {
        this.timeSinceJump = this.jumpInputLag + 1f;
        this.currentRecoilTime = this.recoilTime + 1f;
        this.currentInvincibilityTime = this.invincibilityTime + 1f;
        this.currentSpearTime = this.spearTimeout + 1f;

        this.currentHealth = this.maxHealth;
    }

    private void Update()
    {
        if (this.timeSinceJump <= this.jumpInputLag)
            this.timeSinceJump += Time.deltaTime;
        if (this.currentRecoilTime <= this.recoilTime)
            this.currentRecoilTime += Time.deltaTime;
        if (this.currentInvincibilityTime <= this.invincibilityTime)
            this.currentInvincibilityTime += Time.deltaTime;
        if (this.currentSpearTime <= this.spearTimeout)
            this.currentSpearTime += Time.deltaTime;

        if (this.currentHealth <= 0)
        {
            this.currentMovement = 0f;
            this.jumpHeld = false;

            return;
        }

        this.currentMovement = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
            this.timeSinceJump = 0f;
        this.jumpHeld = Input.GetButton("Jump");

        if (Input.GetButtonDown("Stab"))
            this.Stab();
        else if (!this.stab.Attacking && Input.GetButtonDown("Throw"))
            this.ThrowSpear();

        this.SetGrounded();
    }

    private void Stab()
    {
        this.stab.StartAttack();
        this.animator.SetTrigger("Stab");
    }

    private void ThrowSpear()
    {
        if (this.currentSpearTime <= (this.spearTimeout * this.stats.ThrowInterval))
            return;

        this.animator.SetTrigger("Throw");

        var newSpear = Instantiate(this.spear, this.throwPoint.position, this.spear.transform.rotation).GetComponent<Spear>();
        newSpear.Throw(this.rb.velocity, this.facingLeft, this.stats.ThrowDamage);

        this.currentSpearTime = 0f;
    }

    private void SetGrounded()
    {
        var grounded = this.groundChecks.AnyHasCollision();
        if (grounded != this.isGrounded)
        {
            this.isGrounded = grounded;
            this.animator.SetBool("Grounded", this.isGrounded);
        }
    }

    private void FixedUpdate()
    {
        if (this.currentHealth <= 0 || this.currentRecoilTime <= this.recoilTime)
            return;

        this.rb.velocity = new Vector2(this.currentMovement * this.speed * this.stats.Speed, this.rb.velocity.y);
        if ((this.facingLeft && this.rb.velocity.x > 0) || (!this.facingLeft && this.rb.velocity.x < 0))
            this.Flip();

        if (this.isGrounded && this.timeSinceJump <= this.jumpInputLag)
        {
            this.rb.velocity = new Vector2(this.currentMovement * this.speed, 0f);
            this.rb.AddForce(Vector2.up * this.jumpForce);

            this.jumpSound.Play();
        }
        else if (!this.isGrounded && (this.rb.velocity.y < this.floatThreshold || !this.jumpHeld))
            this.rb.AddForce(Vector2.down * this.extraGravity);
    }

    private void Flip()
    {
        this.facingLeft = !this.facingLeft;
        this.transform.Rotate(Vector2.up * 180f);
    }

    public void Hurt(Vector2 from)
    {
        if (this.currentHealth <= 0)
            return;
        if (this.currentInvincibilityTime <= this.invincibilityTime)
            return;

        this.currentInvincibilityTime = 0f;
        this.currentRecoilTime = 0f;

        this.hurtSound.Play();
        this.blood.Play();

        this.rb.velocity = Vector2.zero;
        this.rb.AddForce((this.transform.position - from.ToVector3()).normalized * this.recoilMultiplier);

        this.currentHealth = Mathf.Max(0, this.currentHealth - 1);
        this.hud.UpdatePlayerHealth(this.currentHealth);

        if (this.currentHealth <= 0)
            this.Die();
        else
        {
            StartCoroutine(this.DoInvincibilityFlash());
        }
    }

    private IEnumerator DoInvincibilityFlash()
    {
        var flashTimer = 0f;
        var flashOn = false;
        var colour = this.sprite.color;

        while (this.currentInvincibilityTime <= this.invincibilityTime)
        {
            if (flashTimer >= this.invincibilityFlashInterval)
            {
                colour.a = flashOn ? 1 : this.invincibilityAlpha;
                this.sprite.color = colour;

                flashOn = !flashOn;
                flashTimer = 0f;
            }

            yield return new WaitForEndOfFrame();
            flashTimer += Time.deltaTime;
        }

        colour.a = 1f;
        this.sprite.color = colour;
    }

    private void Die()
    {
        this.animator.SetTrigger("Die");
        GameManager.GameOver();
    }

    public void GiveUpgrade(Upgrade upgrade)
    {
        if (upgrade.Type == UpgradeType.HealthIncrease)
        {
            this.currentHealth++;
            this.hud.UpdatePlayerHealth(this.currentHealth);
        }
        else
            this.stats.Upgrade(upgrade);
    }
}
