using System.Collections;
using System.Collections.Generic;
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

    [Tooltip("AttackCollider")]
    BoxCollider2D AttackArea;
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
            if (hitState == EHitState.Stun)
            {
                yield return null;
                continue;
            }
            state = Mathf.Abs(player.transform.position.x - transform.position.x) > 1 ? EEnemyState.Trace : EEnemyState.Attack;

            if (state == EEnemyState.Attack)
                yield return StartCoroutine(Attack());
            else
                yield return waitSeconds;
        }
    }
    private IEnumerator Attack()
    {
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        state = EEnemyState.Trace;

        yield return null;
    }
    private void AttackCheck()
    {
        if (AttackCollisionCheck(AttackArea))
        {
            Player.instance.Hp -= attackValue;
        }
    }
    protected override void Hit(float damage)
    {
        base.Hit(damage);
        state = EEnemyState.Stun;


    }
}
