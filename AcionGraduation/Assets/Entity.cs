using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EHitState
{
    Normal,
    Stun,
    Lagdoll
}

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : MonoBehaviour
{
    public virtual float Hp
    {
        get { return hp; }
        set
        {
            value = Mathf.Max(0, value);
            hp = value;
            if (value - hp <= 0) Hit(hp - value);
            if (hitState != EHitState.Lagdoll && value == 0)
            {
                Die();
                return;
            }
        }
    }
    public EHitState hitState;
    public float stunValue;
    public float maxStunValue;
    public float hp;
    protected Rigidbody2D rb;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    protected abstract void Die();
    protected abstract void Hit(float value);
    protected bool isOnAir()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Platform");
        var ray = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, layerMask);
        if (ray.collider == null) return true;
        else return false;
    }
}
