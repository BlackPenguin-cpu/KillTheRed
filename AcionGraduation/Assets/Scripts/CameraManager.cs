using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public float speed;
    public Image FlashScreen;

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
}
