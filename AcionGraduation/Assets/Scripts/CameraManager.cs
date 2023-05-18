using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public float speed;
    public Image flashScreen;

    private Player playerObj;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerObj = FindObjectOfType<Player>();
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerObj.transform.position.x, 0, -10), Time.deltaTime * speed);
    }
    public void Flash(float duration, float startValue = 1f, float endValue = 0.1f)
    {
        float i = startValue;
        Color color = new Color(1, 1, 1, startValue);

        while (i < endValue)
        {
            flashScreen.color = color;
            color.a = Mathf.Lerp(startValue, endValue, i);
            i += Time.deltaTime / duration;
        }

    }
}
