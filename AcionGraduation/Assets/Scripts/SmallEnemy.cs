using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmallEnemy : BaseEnemy
{
    enum EEnemyState
    {
        Idle,
        Attack,
        Trace,
        Dead,
        Stun
    }

    public float speed = 5;
    [SerializeField] private EEnemyState state;

    private Player player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private ParticleSystem HitParticle;

    [Tooltip("AttackCollider")]
    [SerializeField] private BoxCollider2D AttackArea;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        AttackArea = transform.GetChild(0).GetComponent<BoxCollider2D>();
        player = Player.instance;

        StartCoroutine(TracePlayer());
    }
    protected override void Update()
    {
        base.Update();

        if (state == EEnemyState.Trace)
        {
            Move();
        }

        animator.SetInteger("State", (int)state);
        animator.SetInteger("HitState", (int)hitState);
    }
    private void Move()
    {
        int dir = (int)(player.transform.position.x - transform.position.x);
        if (dir == 0) return;
        lookDir = dir / Mathf.Abs(dir);

        transform.position += Vector3.right * lookDir * speed * Time.deltaTime;
        spriteRenderer.flipX = lookDir == 1 ? false : true;
    }
    private IEnumerator TracePlayer()
    {
        WaitForSeconds waitSeconds = new WaitForSeconds(0.1f);

        while (state != EEnemyState.Dead)
        {
            if (hitState != EHitState.Normal)
            {
                state = EEnemyState.Stun;
                yield return null;
                continue;
            }
            state = Mathf.Abs(player.transform.position.x - transform.position.x) > (AttackArea.size.x / 2.15f + AttackArea.offset.x) ? EEnemyState.Trace : EEnemyState.Attack;

            if (state == EEnemyState.Attack)
            {
                while (state == EEnemyState.Attack)
                    yield return null;
            }
            else
                yield return waitSeconds;
        }
    }
    private void AttackEnd()
    {
        state = EEnemyState.Trace;
    }
    private IEnumerator Attack()
    {
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        state = EEnemyState.Trace;

        yield return null;
    }
    private void AttackCheck(int isShake)
    {
        if (gameObject.name.Contains("Big"))
            SoundManager.instance.PlaySound("SFX_Enermy_Attack_02_pig", SoundType.SE, 1.5f);
        else if (gameObject.name.Contains("Tall"))
            SoundManager.instance.PlaySound("SFX_Enermy_Attack_03_jun");
        else
            SoundManager.instance.PlaySound("SFX_Enermy_Attack_01_Suuu");
        if (AttackCollisionCheck(AttackArea))
        {
            player.Hp -= attackValue;
            Vector3 pos = player.transform.position - transform.position;
            player.GetComponent<Rigidbody2D>().AddForce(new Vector3(pos.x / Mathf.Abs(pos.x) * 2, 1), ForceMode2D.Impulse);
        }
        if (isShake == 1)
        {
            Camera.main.DOShakePosition(0.1f, 0.5f);
        }
    }
    protected override void Hit(float damage)
    {
        base.Hit(damage);
        HitParticle.Play();
    }
    protected override void Die()
    {
        base.Die();
    }
}
