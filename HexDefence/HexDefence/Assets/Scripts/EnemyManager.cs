using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int maxEnemies;
    public int activeEnemyCount { get; private set; }
    EnemyData[] enemies = new EnemyData[0];
    List<GameObject> enemyPool = new();
    public GameObject enemyPrefab; // REMOVE
    public float spawnCooldown = 1.0f;
    public Level currentLevel;

    private void OnEnable()
    {
        StartCoroutine(WaitForGameManager());
    }

    private IEnumerator WaitForGameManager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.OnLevelStart += LoadNextLevelEnemies;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelStart -= LoadNextLevelEnemies;
        }
    }

    public void Update()
    {

        if (GameManager.Instance.GamePhase == GamePhase.Defend)
            UpdateEnemies();

        if (
            activeEnemyCount == 0
            && GameManager.Instance.CurrentLevel != null
            && GameManager.Instance.CurrentLevel.enemies.Count == 0
        )
            GameManager.Instance.NoMoreEnemies();
    }

    public void LoadNextLevelEnemies(int level, Level _level)
    {
        if (GameManager.Instance.GamePhase == GamePhase.Income)
            return;

        int _maxEnemies = 0;
        activeEnemyCount = 0;
        foreach (var enemy in currentLevel.enemies)
        {
            _maxEnemies += enemy.amount; // Sum up the amounts of all enemies
        }
        maxEnemies = _maxEnemies;
        EnemyData[] enemies = new EnemyData[maxEnemies];
        int currentIndex = 0; // Keep track of the current index across all enemy types

        foreach (var enemy in currentLevel.enemies)
        {
            for (int i = 0; i < enemy.amount; i++)
            {
                SplineComputer _spline = RoadManager.Instance.GetRandomRoad().splineComputer;
                enemies[currentIndex] = new EnemyData
                {
                    Prefab = enemy.enemy.prefab,
                    Health = enemy.enemy.health,
                    Damage = enemy.enemy.damage,
                    Speed = enemy.enemy.speed,
                    Slow = enemy.enemy.slow,
                    GoldDrop = enemy.enemy.goldDrop,
                    SplinePercentage = 0f,
                    Spline = _spline,
                    SplineLength = _spline.CalculateLength()
                };
                currentIndex++; // Increment the current index after each enemy is added
            }
        }
        StartCoroutine(SpawnEnemiesCoroutine());
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
            enemies[index].Health = 0;
            Currency.Instance.UpdateCurrency(-1, CurrencyType.LifeCurrency);
            Debug.Log("Life Lost");
            return true;
        }

        if (enemies[index].Health <= 0)
        {
            enemies[index].Health = 0;
            Debug.Log("Enemy killed");
            return true;
        }

        Vector3 position = enemies[index]
            .Spline.EvaluatePosition(1 - enemies[index].SplinePercentage);
        Vector3 nextPosition = enemies[index]
            .Spline.EvaluatePosition(Math.Max(0, 1 - (enemies[index].SplinePercentage + 0.01f)));
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

        // Reset all of the EnemyData before deactivating the enemy
        enemies[activeEnemyCount - 1] = new EnemyData
        {
            SplinePercentage = 0f,
            Speed = 0f,
            Health = 0,
            Spline = null,
            SplineLength = 0f,
            // Add any other fields you have in EnemyData and want to reset
        };

        // Deactivate the GameObject
        enemyPool[activeEnemyCount - 1].SetActive(false);
    }

    public void ClearEnemies()
    {
        enemies = new EnemyData[maxEnemies];
        activeEnemyCount = 0;
    }

    private IEnumerator SpawnEnemiesCoroutine()
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
        double percent = index / (double)maxEnemies;
        Vector3 position = enemies[index].Spline.EvaluatePosition(0);
        Vector3 nextPosition = enemies[index].Spline.EvaluatePosition(Math.Min(0, percent + 0.05));
        Vector3 direction = (nextPosition - position).normalized;
    }
}

struct EnemyData
{
    public GameObject Prefab;
    public float Health;
    public int Damage;
    public float Speed;
    public float Slow;
    public int GoldDrop;

    public float SplinePercentage;
    public SplineComputer Spline;
    public float SplineLength;
}
