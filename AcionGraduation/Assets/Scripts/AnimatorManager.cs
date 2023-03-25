using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationClip
{
    [Header("Animation")]
    public List<Sprite> sprites;

    [Header("Info")]
    public bool isLoop;
    public bool isReverse;
    public float delay;
    public Dictionary<int, float> animationDelayInfos = new Dictionary<int, float>();
}
[RequireComponent(typeof(SpriteRenderer))]
public class AnimatorManager : SerializedMonoBehaviour
{
    public Dictionary<string, AnimationClip> animationClips;
    public Action animationUpdate;
    public DelegateDrawer<Action> delegateDrawer;

    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public virtual void AnimationPlay(string animationName)
    {
        StopAllCoroutines();
        StartCoroutine(AnimatorPlay(animationName));

    }
    protected virtual IEnumerator AnimatorPlay(string animationName, Action action = null)
    {

        AnimationClip animationClip = animationClips[animationName];
        WaitForSeconds waitSec = new WaitForSeconds(animationClip.delay);

    Loop:
        for (int i = 0; i < animationClip.sprites.Count; i++)
        {
            spriteRenderer.sprite = animationClip.sprites[i];

            if (animationClip.animationDelayInfos.TryGetValue(i, out float value))
                yield return new WaitForSeconds(value);
            else
                yield return waitSec;
        }

        if (animationClip.isLoop)
            goto Loop;

        if (action != null) action();

    }
}
