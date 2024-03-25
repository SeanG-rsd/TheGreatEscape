using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector2 spawnRadius;
    [SerializeField] private GameObject policemanPrefab;
    [SerializeField] private GameObject carPrefab;
    private int enemiesToSpawn;
    private float policemanOdds;

    [SerializeField] private Vector2 spawnTimer;
    private float timeUntilNextSpawn;

    [SerializeField] private GameObject target;

    public static Action<GameObject> OnSpawnEnemy;
    // Update is called once per frame
    void Update()
    {
        if (enemiesToSpawn > 0)
        {
            timeUntilNextSpawn -= Time.deltaTime;
            if (timeUntilNextSpawn <= 0)
            {
                float which = UnityEngine.Random.value;
                if (which <= policemanOdds)
                {
                    SpawnEnemy(policemanPrefab);
                }
                else
                {
                    SpawnEnemy(carPrefab);
                }

                timeUntilNextSpawn = UnityEngine.Random.Range(spawnTimer.x, spawnTimer.y);
                enemiesToSpawn--;
            }
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        float spawnX = transform.position.x + (UnityEngine.Random.Range(spawnRadius.x, spawnRadius.y) * UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
        float spawnY = transform.position.y + (UnityEngine.Random.Range(spawnRadius.x, spawnRadius.y) * UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);
        enemy.GetComponent<Policeman>().Activate(target);

        OnSpawnEnemy?.Invoke(enemy);
    }

    public void BeginWave(int numberOfEnemies, float policemanOdds)
    {
        timeUntilNextSpawn = UnityEngine.Random.Range(spawnTimer.x, spawnTimer.y);
        enemiesToSpawn += numberOfEnemies;
        this.policemanOdds = policemanOdds;
        Debug.Log(numberOfEnemies);
    }

    public float GetLongestSpawnTime()
    {
        return spawnTimer.y;
    }
}
