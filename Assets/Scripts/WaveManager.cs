using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("---Visual---")]
    [SerializeField] private TMP_Text waveNumberText;
    [SerializeField] private RectTransform waveRectTransform;
    private float waveRectOriginalSize;

    [Header("---Waves---")]
    [SerializeField] private int numberOfWaves;
    private int currentWave;
    private bool isPlaying;

    [SerializeField] private List<EnemySpawner> enemySpawners;
    [SerializeField] private GameObject pillPrefab;
    [SerializeField] private int[] numberOfEnemiesPerWave;
    [SerializeField] private float extraTimeForWave;
    [Range(0f, 1f)]
    [SerializeField] private float[] policemanOddsForWave;

    private float maxWaveDuration;
    private float currentWaveDuration;

    private List<GameObject> currentEnemiesAlive;

    private bool lastWave;

    public static Action OnWinGame = delegate { };

    private void Awake()
    {
        waveRectOriginalSize = waveRectTransform.sizeDelta.x;
        currentEnemiesAlive = new List<GameObject>();

        GameManager.OnStartGame += StartGame;
        EnemySpawner.OnSpawnEnemy += HandleSpawnEnemy;
        Policeman.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDestroy()
    {
        GameManager.OnStartGame -= StartGame;
        EnemySpawner.OnSpawnEnemy -= HandleSpawnEnemy;
        Policeman.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void Update()
    {
        if (isPlaying)
        {
            currentWaveDuration += Time.deltaTime;

            waveRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, waveRectOriginalSize * (currentWaveDuration / maxWaveDuration));

            if (currentWaveDuration > maxWaveDuration)
            {
                NextWave();
            }
        }

        if (lastWave && currentEnemiesAlive.Count == 0)
        {
            WinGame();
            lastWave = false;
        }
    }

    private void StartGame()
    {
        isPlaying = true;
        BeginWave(0);
    }

    private void NextWave()
    {
        currentWaveDuration = 0;
        currentWave++;

        if (currentWave >= numberOfWaves)
        {
            Debug.Log("win game");
            lastWave = true;
            return;
        }

        BeginWave(currentWave);
    }

    private void WinGame()
    {
        OnWinGame?.Invoke();
    }

    private void HandleSpawnEnemy(GameObject enemy)
    {
        currentEnemiesAlive.Add(enemy);
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        currentEnemiesAlive.Remove(enemy);

        if (currentEnemiesAlive.Count == 0)
        {
            NextWave();
        }
    }

    private void BeginWave(int waveNumber)
    {
        waveNumberText.text = "Wave " + (waveNumber + 1);

        int enemyCount = numberOfEnemiesPerWave[waveNumber];
        int enemiesPerSpawner = enemyCount / enemySpawners.Count;

        int mostEnemies = enemiesPerSpawner;

        for (int i = 0; i < enemySpawners.Count; i++)
        {
            if (i < enemyCount % enemySpawners.Count)
            {
                enemySpawners[i].BeginWave(enemiesPerSpawner + 1, policemanOddsForWave[waveNumber]);
                mostEnemies++;
            }
            else
            {
                enemySpawners[i].BeginWave(enemiesPerSpawner, policemanOddsForWave[waveNumber]);
            }
        }

        maxWaveDuration = mostEnemies * enemySpawners[0].GetLongestSpawnTime() + extraTimeForWave;
    }
}
