using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

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
public class Player : Entity
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

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        state = EPlayerState.Idle;
    }
    private void Update()
    {
        PlayerInput();
        Move();
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
            if (Input.GetKey(KeyCode.UpArrow))
                UpperCut();

            state = EPlayerState.Attack;

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(AttackDelay());
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
    private void BaseAttack(int index = 0)
    {
        BoxCollider2D collider2D = null;

        if (attackState == EPlayerAttackState.Upper)
            collider2D = weaponAttackAreaClass.weaponUpperCutArea[playerWeaponState];
        else if (onAir)
            collider2D = weaponAttackAreaClass.weaponOnAirAttackArea[playerWeaponState][index];
        else
            collider2D = weaponAttackAreaClass.weaponGroundAttack[playerWeaponState][index];

        var ray = AttackCollisionCheck(collider2D);
        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(2f, 1f);
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage;
        }

        //if (onAir) rb.velocity = new Vector2(2.5f, 1);

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
        var ray = AttackCollisionCheck(weaponAttackAreaClass.weaponUpperCutArea[playerWeaponState]);
        foreach (Collider2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upperForcePower, ForceMode2D.Impulse);
        }


        rb.AddForce(Vector2.up * upperForcePower, ForceMode2D.Impulse);
    }
    private Collider2D[] AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        return Physics2D.OverlapBoxAll(transform.position + (Vector3)collider2D.offset * lookDir, collider2D.size, 0, layerMask);
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }

    protected override void Hit(float value)
    {
        throw new System.NotImplementedException();
    }
}
