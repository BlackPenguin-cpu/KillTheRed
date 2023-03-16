using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


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
    [DictionaryDrawerSettings]
    public Dictionary<string, AnimationClip> animationClips;

    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {

    }
    public void AnimationPlay(string animationName)
    {
        StopAllCoroutines();
        StartCoroutine(AnimatorPlay(animationName));
    }
    private IEnumerator AnimatorPlay(string animationName)
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
    }
}
