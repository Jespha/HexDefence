using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    const int maxEnemies = 10;
    int activeEnemyCount = 0;
    EnemyData[] enemies = new EnemyData[maxEnemies];
    List<GameObject> enemyPool = new();
    public GameObject enemyPrefab;
    public float spawnCooldown = 1.0f;

    public void Update()
    {
        UpdateEnemies();
    }

    private void UpdateEnemies()
    {
        List<int> enemiesToDeactivate = new List<int>();
        for (int i = 0; i < activeEnemyCount; i++)
        {
            if (UpdateEnemy(i))
            {
                enemiesToDeactivate.Add(i);
            }
        }

        foreach (int index in enemiesToDeactivate)
        {
            DeactivateUnusedEnemy(index);
        }

        activeEnemyCount -= enemiesToDeactivate.Count;
    }

    private bool UpdateEnemy(int index)
    {
        if (index >= activeEnemyCount)
        {
            return false;
        }

        enemies[index].SplinePercentage += enemies[index].Speed * Time.deltaTime;

        if (enemies[index].SplinePercentage >= 1)
        {
            enemies[index].health = 0;
            Currency.Instance.UpdateCurrency(-1, CurrencyType.LifeCurrency);
            Debug.Log("Life Lost");
            return true;
        }

        if (enemies[index].health <= 0)
        {
            enemies[index].health = 0;
            Debug.Log("Enemy killed");
            return true;
        }

        Vector3 position = enemies[index]
            .spline.EvaluatePosition(1 - enemies[index].SplinePercentage);
        Vector3 nextPosition = enemies[index]
            .spline.EvaluatePosition(Math.Max(0, 1 - (enemies[index].SplinePercentage + 0.01f)));
        Vector3 direction = (nextPosition - position).normalized;
        GameObject enemy = GetEnemyFromPool(index);
        enemy.transform.position = position;
        enemy.transform.rotation = Quaternion.LookRotation(direction);

        return false;
    }

    private GameObject GetEnemyFromPool(int index)
    {
        GameObject enemy;
        if (index < enemyPool.Count)
        {
            enemy = enemyPool[index];
            enemy.SetActive(true);
        }
        else
        {
            enemy = Instantiate(enemyPrefab, this.transform);
            enemyPool.Add(enemy);
        }
        return enemy;
    }

    private void DeactivateUnusedEnemy(int index)
    {
        // Swap the enemy to deactivate with the last active enemy
        EnemyData temp = enemies[index];
        enemies[index] = enemies[activeEnemyCount - 1];
        enemies[activeEnemyCount - 1] = temp;

        // Swap the GameObject to deactivate with the last active GameObject
        GameObject tempGameObject = enemyPool[index];
        enemyPool[index] = enemyPool[activeEnemyCount - 1];
        enemyPool[activeEnemyCount - 1] = tempGameObject;

        // Deactivate the GameObject
        enemyPool[activeEnemyCount - 1].SetActive(false);
    }

    public void SpawnEnemies(Level level)
    {
        StartCoroutine(SpawnEnemiesCoroutine(level));
    }

    public void ClearEnemies()
    {
        enemies = new EnemyData[maxEnemies];
        activeEnemyCount = 0;
    }

    private IEnumerator SpawnEnemiesCoroutine(Level level)
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy(i);
            yield return new WaitForSeconds(spawnCooldown);
            activeEnemyCount++;
        }
    }

    private void SpawnEnemy(int index)
    {
        SplineComputer splineComputer = RoadManager.Instance.GetRandomRoad().splineComputer;
        float splineLength = splineComputer.CalculateLength(); // Calculate spline length

        enemies[index] = new EnemyData
        {
            SplinePercentage = 0f,
            Speed = 1.0f / splineLength, // Adjust speed based on spline length
            health = 1,
            spline = splineComputer,
            SplineLength = splineLength, // Store spline length
        };

        double percent = index / (double)maxEnemies;
        Vector3 position = enemies[index].spline.EvaluatePosition(0);
        Vector3 nextPosition = enemies[index].spline.EvaluatePosition(Math.Min(0, percent + 0.05));
        Vector3 direction = (nextPosition - position).normalized;
    }
}

struct EnemyData
{
    public float SplinePercentage;
    public float Speed;
    public int health;
    public SplineComputer spline;
    public float SplineLength;
}
