using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    public BoxCollider2D upperCutArea;
    public BoxCollider2D baseAttackArea;

    public bool onAir;
    public float upperPower;

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
        var spdValue = hor * spd * Time.deltaTime;

        transform.position += Vector3.right * spdValue;
    }
    private void BaseAttack()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        var ray = Physics2D.BoxCastAll(transform.position + (Vector3)upperCutArea.offset, upperCutArea.size, 0, Vector2.right, 0, layerMask);

        foreach (RaycastHit2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(2.8f, 1f);
        }

        if (onAir)
        {
            rb.velocity = new Vector2(2.5f, 1);
        }
    }
    private void UpperCut()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        var ray = Physics2D.BoxCastAll(transform.position + (Vector3)upperCutArea.offset, upperCutArea.size, 0, Vector2.right, 0, layerMask);
        foreach (RaycastHit2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upperPower, ForceMode2D.Impulse);
        }
        rb.AddForce(Vector2.up * upperPower, ForceMode2D.Impulse);
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
