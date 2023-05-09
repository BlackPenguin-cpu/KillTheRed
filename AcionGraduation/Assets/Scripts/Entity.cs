using Sirenix.OdinInspector;
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

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Entity : SerializedMonoBehaviour
{
    public virtual float Hp
    {
        get { return hp; }
        set
        {
            value = Mathf.Clamp(value, 0, maxHp);
            if (value - hp <= 0) Hit(hp - value);
            if (hitState != EHitState.Lagdoll && value == 0)
            {
                Die();
                return;
            }
            hp = value;
        }
    }
    public EHitState hitState;
    public float stunValue;
    public float maxStunValue;
    [SerializeField]
    protected float maxHp;
    [SerializeField]
    protected float hp;
    protected Rigidbody2D rb;
    protected BoxCollider2D col;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }
    protected abstract void Die();
    protected abstract void Hit(float value);
    protected bool isOnAir()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Platform");
        var ray = Physics2D.BoxCast((Vector2)transform.position + col.offset, col.size, 0, Vector2.down, 0.01f, layerMask);
        if (ray.collider == null) return true;
        else return false;
    }
}
