using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
	[SerializeField]
	EnemyManager _enemyManager;

	[SerializeField]
	private ProjectileManager _projectileManager;
	TowerData[] Towers = new TowerData[0];
	List<Collider> _colliders = new List<Collider>();
	public List<HexBuilding> HexBuildings = new List<HexBuilding>(); // Live copy of base HexBuilding
	public static TowerManager Instance { get; private set; }

	[SerializeField]
	private TowerTargetingSystem _targetingSystem;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogWarning("Multiple instances of TowerManager detected. Deleting one.");
			Destroy(this);
		}
	}

	private void Start()
	{
		if (_enemyManager == null)
		{
			_enemyManager = FindFirstObjectByType<EnemyManager>();
			if (_enemyManager == null)
			{
				Debug.Log("EnemyManager not found");
				gameObject.SetActive(false);
			}
		}

		// Get reference to the targeting system
		if (_targetingSystem == null)
		{
			_targetingSystem = FindFirstObjectByType<TowerTargetingSystem>();
			if (_targetingSystem == null)
			{
				Debug.LogWarning("TowerTargetingSystem not found, creating one...");
				GameObject targetingObj = new GameObject("TowerTargetingSystem");
				_targetingSystem = targetingObj.AddComponent<TowerTargetingSystem>();
			}
		}
	}

	public void UnlockTower(HexBuilding newHexBuilding)
	{
		HexBuilding hexBuilding = newHexBuilding.Clone();
		HexBuildings.Add(hexBuilding);
	}

	public void ClearHexBuildings()
	{
		HexBuildings.Clear();
	}

	public void AddTower(HexCell hexCell, PooledObject towerPrefab)
	{
		TowerData[] temp = new TowerData[Towers.Length + 1];
		for (int i = 0; i < Towers.Length; i++)
		{
			temp[i] = Towers[i];
		}
		temp[temp.Length - 1].hexCell = hexCell;
		temp[temp.Length - 1].towerPrefab = towerPrefab;
		if (hexCell.HexBuilding.AimPart != null)
		{
			GameObject aimPartInstance = Instantiate(
				hexCell.HexBuilding.AimPart,
				hexCell.Position,
				Quaternion.identity
			);
			Vector3 originalScale = aimPartInstance.transform.localScale;
			aimPartInstance.transform.parent = towerPrefab.transform;
			aimPartInstance.transform.localScale = originalScale;
			temp[temp.Length - 1].AimPart = aimPartInstance;
		}
		temp[temp.Length - 1].position = hexCell.Position;
		// temp[temp.Length - 1].attackType = hexCell.HexBuilding.AttackType;
		// temp[temp.Length - 1].attackDamage = hexCell.HexBuilding.AttackDamage;
		// temp[temp.Length - 1].attackSpeed = hexCell.HexBuilding.AttackSpeed;
		// temp[temp.Length - 1].attackRange = hexCell.HexBuilding.AttackRange;
		temp[temp.Length - 1].hexBuilding = hexCell.HexBuilding;
		temp[temp.Length - 1].attackPriority = hexCell.HexBuilding.TargetPriority;
		temp[temp.Length - 1].lastAttackTime = 0;

		// List<int> newUpgradeList = new List<int>(); // Local upgrads ( 0 Damage, 1 AttackSpeed, 2 Range)
		// for (int i = 0; i < 3; i++)
		// {
		// 	newUpgradeList.Add(1);
		// }
		// temp[temp.Length - 1].localUpgrades = newUpgradeList;
		Towers = temp;
	}

	public void Update()
	{
		if (GameManager.Instance != null && _enemyManager != null)
		{
			if (
				GameManager.Instance.GamePhase == GamePhase.Defend
				&& _enemyManager != null
				&& _enemyManager.activeEnemies != null
				&& _enemyManager.activeEnemies.Count > 0
			)
			{
				UpdateTowersProjectiles();
			}
		}
	}

	private bool IsEnemyValid(GameObject enemy)
	{
		// Must be active in scene
		if (enemy == null || !enemy.activeInHierarchy)
			return false;

		// Must be in the enemy manager
		if (!_enemyManager.activeEnemies.ContainsKey(enemy))
			return false;

		// Get enemy data
		EnemyData data = _enemyManager.activeEnemies[enemy];

		// Check if it has appropriate enemy properties
		// Add visual debugging to see what's happening
		if (data.Health <= 0)
		{
			Debug.DrawLine(
				enemy.transform.position,
				enemy.transform.position + Vector3.up * 3,
				Color.red,
				1f
			);
			return false;
		}

		// Success - draw debug visual
		Debug.DrawLine(
			enemy.transform.position,
			enemy.transform.position + Vector3.up,
			Color.green,
			0.1f
		);
		return true;
	}

	private void UpdateTowersProjectiles()
	{
		for (int i = 0; i < Towers.Length; i++)
		{
			// Skip if tower is on cooldown
			if (Towers[i].lastAttackTime + Towers[i].hexBuilding.AttackCooldown > Time.time)
				continue;

			// Use targeting system to get best target based on tower's priority
			TargetData targetData = _targetingSystem.GetBestTarget(
				Towers[i].position,
				Towers[i].hexBuilding.AttackRange,
				Towers[i].hexBuilding.TargetPriority,
				Towers[i].hexBuilding.AttackSpeed
			);

			// Double-check target validity with extra safeguards
			if (targetData.targetEnemy == null || !IsEnemyValid(targetData.targetEnemy))
				continue;

			// Add debugging to visualize aim points (can be removed later)
			Debug.DrawLine(Towers[i].position, targetData.aimPosition, Color.red, 2f);

			// Aim at the predicted position instead of directly at the enemy
			if (Towers[i].AimPart != null)
			{
				Towers[i].AimPart.transform.LookAt(targetData.aimPosition);
			}

			// Update attack timer
			Towers[i].lastAttackTime = Time.time;

			// Process attack based on tower type
			switch (Towers[i].hexBuilding.AttackType)
			{
				case AttackType.ProjectileDirect:
				case AttackType.ProjectileInDirect:
					ProjectileLogic(
						Towers[i].hexCell,
						targetData.targetEnemy,
						Towers[i].AimPart,
						targetData.aimPosition
					);
					break;
				case AttackType.Area:
					areaLogic(Towers[i].hexCell);
					break;
				case AttackType.Economy:
					generateCurrencyLogic(Towers[i].hexCell);
					break;
			}
		}
	}

	private void ProjectileLogic(
		HexCell hexCell,
		GameObject enemy,
		GameObject aimPart,
		Vector3 aimPosition
	)
	{
		// Use the overloaded AddProjectile method that takes an aim position
		_projectileManager.AddProjectile(hexCell, enemy, aimPosition);

		if (aimPart == null)
			return;

		// Animate the attack
		AnimationCurve _curve = new AnimationCurve(
			new Keyframe(0f, 0f),
			new Keyframe(0.5f, 1f),
			new Keyframe(1f, 0.5f)
		);

		StartCoroutine(
			AnimationCoroutine.SetScaleVec3Coroutine(
				aimPart.transform,
				new Vector3(1, 1, 0.5f),
				new Vector3(1, 1, 1),
				_curve,
				0.3f
			)
		);
	}

	private void ProjectileLogic(HexCell hexCell, GameObject enemy, GameObject aimPart)
	{
		// Use the enemy's position as the default aim position
		Vector3 defaultAimPos = enemy.transform.position + Vector3.up * 0.5f;
		ProjectileLogic(hexCell, enemy, aimPart, defaultAimPos);
	}

	private void areaLogic(HexCell hexCell)
	{
		// Area logic
	}

	private void hitScanLogic(HexCell hexCell)
	{
		// HitScan logic
	}

	private void generateCurrencyLogic(HexCell hexCell)
	{
		// GenerateCurrency logic
	}
}

struct TowerData
{
	public HexCell hexCell;
	public PooledObject towerPrefab;
	public GameObject AimPart;
	public Vector3 position;
	public HexBuilding hexBuilding;

	// public AttackType attackType;
	// public float attackDamage;
	// public float attackSpeed;
	// public float attackRange;
	public float lastAttackTime;
	public AttackPriority attackPriority;
	// public List<int> localUpgrades;
}
