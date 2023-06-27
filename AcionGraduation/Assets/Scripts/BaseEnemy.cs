using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class BaseEnemy : Entity
{
    [SerializeField]
    protected float stunTime;
    public float attackValue;
    protected int lookDir;
    protected bool inviinvincibility;

    public override float Hp
    {
        get => base.Hp;
        set
        {
            if (inviinvincibility) return;
            base.Hp = value;
        }
    }
    protected virtual void Update()
    {
        StateApply();
    }
    private void StateApply()
    {
        if (hitState == EHitState.Stun && !isOnAir() && stunTime > 0.2f)
        {
            hitState = EHitState.KnockDown;
            rb.mass = 2;
            stunTime = 0;
        }
        else if (hitState == EHitState.Stun)
        {
            stunTime += Time.deltaTime;
        }
        if (stunTime <= 1 && hitState == EHitState.KnockDown)
        {
            stunTime += Time.deltaTime;
            if (stunTime > 1)
            {
                StandUp();
            }
        }
    }

    protected virtual void StandUp()
    {
        hitState = EHitState.Normal;
        rb.gravityScale = 1;
        stunTime = 0;

        inviinvincibility = false;
    }

    protected override void Die()
    {
        if (hitState == EHitState.KnockDown)
            Destroy(gameObject);
    }
    protected override void Hit(float damage)
    {
        switch (hitState)
        {
            case EHitState.Normal:
                stunValue += damage;
                if (stunValue >= maxStunValue)
                {
                    stunValue = 0;
                    hitState = EHitState.Stun;
                    rb.mass = 1f;
                }
                break;
            case EHitState.Stun:
                break;
            case EHitState.KnockDown:
                break;
        }

        Camera.main.DOShakePosition(0.1f, 0.1f);
    }

    protected Collider2D AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        return Physics2D.OverlapBox
            (transform.position + new Vector3(collider2D.offset.x * lookDir, collider2D.offset.y), collider2D.size, 0, layerMask);
    }
}
