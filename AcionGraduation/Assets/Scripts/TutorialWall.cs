using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialWall : Wall
{
    protected override void Die()
    {
        GameStart();
        Player.instance.revivePos = new Vector3(40, 0, 0);
        GameManager.instance.onWall = false;
        SoundManager.instance.PlaySound("SFX_Wall_Die");
        GameManager.instance.waveNum++;

        Player.instance.StartCoroutine(timeDelay());
        IEnumerator timeDelay()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            yield return null;
            Time.timeScale = 0.1f;
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 1;
        }
        foreach (var ps in particleSystems)
        {
            ps.Play();
        }

        gameObject.SetActive(false);
        Destroy(gameObject, 2f);
    }
    protected override void Start()
    {
        base.Start();
        SoundManager.instance.PlaySound("BGM_02_Tutorial", SoundType.BGM, 0.2f);
    }
    async void GameStart()
    {
        SoundManager.instance.AudioSources[SoundType.BGM].volume = 0;
        CameraManager.instance.Fade(3, true);
        await Task.Delay(6000);

        Player.instance.transform.position = new Vector3(40, 0);
        CameraManager.instance.Fade(3, false);
        CameraManager.instance.cameraState = ECameraState.InGame;
        ItemManager.instance.ItemGet(EItemList.Dash);

        await Task.Delay(6000);
        GameManager.instance.WaveStart();
        SoundManager.instance.AudioSources[SoundType.BGM].volume = 0.5f;
        SoundManager.instance.PlaySound("BGM_03_Ingame", SoundType.BGM);
    }
}
