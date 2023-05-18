using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Entity
{
    protected float stunTime;
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
                if (isOnAir())
                {
                    hitState = EHitState.Lagdoll;
                    rb.gravityScale = 0.5f;
                }
                break;
            case EHitState.Lagdoll:
                stunTime = 0;
                break;
        }

        Camera.main.DOShakePosition(0.1f, 0.1f);
    }
}
