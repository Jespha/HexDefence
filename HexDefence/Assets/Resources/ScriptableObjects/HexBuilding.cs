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
    public TypeCollider TypeCollider;
    public Vector3 ColliderSize = new Vector3(1, 1, 1);
    public Vector3 ColliderPosition = new Vector3(0, 0, 0);
    public AttackType AttackType;
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

    public void UpgradeStats(float attackDamageIncrease, float attackSpeedIncrease, float attackRangeIncrease, bool overrideStats = false)
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
    Splash,
    Area,
    Economy,
    Beam,
    Buff,
    Debuff,
    Summon,
    Trap,
    Turret,
    None,
}

public enum TypeCollider
{
    SphereCollider,
    BoxCollider,
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
