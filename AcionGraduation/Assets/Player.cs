using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public BoxCollider2D upperCutArea;

    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float spd;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    private void Update()
    {
        PlayerInput();
        Move();
    }
    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.J) && Input.GetKey(KeyCode.Space))
        {
            UpperCut();
        }
    }
    private void Jump()
    {
        var ray = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.NameToLayer("Platform"));
        if (ray.transform != null) return;

        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }
    private void Move()
    {
        var hor = Input.GetAxisRaw("Horizontal");
        var spdValue = hor * spd * Time.deltaTime;

        transform.position = Vector3.right * spdValue;
    }
    private void UpperCut()
    {
        var ray = Physics2D.BoxCastAll(transform.position + (Vector3)upperCutArea.offset, upperCutArea.size, 0, Vector2.right, 0, LayerMask.NameToLayer("Enemy"));
        foreach (RaycastHit2D physics2D in ray)
        {
            physics2D.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

    }
}
