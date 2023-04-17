using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{

    public enum SpawnState { SPAWNING, WAITING, COUNTING };


    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;
    public int wavesCompleted;
    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI waveStateText;

    private int currentEnemyCount = 0;
    private int maxEnemyCount = 24;

    void Start()
    {
        wavesCompleted = 1;
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced.");
        }
        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            // Check if enemies are still alive
            if (!EnemyIsAlive())
            {
                // Begin a new round
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                // Start spawning wave
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
            waveStateText.text = "Next Round In";
            currentWaveText.text = waveCountdown.ToString("F2");
        }
    }


    void WaveCompleted()
    {
        wavesCompleted++;

        state = SpawnState.COUNTING;
        waveStateText.text = "Next Round In";

        waveCountdown = timeBetweenWaves;



        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
        }
        else
        {
            nextWave++;
        }
        if (wavesCompleted != int.Parse(currentWaveText.text))
        {
            currentWaveText.text = wavesCompleted.ToString();
        }
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.SPAWNING;
        waveStateText.text = "Round";
        if (wavesCompleted != float.Parse(currentWaveText.text))
        {
            currentWaveText.text = wavesCompleted.ToString();
        }

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy, wavesCompleted);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        state = SpawnState.WAITING;
        if (wavesCompleted <= 19)
        {
            _wave.count = Mathf.RoundToInt(6f + (wavesCompleted * 1.2f));
        }
        else
        {
            _wave.count = Mathf.RoundToInt(0.09f * Mathf.Pow(wavesCompleted, 2) - 0.0029f * wavesCompleted + 23.9580f);
        }
        yield break;
    }

    void SpawnEnemy(Transform _enemy, int waveNum)
    {
        GameObject enemyGO = Instantiate(_enemy.gameObject, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        Enemy enemy = enemyGO.GetComponent<Enemy>();

        if (waveNum == 1)
        {
            enemy.maxHealth = 100;
            enemy.attackDamage = 10;
            enemy.moveSpeed = Random.Range(1.45f, 2.25f);
        }
        else if (waveNum >= 2 && waveNum <= 10)
        {
            enemy.maxHealth = 100 + ((waveNum - 1) * 100);
            enemy.attackDamage = 10 + ((waveNum - 1) * 10);
            float standardSpeed = 1.85f + (0.23f * (waveNum - 1));
            enemy.moveSpeed = Random.Range(standardSpeed - 0.4f, standardSpeed + 0.4f);
        }
        else if (waveNum >= 11)
        {
            enemy.maxHealth = Mathf.RoundToInt(enemy.maxHealth * 1.1f);
            enemy.attackDamage = Mathf.RoundToInt(enemy.attackDamage * 1.1f);
            float standardSpeed = 3.14f;
            enemy.moveSpeed = Random.Range(standardSpeed - 0.4f, standardSpeed + 0.4f);
        }
    }
}
