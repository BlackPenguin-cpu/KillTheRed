using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.Serializable]
    public class WavePatterns
    {
        public List<WavePattern> wavePatterns;
    }
    [System.Serializable]
    public class WavePattern
    {
        public float delay;
        public int posNum;
        public BaseEnemy enemy;
    }

    public BaseEnemy[] enemies;

    public bool onWall;
    public int waveNum = 0;

    public WavePatterns[] wavePatternsType1;
    public WavePatterns[] wavePatternsType2;
    public WavePatterns[] wavePatternsType3;
    public WavePatterns[] wavePatternsType4;
    public WavePatterns[] wavePatternsType5;

    public Vector3[] summonPos;
    private void Awake()
    {
        instance = this;
    }
    public void WaveStart()
    {
        SpawnWave();
    }
    private void SpawnWave()
    {
        StartCoroutine(wave());
        IEnumerator wave()
        {
            while (true)
            {
                int num = 0;
                if (onWall) num = Random.Range(0, 2);

                Vector3 pos = summonPos[num] + Player.instance.transform.position;
                WavePatterns wavePatterns = new WavePatterns();

                switch (waveNum)
                {
                    case 1:
                        wavePatterns = wavePatternsType1[Random.Range(0, wavePatternsType1.Length)];
                        break;
                    case 2:
                        wavePatterns = wavePatternsType2[Random.Range(0, wavePatternsType2.Length)];
                        break;
                    case 3:
                        wavePatterns = wavePatternsType3[Random.Range(0, wavePatternsType3.Length)];
                        break;
                    case 4:
                        wavePatterns = wavePatternsType4[Random.Range(0, wavePatternsType4.Length)];
                        break;
                    case 5:
                        wavePatterns = wavePatternsType5[Random.Range(0, wavePatternsType5.Length)];
                        break;
                }

                foreach (WavePattern pattern in wavePatterns.wavePatterns)
                {
                    Instantiate(pattern.enemy, pos, Quaternion.identity);
                    yield return new WaitForSeconds(pattern.delay);
                }
                yield return new WaitForSeconds(15);
            }
        }
    }
}
