using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class BaseEnemy : Entity
{
    protected float stunTime;
    protected float attackValue;
    protected int lookDir;
    protected virtual void Update()
    {
        StandUp();
        if (hitState == EHitState.Stun)
        {
            stunTime += Time.deltaTime;
            if (stunTime > 1)
            {
                stunTime = 0;
                hitState = EHitState.Normal;
            }
        }
    }

    protected virtual void StandUp()
    {
        if (hitState == EHitState.Lagdoll && !isOnAir())
        {
            hitState = EHitState.Normal;
            rb.gravityScale = 1;
            rb.mass = 2f;
        }
    }

    protected override void Die()
    {
        if (hitState == EHitState.Lagdoll)
            Destroy(gameObject);
    }
    protected override void Hit(float damage)
    {
        switch (hitState)
        {
            case EHitState.Normal:
                stunValue += damage;
                if (stunValue < maxStunValue)
                {
                    stunValue = 0;
                    hitState = EHitState.Stun;
                    rb.mass = 1f;
                }
                break;
            case EHitState.Stun:
                if (!isOnAir())
                    hitState = EHitState.Lagdoll;
                else
                    rb.gravityScale = 0.5f;
                break;
            case EHitState.Lagdoll:
                stunTime = 0;
                break;
        }

        Camera.main.DOShakePosition(0.1f, 0.1f);
    }

    protected Collider2D AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        return Physics2D.OverlapBox(transform.position + new Vector3(collider2D.offset.x * lookDir, collider2D.offset.y), collider2D.size, 0, layerMask);
    }
}
