using Jevansmassive.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource hitSound;
    [SerializeField] protected ParticleSystem blood;

    [Header("Health and Damage")]
    [SerializeField] protected float baseDamage = 1f;
    [SerializeField] protected float maxHealth = 5f;

    [Header("Recoil")]
    [SerializeField] protected float recoilTime = 0.3f;
    [SerializeField] protected float recoilMultiplier = 1f;

    [Header("Invincibility")]
    [SerializeField] protected float invincibilityTime = 0.2f;

    protected float currentHealth;
    protected float currentRecoilTime;
    protected float currentInvincibilityTime;

    protected virtual void Start()
    {
        this.currentHealth = this.maxHealth;

        this.currentInvincibilityTime = this.invincibilityTime + 1f;
        this.currentRecoilTime = this.recoilTime + 1f;
    }

    protected virtual void Update()
    {
        if (this.currentInvincibilityTime <= this.invincibilityTime)
            this.currentInvincibilityTime += Time.deltaTime;
        if (this.currentRecoilTime <= this.recoilTime)
            this.currentRecoilTime += Time.deltaTime;
    }

    protected virtual bool CanMove()
    {
        if (GameManager.IsGameOver)
            return false;
        if (this.currentHealth <= 0f)
            return false;
        if (this.currentInvincibilityTime <= this.invincibilityTime)
            return false;
        if (this.currentRecoilTime <= this.recoilTime)
            return false;

        return true;
    }

    public void TakeDamage(float damage, Vector2 from)
    {
        if (this.currentInvincibilityTime <= this.invincibilityTime)
            return;

        this.currentRecoilTime = 0f;
        this.rb.velocity = Vector2.zero;
        this.rb.AddForce((this.transform.position - from.ToVector3()).normalized * this.recoilMultiplier);

        this.hitSound.Play();
        this.blood.Play();

        if (this.currentHealth <= 0f)
            return;

        this.currentHealth = Mathf.Max(0f, this.currentHealth - damage);
        this.currentInvincibilityTime = 0f;

        if (this.currentHealth == 0f)
            this.Die();
    }

    protected virtual void Die()
    {
        RoundManager.LogKill();

        this.animator.SetTrigger("Die");

        Destroy(this.gameObject, 3f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (this.currentHealth <= 0)
            return;

        if (collision.collider.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<PlayerController>();
            if (player == null)
                return;

            player.Hurt(this.transform.position);
        }        
    }
}
