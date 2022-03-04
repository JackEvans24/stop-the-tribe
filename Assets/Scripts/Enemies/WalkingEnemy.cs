using System.Collections;
using UnityEngine;

public class WalkingEnemy : Enemy
{
    [Header("Movement Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float recoveryTime = 0.2f;

    [Header("Delivery behaviour")]
    [SerializeField] protected float collectionWaitTime = 0.2f;
    [SerializeField] protected float deliveryWaitTime = 0.2f;

    protected bool hasPackage;
    private bool facingLeft;

    protected bool updatingTarget;
    protected Transform target;

    protected override void Start()
    {
        base.Start();

        this.target = TargetManager.GetTarget(this.hasPackage);
    }

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
        if (this.target == null)
            return;

        var direction = Mathf.Sign(this.target.position.x - this.transform.position.x);
        this.rb.velocity = new Vector2(this.speed * direction, this.rb.velocity.y);

        if ((this.facingLeft && this.rb.velocity.x > 0) || (!this.facingLeft && this.rb.velocity.x < 0))
            this.Flip();
    }

    private void Flip()
    {
        this.facingLeft = !this.facingLeft;
        this.transform.Rotate(Vector2.up * 180f);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == this.target)
            StartCoroutine(this.UpdateTarget());
    }

    private IEnumerator UpdateTarget()
    {
        if (this.updatingTarget)
            yield break;
        this.updatingTarget = true;

        this.rb.velocity = Vector2.zero;
        this.target = null;

        if (this.hasPackage)
            GameManager.AddDinoHealthPoint();

        this.animator.SetBool("HasFruit", !this.hasPackage);

        var waitTime = this.hasPackage ? this.deliveryWaitTime : this.collectionWaitTime;
        yield return new WaitForSeconds(waitTime);

        this.hasPackage = !this.hasPackage;
        this.target = TargetManager.GetTarget(this.hasPackage);

        this.updatingTarget = false;
    }
}
