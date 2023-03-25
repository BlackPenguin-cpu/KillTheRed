using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float speed;

    private Player playerObj;

    private void Start()
    {
        playerObj = FindObjectOfType<Player>();
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, playerObj.transform.position + new Vector3(0, 0, -10), Time.deltaTime * speed);
    }
}
