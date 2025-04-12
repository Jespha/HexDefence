using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class TowerTargetingSystem : MonoBehaviour
{
	public static TowerTargetingSystem Instance { get; private set; }

	[SerializeField]
	private float predictionAccuracy = 0.1f;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);
	}

	/// <summary>
	/// Gets the best target for a tower based on its priority setting
	/// </summary>
	public TargetData GetBestTarget(
		Vector3 towerPosition,
		float attackRange,
		AttackPriority priority,
		float projectileSpeed
	)
	{
		// Get enemies in range using the Dictionary
		List<GameObject> enemiesInRange = GetEnemiesInRange(towerPosition, attackRange);

		if (enemiesInRange.Count == 0)
			return new TargetData { targetEnemy = null, aimPosition = Vector3.zero };

		// Select target based on priority
		GameObject target = SelectTargetBasedOnPriority(enemiesInRange, towerPosition, priority);

		if (target == null)
			return new TargetData { targetEnemy = null, aimPosition = Vector3.zero };

		// Verify target is valid before predicting position
		if (!IsValidTarget(target))
			return new TargetData { targetEnemy = null, aimPosition = Vector3.zero };

		// Calculate aim position with prediction
		Vector3 aimPosition = PredictTargetPosition(target, towerPosition, projectileSpeed);

		return new TargetData { targetEnemy = target, aimPosition = aimPosition };
	}

	// Add this helper method to verify target validity
	private bool IsValidTarget(GameObject target)
	{
		if (target == null || !target.activeInHierarchy)
			return false;

		EnemyManager enemyManager = GameManager.Instance.EnemyManager;

		// Check if target still exists in activeEnemies dictionary
		if (!enemyManager.activeEnemies.ContainsKey(target))
			return false;

		// Get enemy data
		EnemyData enemyData = enemyManager.activeEnemies[target];

		// Just ensure the enemy has health
		if (enemyData.Health <= 0)
		{
			// Debug line to visualize rejected enemies
			Debug.DrawLine(
				target.transform.position,
				target.transform.position + Vector3.up * 3,
				Color.red,
				0.5f
			);
			return false;
		}

		// Draw debug line for valid targets
		Debug.DrawLine(
			target.transform.position,
			target.transform.position + Vector3.up,
			Color.green,
			0.5f
		);
		return true;
	}

	/// <summary>
	/// Gets all enemies within range of the tower using the Dictionary
	/// </summary>
	private List<GameObject> GetEnemiesInRange(Vector3 towerPosition, float range)
	{
		List<GameObject> enemiesInRange = new List<GameObject>();

		// Dictionary lookup - iterate through keys (GameObjects)
		foreach (GameObject enemy in GameManager.Instance.EnemyManager.activeEnemies.Keys)
		{
			if (Vector3.Distance(enemy.transform.position, towerPosition) <= range)
			{
				enemiesInRange.Add(enemy);
			}
		}

		return enemiesInRange;
	}

	/// <summary>
	/// Selects a target based on the specified priority strategy
	/// </summary>
	private GameObject SelectTargetBasedOnPriority(
		List<GameObject> enemies,
		Vector3 towerPosition,
		AttackPriority priority
	)
	{
		if (enemies.Count == 0)
			return null;

		switch (priority)
		{
			case AttackPriority.First:
				return GetFirstEnemy(enemies);

			case AttackPriority.Last:
				return GetLastEnemy(enemies);

			case AttackPriority.Closest:
				return GetClosestEnemy(enemies, towerPosition);

			case AttackPriority.Strongest:
				return GetStrongestEnemy(enemies);

			case AttackPriority.Weakest:
				return GetWeakestEnemy(enemies);

			default:
				return GetFirstEnemy(enemies);
		}
	}

	/// <summary>
	/// Gets the enemy closest to the end of the path
	/// </summary>
	private GameObject GetFirstEnemy(List<GameObject> enemies)
	{
		GameObject first = null;
		float highestProgress = -1f;
		EnemyManager enemyManager = GameManager.Instance.EnemyManager;

		foreach (GameObject enemy in enemies)
		{
			// Direct dictionary lookup
			EnemyData enemyData = enemyManager.activeEnemies[enemy];
			float progress = enemyData.SplinePercentage;

			if (progress > highestProgress)
			{
				highestProgress = progress;
				first = enemy;
			}
		}

		return first;
	}

	/// <summary>
	/// Gets the enemy furthest from the end of the path
	/// </summary>
	private GameObject GetLastEnemy(List<GameObject> enemies)
	{
		GameObject last = null;
		float lowestProgress = float.MaxValue;
		EnemyManager enemyManager = GameManager.Instance.EnemyManager;

		foreach (GameObject enemy in enemies)
		{
			// Direct dictionary lookup
			EnemyData enemyData = enemyManager.activeEnemies[enemy];
			float progress = enemyData.SplinePercentage;

			if (progress < lowestProgress)
			{
				lowestProgress = progress;
				last = enemy;
			}
		}

		return last;
	}

	/// <summary>
	/// Gets the enemy closest to the tower
	/// </summary>
	private GameObject GetClosestEnemy(List<GameObject> enemies, Vector3 towerPosition)
	{
		GameObject closest = null;
		float minDistance = float.MaxValue;

		foreach (GameObject enemy in enemies)
		{
			float distance = Vector3.Distance(enemy.transform.position, towerPosition);
			if (distance < minDistance)
			{
				minDistance = distance;
				closest = enemy;
			}
		}

		return closest;
	}

	/// <summary>
	/// Gets the enemy with the highest health
	/// </summary>
	private GameObject GetStrongestEnemy(List<GameObject> enemies)
	{
		GameObject strongest = null;
		float maxHealth = -1f;
		EnemyManager enemyManager = GameManager.Instance.EnemyManager;

		foreach (GameObject enemy in enemies)
		{
			// Direct dictionary lookup
			EnemyData enemyData = enemyManager.activeEnemies[enemy];
			float health = enemyData.Health;

			if (health > maxHealth)
			{
				maxHealth = health;
				strongest = enemy;
			}
		}

		return strongest;
	}

	/// <summary>
	/// Gets the enemy with the lowest health
	/// </summary>
	private GameObject GetWeakestEnemy(List<GameObject> enemies)
	{
		GameObject weakest = null;
		float minHealth = float.MaxValue;
		EnemyManager enemyManager = GameManager.Instance.EnemyManager;

		foreach (GameObject enemy in enemies)
		{
			// Direct dictionary lookup
			EnemyData enemyData = enemyManager.activeEnemies[enemy];
			float health = enemyData.Health;

			if (health < minHealth && health > 0)
			{
				minHealth = health;
				weakest = enemy;
			}
		}

		return weakest;
	}

	/// <summary>
	/// Predicts where the enemy will be when the projectile reaches it
	/// </summary>
	public Vector3 PredictTargetPosition(
		GameObject target,
		Vector3 towerPosition,
		float projectileSpeed
	)
	{
		if (projectileSpeed <= 0)
			return target.transform.position + Vector3.up * 0.5f;

		EnemyManager enemyManager = GameManager.Instance.EnemyManager;

		// Direct dictionary lookup
		if (!enemyManager.activeEnemies.ContainsKey(target))
			return target.transform.position + Vector3.up * 0.5f;

		EnemyData enemyData = enemyManager.activeEnemies[target];

		// Calculate distance and time to hit
		float distance = Vector3.Distance(towerPosition, target.transform.position);
		float timeToHit = distance / projectileSpeed;

		// If the enemy is moving along a spline, use that for prediction
		if (!object.ReferenceEquals(enemyData.Road, null) && enemyData.Road.splineComputer != null)
		{
			// Special handling for newly spawned enemies at the start of the path
			if (enemyData.SplinePercentage == 0)
			{
				// For brand new enemies, aim at their current position plus a small "look ahead"
				Vector3 lookAhead = target.transform.forward * enemyData.Speed * timeToHit * 0.5f;
				return target.transform.position + lookAhead + Vector3.up * 0.5f;
			}

			// Normal case - enemy already on the path
			if (enemyData.SplinePercentage < 1.0f)
			{
				// Calculate how far along the path the enemy will be
				float futurePercentage =
					enemyData.SplinePercentage
					+ (enemyData.Speed * timeToHit / enemyData.SplineLength);

				// Ensure we stay within valid range
				futurePercentage = Mathf.Clamp01(futurePercentage);

				// The spline is evaluated from 1->0 (reversed) in your system
				Vector3 futurePosition = enemyData.Road.splineComputer.EvaluatePosition(
					1 - futurePercentage
				);

				return futurePosition + Vector3.up * 0.5f; // Add slight height offset
			}
		}

		// Fallback to simple linear prediction
		Vector3 currentPos = target.transform.position;
		Vector3 nextPos = currentPos;

		if (timeToHit > 0.05f) // Only worth predicting if there's enough time
		{
			// Calculate a simple next position based on current transform direction
			Vector3 direction = target.transform.forward;
			nextPos = currentPos + (direction * enemyData.Speed * timeToHit);
		}

		return nextPos + Vector3.up * 0.5f;
	}
}

/// <summary>
/// Structure containing targeting information
/// </summary>
public struct TargetData
{
	public GameObject targetEnemy;
	public Vector3 aimPosition;
}
