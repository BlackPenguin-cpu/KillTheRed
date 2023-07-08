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
    public ECameraState cameraState;

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
