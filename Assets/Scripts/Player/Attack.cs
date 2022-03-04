using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attacker;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private AudioSource stabSound;

    [Header("Variables")]
    [SerializeField] private float area = 0.8f;
    [SerializeField] private float baseDamage = 1f;
    [SerializeField] private float attackTime = 0.4f;
    [SerializeField] private LayerMask enemyLayer;

    public bool Attacking;

    public void StartAttack()
    {
        StartCoroutine(this.Attack_Coroutine());
    }

    private IEnumerator Attack_Coroutine()
    {
        if (this.Attacking)
            yield break;
        this.Attacking = true;

        var soundPlayed = false;

        var currentAttackTime = 0f;
        while (currentAttackTime <= this.attackTime)
        {
            var enemies = Physics2D.OverlapBoxAll(this.transform.position, Vector2.one * this.area, 0f, this.enemyLayer);
            foreach (var enemyCollider in enemies)
            {
                var enemy = enemyCollider.GetComponent<Enemy>();
                enemy.TakeDamage(this.baseDamage * this.stats.StabDamage, this.attacker.position);

                if (!soundPlayed)
                {
                    this.stabSound.Play();
                    soundPlayed = true;
                }
            }

            yield return new WaitForEndOfFrame();
            currentAttackTime += Time.deltaTime;
        }

        this.Attacking = false;
    }
}
