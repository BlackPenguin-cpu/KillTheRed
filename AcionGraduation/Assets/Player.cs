using UnityEngine;

public enum EPlayerState
{
    Idle,
    Run,
    Jump,
    Attack,
    Dead,

}
public class Player : Entity
{
    public BoxCollider2D upperCutArea;
    public BoxCollider2D baseAttackArea;
    private EPlayerState state;
    public EPlayerState StateProfull
    {
        get { return state; }
        set
        {
            if (state != value)
            {
                animatorManager.AnimationPlay(value.ToString());
            }
            state = value;
        }
    }

    public bool onAir;
    public float upperForcePower;

    public float attackDamage;

    private int lookDir;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float spd;

    private SpriteRenderer spriteRenderer;
    private AnimatorManager animatorManager;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animatorManager = GetComponent<AnimatorManager>();
        animatorManager.AnimationPlay("Idle");
    }
    private void Update()
    {
        PlayerInput();
        Move();
        onAir = isOnAir();
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
    }
    private void Move()
    {
        var hor = Input.GetAxisRaw("Horizontal");
        if (hor != 0)
        {
            lookDir = (int)hor;
            spriteRenderer.flipX = lookDir == 1 ? false : true;
            StateProfull = EPlayerState.Run;
        }
        else
        {
            StateProfull = EPlayerState.Idle;
        }
        var spdValue = hor * spd * Time.deltaTime;

        transform.position += Vector3.right * spdValue;
    }
    private void BaseAttack()
    {
        var ray = AttackCollisionCheck(baseAttackArea);
        foreach (RaycastHit2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(2f, 1f);
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= attackDamage;
        }

        //if (onAir) rb.velocity = new Vector2(2.5f, 1);
    }
    private void UpperCut()
    {
        var ray = AttackCollisionCheck(upperCutArea);
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
