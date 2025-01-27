using UnityEngine;

[CreateAssetMenu(fileName = "HexBuilding", menuName = "ScriptableObjects/HexBuilding", order = 1)]
public class HexBuilding : ScriptableObject
{
    public HexBuildingType HexBuildingType;
    public string Name;
    public PooledObject Prefab;
    public GameObject AimPart;
    public Projectile ProjectilePrefab;
    public Sprite Icon;
    public int Cost;
    public AttackType AttackType;
    public OnHitEffectType OnHitEffectType;
    public float AttackDamage = 1.0f;
    public float AttackSpeed = 1.0f;
    public float AttackRange = 1.0f;
    public AttackPriority TargetPriority;
    public int Level = 0;

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
    Projectile,
    Area,
    Economy,
    Buff,
    None,
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
