using UnityEngine;

public class AnimatorDestory : MonoBehaviour
{
    private float duration = 0;
    private float maxDuration = 0;
    private Animator animator;

    [SerializeField]
    private bool isActive;
    void Start()
    {
        animator = GetComponent<Animator>();
        maxDuration = animator.runtimeAnimatorController.animationClips[0].length;
    }

    void Update()
    {
        duration += Time.deltaTime;
        if (duration > maxDuration)
        {
            duration = 0;
            if (isActive)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }

    }
}
