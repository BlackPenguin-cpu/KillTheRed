using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public BaseEnemy[] enemies;


    public bool bigEnemyOn;
    public bool tallEnemyOn;
    private void Awake()
    {
        instance = this;
    }
    private void WaveStart()
    {
        SpawnWave();
    }
    private void SpawnWave()
    {
        Vector3 pos = Player.instance.transform.position + new Vector3(Random.Range(25, 35), 3);

        Instantiate(enemies[0], pos + Vector3.right * Random.Range(-5, 5), Quaternion.identity);
        if (bigEnemyOn)
            Instantiate(enemies[1], pos + Vector3.right * Random.Range(-5, 5), Quaternion.identity);

        if (tallEnemyOn)
            Instantiate(enemies[2], pos + Vector3.right * Random.Range(-5, 5), Quaternion.identity);
    }
}
