using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossWall : Wall
{
    int phase = 0;

    protected override void Hit(float value)
    {

        foreach (var ps in particleSystems)
        {
            ps.Play();
        }
        if (phase < 20 && Hp / maxHp < 1 - phase * 0.05f)
        {
            phase++;

            switch (Random.Range(0, 4))
            {
                case 0:
                    SoundManager.instance.PlaySound("054 (mp3cut.net)");
                    break;
                case 1:
                    SoundManager.instance.PlaySound("054 (mp3cut.net)");
                    break;
                case 2:
                    SoundManager.instance.PlaySound("056 (mp3cut.net)");
                    break;
                case 3:
                    SoundManager.instance.PlaySound("057 (mp3cut.net)");
                    break;
            }
        }

    }
    protected override void Die()
    {
        SoundManager.instance.PlaySound("054_mp3cut.net_1", SoundType.SE, 2);
        transform.DOShakePosition(5, 3, 20).SetEase(Ease.OutQuad);
        CameraManager.instance.FlashBang(5);

        StartCoroutine(timeDelay());
        IEnumerator timeDelay()
        {
            int i = 0;
            while (i < 50)
            {
                i++;
                foreach (var ps in particleSystems)
                {
                    ps.Play();
                }
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(1);
        }
    }
    protected override void CameraCheck()
    {
        if (cameraManager.transform.position.x > cameraPosX)
        {
            if (!GameManager.instance.onWall)
            {
                SoundManager.instance.PlaySound("X2Download.app_-__OST_128_kbps", SoundType.BGM);
            }
            cameraManager.cameraState = ECameraState.Wall;
            cameraManager.wallCameraPosX = cameraPosX;
            GameManager.instance.onWall = true;
        }
    }
}
