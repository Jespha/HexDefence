using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManager : MonoBehaviour
{
    private int maxEnemies;
    EnemyData[] enemies = new EnemyData[0];
    private List<GameObject> enemyPool = new();
    public List<GameObject> activeEnemies = new();
    public List<SphereCollider> colliders = new();
    private int _defeatedEnemies = 0;
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
        if (GameManager.Instance.GamePhase == GamePhase.Defend && activeEnemies.Count > 0)
            UpdateEnemies();

        if (
            GameManager.Instance.GamePhase == GamePhase.Defend
            && enemyPool.Count == _defeatedEnemies
        )
        {
            Debug.Log("Level Complete");
            StartCoroutine(GameManager.Instance.LevelComplete());
            _defeatedEnemies = 0;
        }
    }

    public void LoadNextLevelEnemies(int level, Level _level)
    {
        if (GameManager.Instance.GamePhase == GamePhase.Income)
            return;
        ClearEnemies();
        currentLevel = _level;
        int _maxEnemies = 0;
        _defeatedEnemies = 0;
        foreach (var enemy in currentLevel.enemies)
        {
            _maxEnemies += enemy.amount;
        }
        maxEnemies = _maxEnemies;
        enemies = new EnemyData[maxEnemies];
        int currentIndex = 0;

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
                    SplineLength = _spline.CalculateLength(),
                };
                currentIndex++; 
            }
        }
        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject enemy = Instantiate(enemies[i].Prefab, this.transform);
            SphereCollider collider = enemy.GetComponent<SphereCollider>();
            colliders.Add(collider);
            enemy.SetActive(false); 
            enemyPool.Add(enemy);
        }

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private void UpdateEnemies()
    {
        // Using a copy of the activeEnemies list to avoid modifying the list while iterating
        List<GameObject> activeEnemiesCopy = new List<GameObject>(activeEnemies);

        for (int i = activeEnemiesCopy.Count - 1; i >= 0; i--)
        {
            GameObject enemy = activeEnemiesCopy[i];
            if (enemy != null && UpdateEnemy(enemy))
            {
                enemy.SetActive(false); 
                activeEnemies.Remove(enemy);
                colliders.Remove(enemy.GetComponent<SphereCollider>());
                _defeatedEnemies++;
            }
        }
    }

    private bool UpdateEnemy(GameObject enemy)
    {
        int index = enemyPool.IndexOf(enemy);

        enemies[index].SplinePercentage +=
            enemies[index].Speed * Time.deltaTime / enemies[index].SplineLength;

        // Clamp the SplinePercentage between 0 and 1
        enemies[index].SplinePercentage = Mathf.Clamp(enemies[index].SplinePercentage, 0, 1);

        if (enemies[index].SplinePercentage >= 1 || enemies[index].Health <= 0)
        {
            if (enemies[index].Health <= 0)
            {
                Currency.Instance.UpdateCurrency(enemies[index].GoldDrop, CurrencyType.GoldCurrency);
            }
            if (enemies[index].SplinePercentage >= 1)
            {
                Currency.Instance.UpdateCurrency(-enemies[index].Damage, CurrencyType.LifeCurrency);
            }
            enemyPool[index].SetActive(false);
            return true;
        }

        Vector3 position = enemies[index]
            .Spline.EvaluatePosition(1 - enemies[index].SplinePercentage);
        Vector3 nextPosition = enemies[index]
            .Spline.EvaluatePosition(1 - Math.Max(0, enemies[index].SplinePercentage - 0.01f));
        Vector3 direction = (nextPosition - position).normalized;

        direction = -direction; // Reversing the direction of the model since we are traveling the spline backwards

        enemy = enemyPool[index];
        enemy.transform.position = position;
        enemy.transform.rotation = Quaternion.LookRotation(direction);

        return false;
    }

    public void ClearEnemies()
    {
        StopAllCoroutines();
        colliders.Clear();
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            DestroyImmediate(enemyPool[i]);
        }
        enemies = new EnemyData[maxEnemies];
        enemyPool.Clear();
        activeEnemies.Clear();
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            if (i < enemyPool.Count)
            {
                yield return new WaitForSeconds(spawnCooldown);
                GameObject enemy = SpawnEnemy(i);
                activeEnemies.Add(enemy);
            }
            else
            {
                yield break;
            }
        }
    }

    private GameObject SpawnEnemy(int index)
    {
        EnemyData enemyData = enemies[index];
        GameObject enemy = enemyPool[index];
        enemy.SetActive(true);
        enemy.transform.position = enemyData.Spline.EvaluatePosition(
            1 - enemyData.SplinePercentage
        );

        Vector3 nextPosition = enemyData.Spline.EvaluatePosition(
            Math.Min(0, enemyData.SplinePercentage + 0.05f)
        );
        Vector3 direction = (nextPosition - enemy.transform.position).normalized;

        enemy.transform.rotation = Quaternion.LookRotation(direction);

        return enemy;
    }

    public void  DamageEnemy(GameObject Enemy, float damage)
    {
        int index = activeEnemies.IndexOf(Enemy);
        enemies[index].Health -= damage;
        // if (enemies[index].Health <= 0)
        // {
        //     Currency.Instance.UpdateCurrency(enemies[index].GoldDrop, CurrencyType.HexCurrency);
        //     activeEnemies.Remove(Enemy);
        //     colliders.Remove(Enemy.GetComponent<SphereCollider>());
        //     _defeatedEnemies++;
        //     Enemy.SetActive(false);
        // }
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
    public SphereCollider Collider;
}
