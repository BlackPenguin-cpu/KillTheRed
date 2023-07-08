using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : BaseEnemy
{
    private Vector3 originPos;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private ParticleSystem[] particleSystems;

    float lastHpParts = 1;

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
        SoundManager.instance.PlaySound("SFX_Wall_Attack");

        Player.instance.StartCoroutine(timeDelay());
        IEnumerator timeDelay()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            yield return null;
            Time.timeScale = 0.1f;
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 1;
        }
        foreach (var ps in particleSystems)
        {
            ps.Play();
        }
        gameObject.SetActive(false);
        Destroy(gameObject, 2f);
    }

    protected override void Hit(float value)
    {
        SoundManager.instance.PlaySound("SFX_Wall_Attack");
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = (Vector3)Random.insideUnitCircle / 10;
            transform.position += new Vector3(pos.x, pos.y / 10);
            yield return new WaitForSeconds(0.02f);
        }
        transform.position = originPos;
    }

    protected override void Start()
    {
        base.Start();
        originPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void WallHpStatus(float value)
    {
        if (value / maxHp < lastHpParts)
        {
            SoundManager.instance.PlaySound("SFX_Wall_Crack");
            spriteRenderer.sprite = sprites[3];
        }
        else if (value / maxHp < lastHpParts)
        {
            SoundManager.instance.PlaySound("SFX_Wall_Crack");
            lastHpParts = 0.2f;
            spriteRenderer.sprite = sprites[2];

        }
        else if (value / maxHp < lastHpParts)
        {
            SoundManager.instance.PlaySound("SFX_Wall_Crack");
            lastHpParts = 0.4f;
            spriteRenderer.sprite = sprites[1];
        }
        else if (value / maxHp < 0.8f)
        {
            SoundManager.instance.PlaySound("SFX_Wall_Crack");
            lastHpParts = 0.6f;
            spriteRenderer.sprite = sprites[0];
        }



    }
    protected override void Update()
    {
        base.Update();
    }
}
