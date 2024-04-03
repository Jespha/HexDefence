using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int maxEnemies;
    private bool doneSpawning = false;
    EnemyData[] enemies = new EnemyData[0];
    private List<GameObject> enemyPool = new();
    public Dictionary<GameObject, EnemyData> activeEnemies { get; private set; }
    private int _defeatedEnemies = 0;
    public float spawnCooldown = 0.75f;
    public Level currentLevel;

    public event Action<GameObject> EnemyAdded;
    public event Action<GameObject> EnemyRemoved;

    private void OnEnable()
    {
        StartCoroutine(WaitForGameManager());
        activeEnemies = new Dictionary<GameObject, EnemyData>();
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
        if (
            GameManager.Instance != null
            && GameManager.Instance.GamePhase == GamePhase.Defend
            && activeEnemies != null
            && activeEnemies.Count > 0
        )
            UpdateEnemies();
        
        
        // Update game phase Logic INCOME -> HEXPLACEMENT -> BUILD -> DEFEND
        // DEFEND -> INCOME
        if (GameManager.Instance.GamePhase == GamePhase.Defend && _defeatedEnemies >= maxEnemies)
        {
            Debug.Log("Level Complete");
            StartCoroutine(GameManager.Instance.LevelComplete());
            doneSpawning = false;
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
        doneSpawning = false;
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
                    MaxHealth = enemy.enemy.health,
                    Damage = enemy.enemy.damage,
                    Speed = enemy.enemy.speed,
                    Slow = enemy.enemy.slow,
                    GoldDrop = enemy.enemy.goldDrop,
                    DeathEffect = enemy.enemy.deathEffect,
                    EnemyDeathAnimation = enemy.enemy.enemyDeathAnimation,
                    SplinePercentage = 0f,
                    Spline = _spline,
                    SplineLength = _spline.CalculateLength(),
                    SpawnOnDeath = enemy.enemy.spawnOnDeath
                };
                currentIndex++;
            }
        }

        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject enemy = Instantiate(enemies[i].Prefab, this.transform);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }

        foreach (var enemy in currentLevel.enemies)
        {
            PooledObjectManager.Instance.AddToPool(
                enemy.enemy.deathEffect,
                Mathf.FloorToInt(enemy.amount / 3) + 1            );
        }

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private void UpdateEnemies()
    {
        // Using a copy of the activeEnemies list to avoid modifying the list while iterating
        List<GameObject> activeEnemiesCopy = new List<GameObject>(activeEnemies.Keys);

        for (int i = activeEnemiesCopy.Count - 1; i >= 0; i--)
        {
            GameObject enemy = activeEnemiesCopy[i];
            if (enemy != null && UpdateEnemy(enemy))
            {
                int index = enemyPool.IndexOf(enemy);
                if (enemies[index].SplinePercentage >= 1 || enemies[index].Health <= 0)
                {
                    if (enemies[index].Health <= 0)
                    {
                        HandleEnemyDeath(enemy, false);
                    }
                    if (enemies[index].SplinePercentage >= 1)
                    {
                        HandleEnemyDeath(enemy, true);
                    }
                }
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
        if (activeEnemies != null)
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy.Key != null)
                {
                    DestroyImmediate(enemy.Key);
                }
            }
            enemies = new EnemyData[maxEnemies];
            enemyPool.Clear();
            activeEnemies.Clear();
        }
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            if (i < enemyPool.Count)
            {
                yield return new WaitForSeconds(spawnCooldown);
                SpawnEnemy(i); // Spawn the enemy without adding it to the dictionary here
            }
            else
            {
                doneSpawning = true;
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
        if (!activeEnemies.ContainsKey(enemy))
        {
            activeEnemies.Add(enemy, enemies[index]);
            EnemyAdded?.Invoke(enemy);
        }
        return enemy;
    }

    private void HandleEnemyDeath(GameObject Enemy, bool reachedEnd)
    {
        if (activeEnemies.ContainsKey(Enemy))
        {
            EnemyData enemyData = activeEnemies[Enemy];
            switch (enemyData.EnemyDeathAnimation.Name)
            {
                case "Fall":
                    StartCoroutine(
                        EnemyAnimationCoroutine.DeathAnimationFallCoroutine(
                            Enemy,
                            reachedEnd,
                            enemyData
                        )
                    );
                    break;
                case "Explode":
                    StartCoroutine(
                        EnemyAnimationCoroutine.DeathAnimationExplodeCoroutine(
                            Enemy,
                            reachedEnd,
                            enemyData
                        )
                    );
                    if (enemyData.SpawnOnDeath != null)
                    {
                        GameObject newEnemy = Instantiate(enemyData.SpawnOnDeath.prefab, this.transform);
                        newEnemy.SetActive(false);
                        enemyPool.Add(newEnemy);
                        activeEnemies.Add(newEnemy, new EnemyData
                        {
                            Prefab = enemyData.SpawnOnDeath.prefab,
                            Health = enemyData.SpawnOnDeath.health,
                            MaxHealth = enemyData.SpawnOnDeath.health,
                            Damage = enemyData.SpawnOnDeath.damage,
                            Speed = enemyData.SpawnOnDeath.speed,
                            Slow = enemyData.SpawnOnDeath.slow,
                            GoldDrop = enemyData.SpawnOnDeath.goldDrop,
                            DeathEffect = enemyData.SpawnOnDeath.deathEffect,
                            EnemyDeathAnimation = enemyData.SpawnOnDeath.enemyDeathAnimation,
                            SplinePercentage = 0f,
                            Spline = enemyData.Spline,
                            SplineLength = enemyData.SplineLength
                        });
                        StartCoroutine(SpawnEnemiesCoroutine());
                    }
                    break;
                // case "burn":
                //     StartCoroutine(DeathAnimationCoroutine(Enemy, reachedEnd, enemyData));
                //     break;
                // case "melt":
                //     StartCoroutine(DeathAnimationCoroutine(Enemy, reachedEnd, enemyData));
                //     break;
                // case "vaporize":
                //     StartCoroutine(DeathAnimationCoroutine(Enemy, reachedEnd, enemyData));
                //     break;
                default:
                    break;
            }
            activeEnemies.Remove(Enemy);
            EnemyRemoved?.Invoke(Enemy);
            _defeatedEnemies++;
        }
    }

    public void DamageEnemy(GameObject Enemy, float damage)
    {
        if (activeEnemies.ContainsKey(Enemy))
        {
            EnemyData enemyData = activeEnemies[Enemy];
            enemyData.Health -= damage;
            StartCoroutine(EnemyAnimationCoroutine.HitAnimationCoroutine(Enemy));
            if (enemyData.Health <= 0)
            {
                HandleEnemyDeath(Enemy, false);
            }
            else
            {
                activeEnemies[Enemy] = enemyData; // Update the enemy data in the dictionary
            }
        }
    }

    /// <summary>
    /// <para>Provides float data from enemy provided the enemy GameObject and the data requested </para>
    /// <para>Floats that can be requested are: Health, MaxHealth, Speed, SplinePercentage</para>
    /// </summary>
    /// <param name="enemy">The enemy GameObject</param>
    /// <param name="data">The data requested</param>
    public float ProvideEnemyFloatData(GameObject enemy, string data)
    {
        EnemyData enemyData = activeEnemies[enemy];
        switch (data)
        {
            case "Health":
                return enemyData.Health;
            case "MaxHealth":
                return enemyData.MaxHealth;
            case "Speed":
                return enemyData.Speed;
            case "SplinePercentage":
                return enemyData.SplinePercentage;
            default:
                return 0;
        }
    }

    /// <summary>
    /// <para>Provides int data from enemy provided the enemy GameObject and the data requested</para>
    /// <para>Ints that can be requested are: Damage, GoldDrop</para>
    /// </summary>
    /// <param name="enemy">The enemy GameObject</param>
    /// <param name="data">The data requested</param>
    public int ProvideEnemyIntData(GameObject enemy, string data)
    {
        int index = enemyPool.IndexOf(enemy);
        switch (data)
        {
            case "Damage":
                return enemies[index].Damage;
            case "GoldDrop":
                return enemies[index].GoldDrop;
            default:
                return 0;
        }
    }
}

public struct EnemyData
{
    public GameObject Prefab;
    public float Health;
    public float MaxHealth;
    public int Damage;
    public float Speed;
    public float Slow;
    public int GoldDrop;
    public PooledObject DeathEffect;
    public EnemyAnimation EnemyDeathAnimation;
    public Enemy SpawnOnDeath;

    public float SplinePercentage;
    public SplineComputer Spline;
    public float SplineLength;
}
