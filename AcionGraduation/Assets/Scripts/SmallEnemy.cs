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
        Dead
    }

    public float speed = 5;
    [SerializeField] private EEnemyState state;

    private Player player;
    private Animator animator;
    private RuntimeAnimatorController runtimeAnimatorController;
    private SpriteRenderer spriteRenderer;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        runtimeAnimatorController = animator.runtimeAnimatorController;
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
        lookDir = dir / Mathf.Abs(dir);

        transform.position += Vector3.right * lookDir * speed * Time.deltaTime;
        spriteRenderer.flipX = lookDir == 1 ? false : true;
    }
    private IEnumerator TracePlayer()
    {
        WaitForSeconds waitSeconds = new WaitForSeconds(0.1f);

        while (state != EEnemyState.Dead)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > 1)
            {
                state = EEnemyState.Trace;
            }
            else
            {
                state = EEnemyState.Attack;
                yield return StartCoroutine(Attack());
            }
            yield return waitSeconds;
        }
    }
    private IEnumerator Attack()
    {
        new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        yield return null;
    }
}
