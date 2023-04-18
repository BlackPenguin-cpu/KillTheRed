using System.Collections.Generic;
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
public enum EPlayerAirState
{
    None,
    OnAir,
    AirAttack
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


    public bool onAir;
    public float upperForcePower;

    public float attackDamage;

    [Header("Serializeable Variable")]

    [SerializeField]
    private WeaponAttackAreaClass weaponAttackAreaClass;
    [SerializeField]
    private EPlayerState state;
    [SerializeField]
    private EPlayerAirState airState;
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
        airState = onAir ? EPlayerAirState.OnAir : EPlayerAirState.None;
        animator.SetInteger("State", (int)state);
        animator.SetInteger("AirState", (int)airState);
        animator.SetInteger("WeaponState", (int)playerWeaponState);
        animator.SetBool("OnAttack", onAttack);
    }
    private void FixedUpdate()
    {
    }
    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.UpArrow))
        {
            UpperCut();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            BaseAttack();
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

        if (airState == EPlayerAirState.OnAir)
            collider2D = weaponAttackAreaClass.weaponUpperCutArea[playerWeaponState];

        state = EPlayerState.Attack;
        onAttack = true;

        var ray = AttackCollisionCheck(weaponAttackAreaClass.weaponGroundAttack[playerWeaponState][index]);
        foreach (RaycastHit2D physics2D in ray)
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
    }
    private void UpperCut()
    {
        var ray = AttackCollisionCheck(weaponAttackAreaClass.weaponUpperCutArea[playerWeaponState]);
        foreach (RaycastHit2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upperForcePower, ForceMode2D.Impulse);
        }
        rb.AddForce(Vector2.up * upperForcePower, ForceMode2D.Impulse);
    }
    private RaycastHit2D[] AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        return Physics2D.BoxCastAll(transform.position + (Vector3)collider2D.offset * lookDir, collider2D.size, 0, Vector2.right, 0, layerMask);
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
