using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Entity
{
    protected virtual void Update()
    {
        isOnAir();
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
                }
                break;
            case EHitState.Stun:
                if (isOnAir())
                {
                    hitState = EHitState.Lagdoll;
                    rb.gravityScale = 0.1f;
                    rb.mass = 1f;
                }
                break;
            case EHitState.Lagdoll:
                if (!isOnAir())
                {
                    hitState = EHitState.Normal;
                    rb.gravityScale = 1;
                    rb.mass = 10f;
                }
                break;
        }
    }
}
