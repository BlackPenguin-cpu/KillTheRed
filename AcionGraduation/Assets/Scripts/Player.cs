using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;

public enum EPlayerWeaponState
{
    Hand,
    Sword,
    Pistol,
    Hammer
}
public enum EPlayerState
{
    Idle,
    Run,
    Jump,
    Attack,
    Dead,

}
public enum EPlayerAttackState
{
    None,
    Upper,
    OnAir
}
public partial class Player : Entity
{
    private class WeaponAttackAreaClass : OdinSerializeAttribute
    {
        [DictionaryDrawerSettings]
        public Dictionary<EPlayerWeaponState, BoxCollider2D[]> weaponGroundAttack;
        [DictionaryDrawerSettings]
        public Dictionary<EPlayerWeaponState, BoxCollider2D[]> weaponOnAirAttackArea;
        [DictionaryDrawerSettings]
        public Dictionary<EPlayerWeaponState, BoxCollider2D> weaponUpperCutArea;
    }

    public Coroutine attackCoroutine;

    public bool onAir;
    public float upperForcePower;

    public float attackDamage;

    [Header("Serializeable Variable")]

    [SerializeField]
    private WeaponAttackAreaClass weaponAttackAreaClass;
    [SerializeField]
    private EPlayerState state;
    [SerializeField]
    private EPlayerAttackState attackState;
    [SerializeField]
    private EPlayerWeaponState playerWeaponState;

    private int lookDir;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float spd;
    private bool onAttack;

    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        state = EPlayerState.Idle;
    }
    private void Update()
    {
        Move();
        PlayerInput();
        onAir = isOnAir();
        animator.SetInteger("State", (int)state);
        animator.SetInteger("AttackState", (int)attackState);
        animator.SetInteger("WeaponState", (int)playerWeaponState);
        animator.SetBool("OnAttack", onAttack);
    }
    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (Input.GetKey(KeyCode.UpArrow) && state != EPlayerState.Attack)
                UpperCut();
            if (onAir)
                attackState = EPlayerAttackState.OnAir;

            state = EPlayerState.Attack;

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(AttackDelay());
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (playerWeaponState == EPlayerWeaponState.Sword)
                playerWeaponState = EPlayerWeaponState.Hand;
            else
                playerWeaponState = EPlayerWeaponState.Sword;
        }
    }
    private void Jump()
    {
        if (onAir) return;
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        state = EPlayerState.Jump;
        onAir = true;
    }
    private void Move()
    {
        if (onAttack) return;
        var hor = Input.GetAxisRaw("Horizontal");
        if (hor != 0)
        {
            lookDir = (int)hor;
            spriteRenderer.flipX = lookDir == 1 ? false : true;
            if (!onAir && rb.velocity.y == 0)
                state = EPlayerState.Run;
        }
        else if (state != EPlayerState.Attack && rb.velocity == Vector2.zero)
        {
            state = EPlayerState.Idle;
        }
        var spdValue = hor * spd * Time.deltaTime;

        transform.position += Vector3.right * spdValue;
    }
    private void BigAttack(int index)
    {
        BoxCollider2D collider2D = null;

        switch (attackState)
        {
            case EPlayerAttackState.None:
                collider2D = weaponAttackAreaClass.weaponGroundAttack[playerWeaponState][index];
                break;
            case EPlayerAttackState.Upper:
                collider2D = weaponAttackAreaClass.weaponUpperCutArea[playerWeaponState];
                break;
            case EPlayerAttackState.OnAir:
                collider2D = weaponAttackAreaClass.weaponOnAirAttackArea[playerWeaponState][index];
                if (playerWeaponState == EPlayerWeaponState.Hand && index == 3)
                {
                    break;
                }
                rb.velocity = Vector3.up * 3;
                break;
        }
        var ray = AttackCollisionCheck(collider2D);
        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage * 2;
            physics2D.transform.GetComponent<Rigidbody2D>().AddForce(new Vector3(lookDir * 3, 2.5f), ForceMode2D.Impulse);
        }
        Camera.main.DOShakePosition(0.2f,2);
    }
    private void BaseAttack(int index = 0)
    {
        BoxCollider2D collider2D = null;

        if (attackState != EPlayerAttackState.Upper)
            rb.AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * 2f, ForceMode2D.Impulse);

        switch (attackState)
        {
            case EPlayerAttackState.None:
                collider2D = weaponAttackAreaClass.weaponGroundAttack[playerWeaponState][index];
                break;
            case EPlayerAttackState.Upper:
                collider2D = weaponAttackAreaClass.weaponUpperCutArea[playerWeaponState];
                break;
            case EPlayerAttackState.OnAir:
                collider2D = weaponAttackAreaClass.weaponOnAirAttackArea[playerWeaponState][index];
                if (playerWeaponState == EPlayerWeaponState.Hand && index == 3)
                {
                    return;
                }
                rb.velocity = Vector3.up * 3;
                break;
        }
        var ray = AttackCollisionCheck(collider2D);
        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage;

            switch (attackState)
            {
                case EPlayerAttackState.None:
                    physics2D.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(lookDir * 2, 1f);
                    break;
                case EPlayerAttackState.Upper:
                    physics2D.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upperForcePower, ForceMode2D.Impulse);
                    break;
                case EPlayerAttackState.OnAir:
                    physics2D.transform.GetComponent<Rigidbody2D>().velocity = Vector3.up * 3;
                    break;
            }
        }
    }
    public void AttackEnd()
    {
        onAttack = false;
        state = EPlayerState.Idle;
        attackState = EPlayerAttackState.None;
    }
    public void AttackStart()
    {
        onAttack = true;
    }
    private IEnumerator AttackDelay()
    {
        yield return null;
        onAttack = true;
        yield return new WaitForSeconds(0.5f);
        AttackEnd();
    }
    private void UpperCut()
    {
        attackState = EPlayerAttackState.Upper;
        rb.AddForce(Vector2.up * upperForcePower, ForceMode2D.Impulse);
    }
    private Collider2D[] AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        return Physics2D.OverlapBoxAll(transform.position + new Vector3(collider2D.offset.x * lookDir, collider2D.offset.y), collider2D.size, 0, layerMask);
    }

    #region PlayerSkillEffect

    #region Hand
    private void HandAirAttackAxeKick()
    {
        var objs = AttackCollisionCheck(weaponAttackAreaClass.weaponOnAirAttackArea[EPlayerWeaponState.Hand][2]);
        foreach (Collider2D obj in objs)
        {
            obj.GetComponent<Rigidbody2D>().AddForce(Vector3.down * 10, ForceMode2D.Impulse);
        }
    }
    private void HandAirAttackFinish()
    {
        int layerMask = LayerMask.NameToLayer("Platform");
        RaycastHit2D obj = Physics2D.Raycast(transform.position, Vector2.down, 15, 1 << layerMask);

        transform.position = new Vector2(obj.point.x, obj.point.y);
    }
    #endregion
    #region Sword
    private void SwordDownAttack()
    {
        var objs = AttackCollisionCheck(weaponAttackAreaClass.weaponOnAirAttackArea[EPlayerWeaponState.Sword][3]);
        foreach (Collider2D obj in objs)
        {
            obj.GetComponent<Entity>().Hp -= attackDamage;
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector3(20 * lookDir, -15), ForceMode2D.Impulse);
        }

        Camera.main.DOShakePosition(0.2f,2);
    }
    #endregion
    #endregion
    protected override void Die()
    {
        Destroy(gameObject);
    }

    protected override void Hit(float value)
    {

    }
}
