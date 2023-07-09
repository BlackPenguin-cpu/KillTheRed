using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingText : MonoBehaviour
{
    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * 110;
    }
}
