using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HexBuilding", menuName = "ScriptableObjects/HexBuilding", order = 1)]
public class HexBuilding : ScriptableObject
{
	public string Name = "Tower";
	public PooledObject Prefab;
	public GameObject AimPart;
	public Projectile ProjectilePrefab;
	public Sprite Icon;
	public Sprite Sprite;
	public int Cost = 100;
	public int Level = 1;

	public HexBuildingType HexBuildingType;

	[ShowIfGroup("HexBuildingType", Value = HexBuildingType.Tower)]
	[BoxGroup("HexBuildingType/Tower")]
	public AttackType AttackType;

	[BoxGroup("HexBuildingType/Tower")]
	public ProjectileMovementType ProjectileMovementType;

	[BoxGroup("HexBuildingType/Tower")]
	public OnHitEffectType OnHitEffectType;

	[ShowIfGroup("HexBuildingType/Tower/OnHitEffectType", Value = OnHitEffectType.Splash)]
	[Range(0, 10)]
	[BoxGroup("HexBuildingType/Tower/OnHitEffectType")]
	public float SplashSize = 0;

	[BoxGroup("HexBuildingType/Tower")]
	public float AttackDamage = 1.0f;

	[BoxGroup("HexBuildingType/Tower")]
	public float AttackSpeed = 1.0f;

	[BoxGroup("HexBuildingType/Tower")]
	public float AttackRange = 1.0f;

	[BoxGroup("HexBuildingType/Tower")]
	public float AttackCooldown = 1.0f;

	[BoxGroup("HexBuildingType/Tower")]
	public AttackPriority TargetPriority;

	// Call when unlocked
	public HexBuilding Clone()
	{
		HexBuilding clone = Instantiate(this);
		return clone;
	}

	public void UpgradeStats(
		float attackDamageIncrease,
		float attackSpeedIncrease,
		float attackRangeIncrease,
		bool overrideStats = false
	)
	{
		if (overrideStats)
		{
			this.AttackDamage = attackDamageIncrease;
			this.AttackSpeed = attackSpeedIncrease;
			this.AttackRange = attackRangeIncrease;
			return;
		}

		this.AttackDamage += attackDamageIncrease;
		this.AttackSpeed += attackSpeedIncrease;
		this.AttackRange += attackRangeIncrease;
	}

	public static string AttackTypeToSprite(AttackType attackType)
	{
		return "<sprite name=\"" + attackType + "\">";
	}
}

public enum AttackType
{
	ProjectileDirect,
	ProjectileInDirect,
	Area,
	Economy,
	Buff,
	None,
}

public enum ProjectileMovementType
{
	Direct,
	Arc,
	Spiral,
	Wave,
	Zigzag
}

public enum AttackPriority
{
	First,
	Last,
	Closest,
	Strongest,
	Weakest,
	Random
}

public enum HexBuildingType
{
	None,
	Base,
	Tower,
}
