using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private AudioSource throwSound;

    [Header("Variables")]
    [SerializeField] private Vector2 initialVelocity = Vector2.one * 200;
    [SerializeField] private float momentumMultiplier = 100;
    [SerializeField] private float baseDamage = 1f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private bool enemySpear;

    private float multiplier = 1f;
    private float currentLifetime;

    private void Update()
    {
        if (this.currentLifetime > this.lifetime)
            Destroy(this.gameObject);

        this.currentLifetime += Time.deltaTime;
    }

    public void Throw(Vector2 playerVelocity, bool left, float damageMultiplier)
    {
        this.multiplier = damageMultiplier;

        var spearForce = new Vector2(this.initialVelocity.x * (left ? -1 : 1), this.initialVelocity.y);
        var momentumForce = playerVelocity * this.momentumMultiplier;
        
        this.rb.AddForce(spearForce + momentumForce);
        this.throwSound.Play();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!this.enemySpear && collision.CompareTag(Tags.ENEMY))
        {
            var enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(this.baseDamage * this.multiplier, this.transform.position);
        }
        else if (this.enemySpear && collision.CompareTag(Tags.PLAYER))
        {
            var player = collision.GetComponent<PlayerController>();
            player.Hurt(this.transform.position);
        }
        else if (collision.CompareTag(Tags.GROUND))
        {
            this.rb.bodyType = RigidbodyType2D.Static;
            this.col.enabled = false;
        }
    }
}
