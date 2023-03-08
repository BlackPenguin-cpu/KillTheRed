using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float spd;


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
    }
    private void Jump()
    {
        var ray = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, LayerMask.NameToLayer("Platform"));
        if (ray.transform != null) return;

        rb.AddForce(Vector2.up * jumpPower);
    }
    private void Move()
    {
        var hor = Input.GetAxisRaw("Horizontal");
        var spdValue = hor * spd * Time.deltaTime;

        transform.position = Vector3.right * spdValue;
    }
}
