using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum ECameraState
{
    Tutorial,
    InGame,
    Wall
}
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public float speed;
    public Image flashScreen;
    public Image fadeScreen;
    public ECameraState cameraState;
    public float wallCameraPosX;

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
        if (cameraState == ECameraState.Tutorial) return;

        if (cameraState == ECameraState.Wall)
            transform.position = Vector3.Lerp(transform.position, new Vector3(wallCameraPosX, 0, -10), Time.deltaTime * speed);
        else
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerObj.transform.position.x, 0, -10), Time.deltaTime * speed);
    }
    public void Flash(float duration, float startValue = 0.2f, float endValue = 0f)
    {
        float i = 1 - startValue;
        Color color = new Color(1, 1, 1, startValue);

        StartCoroutine(fade());
        IEnumerator fade()
        {

            while (i <= 1)
            {
                flashScreen.color = color;
                color.a = Mathf.Lerp(1, endValue, i);
                i += Time.deltaTime / duration;
                yield return null;
            }
            color.a = 0;
            flashScreen.color = color;
        }

    }
    public void Fade(float duration, bool isFadeOut)
    {
        float i = 0;
        Color color = new Color(0, 0, 0, 0);

        if (isFadeOut)
            StartCoroutine(fadeOut());
        else
            StartCoroutine(fadeIn());

        IEnumerator fadeOut()
        {
            while (i <= 1)
            {
                fadeScreen.color = color;
                color.a = Mathf.Lerp(0, 1, i);
                i += Time.deltaTime / duration;
                yield return null;
            }
            color.a = 1;
            fadeScreen.color = color;
        }
        IEnumerator fadeIn()
        {
            fadeScreen.color = Color.black;
            while (i <= 1)
            {
                fadeScreen.color = color;
                color.a = 1 - Mathf.Lerp(0, 1, i);
                i += Time.deltaTime / duration;
                yield return null;
            }
            color.a = 0;
            fadeScreen.color = color;
        }
    }
}
