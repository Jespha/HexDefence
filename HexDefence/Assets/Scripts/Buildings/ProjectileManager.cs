using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
	public List<PooledObject> activeProjectiles = new List<PooledObject>();
	public List<GameObject> activeProjectilesTarget = new List<GameObject>();
	private ProjectileData[] projectileData = new ProjectileData[0];

	[SerializeField]
	private GameObject _projectileParent;

	private void OnEnable()
	{
		StartCoroutine(WaitForGameManager());
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnLevelStart -= ClearProjectiles;
		}
	}

	private IEnumerator WaitForGameManager()
	{
		yield return new WaitUntil(() => GameManager.Instance != null);
		GameManager.Instance.OnLevelStart += ClearProjectiles;
	}

	public void AddProjectile(HexCell hexcell, GameObject enemy, Vector3 aimPosition)
	{
		IProjectileMovementPattern movementPattern;
		switch (hexcell.HexBuilding.ProjectileMovementType)
		{
			case ProjectileMovementType.Arc:
				movementPattern = new ArcMovementPattern();
				break;
			case ProjectileMovementType.Spiral:
				movementPattern = new SpiralMovementPattern();
				break;
			default:
				movementPattern = new DirectMovementPattern();
				break;
		}

		Projectile projectilePrefab = hexcell.HexBuilding.ProjectilePrefab;
		PooledObject projectile = PooledObjectManager.Instance.Get(
			projectilePrefab.ProjectilePrefab
		);
		projectile.gameObject.transform.SetParent(_projectileParent.transform);

		Vector3 startPosition = hexcell.Position + new Vector3(0, 1, 0);
		projectile.transform.position = startPosition;
		projectile.transform.LookAt(aimPosition); // Look at prediction point

		PooledObject launch = PooledObjectManager.Instance.Get(
			hexcell.HexBuilding.ProjectilePrefab.LaunchVFXPrefab
		);
		launch.transform.position = startPosition;
		launch.transform.LookAt(aimPosition); // Look at prediction point

		ProjectileData[] temp = new ProjectileData[projectileData.Length + 1];
		for (int i = 0; i < projectileData.Length; i++)
		{
			temp[i] = projectileData[i];
		}

		temp[temp.Length - 1].HexBuilding = hexcell.HexBuilding;
		temp[temp.Length - 1].projectile = projectile;
		temp[temp.Length - 1].startPosition = startPosition;
		temp[temp.Length - 1].endPosition = aimPosition; // Use prediction point
		temp[temp.Length - 1].speed = hexcell.HexBuilding.AttackSpeed;
		temp[temp.Length - 1].damage = hexcell.HexBuilding.AttackDamage;
		temp[temp.Length - 1].launchVFX = launch;
		temp[temp.Length - 1].impactVFX = hexcell.HexBuilding.ProjectilePrefab.ImpactVFXPrefab;
		temp[temp.Length - 1].movementPattern = movementPattern;
		temp[temp.Length - 1].progress = 0f;
		temp[temp.Length - 1].lastPosition = startPosition;
		projectileData = temp;
		activeProjectiles.Add(projectile);
		activeProjectilesTarget.Add(enemy);
	}

	public void AddProjectile(HexCell hexcell, GameObject enemy)
	{
		Vector3 defaultAimPos = enemy.transform.position + Vector3.up * 0.5f;
		AddProjectile(hexcell, enemy, defaultAimPos);
	}

	public void ClearProjectiles(int levelNumber, Level level)
	{
		foreach (var projectile in activeProjectiles)
		{
			PooledObjectManager.Instance.ReturnToPool(projectile);
		}
		activeProjectiles.Clear();
		activeProjectilesTarget.Clear();
		projectileData = new ProjectileData[0]; // Reset the array
	}

	public void Update()
	{
		if (activeProjectiles.Count > 0)
		{
			// Make sure collections stay in sync
			if (
				activeProjectiles.Count != projectileData.Length
				|| activeProjectiles.Count != activeProjectilesTarget.Count
			)
			{
				Debug.LogError(
					$"Collection size mismatch: Projectiles={activeProjectiles.Count}, "
						+ $"ProjectileData={projectileData.Length}, "
						+ $"Targets={activeProjectilesTarget.Count}"
				);

				// Attempt to recover by clearing all
				ClearProjectiles(0, null);
				return;
			}

			UpdateProjectiles();
		}
	}

	private void UpdateProjectiles()
	{
		// Iterate backwards to safely handle removals
		for (int i = activeProjectiles.Count - 1; i >= 0; i--)
		{
			// Check if target enemy still exists and is valid
			GameObject targetEnemy = activeProjectilesTarget[i];
			if (
				targetEnemy == null
				|| !GameManager.Instance.EnemyManager.activeEnemies.ContainsKey(targetEnemy)
			)
			{
				// Target no longer exists - destroy this projectile
				if (projectileData[i].impactVFX != null)
				{
					// Create impact effect at last known position
					PooledObject impact = PooledObjectManager.Instance.Get(
						projectileData[i].impactVFX
					);
					impact.transform.position = projectileData[i].projectile.transform.position;
					impact.transform.LookAt(projectileData[i].startPosition);
				}

				PooledObjectManager.Instance.ReturnToPool(projectileData[i].projectile);
				activeProjectilesTarget.RemoveAt(i);
				activeProjectiles.RemoveAt(i);
				RemoveProjectileData(i);
				continue;
			}

			// For homing projectiles, update the end position to the current enemy position
			if (projectileData[i].movementPattern.IsHoming)
			{
				// Update end position to the current enemy position
				projectileData[i].endPosition = targetEnemy.transform.position + Vector3.up * 0.5f;
			}

			// Check if we've reached the target
			if (
				Vector3.Distance(
					projectileData[i].projectile.transform.position,
					projectileData[i].endPosition
				) < 0.1f
			)
			{
				// Normal hit logic
				if (projectileData[i].impactVFX != null)
				{
					projectileData[i].projectile.transform.LookAt(projectileData[i].endPosition);
					PooledObject impact = PooledObjectManager.Instance.Get(
						projectileData[i].impactVFX
					);
					impact.transform.position = projectileData[i].endPosition;
					impact.transform.LookAt(projectileData[i].startPosition);
				}

				GameManager.Instance.EnemyManager.DamageEnemy(
					activeProjectilesTarget[i],
					projectileData[i].damage
				);

				activeProjectilesTarget.RemoveAt(i);
				PooledObjectManager.Instance.ReturnToPool(projectileData[i].projectile);
				activeProjectiles.RemoveAt(i);
				RemoveProjectileData(i);
				continue;
			}

			Vector3 currentPosition = projectileData[i].projectile.transform.position;

			// Calculate next position using the movement pattern
			Vector3 nextPosition = projectileData[i]
				.movementPattern.CalculateNextPosition(
					currentPosition,
					projectileData[i].startPosition,
					projectileData[i].endPosition,
					projectileData[i].progress,
					projectileData[i].speed,
					Time.deltaTime
				);

			// Apply the position
			projectileData[i].projectile.transform.position = nextPosition;

			// Calculate movement direction for rotation
			Vector3 moveDirection = nextPosition - currentPosition;

			// Update rotation based on movement pattern
			if (projectileData[i].movementPattern.ShouldLookAtTarget())
			{
				projectileData[i].projectile.transform.LookAt(projectileData[i].endPosition);
			}
			else if (moveDirection.magnitude > 0.001f)
			{
				projectileData[i].projectile.transform.rotation = projectileData[i]
					.movementPattern.GetRotation(
						currentPosition,
						projectileData[i].startPosition,
						projectileData[i].endPosition,
						moveDirection
					);
			}

			// Update progress and last position
			float distanceTotal = Vector3.Distance(
				projectileData[i].startPosition,
				projectileData[i].endPosition
			);
			if (distanceTotal > 0.001f)
			{
				float progressDelta = (projectileData[i].speed * Time.deltaTime) / distanceTotal;
				projectileData[i].progress += progressDelta;
			}

			projectileData[i].lastPosition = currentPosition;
		}
	}

	private void RemoveProjectileData(int index)
	{
		ProjectileData[] temp = new ProjectileData[projectileData.Length - 1];
		for (int i = 0; i < temp.Length; i++)
		{
			if (i < index)
			{
				temp[i] = projectileData[i];
			}
			else
			{
				temp[i] = projectileData[i + 1];
			}
		}
		projectileData = temp;
	}
}

struct ProjectileData
{
	public HexBuilding HexBuilding;
	public PooledObject projectile;
	public PooledObject launchVFX;
	public PooledObject impactVFX;
	public Vector3 startPosition;
	public Vector3 endPosition;
	public float speed;
	public float damage;
	public IProjectileMovementPattern movementPattern;
	public float progress; // Track progress for some movement patterns
	public Vector3 lastPosition; // Store previous position for movement direction
}
