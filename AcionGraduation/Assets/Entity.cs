using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EHitState
{
    Normal,
    Stun,
    Lagdoll
}

public abstract class Entity : MonoBehaviour
{
    public virtual float Hp
    {
        get { return hp; }
        set
        {
            value = Mathf.Max(0, value);

            hp = value;
            if (EHitState != EHitState.Lagdoll && value == 0)
            {
                Die();
                return;
            }
        }
    }
    public EHitState EHitState;
    public float hp;
    protected abstract void Die();
    protected abstract void Hit();
}
