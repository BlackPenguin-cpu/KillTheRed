using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialWall : Wall
{
    protected override void Die()
    {
        GameStart();
        base.Die();
    }
    async void GameStart()
    {
        CameraManager.instance.Fade(3, true);
        await Task.Delay(6000);

        Player.instance.transform.position = new Vector3(40, 0);
        CameraManager.instance.Fade(3, false);
        CameraManager.instance.cameraState = ECameraState.InGame;

        await Task.Delay(6000);
        GameManager.instance.WaveStart();
    }
}
