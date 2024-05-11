using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyAnimationCoroutine
{
    public static IEnumerator HitAnimationCoroutine(GameObject Enemy)
    {
        Enemy.TryGetComponent<MeshRenderer>(out MeshRenderer enemyMesh);
        if (enemyMesh == null)
        {
            yield break;
        }

        enemyMesh.material.SetInt("_Hit", 1);
        float time = 0;
        float _duration = 0.3f;
        AnimationCurve _animationCurve = new AnimationCurve(
            new Keyframe(0, 0.7f),
            new Keyframe(0.5f, 1.3f),
            new Keyframe(1, 1)
        );

        while (time < _duration)
        {
            float scale = _animationCurve.Evaluate(time / _duration);
            Enemy.transform.localScale = new Vector3(scale, scale, scale);
            if (time > _duration / 2)
            {
                enemyMesh.material.SetInt("_Hit", 0);
            }
            time += Time.deltaTime;
            yield return null;
        }

        float finalScale = _animationCurve.Evaluate(1);
        Enemy.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
    }

    public static IEnumerator DeathAnimationFallCoroutine(
        GameObject Enemy,
        bool reachedEnd,
        EnemyData enemyData
    )
    {
        if (reachedEnd)
        {
            Currency.Instance.UpdateCurrency(
                -enemyData.Damage,
                CurrencyType.LifeCurrency,
                Camera.main.WorldToScreenPoint(Enemy.transform.position)
            );
        }
        else
        {
            Currency.Instance.UpdateCurrency(
                enemyData.GoldDrop,
                CurrencyType.GoldCurrency,
                Camera.main.WorldToScreenPoint(Enemy.transform.position)
            );
        }

        float time = 0;
        float _duration = enemyData.EnemyDeathAnimation._duration;
        AnimationCurve _animationCurve = enemyData.EnemyDeathAnimation._animationCurve;

        while (time < _duration)
        {
            float scale = _animationCurve.Evaluate(time / _duration);
            Enemy.transform.localScale = new Vector3(scale, scale, scale);

            time += Time.deltaTime;
            yield return null;
        }

        float finalScale = _animationCurve.Evaluate(1);
        Enemy.transform.localScale = new Vector3(finalScale, finalScale, finalScale);

        Enemy.SetActive(false);
    }

    public static IEnumerator DeathAnimationExplodeCoroutine(
        GameObject Enemy,
        bool reachedEnd,
        EnemyData enemyData
    )
    {
        if (reachedEnd)
        {
            Currency.Instance.UpdateCurrency(
                -enemyData.Damage,
                CurrencyType.LifeCurrency,
                Camera.main.WorldToScreenPoint(Enemy.transform.position)
            );
        }
        else
        {
            
            Currency.Instance.UpdateCurrency(
                enemyData.GoldDrop,
                CurrencyType.GoldCurrency,
                Camera.main.WorldToScreenPoint(Enemy.transform.position)
            );
            PooledObjectManager.Instance.Get(enemyData.DeathEffect).transform.position = Enemy
                .transform
                .position;
        }

        float time = 0;
        float _duration = enemyData.EnemyDeathAnimation._duration;
        AnimationCurve _animationCurve = enemyData.EnemyDeathAnimation._animationCurve;

        while (time < _duration)
        {
            float scale = _animationCurve.Evaluate(time / _duration);
            float xScale = enemyData.EnemyDeathAnimation._x ? scale : Enemy.transform.localScale.x;
            float yScale = enemyData.EnemyDeathAnimation._y ? scale : Enemy.transform.localScale.y;
            float zScale = enemyData.EnemyDeathAnimation._z ? scale : Enemy.transform.localScale.z;

            Enemy.transform.localScale = new Vector3(xScale, yScale, zScale);
            time += Time.deltaTime;
            yield return null;
        }

        float finalScale = _animationCurve.Evaluate(1);
        Enemy.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        Enemy.SetActive(false);
    }
}
