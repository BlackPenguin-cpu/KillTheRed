using UnityEditor;
using UnityEngine;

public class Granade : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D collider2D;

    private float duration = 0;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        duration += Time.deltaTime;
        if (duration > 5)
        {
            animator.SetBool(0, true);
        }
    }
    private void Boom()
    {
        var ray = AttackCollisionCheck(collider2D);

        foreach (Collider2D col in ray)
        {
            col.GetComponent<Entity>().Hp -= Player.instance.attackDamage * 2;
        }
    }
    private Collider2D[] AttackCollisionCheck(BoxCollider2D collider2D)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        return Physics2D.OverlapBoxAll(transform.position + (Vector3)collider2D.offset, collider2D.size, 0, layerMask);
    }
}
