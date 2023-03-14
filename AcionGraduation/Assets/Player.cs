using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    public BoxCollider2D upperCutArea;
    public BoxCollider2D baseAttackArea;

    public bool onAir;
    public float upperPower;

    private int lookRight;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float spd;

    // Update is called once per frame
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
        if (hor != 0) lookRight = (int)hor;
        var spdValue = hor * spd * Time.deltaTime;

        transform.position += Vector3.right * spdValue;
    }
    private void BaseAttack()
    {
        var ray = AttackCollisionCheck(baseAttackArea);
        foreach (RaycastHit2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(2f, 1f);
            physics2D.transform.GetComponent<BaseEnemy>().Hp -= 1;
        }

        //if (onAir) rb.velocity = new Vector2(2.5f, 1);
    }
    private void UpperCut()
    {
        var ray = AttackCollisionCheck(upperCutArea);
        foreach (RaycastHit2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upperPower, ForceMode2D.Impulse);
        }
        rb.AddForce(Vector2.up * upperPower, ForceMode2D.Impulse);
    }
    private RaycastHit2D[] AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        return Physics2D.BoxCastAll(transform.position + (Vector3)collider2D.offset, collider2D.size, 0, lookRight == 1 ? Vector2.right : Vector2.left, 0, layerMask);
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
