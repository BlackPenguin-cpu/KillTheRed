using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : BaseEnemy
{
    private Vector3 originPos;
    private SpriteRenderer spriteRenderer;

    public Sprite[] sprites;
    [SerializeField]
    protected ParticleSystem[] particleSystems;

    public EItemList earnItemType;
    public float cameraPosX;

    protected CameraManager cameraManager;
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
        Player.instance.revivePos = originPos;
        Player.instance.Hp += 50;
        GameManager.instance.onWall = false;
        SoundManager.instance.PlaySound("SFX_Wall_Die");
        GameManager.instance.waveNum++;

        transform.GetChild(0).parent = null;
        foreach (var ps in particleSystems)
        {
            ps.Play();
        }

        Player.instance.StartCoroutine(timeDelay());
        IEnumerator timeDelay()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            yield return null;
            Time.timeScale = 0.1f;
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 1;
            cameraManager.cameraState = ECameraState.InGame;
            yield return new WaitForSecondsRealtime(1f);
            ItemManager.instance.ItemGet(earnItemType);
        }


        gameObject.SetActive(false);
        Destroy(gameObject, 3f);
    }

    protected override void Hit(float value)
    {

        StartCoroutine(Shake());
        SoundManager.instance.PlaySound("SFX_Wall_Attack");
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
        cameraManager = CameraManager.instance;
    }

    void WallHpStatus(float value)
    {
        if (value / maxHp < 0.2f)
        {
            spriteRenderer.sprite = sprites[3];
        }
        else if (value / maxHp < 0.4f)
        {
            spriteRenderer.sprite = sprites[2];

        }
        else if (value / maxHp < 0.6f)
        {
            spriteRenderer.sprite = sprites[1];
        }
        else if (value / maxHp < 0.8f)
        {
            spriteRenderer.sprite = sprites[0];
        }



    }
    protected override void Update()
    {
        base.Update();
    }
    protected void FixedUpdate()
    {
        CameraCheck();
    }
    protected virtual void CameraCheck()
    {
        if (cameraManager.transform.position.x > cameraPosX)
        {
            cameraManager.cameraState = ECameraState.Wall;
            cameraManager.wallCameraPosX = cameraPosX;
            GameManager.instance.onWall = true;
        }
    }
}
