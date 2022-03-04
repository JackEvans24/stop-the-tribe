using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Movement Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float recoveryTime = 0.2f;

    [Header("Target")]
    [SerializeField] private Rect targetArea;
    [SerializeField] private float targetReachedDistance = 0.1f;

    [Header("Rock throw")]
    [SerializeField] private GameObject rock;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float pauseTime = 0.4f;

    private bool facingLeft;
    private bool throwing;
    protected Vector3 target;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (this.currentHealth > 0 && this.currentRecoilTime > this.recoilTime && this.currentRecoilTime <= this.recoilTime + this.recoveryTime)
        {
            this.currentRecoilTime += Time.deltaTime;
            this.rb.velocity = Vector2.zero;
        }

        if (!this.CanMove())
            return;

        this.MoveToTarget();
    }

    protected override bool CanMove()
    {
        if (!base.CanMove())
            return false;
        if (this.currentRecoilTime <= this.recoilTime + this.recoveryTime)
            return false;

        return true;
    }

    private void MoveToTarget()
    {
        if (this.throwing)
            return;

        if (this.target == null)
            this.UpdateTarget();

        if (Vector3.Distance(this.transform.position, this.target) < this.targetReachedDistance)
        {
            StartCoroutine(ThrowRock());
            return;
        }

        var direction = this.target - this.transform.position;
        this.rb.velocity = direction.normalized * this.speed;

        if ((this.facingLeft && this.rb.velocity.x > 0) || (!this.facingLeft && this.rb.velocity.x < 0))
            this.Flip();
    }

    private void UpdateTarget()
    {
        this.target = new Vector2(
            Random.Range(this.targetArea.x, this.targetArea.x + this.targetArea.width),
            Random.Range(this.targetArea.y, this.targetArea.y + this.targetArea.height)
        );
    }

    private IEnumerator ThrowRock()
    {
        if (this.throwing)
            yield break;
        this.throwing = true;

        this.rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(this.pauseTime);

        var player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
        if ((this.facingLeft && player.transform.position.x > this.transform.position.x) || (!this.facingLeft && player.transform.position.x < this.transform.position.x))
            this.Flip();

        yield return new WaitForSeconds(this.pauseTime);

        var newRock = Instantiate(this.rock, this.throwPoint.position, this.rock.transform.rotation).GetComponent<Spear>();
        newRock.Throw(this.rb.velocity, this.facingLeft, 1f);

        yield return new WaitForSeconds(this.pauseTime);

        this.UpdateTarget();
        this.throwing = false;
    }

    private void Flip()
    {
        this.facingLeft = !this.facingLeft;
        this.transform.Rotate(Vector2.up * 180f);
    }

    protected override void Die()
    {
        this.rb.bodyType = RigidbodyType2D.Dynamic;
        base.Die();
    }

    private void OnDrawGizmosSelected()
    {
        var center = new Vector2(
            this.targetArea.x + (this.targetArea.width / 2),
            this.targetArea.y + (this.targetArea.height / 2)
        );
        var size = new Vector2(this.targetArea.width, this.targetArea.height);

        Gizmos.DrawWireCube(center, size);
    }
}
