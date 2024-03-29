using UnityEngine;

public class Granade : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D attackCollider;

    [SerializeField]
    private Animator animator;
    private Rigidbody2D rigid;
    private float duration = 0;
    void Start()
    {
        SoundManager.instance.PlaySound("X2Download.app-PUBG___GRENADE_OPENING_SOUND___GRENADE_SOUND_EFFECT___GRENADE_PIN_OPENING___NOTIFICATION_mp3cut.net",SoundType.SE,0.5f);
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        duration += Time.deltaTime;


        if (duration > 2)
        {
            animator.enabled = true;
        }
    }
    private void Boom()
    {
        SoundManager.instance.PlaySound("X2Download_mp3cut.net", SoundType.SE, 0.5f);

        transform.eulerAngles = Vector3.zero;
        var ray = AttackCollisionCheck(attackCollider);

        foreach (Collider2D col in ray)
        {
            col.GetComponent<Entity>().Hp -= Player.instance.attackDamage * 2;
            col.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 15, ForceMode2D.Impulse);
        }
    }
    private Collider2D[] AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        return Physics2D.OverlapBoxAll(transform.position + (Vector3)collider2D.offset, collider2D.size, 0, layerMask);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)//Platform
        {
            rigid.velocity = Vector3.zero;
            rigid.isKinematic = true;
        }
    }
    private void AnimEnd()
    {
        Destroy(gameObject);
    }
}
