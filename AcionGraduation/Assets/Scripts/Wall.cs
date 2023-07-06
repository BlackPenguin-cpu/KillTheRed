using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : BaseEnemy
{
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite[] sprites;

    public override float Hp
    {
        get => base.Hp;
        set
        {
            WallHpStatus(value);
            base.Hp = value;
        }
    }
    protected override void Die()
    {
        Destroy(gameObject);
    }

    protected override void Hit(float value)
    {
    }

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void WallHpStatus(float value)
    {
        if (value / 100 < 0.2f)
        {
            spriteRenderer.sprite = sprites[3];
        }
        else if (value / 100 < 0.4f)
        {
            spriteRenderer.sprite = sprites[2];

        }
        else if (value / 100 < 0.6f)
        {
            spriteRenderer.sprite = sprites[1];
        }
        else if (value / 100 < 0.8f)
        {
            spriteRenderer.sprite = sprites[0];
        }



    }
    void Update()
    {

    }
}
