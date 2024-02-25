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
    public bool SpawnEnemiesTest = false;
    // public SplineComputer spline;
    public GameObject enemyPrefab;
    public float spawnCooldown = 1.0f;

    public void Update()
    {
        UpdateEnemies();
        if (SpawnEnemiesTest)
        {
            Level level = ScriptableObject.CreateInstance<Level>();
            SpawnEnemies(level);
        }
    }

    private void UpdateEnemies()
    {
        List<int> enemiesToDeactivate = new List<int>();
        for (int i = 0; i < activeEnemyCount; i++)
        {
            UpdateEnemy(i);
        }

        foreach (int index in enemiesToDeactivate)
        {
            DeactivateUnusedEnemy(index);
            activeEnemyCount--;
        }
    }

    private void UpdateEnemy(int index)
    {
        if (index >= activeEnemyCount)
        {
            return;
        }

        enemies[index].SplinePercentage += enemies[index].Speed * Time.deltaTime * 0.05f;
        if (enemies[index].SplinePercentage >= 1)
        {
            enemies[index].health = 0;
            Currency.Instance.UpdateCurrency(-1, CurrencyType.LifeCurrency);
            Debug.Log("Life Lost");
            DeactivateUnusedEnemy(index);
            return;
        }

        if (enemies[index].health <= 0)
        {
            enemies[index].health = 0;
            Debug.Log("Enemy killed");
            DeactivateUnusedEnemy(index);
            return;
        }

        // Reverse the direction of the spline
        Vector3 position = enemies[index].spline.EvaluatePosition(1 - enemies[index].SplinePercentage);
        Vector3 nextPosition = enemies[index].spline.EvaluatePosition(
            Math.Max(0, 1 - (enemies[index].SplinePercentage + 0.01f))
        );
        Vector3 direction = (nextPosition - position).normalized;
        GameObject enemy = GetEnemyFromPool(index);
        enemy.transform.position = position;
        enemy.transform.rotation = Quaternion.LookRotation(direction);
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
        enemyPool[index].SetActive(false);
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
        enemies[index] = new EnemyData
        {
            SplinePercentage = 0f,
            Speed = 1.0f,
            health = 1,
            spline = RoadManager.Instance.GetRandomRoad().splineComputer,
        };
        
        double percent = index / (double)maxEnemies;
        Vector3 position = enemies[index].spline.EvaluatePosition(0);
        Vector3 nextPosition = enemies[index].spline.EvaluatePosition(Math.Min(0,  percent + 0.05));
        Vector3 direction = (nextPosition - position).normalized;
    }
}

struct EnemyData
{
    public float SplinePercentage;
    public float Speed;
    public int health;
    public SplineComputer spline;
}
